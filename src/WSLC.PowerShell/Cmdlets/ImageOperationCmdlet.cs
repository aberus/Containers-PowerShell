using System.Management.Automation;
using Microsoft.WSL.Containers;

namespace WSLC.PowerShell.Cmdlets;

public abstract class ImageOperationCmdlet : WslcCmdlet
{
    #region Parameters

    /// <summary>
    /// The names or ids of the images to operate on.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(ImageArgumentCompleter))]
    [Alias("ImageName", "ImageId")]
    public string[]? ImageIdOrName { get; set; }

    /// <summary>
    /// The images to operate on.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public ImageInfo[]? Image { get; set; }

    #endregion
}
