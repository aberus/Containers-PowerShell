using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace Docker.PowerShell.Objects;

/// <summary>
/// Daemon lookups shared by the network cmdlets.
/// </summary>
internal static class NetworkOperations
{
    /// <summary>
    /// Finds the networks whose id starts with the given text.
    /// </summary>
    internal static Task<IList<NetworkResponse>> GetNetworksById(string id, DotNet.DockerClient dkrClient)
    {
        return dkrClient.Networks.ListNetworksAsync(new NetworksListParameters
        {
            Filters = new Dictionary<string, IDictionary<string, bool>>
                    {
                        {"id", new Dictionary<string, bool>
                            {
                                {id, true}
                            }
                        }
                    }
        });
    }

    /// <summary>
    /// Finds the networks carrying the given name.
    /// </summary>
    internal static Task<IList<NetworkResponse>> GetNetworksByName(string name, DotNet.DockerClient dkrClient)
    {
        return dkrClient.Networks.ListNetworksAsync(new NetworksListParameters
        {
            Filters = new Dictionary<string, IDictionary<string, bool>>
                    {
                        {"name", new Dictionary<string, bool>
                            {
                                {name, true}
                            }
                        }
                    }
        });
    }
}