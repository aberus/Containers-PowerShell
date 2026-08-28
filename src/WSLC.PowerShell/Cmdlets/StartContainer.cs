using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsLifecycle.Start, "Container",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(Container))]
public class StartContainer : MultiContainerOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// If specified, the resulting output from STDOUT and STDERR will be written to the
    /// console and the cmdlet waits until the container's init process exits.
    /// </summary>
    [Parameter]
    public SwitchParameter Attach { get; set; }

    /// <summary>
    /// If specified, the resulting container object will be output after it has finished
    /// starting.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    #endregion

    #region Overrides

    protected override async Task ProcessRecordAsync()
    {
        foreach (var container in ParameterResolvers.GetContainers(Container, ContainerIdOrName, () => WslcSession))
        {
            if (Attach)
            {
                await ProcessOperations.AttachAndWaitAsync(
                    container.InitProcess,
                    data => Host.UI.Write(Encoding.UTF8.GetString(data)),
                    data => Host.UI.Write(Encoding.UTF8.GetString(data)),
                    container.Start,
                    CmdletCancellationToken);
            }
            else
            {
                container.Start();
            }

            if (PassThru)
            {
                WriteObject(container);
            }
        }
    }

    #endregion
}
