using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace Docker.PowerShell.Objects;

internal static class VolumeOperations
{
    /// <summary>
    /// Gets the volumes matching a name. The daemon's name filter matches on substrings,
    /// so the results are narrowed down to the name that was actually asked for.
    /// </summary>
    /// <param name="name">The volume name to retrieve.</param>
    /// <param name="dkrClient">The client to request the volumes from.</param>
    /// <param name="cancellationToken">Token used to cancel the request.</param>
    /// <returns>The volumes named <paramref name="name"/>.</returns>
    internal static async Task<IList<VolumeResponse>> GetVolumesByName(
        string name,
        DotNet.DockerClient dkrClient,
        CancellationToken cancellationToken = default)
    {
        var response = await dkrClient.Volumes.ListAsync(
            new VolumesListParameters
            {
                Filters = new Dictionary<string, IDictionary<string, bool>>
                        {
                            {"name", new Dictionary<string, bool>
                                {
                                    {name, true}
                                }
                            }
                        }
            },
            cancellationToken);

        return [.. (response.Volumes ?? []).Where(volume => volume.Name == name)];
    }
}
