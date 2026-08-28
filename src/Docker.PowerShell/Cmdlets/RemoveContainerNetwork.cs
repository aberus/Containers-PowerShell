using System.Management.Automation;
using System.Threading.Tasks;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Removes networks, as <c>docker network rm</c> does.
/// </summary>
[Cmdlet(VerbsCommon.Remove, "ContainerNet",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
public class RemoveContainerNet : NetworkOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// Whether or not to force the removal of the image.
    /// </summary>
    [Parameter]
    public SwitchParameter Force { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Removes each network the caller confirms.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        foreach (var id in ParameterResolvers.GetNetworkIds(Network, Id))
        {
            if (!ShouldProcess(id, "Remove network"))
            {
                continue;
            }

            await DkrClient.Networks.DeleteNetworkAsync(id);
        }
    }

    #endregion
}
