using System.Management.Automation;
using Docker.PowerShell.Objects;
using Docker.DotNet.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Waits for containers to exit and reports a non-zero exit code as an error, as
/// <c>docker wait</c> does.
/// </summary>
[Cmdlet(VerbsLifecycle.Wait, "Container",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(ContainerListResponse))]
public class WaitContainer : MultiContainerOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// If specified, the resulting container object will be output after the operation has
    /// finished.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Waits for each container and throws when one exits with a non-zero code.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        foreach (var id in ParameterResolvers.GetContainerIds(Container, ContainerIdOrName))
        {
            var waitResponse = await DkrClient.Containers.WaitContainerAsync(
                id,
                CmdletCancellationToken);

            WriteVerbose("Status Code: " + waitResponse.StatusCode.ToString());
            ContainerOperations.ThrowOnProcessExitCode(waitResponse.StatusCode);

            if (PassThru)
            {
                WriteObject((await ContainerOperations.GetContainersByIdOrNameAsync(id, DkrClient)).Single());
            }
        }
    }

    #endregion
}
