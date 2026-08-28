using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace WSLC.PowerShell.Support;

/// <summary>
/// The on-disk record of the containers this module created or opened, per session under
/// &lt;LocalAppData&gt;\WSLC.PowerShell\sessions. The SDK cannot enumerate containers, so
/// listing them in a new process means remembering which exist. Kept outside the session's
/// own storage directory, which the service requires to be empty.
/// </summary>
internal static class ContainerIndex
{
    private const string StateSubPath = @"WSLC.PowerShell\sessions";

    // The index file doubles as its own lock, so a concurrent writer waits rather than
    // losing its entry.
    private const int LockAttempts = 20;
    private const int LockRetryDelayMs = 25;

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// The containers recorded for one session, with the storage path needed to reconnect.
    /// </summary>
    internal sealed class SessionRecord
    {
        public string SessionName { get; set; } = string.Empty;

        public string? StoragePath { get; set; }

        public List<ContainerRecord> Containers { get; set; } = [];
    }

    /// <summary>
    /// One container: the name it is tracked under and the id used to open it again.
    /// </summary>
    internal sealed class ContainerRecord
    {
        public string Name { get; set; } = string.Empty;

        public string Id { get; set; } = string.Empty;
    }

    /// <summary>
    /// Records a container, replacing any entry with the same name.
    /// </summary>
    public static void Add(string sessionName, string? storagePath, string containerName, string containerId)
    {
        Update(sessionName, storagePath, record =>
        {
            record.Containers.RemoveAll(c => NameMatches(c, containerName));
            record.Containers.Add(new ContainerRecord { Name = containerName, Id = containerId });
        });
    }

    /// <summary>
    /// Forgets a container, so a deleted one is not offered again.
    /// </summary>
    public static void Remove(string sessionName, string containerName)
    {
        RemoveRange(sessionName, [containerName]);
    }

    /// <summary>
    /// Forgets several containers in a single rewrite of the session's index.
    /// </summary>
    public static void RemoveRange(string sessionName, IEnumerable<string> containerNames)
    {
        var names = containerNames.ToArray();
        if (names.Length == 0 || !File.Exists(GetIndexPath(sessionName)))
        {
            return;
        }

        Update(sessionName, null, record =>
            record.Containers.RemoveAll(c => names.Any(name => NameMatches(c, name))));
    }

    /// <summary>
    /// The containers recorded for a session, or null when the session has none.
    /// </summary>
    public static SessionRecord? Read(string sessionName)
    {
        return Read(new FileInfo(GetIndexPath(sessionName)));
    }

    /// <summary>
    /// The containers recorded for every session this module has created one in.
    /// </summary>
    public static SessionRecord[] ReadAll()
    {
        var directory = new DirectoryInfo(GetIndexDirectory());
        if (!directory.Exists)
        {
            return [];
        }

        return directory.EnumerateFiles("*.json")
            .Select(Read)
            .Where(record => record is not null)
            .Select(record => record!)
            .ToArray();
    }

    /// <summary>
    /// The name a container id is recorded under, or null — so a container opened by id
    /// keeps its name.
    /// </summary>
    public static string? FindName(string sessionName, string containerId)
    {
        return Read(sessionName)?.Containers
            .FirstOrDefault(c => string.Equals(c.Id, containerId, StringComparison.OrdinalIgnoreCase))?.Name;
    }

    private static SessionRecord? Read(FileInfo file)
    {
        for (var attempt = 1; ; attempt++)
        {
            try
            {
                using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                return Deserialize(stream);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
            catch (IOException) when (attempt < LockAttempts)
            {
                Thread.Sleep(LockRetryDelayMs);
            }
        }
    }

    private static void Update(string sessionName, string? storagePath, Action<SessionRecord> mutate)
    {
        var path = GetIndexPath(sessionName);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        for (var attempt = 1; ; attempt++)
        {
            try
            {
                using var stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

                var record = Deserialize(stream) ?? new SessionRecord();
                record.SessionName = sessionName;
                record.StoragePath = storagePath ?? record.StoragePath;
                mutate(record);

                stream.SetLength(0);
                stream.Position = 0;
                JsonSerializer.Serialize(stream, record, SerializerOptions);
                return;
            }
            catch (IOException) when (attempt < LockAttempts)
            {
                Thread.Sleep(LockRetryDelayMs);
            }
        }
    }

    private static SessionRecord? Deserialize(Stream stream)
    {
        if (stream.Length == 0)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<SessionRecord>(stream);
        }
        catch (JsonException)
        {
            // A truncated or hand-edited index is rebuilt rather than failing the cmdlet.
            return null;
        }
    }

    private static bool NameMatches(ContainerRecord record, string containerName)
    {
        return string.Equals(record.Name, containerName, StringComparison.OrdinalIgnoreCase);
    }

    private static string GetIndexDirectory()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            StateSubPath);
    }

    private static string GetIndexPath(string sessionName)
    {
        var fileName = string.Join('_', sessionName.Split(Path.GetInvalidFileNameChars()));
        return Path.Combine(GetIndexDirectory(), fileName + ".json");
    }
}
