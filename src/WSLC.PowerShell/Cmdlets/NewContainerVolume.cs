using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.New, "ContainerVolume",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
public class NewContainerVolume : WslcCmdlet
{
    #region Parameters

    /// <summary>
    /// The name of the volume to create, as used by New-Container -Volume.
    /// </summary>
    [Parameter(ValueFromPipeline = true, Position = 0, Mandatory = true)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(VolumeArgumentCompleter))]
    [Alias("VolumeName")]
    public string[] Name { get; set; } = [];

    /// <summary>
    /// The size of the backing VHD in megabytes.
    /// </summary>
    [Parameter(Position = 1)]
    [ValidateRange(1, uint.MaxValue)]
    public ulong SizeMB { get; set; } = 1024;

    /// <summary>
    /// Whether the VHD is allocated up front rather than growing on demand.
    /// </summary>
    [Parameter]
    public SwitchParameter Fixed { get; set; }

    /// <summary>
    /// The Linux uid owning the volume's root directory. Requires -Gid.
    /// </summary>
    [Parameter]
    public uint? Uid { get; set; }

    /// <summary>
    /// The Linux gid owning the volume's root directory. Requires -Uid.
    /// </summary>
    [Parameter]
    public uint? Gid { get; set; }

    /// <summary>
    /// If specified, the created volume names are output.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    #endregion

    #region Overrides

    protected override Task ProcessRecordAsync()
    {
        if (Uid.HasValue != Gid.HasValue)
        {
            throw new PSArgumentException("-Uid and -Gid must be supplied together.");
        }

        foreach (var name in Name)
        {
            if (!ShouldProcess(name, "Create volume"))
            {
                continue;
            }

            var options = new VhdOptions(name, SizeMB * 1024 * 1024, Fixed ? VhdType.Fixed : VhdType.Dynamic);
            if (Uid.HasValue && Gid.HasValue)
            {
                options.Owner = new VhdOwner(Uid.Value, Gid.Value);
            }

            WslcSession.CreateVhdVolume(options);
            VolumeIndex.Add(WslcSessionName, name);

            if (PassThru)
            {
                WriteObject(name);
            }
        }

        return Task.CompletedTask;
    }

    #endregion
}
