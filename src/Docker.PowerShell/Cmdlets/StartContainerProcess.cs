using System.Management.Automation;
using Docker.DotNet.Models;
using System.Threading.Tasks;
using Docker.PowerShell.Support;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Runs a command inside a running container, as <c>docker exec</c> does. Aliased as
/// Exec-Container.
/// </summary>
[Cmdlet(VerbsLifecycle.Start, "ContainerProcess",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(ContainerListResponse))]
[Alias("Exec-Container")]
public class StartContainerProcess : SingleContainerOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// The command to use by default when starting new container.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromRemainingArguments = true,
        Position = 1)]
    [Parameter(ParameterSetName = CommonParameterSetNames.ContainerObject,
        ValueFromRemainingArguments = true,
        Position = 1)]
    [ValidateNotNullOrEmpty]
    public string[] Command { get; set; }

    /// <summary>
    /// Whether or not to start the process in detached mode.
    /// </summary>
    [Parameter]
    public SwitchParameter Detached { get; set; }

    /// <summary>
    /// Whether or not to use stdin of the started process.
    /// </summary>
    [Parameter]
    public SwitchParameter Input { get; set; }

    /// <summary>
    /// Whether or not to use terminal emulation.
    /// </summary>
    [Parameter]
    public SwitchParameter Terminal { get; set; }

    /// <summary>
    /// Whether or not to start the process in privileged mode.
    /// </summary>
    [Parameter]
    public SwitchParameter Privileged { get; set; }

    /// <summary>
    /// The user context under which the process should be started.
    /// </summary>
    [Parameter]
    public string User { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Creates the process in the container and runs it, attached unless -Detached was given.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        var id = ContainerIdOrName ?? Container.ID;

        var commandLine = Command == null ? "the image's default command" : string.Join(" ", Command);

        if (!ShouldProcess(id, string.Format("Run \"{0}\" inside the container", commandLine)))
        {
            return;
        }

        var execCreate = new ContainerExecCreateParameters
        {
            Cmd = Command,
            Privileged = Privileged,
            User = User,
            AttachStdin = !Detached && Input,
            AttachStdout = !Detached,
            AttachStderr = !Detached,
            TTY = Terminal,
        };

        var procCreateResponse = await DkrClient.Exec.CreateContainerExecAsync(id, execCreate);

        if (Detached)
        {
            await DkrClient.Exec.StartContainerExecAsync(procCreateResponse.ID, new(), CmdletCancellationToken);
            WriteObject(await DkrClient.Exec.InspectContainerExecAsync(procCreateResponse.ID, CmdletCancellationToken));
        }
        else
        {
            var execStart = new ContainerExecStartParameters
            {
                Detach = Detached,
                TTY = Terminal,
            };


            using var stream = await DkrClient.Exec.StartContainerExecAsync(procCreateResponse.ID, execStart, CmdletCancellationToken);
            await stream.CopyToConsoleAsync(Terminal, Input, CmdletCancellationToken);
        }
    }

    #endregion
}
