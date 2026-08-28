using System;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsLifecycle.Stop, "Container",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(Container))]
public class StopContainer : MultiContainerOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// Whether or not to force the termination of the container (SIGKILL, no grace
    /// period) — the equivalent of `wslc container kill`.
    /// </summary>
    [Parameter]
    public SwitchParameter Force { get; set; }

    /// <summary>
    /// The signal sent to the container's init process, like `wslc container stop -s`.
    /// </summary>
    [Parameter]
    public Signal Signal { get; set; } = Signal.SIGTERM;

    /// <summary>
    /// Seconds to wait for the container to exit before it is killed, like
    /// `wslc container stop --time`.
    /// </summary>
    [Parameter]
    public int TimeoutSeconds { get; set; } = 10;

    /// <summary>
    /// If specified, the resulting container object will be output after it has finished
    /// stopping.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    #endregion

    #region Overrides

    protected override Task ProcessRecordAsync()
    {
        foreach (var container in ParameterResolvers.GetContainers(Container, ContainerIdOrName, () => WslcSession))
        {
            if (Force)
            {
                container.Stop(Signal.SIGKILL, TimeSpan.Zero);
            }
            else
            {
                container.Stop(Signal, TimeSpan.FromSeconds(TimeoutSeconds));
            }

            if (PassThru)
            {
                WriteObject(container);
            }
        }

        return Task.CompletedTask;
    }

    #endregion
}
