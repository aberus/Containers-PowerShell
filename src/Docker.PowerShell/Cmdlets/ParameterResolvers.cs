using System.Collections.Generic;
using System.Linq;
using Docker.DotNet.Models;

namespace Docker.PowerShell.Cmdlets;

internal static class ParameterResolvers
{
    /// <summary>
    /// Uses either the list of IDs, or gets the list of IDs from the list of Images.
    /// </summary>
    /// <param name="images">The list of image objects to get values from.</param>
    /// <param name="ids">The list of ids.</param>
    /// <returns>List of IDs to process.</returns>
    internal static IEnumerable<string> GetImageIds(ImagesListResponse[] images, string[] ids)
    {
        if (ids != null && ids.Length != 0)
        {
            return ids;
        }

        return images.Select(i => i.ID);
    }

    /// <summary>
    /// Uses either the list of IDs, or gets the list of IDs from the list of containers.
    /// </summary>
    /// <param name="conatiners">The list of container objects to get values from.</param>
    /// <param name="ids">The list of ids.</param>
    /// <returns>List of IDs to process.</returns>
    internal static IEnumerable<string> GetContainerIds(ContainerListResponse[] containers, string[] ids)
    {
        if (ids != null && ids.Length != 0)
        {
            return ids;
        }

        return containers.Select(c => c.ID);
    }

    /// <summary>
    /// Pairs each container id with the text that names it in -WhatIf output and in
    /// confirmation prompts. Objects that arrived on the pipeline already carry their names,
    /// and a caller who typed a name gave us the readable form themselves, so phrasing a
    /// prompt never costs a round trip to the daemon.
    /// </summary>
    /// <param name="containers">The list of container objects to get values from.</param>
    /// <param name="ids">The list of ids.</param>
    /// <returns>List of IDs to process, each with a description of the container.</returns>
    internal static IEnumerable<(string Id, string Description)> GetContainerTargets(
        ContainerListResponse[] containers,
        string[] ids)
    {
        if (ids != null && ids.Length != 0)
        {
            return ids.Select(id => (Id: id, Description: id));
        }

        return containers.Select(c => (Id: c.ID, Description: DescribeContainer(c)));
    }

    /// <summary>
    /// Names a container the way the docker CLI does, by its names and the short form of its
    /// id, so that a prompt does not read as a 64 character hash.
    /// </summary>
    /// <param name="container">The container to describe.</param>
    /// <returns>Text identifying the container to a person.</returns>
    private static string DescribeContainer(ContainerListResponse container)
    {
        var shortId = container.ID != null && container.ID.Length > 12
            ? container.ID.Substring(0, 12)
            : container.ID;

        var names = container.Names?
            .Select(n => n.TrimStart('/'))
            .Where(n => !string.IsNullOrEmpty(n))
            .ToList();

        return names == null || names.Count == 0
            ? shortId
            : $"{string.Join(", ", names)} ({shortId})";
    }

    /// <summary>
    /// Uses either the list of IDs, or gets the list of IDs from the list of networks.
    /// </summary>
    /// <param name="networks">The list of network objects to get values from.</param>
    /// <param name="ids">The list of ids.</param>
    /// <returns>List of IDs to process.</returns>
    internal static IEnumerable<string> GetNetworkIds(NetworkResponse[] networks, string[] ids)
    {
        if (ids != null && ids.Length != 0)
        {
            return ids;
        }

        return networks.Select(c => c.ID);
    }

    /// <summary>
    /// Uses either the list of names, or gets the list of names from the list of volumes.
    /// </summary>
    /// <param name="volumes">The list of volume objects to get values from.</param>
    /// <param name="names">The list of names.</param>
    /// <returns>List of names to process.</returns>
    internal static IEnumerable<string> GetVolumeNames(VolumeResponse[] volumes, string[] names)
    {
        if (names != null && names.Length != 0)
        {
            return names;
        }

        return volumes.Select(v => v.Name);
    }
}
