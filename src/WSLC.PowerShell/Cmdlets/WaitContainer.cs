using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsLifecycle.Wait, "Container",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(Container))]
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

    protected override async Task ProcessRecordAsync()
    {
        foreach (var container in ParameterResolvers.GetContainers(Container, ContainerIdOrName, () => WslcSession))
        {
            var exitCode = await ProcessOperations.WaitForExitAsync(
                container.InitProcess,
                CmdletCancellationToken);

            WriteVerbose("Exit Code: " + exitCode);
            if (exitCode != 0)
            {
                throw new ContainerProcessExitException(exitCode);
            }

            if (PassThru)
            {
                WriteObject(container);
            }
        }
    }

    #endregion
}
