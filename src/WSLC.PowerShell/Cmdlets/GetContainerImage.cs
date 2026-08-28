using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Get, "ContainerImage",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(ImageInfo))]
public class GetContainerImage : WslcCmdlet
{
    #region Parameters

    /// <summary>
    /// The specific image names or ids to get.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0)]
    [ValidateNotNullOrEmpty]
    [Alias("ImageName", "ImageId")]
    public string[]? ImageIdOrName { get; set; }

    #endregion

    #region Overrides
    /// <summary>
    /// Outputs container image objects for each image matching the provided parameters.
    /// </summary>
    protected override Task ProcessRecordAsync()
    {
        foreach (var image in WslcSession.GetImages())
        {
            if (ImageIdOrName is null || ImageIdOrName.Any(i => ParameterResolvers.ImageMatches(image, i)))
            {
                WriteObject(image);
            }
        }

        return Task.CompletedTask;
    }

    #endregion
}
