using System.IO;
using System.Management.Automation;
using System.Threading.Tasks;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsData.Import, "ContainerImage",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[Alias("Load-ContainerImage")]
public class ImportContainerImage : WslcCmdlet
{
    #region Parameters

    [Parameter(Position = 0,
        Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string[] FilePath { get; set; } = [];

    /// <summary>
    /// Import the files as a root filesystem image (like `wslc image import`) instead
    /// of loading a saved image archive (like `wslc image load`).
    /// </summary>
    [Parameter]
    public SwitchParameter RootFilesystem { get; set; }

    /// <summary>
    /// The name to give an image imported with -RootFilesystem.
    /// </summary>
    [Parameter]
    [ValidateNotNullOrEmpty]
    public string? ImageName { get; set; }

    #endregion

    #region Overrides

    protected override async Task ProcessRecordAsync()
    {
        foreach (var item in FilePath)
        {
            var filePath = Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, item);

            if (!ShouldProcess(filePath, "Import the image saved in this file"))
            {
                continue;
            }

            var operation = RootFilesystem
                ? WslcSession.ImportImageAsync(filePath, ImageName ?? string.Empty)
                : WslcSession.LoadImageAsync(filePath);

            var activity = RootFilesystem ? $"Importing {item}" : $"Loading {item}";
            var progressId = GetNextProgressId();
            operation.Progress = CreateImageProgressHandler(progressId, activity);
            await operation;
            CompleteProgress(progressId, activity, "Completed");
        }
    }

    #endregion
}
