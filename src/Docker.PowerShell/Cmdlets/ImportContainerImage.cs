using System;
using System.Threading.Tasks;
using System.Management.Automation;
using Docker.DotNet.Models;
using System.IO;
using System.Linq;
using Docker.PowerShell.Objects;

namespace Docker.PowerShell.Cmdlets;

[Cmdlet(VerbsData.Import, "ContainerImage",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[Alias("Load-ContainerImage")]
[OutputType(typeof(ImagesListResponse))]
public class ImportContainerImage : DkrCmdlet
{
    private const string LoadedImage = "Loaded image: ";

    #region Parameters

    [Parameter(Position = 0,
        Mandatory = true)]
    [ValidateNotNullOrEmpty]
    public string[] FilePath { get; set; }

    #endregion

    #region Overrides

    protected override async Task ProcessRecordAsync()
    {
        foreach (var item in FilePath)
        {
            var filePath = Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, item);

            string imageId = null;
            bool failed = false;

            var messageWriter = new JsonMessageWriter(this);
            var progress = new Progress<JSONMessage>((message) =>
            {
                if (message.Stream?.StartsWith(LoadedImage) == true)
                {
                    // This is probably the image ID.
                    imageId = message.Stream.Substring(LoadedImage.Length).Trim();
                }

                if (message.Error != null)
                {
                    failed = true;
                }

                messageWriter.WriteJsonMessage(message);
            });

            using var file = File.OpenRead(filePath);
            await DkrClient.Images.LoadImageAsync(new ImageLoadParameters { Quiet = false }, file, progress, CmdletCancellationToken);

            messageWriter.ClearProgress();
            if (imageId != null)
            {
                WriteObject((await ContainerOperations.GetImagesByRepoTag(imageId, DkrClient)).Single());
            }
            else if (!failed)
            {
                throw new Exception("Could not find image, but no error was returned");
            }
        }
    }

    #endregion
}