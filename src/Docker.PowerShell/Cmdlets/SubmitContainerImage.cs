using Docker.DotNet.Models;
using Docker.PowerShell.Objects;
using System;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Pushes an image to a registry, as <c>docker push</c> does. Aliased as
/// Push-ContainerImage.
/// </summary>
[Cmdlet(VerbsLifecycle.Submit, "ContainerImage",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(ContainerListResponse))]
[Alias("Push-ContainerImage")]
public class SubmitContainerImage : DkrCmdlet
{
    /// <summary>
    /// The repository and tag to push.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(ImageArgumentCompleter))]
    [Alias("ImageName", "ImageId")]
    public string ImageIdOrName { get; set; }

    /// <summary>
    /// The image to push. It must carry exactly one repository and tag.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public ImagesListResponse Image { get; set; }

    /// <summary>
    /// Writes the pushed image once the push finishes.
    /// </summary>
    [Parameter]
    public SwitchParameter PassThru { get; set; }

    /// <summary>
    /// The registry credentials to authenticate with.
    /// </summary>
    [Parameter]
    public AuthConfig Authorization { get; set; }

    /// <summary>
    /// Pushes without asking for confirmation that the image should be published.
    /// </summary>
    [Parameter]
    public SwitchParameter Force { get; set; }

    #region Overrides
    /// <summary>
    /// Pushes the image and reports the daemon's progress.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        string repoTag = null;

        if (ImageIdOrName != null)
        {
            repoTag = ImageIdOrName;
        }
        else if (Image.RepoTags.Count != 1)
        {
            throw new Exception("Ambiguous repository and tag: Supplied image must have only one repository:tag.");
        }
        else
        {
            repoTag = Image.RepoTags[0];
        }

        if (!ShouldProcessAndContinue(
                repoTag,
                "Push image",
                $"Pushing \"{repoTag}\" publishes it to its registry, where anyone with access can pull it and this module cannot withdraw it. Push it?",
                Force))
        {
            return;
        }

        var messageWriter = new JsonMessageWriter(this);
        var progress = new Progress<JSONMessage>(messageWriter.WriteJsonMessage);

        await DkrClient.Images.PushImageAsync(repoTag, new ImagePushParameters(), Authorization, progress, CmdletCancellationToken);
        messageWriter.ClearProgress();

        if (PassThru)
        {
            WriteObject((await ContainerOperations.GetImagesByRepoTagAsync(repoTag, DkrClient)).Single());
        }
    }

    #endregion
}
