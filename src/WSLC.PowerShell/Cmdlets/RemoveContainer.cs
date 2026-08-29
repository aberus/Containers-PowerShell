using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Remove, "Container",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
public class RemoveContainer : MultiContainerOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// Whether or not to force the removal of the container. It also answers the
    /// confirmation prompt about discarding the container's writable layer.
    /// </summary>
    [Parameter]
    public SwitchParameter Force { get; set; }

    #endregion

    #region Overrides

    protected override Task ProcessRecordAsync()
    {
        foreach (var (description, resolve) in ParameterResolvers.GetContainerTargets(Container, ContainerIdOrName, () => WslcSession))
        {
            if (!ShouldProcessAndContinue(
                    description,
                    "Remove container",
                    $"Removing container \"{description}\" discards its writable layer. Anything written inside it that was not committed to an image or kept in a volume goes with it. Remove it?",
                    Force))
            {
                continue;
            }

            var container = resolve();
            container.Delete(Force ? DeleteContainerOption.Force : DeleteContainerOption.None);
            WslcRuntime.RemoveContainer(container);
        }

        return Task.CompletedTask;
    }

    #endregion
}
