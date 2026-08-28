using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Remove, "Container",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
public class RemoveContainer : MultiContainerOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// Whether or not to force the removal of the container.
    /// </summary>
    [Parameter]
    public SwitchParameter Force { get; set; }

    #endregion

    #region Overrides

    protected override Task ProcessRecordAsync()
    {
        foreach (var container in ParameterResolvers.GetContainers(Container, ContainerIdOrName, () => WslcSession))
        {
            container.Delete(Force ? DeleteContainerOption.Force : DeleteContainerOption.None);
            WslcRuntime.RemoveContainer(container);
        }

        return Task.CompletedTask;
    }

    #endregion
}
