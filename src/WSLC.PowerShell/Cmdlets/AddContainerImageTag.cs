using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

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
    public string Repository { get; set; } = string.Empty;

    [Parameter(Position = 2)]
    [ValidateNotNullOrEmpty]
    public string? Tag { get; set; }

    #endregion

    #region Overrides

    protected override Task ProcessRecordAsync()
    {
        var repoTag = Tag is null ? Repository : $"{Repository}:{Tag}";

        foreach (var imageName in ParameterResolvers.GetImageNames(Image, ImageIdOrName))
        {
            if (!ShouldProcess(imageName, $"Add the tag {repoTag}"))
            {
                continue;
            }

            WslcSession.TagImage(new TagImageOptions(imageName, Repository, Tag ?? "latest"));
        }

        return Task.CompletedTask;
    }

    #endregion
}
