using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Docker.PowerShell.Objects;

namespace Docker.PowerShell.Cmdlets;

[Cmdlet(VerbsLifecycle.Invoke, "ContainerImage",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[Alias("Run-ContainerImage", "Run-Container")]
[OutputType(typeof(ContainerListResponse))]
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
    /// If specified, the container will run in detached mode without connecting to input/output pipes.
    /// </summary>
    [Parameter]
    public SwitchParameter Detach { get; set; }

    #endregion

    private string createdId;

    #region Overrides

    /// <summary>
    /// Creates a new container and lists it to output.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        ThrowIfContainerWasPiped();

        foreach (var id in ParameterResolvers.GetImageIds(Image, ImageIdOrName))
        {
            var createResult = await ContainerOperations.CreateContainerAsync(
                id,
                MemberwiseClone() as CreateContainerCmdlet,
                DkrClient,
                CmdletCancellationToken);

            if (createResult.Warnings != null)
            {
                foreach (var w in createResult.Warnings)
                {
                    if (!string.IsNullOrEmpty(w))
                    {
                        WriteWarning(w);
                    }
                }
            }

            if (!string.IsNullOrEmpty(createResult.ID))
            {

                createdId = createResult.ID;

                ContainerAttachParameters attachParams = null;
                if (!Detach)
                {
                    attachParams = new ContainerAttachParameters
                    {
                        Stdin = Input,
                        Stdout = true,
                        Stderr = true,
                        Stream = true
                    };
                }

                await ContainerOperations.StartContainerAsync(
                    DkrClient,
                    createResult.ID,
                    attachParams,
                    Terminal,
                    null,
                    CmdletCancellationToken);

                if (RemoveAutomatically && !Detach)
                {
                    await DkrClient.Containers.RemoveContainerAsync(createResult.ID,
                        new ContainerRemoveParameters());
                }
                else if (PassThru)
                {
                    WriteObject((await ContainerOperations.GetContainersByIdAsync(createResult.ID, DkrClient)).Single());
                }
            }
        }
    }

    protected override async Task StopProcessingAsync()
    {
        if (!string.IsNullOrEmpty(createdId))
        {

            await DkrClient.Containers.StopContainerAsync(
                createdId,
                new ContainerStopParameters(),
                CmdletCancellationToken);
        }
    }

    #endregion
}
