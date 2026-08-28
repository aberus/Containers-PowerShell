using System.Management.Automation;
using System.Net;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace Docker.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Remove, "ContainerVolume",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
public class RemoveContainerVolume : DkrCmdlet
{
    #region Parameters

    /// <summary>
    /// The names of the volumes to remove.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(VolumeArgumentCompleter))]
    public string[] Name { get; set; }

    /// <summary>
    /// The volume objects to remove.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.VolumeObject,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public VolumeResponse[] Volume { get; set; }

    /// <summary>
    /// Whether or not to force the removal of the volume. As with `docker volume rm
    /// --force`, a volume that is already gone is treated as success. It also answers the
    /// confirmation prompt about losing the volume's contents.
    /// </summary>
    [Parameter]
    public SwitchParameter Force { get; set; }

    #endregion

    #region Overrides

    protected override async Task ProcessRecordAsync()
    {
        foreach (var name in ParameterResolvers.GetVolumeNames(Volume, Name))
        {
            if (!ShouldProcessAndContinue(
                    name,
                    "Remove volume",
                    $"Removing volume \"{name}\" deletes the data it holds, and nothing in this module can bring it back. Remove it?",
                    Force))
            {
                continue;
            }

            try
            {
                await DkrClient.Volumes.RemoveAsync(name, Force.ToBool(), CmdletCancellationToken);
            }
            catch (DockerApiException e) when (Force.IsPresent && e.StatusCode == HttpStatusCode.NotFound)
            {
                // Force is handed to the client above so this picks itself up once the
                // library forwards it, but Docker.DotNet 4.3.3 drops the flag before
                // building the request, so swallow the not-found the daemon would have.
                WriteVerbose($"No such volume: {name}");
            }
        }
    }

    #endregion
}
