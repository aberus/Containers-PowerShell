using System.Management.Automation;
using Docker.DotNet.Models;
using System.Threading.Tasks;
using Docker.PowerShell.Objects;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Lists containers, or gets the ones named.
/// </summary>
[Cmdlet(VerbsCommon.Get, "Container",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(ContainerListResponse))]
public class GetContainer : DkrCmdlet
{
    /// <summary>
    /// The names or ids of the containers to get. Every container is listed when omitted.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(ContainerArgumentCompleter))]
    [Alias("Name", "Id")]
    public string[] ContainerIdOrName { get; set; }

    /// <summary>
    /// Show all containers (default shows just running)
    /// </summary>
    [Parameter]
    public SwitchParameter All { get; set; }

    #region Overrides
    /// <summary>
    /// Outputs container objects for each container matching the provided parameters.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        if (ContainerIdOrName != null)
        {
            foreach (var id in ContainerIdOrName)
            {
                WriteObject(await ContainerOperations.GetContainersByIdOrNameAsync(id, DkrClient));
            }
        }
        else
        {
            foreach (var c in await DkrClient.Containers.ListContainersAsync(
                new ContainersListParameters() { All = All }))
            {
                WriteObject(c);
            }
        }
    }

    #endregion
}
