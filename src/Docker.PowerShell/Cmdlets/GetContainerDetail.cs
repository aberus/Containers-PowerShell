using System.Management.Automation;
using Docker.DotNet.Models;
using System.Threading.Tasks;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Gets the daemon's full inspection record for a container, as <c>docker inspect</c> does.
/// </summary>
[Cmdlet(VerbsCommon.Get, "ContainerDetail",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(ContainerInspectResponse))]
public class GetContainerDetail : MultiContainerOperationCmdlet
{
    #region Overrides

    /// <summary>
    /// Writes the inspection record for each container.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        foreach (var id in ParameterResolvers.GetContainerIds(Container, ContainerIdOrName))
        {
            WriteObject(await DkrClient.Containers.InspectContainerAsync(id));
        }
    }

    #endregion
}
