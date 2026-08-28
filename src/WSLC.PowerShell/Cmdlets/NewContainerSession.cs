using System.IO;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.New, "ContainerSession",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(Session))]
public class NewContainerSession : WslcCmdlet
{
    #region Parameters

    /// <summary>
    /// The name of the session to create or get. Defaults to the module's default
    /// session. If a session with this name is already registered in the current
    /// process, it is returned instead of creating a new one.
    /// </summary>
    [Parameter(Position = 0)]
    [ValidateNotNullOrEmpty]
    public string Name { get; set; } = WslcRuntime.DefaultSessionName;

    /// <summary>
    /// The storage path for the session. Defaults to a per-session directory under
    /// the local application data folder.
    /// </summary>
    [Parameter(Position = 1)]
    [ValidateNotNullOrEmpty]
    public string? StoragePath { get; set; }

    [Parameter]
    public uint? CpuCount { get; set; }

    [Parameter]
    public uint? MemoryMB { get; set; }

    [Parameter]
    public SwitchParameter EnableGpu { get; set; }

    #endregion

    #region Overrides

    protected override Task ProcessRecordAsync()
    {
        // Create-or-get: an already registered session is returned as-is, so the
        // cmdlet is safe to run repeatedly (and in scripts that just need a session).
        if (WslcRuntime.TryGetSession(Name, out var existing) && existing is not null)
        {
            if (StoragePath is not null || CpuCount.HasValue || MemoryMB.HasValue || EnableGpu)
            {
                WriteWarning($"WSLC session '{Name}' already exists in this process; the requested settings were ignored.");
            }

            WriteObject(existing);
            return Task.CompletedTask;
        }

        var storagePath = StoragePath ?? WslcRuntime.GetDefaultStoragePath(Name);
        Directory.CreateDirectory(storagePath);

        var settings = new SessionSettings(Name, storagePath);

        if (CpuCount.HasValue)
        {
            settings.CpuCount = CpuCount.Value;
        }

        if (MemoryMB.HasValue)
        {
            settings.MemorySizeInMB = MemoryMB.Value;
        }

        if (EnableGpu)
        {
            settings.EnableGpu = true;
        }

        WriteObject(WslcRuntime.GetOrCreateSession(Name, settings));
        return Task.CompletedTask;
    }

    #endregion
}
