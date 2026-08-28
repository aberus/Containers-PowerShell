using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WSL.Containers;

namespace WSLC.PowerShell.Support;

internal static class ParameterResolvers
{
    /// <summary>
    /// Uses either the list of container objects, or resolves the list of names/ids
    /// against the containers tracked in the current process.
    /// </summary>
    /// <param name="containers">The list of container objects to use.</param>
    /// <param name="idsOrNames">The list of container names or ids.</param>
    /// <param name="session">
    /// Opens containers this process did not create. A factory, enumerated lazily, so that
    /// resolving objects or already-tracked names never starts a session.
    /// </param>
    /// <returns>The containers to process.</returns>
    internal static IEnumerable<Container> GetContainers(
        Container[]? containers,
        string[]? idsOrNames,
        Func<Session>? session = null)
    {
        if (idsOrNames is { Length: > 0 })
        {
            return idsOrNames.Select(idOrName => WslcRuntime.GetContainer(idOrName, session));
        }

        return containers ?? [];
    }

    /// <summary>
    /// Uses either the container object, or resolves the name/id against the containers
    /// tracked in the current process.
    /// </summary>
    /// <param name="container">The container object to use.</param>
    /// <param name="idOrName">The container name or id.</param>
    /// <param name="session">
    /// Opens a container this process did not create. A factory, so resolving a container
    /// object never starts a session.
    /// </param>
    /// <returns>The container to process.</returns>
    internal static Container GetContainer(Container? container, string? idOrName, Func<Session>? session = null)
    {
        if (!string.IsNullOrEmpty(idOrName))
        {
            return WslcRuntime.GetContainer(idOrName!, session);
        }

        return container ?? throw new InvalidOperationException("No container was specified.");
    }

    /// <summary>
    /// Uses either the list of image names/ids, or gets the names from the list of image objects.
    /// </summary>
    /// <param name="images">The list of image objects to get values from.</param>
    /// <param name="idsOrNames">The list of image names or ids.</param>
    /// <returns>The image names to process.</returns>
    internal static IEnumerable<string> GetImageNames(ImageInfo[]? images, string[]? idsOrNames)
    {
        if (idsOrNames is { Length: > 0 })
        {
            return idsOrNames;
        }

        return (images ?? []).Select(i => i.Name);
    }

    /// <summary>
    /// Determines whether an image matches a user-supplied name, name without tag, or sha256 id prefix.
    /// </summary>
    internal static bool ImageMatches(ImageInfo image, string idOrName)
    {
        if (string.Equals(image.Name, idOrName, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(image.Name, idOrName + ":latest", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var sha = GetImageId(image);
        return sha is not null && sha.StartsWith(idOrName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the sha256 id of an image as a hex string, or null if unavailable.
    /// </summary>
    internal static string? GetImageId(ImageInfo image)
    {
        try
        {
            var buffer = image.Sha256;
            if (buffer is null || buffer.Length == 0)
            {
                return null;
            }

            var bytes = new byte[buffer.Length];
            using var reader = Windows.Storage.Streams.DataReader.FromBuffer(buffer);
            reader.ReadBytes(bytes);
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
        catch
        {
            return null;
        }
    }
}
