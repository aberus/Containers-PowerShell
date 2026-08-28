using System.Management.Automation;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Docker.PowerShell.Objects;

namespace Docker.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Get, "ContainerVolume",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(VolumeResponse))]
public class GetContainerVolume : DkrCmdlet
{
    #region Parameters

    /// <summary>
    /// The names of the volumes to get. If not specified, every volume is returned.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
        Position = 0)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(VolumeArgumentCompleter))]
    public string[] Name { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Outputs volume objects for each volume matching the provided parameters.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        if (Name != null)
        {
            foreach (var name in Name)
            {
                WriteObject(await VolumeOperations.GetVolumesByName(name, DkrClient, CmdletCancellationToken), true);
            }
        }
        else
        {
            var response = await DkrClient.Volumes.ListAsync(new VolumesListParameters(), CmdletCancellationToken);

            // The daemon reports drivers it could not reach here rather than failing the call.
            foreach (var warning in response.Warnings ?? [])
            {
                WriteWarning(warning);
            }

            foreach (var volume in response.Volumes ?? [])
            {
                WriteObject(volume);
            }
        }
    }

    #endregion
}
