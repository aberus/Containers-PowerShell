using System.Management.Automation;
using Docker.DotNet.Models;
using System.Threading.Tasks;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Adds a repository and tag to an existing image, as <c>docker tag</c> does.
/// </summary>
[Cmdlet(VerbsCommon.Add, "ContainerImageTag",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[Alias("Tag-ContainerImage")]
public class AddContainerImageTag : ImageOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// The repository name to tag the image with.
    /// </summary>
    [Parameter(Position = 1,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string Repository { get; set; }

    /// <summary>
    /// The tag to apply. The daemon chooses when this is omitted.
    /// </summary>
    [Parameter(Position = 2)]
    [ValidateNotNullOrEmpty]
    public string Tag { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Tags each of the given images.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        var repoTag = string.IsNullOrEmpty(Tag) ? Repository : $"{Repository}:{Tag}";

        foreach (var id in ParameterResolvers.GetImageIds(Image, ImageIdOrName))
        {
            if (!ShouldProcess(id, $"Add the tag {repoTag}"))
            {
                continue;
            }

            var tagParams = new ImageTagParameters() { RepositoryName = Repository };

            if (!string.IsNullOrEmpty(Tag))
            {
                tagParams.Tag = Tag;
            }

            await DkrClient.Images.TagImageAsync(id, tagParams);
        }
    }

    #endregion
}
