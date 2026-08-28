using System.Threading.Tasks;
using System.Management.Automation;
using System.IO;

namespace Docker.PowerShell.Cmdlets;

[Cmdlet(VerbsData.Export, "ContainerImage",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[Alias("Save-ContainerImage")]
public class ExportContainerImage : ImageOperationCmdlet
{
    #region Parameters

    [Parameter(Position = 1,
        Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string DestinationFilePath { get; set; }

    #endregion

    #region Overrides

    protected override async Task ProcessRecordAsync()
    {
        var filePath = Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, DestinationFilePath);

        using (var fs = File.Create(filePath))
        using (var stream = await DkrClient.Images.SaveImagesAsync([.. ParameterResolvers.GetImageIds(Image, ImageIdOrName)], CmdletCancellationToken))
        using (CmdletCancellationToken.Register(stream.Dispose))
        {
            await stream.CopyToAsync(fs);
        }
    }

    #endregion
}