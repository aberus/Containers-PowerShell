using System.Management.Automation;
using Docker.DotNet.Models;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Base class for cmdlets that act on one or more images, named either by string or by
/// object.
/// </summary>
public abstract class ImageOperationCmdlet : DkrCmdlet
{
    #region Parameters

    /// <summary>
    /// The names or ids of the images to act on.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    [ArgumentCompleter(typeof(ImageArgumentCompleter))]
    [Alias("ImageName", "ImageId")]
    public string[] ImageIdOrName { get; set; }

    /// <summary>
    /// The image objects to act on.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject,
        ValueFromPipeline = true,
               Position = 0,
               Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public ImagesListResponse[] Image { get; set; }

    #endregion
}
