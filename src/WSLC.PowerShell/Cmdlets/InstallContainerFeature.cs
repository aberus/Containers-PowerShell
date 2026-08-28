using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WslcComponent = Microsoft.WSL.Containers.Component;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsLifecycle.Install, "ContainerFeature", SupportsShouldProcess = true)]
[OutputType(typeof(InstallProgress))]
public sealed class InstallContainerFeature : WslcCmdlet
{
    /// <summary>
    /// The components installable on demand. SdkNeedsUpdate reports a stale SDK rather
    /// than naming something to install, so it is left out of the default repair set.
    /// </summary>
    private static readonly WslcComponent[] InstallableComponents =
        [WslcComponent.VirtualMachinePlatform, WslcComponent.WslPackage];

    #region Parameters

    /// <summary>
    /// The components to install. Defaults to the ones WSLC reports as missing, the same
    /// list Get-ContainerComponent outputs.
    /// </summary>
    [Parameter(Position = 0, ValueFromPipeline = true)]
    [ValidateNotNullOrEmpty]
    public WslcComponent[]? Component { get; set; }

    /// <summary>
    /// Reinstalls components that are already installed. Without -Component, everything
    /// installable is repaired.
    /// </summary>
    [Parameter]
    public SwitchParameter Repair { get; set; }

    /// <summary>
    /// If specified, a summary of the install is output once it completes.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    #endregion

    #region Overrides

    protected override async Task ProcessRecordAsync()
    {
        var components = ResolveComponents();
        if (components.Count == 0)
        {
            WriteVerbose("All required WSLC components are already installed.");
            return;
        }

        var action = Repair ? "Reinstall WSLC components" : "Install WSLC components";
        if (!ShouldProcess(string.Join(", ", components), action))
        {
            return;
        }

        const string activity = "Installing WSLC dependencies";
        var options = new InstallOptions
        {
            Components = components,
            Repair = Repair
        };

        var operation = WslcService.InstallWithDependenciesAsync(options);
        var progressId = GetNextProgressId();
        operation.Progress = CreateInstallProgressHandler(progressId, activity);
        await operation;
        CompleteProgress(progressId, activity, "Completed");

        if (PassThru)
        {
            WriteObject(new
            {
                Installed = true,
                Components = components.ToArray(),
                Repair = Repair.IsPresent,
                Completed = operation.Status.ToString()
            });
        }
    }

    #endregion

    /// <summary>
    /// The requested components, otherwise the missing ones. A repair with nothing missing
    /// must name them itself, since the SDK only acts on the components it is given.
    /// </summary>
    private IReadOnlyList<WslcComponent> ResolveComponents()
    {
        if (Component is { Length: > 0 })
        {
            return Component.Distinct().ToArray();
        }

        var missing = WslcService.GetMissingComponents();
        return missing.Count == 0 && Repair ? InstallableComponents : missing;
    }
}
