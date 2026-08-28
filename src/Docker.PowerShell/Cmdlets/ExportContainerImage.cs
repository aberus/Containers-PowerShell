using System.Threading.Tasks;
using System.Management.Automation;
using System.IO;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Writes images to a tar archive on disk, as <c>docker save</c> does. Aliased as
/// Save-ContainerImage.
/// </summary>
[Cmdlet(VerbsData.Export, "ContainerImage",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[Alias("Save-ContainerImage")]
public class ExportContainerImage : ImageOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// The file to write the archive to, relative to the current directory.
    /// </summary>
    [Parameter(Position = 1,
        Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string DestinationFilePath { get; set; }

    /// <summary>
    /// Replaces an existing destination file without asking first.
    /// </summary>
    [Parameter]
    public SwitchParameter Force { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Saves the given images into the destination file.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        var filePath = Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, DestinationFilePath);

        // File.Create below truncates whatever is already there, so an existing file gets the
        // extra prompt; writing a new one only needs the ordinary ShouldProcess.
        if (File.Exists(filePath))
        {
            if (!ShouldProcessAndContinue(
                    filePath,
                    "Export image",
                    $"The file \"{filePath}\" already exists and its contents will be replaced. Overwrite it?",
                    Force))
            {
                return;
            }
        }
        else if (!ShouldProcess(filePath, "Export image"))
        {
            return;
        }

        using (var fs = File.Create(filePath))
        using (var stream = await DkrClient.Images.SaveImagesAsync([.. ParameterResolvers.GetImageIds(Image, ImageIdOrName)], CmdletCancellationToken))
        using (CmdletCancellationToken.Register(stream.Dispose))
        {
            await stream.CopyToAsync(fs);
        }
    }

    #endregion
}