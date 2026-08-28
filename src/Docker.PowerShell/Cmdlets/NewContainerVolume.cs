using Docker.DotNet.Models;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Docker.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.New, "ContainerVolume",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(VolumeResponse))]
public class NewContainerVolume : DkrCmdlet
{
    #region Parameters

    /// <summary>
    /// The volume name to use. If not specified, the daemon generates one.
    /// </summary>
    [Parameter(Position = 0)]
    [ValidateNotNullOrEmpty]
    public string Name { get; set; }

    /// <summary>
    /// The name of the volume driver plugin to use. If not specified, uses the default
    /// configured on the daemon.
    /// </summary>
    [Parameter(Position = 1)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(VolumeArgumentCompleter))]
    public string Driver { get; set; }

    /// <summary>
    /// A dictionary containing driver specific volume options.
    /// </summary>
    [Parameter]
    public IDictionary<string, string> Options { get; set; }

    /// <summary>
    /// A dictionary containing labels to set on the volume.
    /// </summary>
    [Parameter]
    public IDictionary<string, string> Labels { get; set; }

    #endregion

    #region Overrides

    protected override async Task ProcessRecordAsync()
    {
        if (!ShouldProcess(Name ?? "(name chosen by the daemon)", "Create volume"))
        {
            return;
        }

        WriteObject(await DkrClient.Volumes.CreateAsync(
            new VolumesCreateParameters
            {
                Name = Name,
                Driver = Driver,
                DriverOpts = Options,
                Labels = Labels
            },
            CmdletCancellationToken));
    }

    #endregion
}
