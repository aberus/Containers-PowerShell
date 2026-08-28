using Docker.DotNet.Models;
using Docker.PowerShell.Objects;
using Docker.PowerShell.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Builds an image from a Dockerfile, as <c>docker build</c> does. Aliased as
/// Build-ContainerImage.
/// </summary>
[Cmdlet(VerbsCommon.New, "ContainerImage",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[Alias("Build-ContainerImage")]
[OutputType(typeof(ImagesListResponse))]
public class NewContainerImage : DkrCmdlet
{
    private string SuccessfullyBuilt = "Successfully built ";

    #region Parameters

    /// <summary>
    /// The directory holding the Dockerfile and its build context. Defaults to the current
    /// directory.
    /// </summary>
    [Parameter(Position = 0)]
    [ValidateNotNullOrEmpty]
    public string Path { get; set; }

    /// <summary>
    /// The repository name to give the built image.
    /// </summary>
    [Parameter]
    [ValidateNotNullOrEmpty]
    public string Repository { get; set; }

    /// <summary>
    /// The tag to give the built image. Requires -Repository.
    /// </summary>
    [Parameter]
    [ValidateNotNullOrEmpty]
    public string Tag { get; set; }

    /// <summary>
    /// Builds every layer afresh instead of reusing cached ones.
    /// </summary>
    [Parameter]
    public SwitchParameter SkipCache { get; set; }

    /// <summary>
    /// Removes the intermediate containers even when the build fails.
    /// </summary>
    [Parameter]
    public SwitchParameter ForceRemoveIntermediateContainers { get; set; }

    /// <summary>
    /// Keeps the intermediate containers after a successful build.
    /// </summary>
    [Parameter]
    public SwitchParameter PreserveIntermediateContainers { get; set; }

    /// <summary>
    /// The registry credentials to authenticate with.
    /// </summary>
    [Parameter]
    public AuthConfig Authorization { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Uploads the build context, reports the daemon's progress, and writes the built image.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        var directory = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Path ?? "");

        // Ensure the path is a directory.
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException(directory);
        }

        if (!ShouldProcess(directory, "Build an image from the Dockerfile in this directory"))
        {
            return;
        }

        WriteVerbose(string.Format("Archiving the contents of {0}", directory));

        using (var reader = Archiver.CreateTarStream([directory], CmdletCancellationToken))
        {
            var parameters = new ImageBuildParameters
            {
                NoCache = SkipCache,
                ForceRemove = ForceRemoveIntermediateContainers,
                Remove = !PreserveIntermediateContainers,
            };

            string repoTag = null;
            if (!string.IsNullOrEmpty(Repository))
            {
                repoTag = Repository;
                if (!string.IsNullOrEmpty(Tag))
                {
                    repoTag += ":";
                    repoTag += Tag;
                }

                parameters.Tags = new List<string>
                {
                    repoTag
                };
            }
            else if (!string.IsNullOrEmpty(Tag))
            {
                throw new Exception("You must specify a repository name in order to specify a tag.");
            }

            string imageId = null;
            bool failed = false;

            var uploadProgress = new Progress<ProgressReader.Status>();
            var uploadProgressRecord = new ProgressRecord(0, "Dockerfile context", "Uploading");
            uploadProgress.ProgressChanged += (o, status) =>
            {
                if (status.Complete)
                {
                    uploadProgressRecord.CurrentOperation = null;
                    uploadProgressRecord.StatusDescription = "Processing";
                }
                else
                {
                    uploadProgressRecord.StatusDescription = string.Format("Uploaded {0} bytes", status.TotalBytesRead);
                }

                WriteProgress(uploadProgressRecord);
            };
            var contents = new ProgressReader(reader, uploadProgress, 512 * 1024);

            var messageWriter = new JsonMessageWriter(this);
            var progress = new Progress<JSONMessage>((message) =>
            {
                if (message.Stream?.StartsWith(SuccessfullyBuilt) == true)
                {
                    // This is probably the image ID.
                    imageId = message.Stream.Substring(SuccessfullyBuilt.Length).Trim();
                }

                if (message.Error != null)
                {
                    failed = true;
                }

                messageWriter.WriteJsonMessage(message);
            });

            await DkrClient.Images.BuildImageFromDockerfileAsync(parameters, contents, [Authorization], new Dictionary<string, string>(), progress, CmdletCancellationToken);
            // Complete the upload uploadProgress bar.
            uploadProgressRecord.RecordType = ProgressRecordType.Completed;
            WriteProgress(uploadProgressRecord);

            messageWriter.ClearProgress();
            if (imageId != null)
            {
                WriteObject(await ContainerOperations.GetImageById(imageId, DkrClient));
            }
            else if (!failed)
            {
                throw new Exception("Could not find image, but no error was returned");
            }
        }
    }

    #endregion
}
