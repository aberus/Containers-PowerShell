using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsLifecycle.Submit, "ContainerImage",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(ImageInfo))]
[Alias("Push-ContainerImage")]
public class SubmitContainerImage : WslcCmdlet
{
    #region Parameters

    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    [Alias("ImageName", "ImageId")]
    public string? ImageIdOrName { get; set; }

    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNull]
    public ImageInfo? Image { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    /// <summary>
    /// The registry authentication for the push. Defaults to the token stored by
    /// Connect-ContainerRegistry for that registry.
    /// </summary>
    [Parameter]
    [ValidateNotNullOrEmpty]
    public string? RegistryAuth { get; set; }

    #endregion

    #region Overrides
    protected override async Task ProcessRecordAsync()
    {
        var imageName = ImageIdOrName ?? Image!.Name;

        var registryAuth = RegistryAuth ?? RegistryAuthStore.Find(WslcSessionName, imageName) ?? string.Empty;

        var operation = WslcSession.PushImageAsync(new PushImageOptions(imageName, registryAuth));
        var progressId = GetNextProgressId();
        operation.Progress = CreateImageProgressHandler(progressId, $"Pushing {imageName}");
        await operation;
        CompleteProgress(progressId, $"Pushing {imageName}", "Completed");

        if (PassThru)
        {
            foreach (var image in WslcSession.GetImages())
            {
                if (ParameterResolvers.ImageMatches(image, imageName))
                {
                    WriteObject(image);
                    break;
                }
            }
        }
    }

    #endregion
}
