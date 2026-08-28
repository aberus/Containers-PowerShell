using System;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Docker.PowerShell.Support;
using Tar;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Copies files between the host and a container, as <c>docker cp</c> does.
/// </summary>
[Cmdlet(VerbsCommon.Copy, "ContainerFile",
        SupportsShouldProcess = true,
        DefaultParameterSetName = CommonParameterSetNames.Default)]
public class CopyContainerFile : SingleContainerOperationCmdlet
{
    /// <summary>
    /// The files to copy: paths on the host when -ToContainer is given, paths inside the
    /// container otherwise.
    /// </summary>
    [Parameter(Mandatory = true, Position = 1)]
    [ValidateNotNullOrEmpty]
    public string[] Path { get; set; }

    /// <summary>
    /// Where to put the copies. Defaults to the container's working directory, or to the
    /// current directory on the host.
    /// </summary>
    [Parameter]
    [ValidateNotNullOrEmpty]
    public string Destination { get; set; }

    /// <summary>
    /// Copies into the container instead of out of it.
    /// </summary>
    [Parameter]
    public SwitchParameter ToContainer { get; set; }

    /// <summary>
    /// Copies the files in whichever direction was asked for.
    /// </summary>
    protected override async Task ProcessRecordAsync()
    {
        if (Container != null)
        {
            ContainerIdOrName = Container.ID;
        }

        if (ToContainer)
        {
            var hostPaths = Path.SelectMany(path =>
            {
                ProviderInfo provider;
                var resolvedPaths = SessionState.Path.GetResolvedProviderPathFromPSPath(path, out provider);
                if (provider.Name != "FileSystem")
                {
                    throw new Exception(string.Format("The path {0} is not in the file system.", path));
                }

                return resolvedPaths;
            }).ToList();

            var p = new CopyToContainerParameters
            {
                Path = Destination ?? "."
            };

            if (!ShouldProcess(ContainerIdOrName, string.Format("Copy {0} into the container", string.Join(", ", hostPaths))))
            {
                return;
            }

            var progress = new Progress<string>();
            progress.ProgressChanged += (o, s) => WriteVerbose(string.Format("Sending {0}", s));

            using (var reader = Archiver.CreateTarStream(hostPaths, CmdletCancellationToken, progress))
            {
                await DkrClient.Containers.ExtractArchiveToContainerAsync(ContainerIdOrName, p, reader, CmdletCancellationToken);
            }
        }
        else
        {
            var hostPath = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Destination ?? "");
            foreach (var singlePath in Path)
            {
                if (!ShouldProcess(hostPath, string.Format("Copy {0} out of container {1}", singlePath, ContainerIdOrName)))
                {
                    continue;
                }

                var p = new ContainerPathStatParameters
                {
                    Path = singlePath
                };

                var response = await DkrClient.Containers.GetArchiveFromContainerAsync(ContainerIdOrName, p, false, CmdletCancellationToken);
                using (var stream = response.Stream)
                {
                    var progress = new Progress<string>();
                    progress.ProgressChanged += (_, s) => WriteVerbose(string.Format("Extracting {0}", s));

                    var tarReader = new TarReader(stream);
                    await tarReader.ExtractDirectoryAsync(hostPath, CmdletCancellationToken, progress);
                }
            }
        }
    }
}