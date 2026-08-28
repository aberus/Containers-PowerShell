using System;
using System.Collections;
using Docker.DotNet;
using Docker.DotNet.X509;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Builds Docker clients from the connection parameters the cmdlets share.
/// </summary>
public static class DockerFactory
{
    /// <summary>
    /// Creates a Docker client. Connection precedence follows the Docker CLI:
    /// <list type="number">
    /// <item>an explicit host (<paramref name="dockerHost"/> parameter or the
    /// <c>DOCKER_HOST</c> environment variable) wins over everything;</item>
    /// <item>otherwise a named docker context (<paramref name="context"/> parameter or the
    /// <c>DOCKER_CONTEXT</c> environment variable) is used;</item>
    /// <item>otherwise the builder resolves the active context, then the OS-specific
    /// default endpoint (npipe on Windows, unix socket on Linux/macOS).</item>
    /// </list>
    /// An <c>ssh://</c> endpoint from any of those sources is served by
    /// <c>Docker.DotNet.Ssh</c>, which tunnels through the local ssh client.
    /// TLS material for remote hosts comes from the docker context, so
    /// <paramref name="certificateLocation"/> is currently unused (the 4.x
    /// <see cref="DockerClientBuilder"/> exposes no per-certificate hook).
    /// </summary>
    public static DockerClient CreateClient(string dockerHost, string context, string certificateLocation)
    {
        dockerHost ??= Environment.GetEnvironmentVariable("DOCKER_HOST");
        context ??= Environment.GetEnvironmentVariable("DOCKER_CONTEXT");

        var builder = new DockerClientBuilder();

        if (!string.IsNullOrWhiteSpace(dockerHost))
        {
            // An explicit host must win over any context and the default socket.
            var endpoint = new Uri(dockerHost);

            builder = builder.WithEndpoint(endpoint);
        }
        else if (!string.IsNullOrWhiteSpace(context))
        {

            builder = builder.WithContext(context);
        }


        return builder.Build();
    }



    /// <summary>
    /// Creates a client from the parameters bound so far on the command line, for argument
    /// completers that must reach the daemon while the user is still typing.
    /// </summary>
    public static DockerClient CreateClient(IDictionary fakeBoundParameters)
    {
        var hostAddress = fakeBoundParameters["HostAddress"] as string;
        var context = fakeBoundParameters["Context"] as string;
        var certificateLocation = fakeBoundParameters["CertificateLocation"] as string;
        return CreateClient(hostAddress, context, certificateLocation);
    }
}
