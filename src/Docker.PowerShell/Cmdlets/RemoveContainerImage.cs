using System.Management.Automation;
using Docker.DotNet.Models;
using System.Threading.Tasks;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Removes images, as <c>docker rmi</c> does.
/// </summary>
[Cmdlet(VerbsCommon.Remove, "ContainerImage",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
public class RemoveContainerImage : ImageOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// Whether or not to force the removal of the image.
    /// </summary>
    [Parameter]
    public SwitchParameter Force { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Removes each image the caller confirms.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        foreach (var id in ParameterResolvers.GetImageIds(Image, ImageIdOrName))
        {
            if (!ShouldProcess(id, "Remove image"))
            {
                continue;
            }

            await DkrClient.Images.DeleteImageAsync(id,
                new ImageDeleteParameters() { Force = Force.ToBool() }
                );
        }
    }

    #endregion
}
