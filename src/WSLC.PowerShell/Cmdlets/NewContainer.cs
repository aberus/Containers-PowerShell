using System.Management.Automation;
using System.Threading.Tasks;
using Microsoft.WSL.Containers;
using WSLC.PowerShell.Support;

namespace WSLC.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.New, "Container",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(Container))]
public class NewContainer : CreateContainerCmdlet
{
    #region Overrides

    /// <summary>
    /// Creates a new container and lists it to output.
    /// </summary>
    protected override Task ProcessRecordAsync()
    {
        foreach (var imageName in ParameterResolvers.GetImageNames(Image, ImageIdOrName))
        {
            var settings = BuildContainerSettings(imageName);
            var container = WslcSession.CreateContainer(settings);
            WslcRuntime.RegisterContainer(
                WslcRuntime.GetSessionName(WslcSession),
                Name ?? container.Id,
                container);
            WriteObject(container);
        }

        return Task.CompletedTask;
    }

    #endregion
}
