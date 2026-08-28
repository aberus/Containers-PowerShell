using System.Management.Automation;
using Docker.PowerShell.Objects;
using Docker.DotNet.Models;
using System.Threading.Tasks;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Commits a container's current state to a new image, as <c>docker commit</c> does.
/// Aliased as Commit-Container.
/// </summary>
[Cmdlet(VerbsData.ConvertTo, "ContainerImage",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[Alias("Commit-Container")]
[OutputType(typeof(ImagesListResponse))]
public class ConvertToContainerImage : MultiContainerOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// The repository name for the created image.
    /// </summary>
    [Parameter]
    public string Repository { get; set; }

    /// <summary>
    /// The tag name for the created image.
    /// </summary>
    [Parameter]
    public string Tag { get; set; }

    /// <summary>
    /// A message to be associated with the created image.
    /// </summary>
    [Parameter]
    public string Message { get; set; }

    /// <summary>
    /// The author of the created image.
    /// </summary>
    [Parameter]
    public string Author { get; set; }

    /// <summary>
    /// The advanced configuration to be used for the image.
    /// </summary>
    [Parameter]
    public ContainerConfig Configuration { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Commits each container and writes the image it produced.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        foreach (var (id, description) in ParameterResolvers.GetContainerTargets(Container, ContainerIdOrName))
        {
            if (!ShouldProcess(description, "Commit the container's changes to a new image"))
            {
                continue;
            }

            var commitResult = await DkrClient.Images.CommitContainerChangesAsync(
                new CommitContainerChangesParameters(Configuration)
                {
                    ContainerID = id,
                    RepositoryName = Repository,
                    Tag = Tag,
                    Comment = Message,
                    Author = Author,
                });

            WriteObject(await ContainerOperations.GetImageById(commitResult.ID, DkrClient));
        }
    }

    #endregion
}
