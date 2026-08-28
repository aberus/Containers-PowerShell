using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsLifecycle.Start, "ContainerProcess",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(Process))]
[Alias("Exec-Container")]
public class StartContainerProcess : SingleContainerOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// The command to run in the container.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromRemainingArguments = true,
        Position = 1,
        Mandatory = true)]
    [Parameter(ParameterSetName = CommonParameterSetNames.ContainerObject,
        ValueFromRemainingArguments = true,
        Position = 1,
        Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string[] Command { get; set; } = [];

    /// <summary>
    /// Whether or not to start the process in detached mode. The process object is
    /// output instead of streaming its output to the console.
    /// </summary>
    [Parameter]
    public SwitchParameter Detached { get; set; }

    /// <summary>
    /// The working directory for the process.
    /// </summary>
    [Parameter]
    [ValidateNotNullOrEmpty]
    public string? WorkingDirectory { get; set; }

    /// <summary>
    /// Environment variables for the process, in "NAME=VALUE" form.
    /// </summary>
    [Parameter]
    [ValidateNotNullOrEmpty]
    public string[]? Environment { get; set; }

    #endregion

    #region Overrides

    protected override async Task ProcessRecordAsync()
    {
        var container = ParameterResolvers.GetContainer(Container, ContainerIdOrName, () => WslcSession);

        var settings = new ProcessSettings
        {
            CommandLine = Command,
            OutputMode = Detached ? ProcessOutputMode.Discard : ProcessOutputMode.Event
        };

        if (WorkingDirectory is not null)
        {
            settings.WorkingDirectory = WorkingDirectory;
        }

        if (Environment is { Length: > 0 })
        {
            var variables = new Dictionary<string, string>();
            foreach (var entry in Environment)
            {
                var separator = entry.IndexOf('=');
                if (separator <= 0)
                {
                    throw new System.ArgumentException($"Invalid environment variable '{entry}'. Expected NAME=VALUE.");
                }

                variables[entry[..separator]] = entry[(separator + 1)..];
            }

            settings.EnvironmentVariables = variables;
        }

        var process = container.CreateProcess(settings);

        if (Detached)
        {
            process.Start();
            WriteObject(process);
        }
        else
        {
            var exitCode = await ProcessOperations.AttachAndWaitAsync(
                process,
                data => Host.UI.Write(Encoding.UTF8.GetString(data)),
                data => Host.UI.Write(Encoding.UTF8.GetString(data)),
                process.Start,
                CmdletCancellationToken);

            WriteVerbose("Exit Code: " + exitCode);
        }
    }

    #endregion
}
