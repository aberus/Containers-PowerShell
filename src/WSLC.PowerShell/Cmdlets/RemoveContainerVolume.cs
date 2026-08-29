using System.Management.Automation;
using System.Threading.Tasks;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Remove, "ContainerVolume",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
public class RemoveContainerVolume : WslcCmdlet
{
    #region Parameters

    /// <summary>
    /// The names of the volumes to delete.
    /// </summary>
    [Parameter(ValueFromPipeline = true, Position = 0, Mandatory = true)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(VolumeArgumentCompleter))]
    [Alias("VolumeName")]
    public string[] Name { get; set; } = [];

    /// <summary>
    /// Answers the confirmation prompt about discarding the volume's contents.
    /// </summary>
    [Parameter]
    public SwitchParameter Force { get; set; }

    #endregion

    #region Overrides

    protected override Task ProcessRecordAsync()
    {
        foreach (var name in Name)
        {
            if (!ShouldProcessAndContinue(
                    name,
                    "Remove volume",
                    $"Removing volume \"{name}\" deletes its backing VHD and everything stored in it. Remove it?",
                    Force))
            {
                continue;
            }

            WslcSession.DeleteVhdVolume(name);
            VolumeIndex.Remove(WslcSessionName, name);
        }

        return Task.CompletedTask;
    }

    #endregion
}
