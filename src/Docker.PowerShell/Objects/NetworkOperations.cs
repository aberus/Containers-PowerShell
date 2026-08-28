using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace Docker.PowerShell.Objects;

internal static class NetworkOperations
{
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