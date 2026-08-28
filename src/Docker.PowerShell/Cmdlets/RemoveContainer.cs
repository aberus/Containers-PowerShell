using System.Management.Automation;
using Docker.DotNet.Models;
using System.Threading.Tasks;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Removes containers, as <c>docker rm</c> does.
/// </summary>
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

    /// <summary>
    /// Removes each container the caller confirms.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        foreach (var (id, description) in ParameterResolvers.GetContainerTargets(Container, ContainerIdOrName))
        {
            if (!ShouldProcessAndContinue(
                    description,
                    "Remove container",
                    $"Removing container \"{description}\" discards its writable layer. Anything written inside it that was not committed to an image or kept in a volume goes with it. Remove it?",
                    Force))
            {
                continue;
            }

            await DkrClient.Containers.RemoveContainerAsync(id,
                new ContainerRemoveParameters() { Force = Force.ToBool() },
                CmdletCancellationToken
                );
        }
    }

    #endregion
}
