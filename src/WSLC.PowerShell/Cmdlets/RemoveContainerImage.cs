using System.Management.Automation;
using System.Threading.Tasks;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.Remove, "ContainerImage",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
public class RemoveContainerImage : ImageOperationCmdlet
{
    #region Overrides

    protected override Task ProcessRecordAsync()
    {
        foreach (var imageName in ParameterResolvers.GetImageNames(Image, ImageIdOrName))
        {
            WslcSession.DeleteImage(imageName);
        }

        return Task.CompletedTask;
    }

    #endregion
}
