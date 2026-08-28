using System;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.WSL.Containers;

namespace WSLC.PowerShell.Support;

/// <summary>
/// Holds the identity tokens from Session.Authenticate so pulls and pushes can reuse them,
/// like `docker login`. Per session, process lifetime only; nothing is written to disk.
/// </summary>
internal static class RegistryAuthStore
{
    /// <summary>
    /// Docker Hub, used when no server is given.
    /// </summary>
    public const string DefaultServerAddress = "https://index.docker.io/v1/";

    private const string DockerHub = "docker.io";

    private static readonly ConcurrentDictionary<(string Session, string Registry), AuthenticateResult> Tokens = new();

    /// <summary>
    /// Stores the token for a registry, replacing any already held.
    /// </summary>
    public static void Set(string sessionName, string serverAddress, AuthenticateResult result)
    {
        Tokens[(sessionName, RegistryFromServer(serverAddress))] = result;
    }

    /// <summary>
    /// Forgets a registry's token; false when none was held.
    /// </summary>
    public static bool Remove(string sessionName, string serverAddress)
    {
        return Tokens.TryRemove((sessionName, RegistryFromServer(serverAddress)), out _);
    }

    /// <summary>
    /// Forgets every token in a session, returning the count removed.
    /// </summary>
    public static int RemoveAll(string sessionName)
    {
        var keys = Tokens.Keys
            .Where(k => string.Equals(k.Session, sessionName, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return keys.Count(key => Tokens.TryRemove(key, out _));
    }

    /// <summary>
    /// The token stored for the registry an image reference points at, or null.
    /// </summary>
    public static string? Find(string sessionName, string imageReference)
    {
        return Tokens.TryGetValue((sessionName, RegistryFromImage(imageReference)), out var result)
            ? result.IdentityToken
            : null;
    }

    /// <summary>
    /// The absolute URI Session.Authenticate expects; a bare host name defaults to https.
    /// </summary>
    public static Uri ToServerUri(string serverAddress)
    {
        if (Uri.TryCreate(serverAddress, UriKind.Absolute, out var absolute) && absolute.Scheme is "http" or "https")
        {
            return absolute;
        }

        if (Uri.TryCreate("https://" + serverAddress, UriKind.Absolute, out var assumed))
        {
            return assumed;
        }

        throw new ArgumentException($"Invalid registry server address '{serverAddress}'.", nameof(serverAddress));
    }

    /// <summary>
    /// The key a server address is stored under: its host, plus port when given.
    /// </summary>
    private static string RegistryFromServer(string serverAddress)
    {
        var uri = ToServerUri(serverAddress);
        var host = uri.IsDefaultPort ? uri.Host : $"{uri.Host}:{uri.Port}";
        return Normalize(host);
    }

    /// <summary>
    /// The key an image reference resolves to. Like docker, a leading segment counts as the
    /// registry only when it looks like a host; anything else is a Docker Hub repository.
    /// </summary>
    private static string RegistryFromImage(string imageReference)
    {
        var separator = imageReference.IndexOf('/');
        if (separator <= 0)
        {
            return DockerHub;
        }

        var candidate = imageReference[..separator];
        var isHost = candidate.Contains('.') ||
                     candidate.Contains(':') ||
                     string.Equals(candidate, "localhost", StringComparison.OrdinalIgnoreCase);

        return isHost ? Normalize(candidate) : DockerHub;
    }

    /// <summary>
    /// Collapses the interchangeable Docker Hub host names onto one key.
    /// </summary>
    private static string Normalize(string host)
    {
        host = host.ToLowerInvariant();
        return host is "index.docker.io" or "registry-1.docker.io" or "registry.hub.docker.com" ? DockerHub : host;
    }
}
