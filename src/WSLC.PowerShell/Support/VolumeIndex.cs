using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace WSLC.PowerShell.Support;

/// <summary>
/// The volume names this module has created, per session. The SDK can create and delete a
/// VHD volume but cannot list them, so this is only what tab completion needs; it is not a
/// complete picture of the session's volumes.
/// </summary>
internal static class VolumeIndex
{
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> Volumes =
        new(StringComparer.OrdinalIgnoreCase);

    public static void Add(string sessionName, string volumeName)
    {
        Volumes.GetOrAdd(sessionName, _ => new(StringComparer.OrdinalIgnoreCase))[volumeName] = 0;
    }

    public static void Remove(string sessionName, string volumeName)
    {
        if (Volumes.TryGetValue(sessionName, out var names))
        {
            names.TryRemove(volumeName, out _);
        }
    }

    /// <summary>
    /// The volume names recorded for a session, or for every session when none is given.
    /// </summary>
    public static IEnumerable<string> Read(string? sessionName)
    {
        if (sessionName is null)
        {
            return Volumes.Values.SelectMany(names => names.Keys).Distinct(StringComparer.OrdinalIgnoreCase);
        }

        return Volumes.TryGetValue(sessionName, out var found) ? found.Keys : [];
    }
}
