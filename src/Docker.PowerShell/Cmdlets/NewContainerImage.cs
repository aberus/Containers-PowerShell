using Docker.DotNet.Models;
using Docker.PowerShell.Objects;
using Docker.PowerShell.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Docker.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.New, "ContainerImage",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[Alias("Build-ContainerImage")]
[OutputType(typeof(ImagesListResponse))]
public class NewContainerImage : DkrCmdlet
{
    private string SuccessfullyBuilt = "Successfully built ";

    #region Parameters

    [Parameter(Position = 0)]
    [ValidateNotNullOrEmpty]
    public string Path { get; set; }

    [Parameter]
    [ValidateNotNullOrEmpty]
    public string Repository { get; set; }

    [Parameter]
    [ValidateNotNullOrEmpty]
    public string Tag { get; set; }

    [Parameter]
    public SwitchParameter SkipCache { get; set; }

    [Parameter]
    public SwitchParameter ForceRemoveIntermediateContainers { get; set; }

    [Parameter]
    public SwitchParameter PreserveIntermediateContainers { get; set; }

    [Parameter]
    public AuthConfig Authorization { get; set; }

    #endregion

    #region Overrides

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
