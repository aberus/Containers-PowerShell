using System.Management.Automation;
using Docker.PowerShell.Objects;
using Docker.DotNet.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Docker.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.New, "Container",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(ContainerListResponse))]
public class NewContainer : CreateContainerCmdlet
{
    #region Overrides

    /// <summary>
    /// Creates a new container and lists it to output.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        ThrowIfContainerWasPiped();

        foreach (var id in ParameterResolvers.GetImageIds(Image, ImageIdOrName))
        {
            if (!ShouldProcess(id, "Create a container from image"))
            {
                continue;
            }

            var createResult = await ContainerOperations.CreateContainerAsync(
                id,
                MemberwiseClone() as CreateContainerCmdlet,
                DkrClient,
                CmdletCancellationToken);

            if (createResult.Warnings != null)
            {
                foreach (var w in createResult.Warnings)
                {
                    if (!string.IsNullOrEmpty(w))
                    {
                        WriteWarning(w);
                    }
                }
            }

            if (!string.IsNullOrEmpty(createResult.ID))
            {
                WriteObject((await ContainerOperations.GetContainersByIdAsync(createResult.ID, DkrClient)).Single());
            }
        }
    }

    #endregion
}
