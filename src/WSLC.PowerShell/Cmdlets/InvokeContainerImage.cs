using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsLifecycle.Invoke, "ContainerImage",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[Alias("Run-ContainerImage", "Run-Container")]
[OutputType(typeof(Container))]
public class InvokeContainerImage : CreateContainerCmdlet
{
    #region Parameters

    /// <summary>
    /// If specified, the resulting container will get deleted after it has finished
    /// running.
    /// </summary>
    [Parameter]
    public SwitchParameter RemoveAutomatically { get; set; }

    /// <summary>
    /// If specified, the resulting container object will be output after it has finished
    /// running.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    /// <summary>
    /// If specified, the container will run in detached mode without streaming its
    /// output to the console.
    /// </summary>
    [Parameter]
    public SwitchParameter Detach { get; set; }

    #endregion

    private Container? createdContainer;

    #region Overrides

    /// <summary>
    /// Creates a new container from the image, starts it, and optionally streams its
    /// output until the init process exits.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        foreach (var imageName in ParameterResolvers.GetImageNames(Image, ImageIdOrName))
        {
            if (!ShouldProcess(imageName, "Create and start a container from image"))
            {
                continue;
            }

            var settings = BuildContainerSettings(imageName);
            settings.EnableAutoRemove = RemoveAutomatically;

            var container = WslcSession.CreateContainer(settings);
            createdContainer = container;

            // Auto-removed containers vanish when they exit, so don't track them.
            if (!RemoveAutomatically)
            {
                WslcRuntime.RegisterContainer(
                    WslcRuntime.GetSessionName(WslcSession),
                    Name ?? container.Id,
                    container);
            }

            if (Detach)
            {
                container.Start();
            }
            else
            {
                await ProcessOperations.AttachAndWaitAsync(
                    container.InitProcess,
                    data => Host.UI.Write(Encoding.UTF8.GetString(data)),
                    data => Host.UI.Write(Encoding.UTF8.GetString(data)),
                    container.Start,
                    CmdletCancellationToken);
            }

            if (PassThru)
            {
                WriteObject(container);
            }
        }
    }

    protected override Task StopProcessingAsync()
    {
        createdContainer?.Stop(Signal.SIGTERM, System.TimeSpan.FromSeconds(10));
        return Task.CompletedTask;
    }

    #endregion
}
