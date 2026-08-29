using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Principal;
using Microsoft.WSL.Containers;

namespace WSLC.PowerShell.Support;

// https://learn.microsoft.com/en-us/windows/wsl/wsl-container?tabs=csharp
// https://wsl.dev/api-reference/csharp/
// %LocalAppData%\wslc\sessions

internal static class WslcRuntime
{
    // Mirrors the wslc CLI's default-session conventions (WSLCSessionDefaults.h /
    // WSLCSessionManager::ResolveDefaultSessionName): a per-user, elevation-aware
    // name and storage under <LocalAppData>\wslc\sessions\<name>. The CLI's
    // "wslc-cli" prefix is reserved by the service, so this module uses its own base.
    private const string DefaultSessionBaseName = "wslc-pwsh";
    private const string DefaultAdminSessionBaseName = "wslc-pwsh-admin";
    private const string DefaultStorageSubPath = @"wslc\sessions";

    private static readonly Lazy<string> ResolvedDefaultSessionName = new(ResolveDefaultSessionName);

    /// <summary>
    /// The default session name for the current user, qualified with the username and
    /// elevation status (e.g. "wslc-psh-alice", "wslc-psh-admin-alice"), so each user
    /// gets their own default session — the same scheme the wslc CLI's service uses.
    /// </summary>
    public static string DefaultSessionName => ResolvedDefaultSessionName.Value;

    private static string ResolveDefaultSessionName()
    {
        using var identity = WindowsIdentity.GetCurrent();
        var elevated = new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
        var baseName = elevated ? DefaultAdminSessionBaseName : DefaultSessionBaseName;
        return $"{baseName}-{Environment.UserName}";
    }
    private static readonly ConcurrentDictionary<string, Session> Sessions = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<Session, string> SessionNames = new();
    private static readonly ConcurrentDictionary<string, string> SessionStoragePaths = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<string, Container> Containers = new(StringComparer.OrdinalIgnoreCase);
    private static readonly ConcurrentDictionary<string, string> ContainerSessions = new(StringComparer.OrdinalIgnoreCase);

    public static Session RegisterSession(string name, Session session, string? storagePath = null)
    {
        Sessions[name] = session;
        SessionNames[session] = name;

        if (storagePath is not null)
        {
            SessionStoragePaths[name] = storagePath;
        }

        return session;
    }

    public static Session GetOrCreateDefaultSession()
    {
        return GetOrCreateSession(DefaultSessionName);
    }

    /// <summary>
    /// Returns the session registered under the given name, or creates, starts, and
    /// registers a new one. When <paramref name="settings"/> is null, defaults are used
    /// (storage under the local application data folder).
    /// </summary>
    public static Session GetOrCreateSession(string resolvedName, SessionSettings? settings = null)
    {
        if (TryGetSession(resolvedName, out var existing) && existing is not null)
        {
            return existing;
        }

        if (settings is null)
        {
            var storagePath = GetDefaultStoragePath(resolvedName);
            Directory.CreateDirectory(storagePath);
            settings = new SessionSettings(resolvedName, storagePath);
        }

        var session = new Session(settings);
        session.Start();
        return RegisterSession(resolvedName, session, settings.StoragePath);
    }

    /// <summary>
    /// The storage directory for a session, following the CLI's convention:
    /// &lt;LocalAppData&gt;\wslc\sessions\&lt;name&gt; (the service creates storage.vhdx inside).
    /// </summary>
    public static string GetDefaultStoragePath(string sessionName)
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            DefaultStorageSubPath,
            sessionName);
    }

    public static Session ResolveSession(Session? session, string? name)
    {
        if (session is not null)
        {
            EnsureRegistered(session);
            return session;
        }

        return string.IsNullOrWhiteSpace(name) ? GetOrCreateDefaultSession() : GetSession(name!);
    }

    public static bool TryGetSession(string name, out Session? session)
    {
        return Sessions.TryGetValue(name, out session);
    }

    public static Session GetSession(string name)
    {
        if (!TryGetSession(name, out var session) || session is null)
        {
            throw new InvalidOperationException($"WSLC session '{name}' was not found in the current PowerShell process.");
        }

        return session;
    }

    public static bool RemoveSession(string name, out Session? session)
    {
        var removed = Sessions.TryRemove(name, out session);
        if (removed && session is not null)
        {
            SessionNames.TryRemove(session, out _);
        }

        return removed;
    }

    public static Session[] GetSessions()
    {
        return Sessions.Values.ToArray();
    }

    public static string GetSessionName(Session session)
    {
        if (SessionNames.TryGetValue(session, out var name))
        {
            return name;
        }

        throw new InvalidOperationException("WSLC session name is not tracked in the current PowerShell process.");
    }

    public static bool TryGetSessionName(Session session, out string name)
    {
        if (SessionNames.TryGetValue(session, out var found))
        {
            name = found;
            return true;
        }

        name = string.Empty;
        return false;
    }

    /// <summary>
    /// The storage path a session was created with, recorded so another process can
    /// reconnect to a session that does not use the default location.
    /// </summary>
    public static string? GetSessionStoragePath(string name)
    {
        return SessionStoragePaths.TryGetValue(name, out var storagePath) ? storagePath : null;
    }

    private static void EnsureRegistered(Session session)
    {
        if (!SessionNames.ContainsKey(session))
        {
            RegisterSession($"Session-{session.GetHashCode():x8}", session);
        }
    }

    /// <summary>
    /// Tracks a container in this process and on disk, so Get-Container still lists it in
    /// a new process.
    /// </summary>
    public static Container RegisterContainer(string sessionName, string containerName, Container container)
    {
        TrackContainer(sessionName, containerName, container);
        ContainerIndex.Add(sessionName, GetSessionStoragePath(sessionName), containerName, container.Id);
        return container;
    }

    /// <summary>
    /// Tracks a container in this process only, for ones already recorded on disk.
    /// </summary>
    private static Container TrackContainer(string sessionName, string containerName, Container container)
    {
        Containers[containerName] = container;
        ContainerSessions[containerName] = sessionName;
        return container;
    }

    public static bool TryGetContainer(string name, out Container? container)
    {
        return Containers.TryGetValue(name, out container);
    }

    /// <summary>
    /// Resolves a container by name or id prefix, preferring ones tracked in this process.
    /// Otherwise it is opened from <paramref name="session"/>, a factory so that a cache
    /// hit never starts one.
    /// </summary>
    public static Container GetContainer(string nameOrId, Func<Session>? session = null)
    {
        if (TryGetContainer(nameOrId, out var container) && container is not null)
        {
            return container;
        }

        var matches = Containers.Values
            .Where(c => c.Id.StartsWith(nameOrId, StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .ToArray();

        if (matches.Length > 1)
        {
            throw new InvalidOperationException($"WSLC container id prefix '{nameOrId}' is ambiguous.");
        }

        if (matches.Length == 1)
        {
            return matches[0];
        }

        if (session is null)
        {
            throw new InvalidOperationException($"WSLC container '{nameOrId}' was not found in the current PowerShell process.");
        }

        return OpenContainer(session(), nameOrId);
    }

    /// <summary>
    /// Opens a container that exists in the session but is not tracked here, and tracks it.
    /// Event output mode is what Start-Container -Attach and Wait-Container need.
    /// </summary>
    public static Container OpenContainer(Session session, string nameOrId)
    {
        var sessionName = GetSessionName(session);

        Container container;
        try
        {
            container = session.OpenContainer(nameOrId, ProcessOutputMode.Event);
        }
        catch (Exception e) when (e.HResult == (int)Error.ContainerNotFound)
        {
            throw new InvalidOperationException(
                $"WSLC container '{nameOrId}' was not found in session '{sessionName}'.", e);
        }
        catch (Exception e) when (e.HResult == (int)Error.ContainerPrefixAmbiguous)
        {
            throw new InvalidOperationException(
                $"WSLC container id prefix '{nameOrId}' is ambiguous in session '{sessionName}'.", e);
        }

        // Prefer the recorded name so a container opened by id keeps displaying it;
        // otherwise key by id unless the caller typed a name.
        var key = ContainerIndex.FindName(sessionName, container.Id)
            ?? (container.Id.StartsWith(nameOrId, StringComparison.OrdinalIgnoreCase)
                ? container.Id
                : nameOrId);

        return RegisterContainer(sessionName, key, container);
    }

    public static string GetContainerName(Container container)
    {
        foreach (var pair in Containers)
        {
            if (pair.Value.Equals(container))
            {
                return pair.Key;
            }
        }

        return container.Id;
    }

    public static bool RemoveContainer(string name, out Container? container)
    {
        if (ContainerSessions.TryRemove(name, out var sessionName))
        {
            ContainerIndex.Remove(sessionName, name);
        }

        return Containers.TryRemove(name, out container);
    }

    public static void RemoveContainer(Container container)
    {
        foreach (var pair in Containers.Where(p => p.Value.Equals(container)).ToArray())
        {
            RemoveContainer(pair.Key, out _);
        }
    }

    /// <summary>
    /// Re-opens the containers recorded on disk, since the SDK cannot enumerate a session's.
    /// Restores <paramref name="sessionName"/>, or every recorded session when it is null.
    /// Containers the service no longer knows are dropped; other failures go to
    /// <paramref name="warn"/>.
    /// </summary>
    public static void RestoreContainers(string? sessionName = null, Action<string>? warn = null)
    {
        ContainerIndex.SessionRecord[] records;
        if (sessionName is null)
        {
            records = ContainerIndex.ReadAll();
        }
        else
        {
            var record = ContainerIndex.Read(sessionName);
            records = record is null ? [] : [record];
        }

        foreach (var record in records)
        {
            RestoreContainers(record, warn);
        }
    }

    private static void RestoreContainers(ContainerIndex.SessionRecord record, Action<string>? warn)
    {
        var missing = new List<string>();
        Session? session = null;

        foreach (var entry in record.Containers)
        {
            if (Containers.ContainsKey(entry.Name))
            {
                continue;
            }

            // Opened lazily, so a session with nothing to restore is never started.
            if (session is null)
            {
                try
                {
                    session = GetOrCreateSession(
                        record.SessionName,
                        record.StoragePath is null ? null : new SessionSettings(record.SessionName, record.StoragePath));
                }
                catch (Exception e)
                {
                    warn?.Invoke($"WSLC session '{record.SessionName}' could not be opened: {e.Message}");
                    return;
                }
            }

            try
            {
                TrackContainer(record.SessionName, entry.Name, session.OpenContainer(entry.Id, ProcessOutputMode.Event));
            }
            catch (Exception e) when (e.HResult == (int)Error.ContainerNotFound)
            {
                // Deleted from outside this module (or by an auto-removing run).
                missing.Add(entry.Name);
            }
            catch (Exception e)
            {
                warn?.Invoke($"WSLC container '{entry.Name}' could not be opened in session '{record.SessionName}': {e.Message}");
            }
        }

        ContainerIndex.RemoveRange(record.SessionName, missing);
    }

    /// <summary>
    /// The container names and ids known for a session — those tracked in this process plus
    /// those only recorded on disk. Nothing is opened, so this is safe for tab completion.
    /// </summary>
    public static IEnumerable<(string Name, string Id)> GetContainerCandidates(string? sessionName = null)
    {
        var tracked = Containers
            .Where(p => sessionName is null ||
                        string.Equals(GetContainerSessionName(p.Key), sessionName, StringComparison.OrdinalIgnoreCase))
            .Select(p => (Name: p.Key, Id: p.Value.Id));

        ContainerIndex.SessionRecord[] records;
        if (sessionName is null)
        {
            records = ContainerIndex.ReadAll();
        }
        else
        {
            var record = ContainerIndex.Read(sessionName);
            records = record is null ? [] : [record];
        }

        var recorded = records.SelectMany(r => r.Containers).Select(c => (c.Name, c.Id));

        return tracked.Concat(recorded).DistinctBy(c => c.Name, StringComparer.OrdinalIgnoreCase);
    }

    public static Container[] GetContainers(string? sessionName = null)
    {
        if (string.IsNullOrEmpty(sessionName))
        {
            return Containers.Values.ToArray();
        }

        return Containers
            .Where(p => string.Equals(GetContainerSessionName(p.Key), sessionName, StringComparison.OrdinalIgnoreCase))
            .Select(p => p.Value)
            .ToArray();
    }

    public static string? GetContainerSessionName(string containerName)
    {
        return ContainerSessions.TryGetValue(containerName, out var sessionName) ? sessionName : null;
    }
}
