using System.Management.Automation;
using Docker.DotNet.Models;
using System.Threading.Tasks;

namespace Docker.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Add, "ContainerImageTag",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[Alias("Tag-ContainerImage")]
public class AddContainerImageTag : ImageOperationCmdlet
{
    #region Parameters

    [Parameter(Position = 1,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string Repository { get; set; }

    [Parameter(Position = 2)]
    [ValidateNotNullOrEmpty]
    public string Tag { get; set; }

    #endregion

    #region Overrides

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
