using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Docker.PowerShell.Objects;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Starts existing containers, as <c>docker start</c> does.
/// </summary>
[Cmdlet(VerbsLifecycle.Start, "Container",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(ContainerListResponse))]
public class StartContainer : MultiContainerOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// If specified, the resulting output from STDOUT and STDERR will be written to the
    /// console.
    /// </summary>
    [Parameter]
    public SwitchParameter Attach { get; set; }

    /// <summary>
    /// If specified, the container expects to give input to STDIN.
    /// </summary>
    [Parameter]
    public SwitchParameter Input { get; set; }

    /// <summary>
    /// If specified, the resulting container object will be output after it has finished
    /// starting.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Starts each container, optionally attaching to its streams.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        foreach (var (id, description) in ParameterResolvers.GetContainerTargets(Container, ContainerIdOrName))
        {
            if (!ShouldProcess(description, "Start container"))
            {
                continue;
            }

            ContainerAttachParameters attachParams = null;
            if (Attach)
            {
                attachParams = new ContainerAttachParameters
                {
                    Stdin = Input,
                    Stdout = true,
                    Stderr = true,
                    Stream = true
                };
            }

            var cDetail = await DkrClient.Containers.InspectContainerAsync(id);

            await ContainerOperations.StartContainerAsync(
                DkrClient,
                id,
                attachParams,
                cDetail.Config.Tty,
                null,
                CmdletCancellationToken);

            if (PassThru)
            {
                WriteObject((await ContainerOperations.GetContainersByIdOrNameAsync(id, DkrClient)).Single());
            }
        }
    }

    #endregion
}
