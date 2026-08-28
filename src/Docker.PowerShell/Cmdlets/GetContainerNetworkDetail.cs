using System.Management.Automation;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Gets the daemon's full inspection record for a network.
/// </summary>
[Cmdlet(VerbsCommon.Get, "ContainerNetDetail")]
[OutputType(typeof(NetworkResponse))]
public class GetContainerNetDetail : NetworkOperationCmdlet
{
    /// <summary>
    /// Writes the inspection record for each network.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        foreach (var id in ParameterResolvers.GetNetworkIds(Network, Id))
        {
            var n = await DkrClient.Networks.InspectNetworkAsync(id);
            WriteObject(n);
        }
    }
}