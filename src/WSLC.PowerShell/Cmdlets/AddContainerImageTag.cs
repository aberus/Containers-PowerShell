using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Add, "ContainerImageTag",
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
        foreach (var imageName in ParameterResolvers.GetImageNames(Image, ImageIdOrName))
        {
            WslcSession.TagImage(new TagImageOptions(imageName, Repository, Tag ?? "latest"));
        }

        return Task.CompletedTask;
    }

    #endregion
}
