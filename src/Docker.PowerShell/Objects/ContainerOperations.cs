using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Docker.PowerShell.Cmdlets;
using Docker.PowerShell.Support;

namespace Docker.PowerShell.Objects;

/// <summary>
/// Daemon calls shared by the container cmdlets: creating containers, looking them up by
/// id or name, and running them with their streams attached.
/// </summary>
internal static class ContainerOperations
{
    /// <summary>
    /// Creates the container
    /// </summary>
    /// <param name="id">The image identifier to retrieve.</param>
    /// <param name="cmdlet">The cmdlet that is requesting the container creation.</param>
    /// <param name="dkrClient">The client to request the image from.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The single container object matching the id.</returns>
    internal static async Task<CreateContainerResponse> CreateContainerAsync(
        string id,
        CreateContainerCmdlet cmdlet,
        DockerClient dkrClient,
        CancellationToken cancellationToken = default)
    {
        var configuration = cmdlet.Configuration ?? new ContainerConfig();

        if (!string.IsNullOrEmpty(id))
        {
            configuration.Image = id;
        }

        if (cmdlet.Command != null)
        {
            configuration.Cmd = cmdlet.Command;
        }

        var hostConfiguration = cmdlet.HostConfiguration ?? new HostConfig();

        if (cmdlet.Publish?.Length > 0)
        {
            hostConfiguration.PortBindings ??= new Dictionary<string, IList<PortBinding>>();
            var portBindings = NatParser.ParsePortSpecs(cmdlet.Publish);
            hostConfiguration.PortBindings =
                hostConfiguration.PortBindings.Concat(portBindings).ToDictionary(x => x.Key, x => x.Value);
        }

        if (string.IsNullOrEmpty(hostConfiguration.Isolation))
        {
            hostConfiguration.Isolation = cmdlet.Isolation.ToString();
        }

        configuration.Tty = cmdlet.Terminal;
        configuration.OpenStdin = cmdlet.Input;
        configuration.AttachStdin = cmdlet.Input;
        configuration.AttachStdout = true;
        configuration.AttachStderr = true;

        var createParameters = new CreateContainerParameters(configuration)
        {
            Name = cmdlet.Name,
            HostConfig = hostConfiguration
        };

        try
        {
            return await dkrClient.Containers.CreateContainerAsync(createParameters, cancellationToken);
        }
        catch (DockerImageNotFoundException) when (!string.IsNullOrEmpty(configuration.Image))
        {
            // The engine's create endpoint never pulls a missing image. Mirror
            // `docker create`/`docker run` by pulling it, then retrying the create once.
            await PullImageForCreateAsync(configuration.Image, cmdlet, dkrClient, cancellationToken);
            return await dkrClient.Containers.CreateContainerAsync(createParameters, cancellationToken);
        }
    }

    /// <summary>
    /// Pulls an image that a container create depends on, reporting progress through the cmdlet.
    /// </summary>
    /// <param name="image">The image to pull.</param>
    /// <param name="cmdlet">The cmdlet that is requesting the image pull.</param>
    /// <param name="dkrClient">The client to request the image from.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    private static async Task PullImageForCreateAsync(
        string image,
        PSCmdlet cmdlet,
        DockerClient dkrClient,
        CancellationToken cancellationToken)
    {
        var (fromImage, tag) = SplitImageReference(image);

        cmdlet.WriteVerbose($"Unable to find image '{image}' locally; pulling from registry.");

        var messageWriter = new JsonMessageWriter(cmdlet);
        var progress = new Progress<JSONMessage>(messageWriter.WriteJsonMessage);

        await dkrClient.Images.CreateImageAsync(
            new ImagesCreateParameters { FromImage = fromImage, Tag = tag },
            null,
            progress,
            cancellationToken);

        messageWriter.ClearProgress();
    }

    /// <summary>
    /// Splits an image reference into the repository and tag components for a pull. A colon
    /// that precedes the final path segment is treated as a registry port, not a tag, and
    /// digest references (repo@sha256:...) are passed through untouched.
    /// </summary>
    /// <param name="image">The image reference to split.</param>
    /// <returns>A tuple containing the repository and tag components.</returns>
    private static (string fromImage, string tag) SplitImageReference(string image)
    {
        if (image.Contains('@'))
        {
            return (image, null);
        }

        var lastColon = image.LastIndexOf(':');
        if (lastColon > image.LastIndexOf('/'))
        {
            return (image.Substring(0, lastColon), image.Substring(lastColon + 1));
        }

        return (image, "latest");
    }

    /// <summary>
    /// Finds the containers whose id starts with the given text.
    /// </summary>
    /// <param name="id">The container ID to search for.</param>
    /// <param name="dkrClient">The client to request the container list from.</param>
    /// <returns>A list of containers whose ID starts with the given text.</returns>
    internal static Task<IList<ContainerListResponse>> GetContainersByIdAsync(string id, DockerClient dkrClient)
    {
        return dkrClient.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true,
            Filters = new Dictionary<string, IDictionary<string, bool>>
                    {
                        {"id", new Dictionary<string, bool>
                            {
                                {id, true}
                            }
                        }
                    }
        });
    }

    /// <summary>
    /// Finds the containers carrying the given name.
    /// </summary>
    /// <param name="name">The container name to search for.</param>
    /// <param name="dkrClient">The client to request the container list from.</param>
    /// <returns>A list of containers whose name matches the given text.</returns>
    internal static Task<IList<ContainerListResponse>> GetContainersByNameAsync(string name, DockerClient dkrClient)
    {
        return dkrClient.Containers.ListContainersAsync(new ContainersListParameters
        {
            All = true,
            Filters = new Dictionary<string, IDictionary<string, bool>>
                    {
                        {"name", new Dictionary<string, bool>
                            {
                                {name, true}
                            }
                        }
                    }
        });
    }

    /// <summary>
    /// Gets a single container object from the client by id or name.
    /// </summary>
    /// <param name="id">The container identifier to retrieve.</param>
    /// <param name="dkrClient">The client to request the container from.</param>
    /// <returns>The single container object matching the id.</returns>
    internal static async Task<IList<ContainerListResponse>> GetContainersByIdOrNameAsync(string id, DockerClient dkrClient)
    {
        return (await GetContainersByNameAsync(id, dkrClient)).Where(c => c.Names.Contains($"/{id}")).Concat(await GetContainersByIdAsync(id, dkrClient)).ToList();
    }

    /// <summary>
    /// Gets a single image object from the client by id.
    /// </summary>
    /// <param name="id">The image identifier to retrieve.</param>
    /// <param name="dkrClient">The client to request the image from.</param>
    /// <returns>The single image object matching the id.</returns>
    internal static async Task<ImagesListResponse> GetImageById(string id, DockerClient dkrClient)
    {
        var shaId = id;
        if (!shaId.StartsWith("sha256:"))
        {
            shaId = "sha256:" + shaId;
        }
        // TODO - Have a better way to get the image list response given the ID.
        return (await dkrClient.Images.ListImagesAsync(new ImagesListParameters() { All = true }))
            .Single(i => i.ID.StartsWith(shaId));
    }

    /// <summary>
    /// Gets any image objects from the client by matching repository:tag.
    /// </summary>
    /// <param name="repoTag">The image repository:tag to look for.</param>
    /// <param name="dkrClient">The client to request the image from.</param>
    /// <returns>The image objects matching the repository:tag.</returns>
    internal static async Task<IList<ImagesListResponse>> GetImagesByRepoTagAsync(string repoTag, DockerClient dkrClient)
    {
        return (await dkrClient.Images.ListImagesAsync(new ImagesListParameters() { All = true }))
            .Where(i => i.RepoTags.Any(rt => repoTag.Split('/').Last().Contains(":") ? rt == repoTag : rt == (repoTag + ":latest"))).ToList();
    }

    /// <summary>
    /// Throws a ContainerProcessExitException if the given exit code is non-zero.
    /// </summary>
    /// <param name="exitCode">The process exit code.</param>
    internal static void ThrowOnProcessExitCode(long exitCode)
    {
        if (exitCode != 0)
        {
            throw new ContainerProcessExitException(exitCode);
        }
    }

    /// <summary>
    /// Starts a container, attaching to its streams first when attach parameters are given so
    /// that no early output is missed.
    /// </summary>
    /// <param name="client">The Docker client.</param>
    /// <param name="containerId">The container ID.</param>
    /// <param name="attachParams">The attach parameters.</param>
    /// <param name="isTTY">Indicates whether the container is running in TTY mode.</param>
    internal static async Task StartContainerAsync(
        DockerClient client,
        string containerId,
        ContainerAttachParameters attachParams,
        bool? isTTY,
        ContainerStartParameters startParams,
        CancellationToken token)
    {
        MultiplexedStream stream = null;
        Task streamTask = null;

        try
        {
            if (attachParams != null)
            {
                stream = await client.Containers.AttachContainerAsync(containerId, attachParams, token);
                streamTask = stream.CopyToConsoleAsync(isTTY.GetValueOrDefault(), attachParams.Stdin.GetValueOrDefault(), token);
            }

            if (!await client.Containers.StartContainerAsync(containerId, new ContainerStartParameters()))
            {
                throw new ApplicationFailedException("The container has already started.");
            }

            if (attachParams != null)
            {
                await streamTask;
            }
        }
        finally
        {
            stream?.Dispose();
        }
    }
}
