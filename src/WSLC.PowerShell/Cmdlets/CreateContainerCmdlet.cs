using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.WSL.Containers;
using System.Collections.Generic;
using Windows.Networking;

namespace WSLC.PowerShell.Cmdlets;

public abstract class CreateContainerCmdlet : ImageOperationCmdlet
{
    #region Parameters

    /// <summary>
    /// The name to use for the new container.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default)]
    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject)]
    [ValidateNotNullOrEmpty]
    public string? Name { get; set; }

    /// <summary>
    /// Ports to publish to the Windows host, in "windowsPort:containerPort[/tcp|/udp]"
    /// or "port" form.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default)]
    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject)]
    [ValidateNotNullOrEmpty]
    public string[]? Publish { get; set; }

    /// <summary>
    /// The command to use by default when starting the new container.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default,
        ValueFromRemainingArguments = true,
        Position = 1)]
    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject,
        ValueFromRemainingArguments = true,
        Position = 1)]
    [ValidateNotNullOrEmpty]
    public string[]? Command { get; set; }

    /// <summary>
    /// Environment variables for the container's init process, in "NAME=VALUE" form.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default)]
    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject)]
    [ValidateNotNullOrEmpty]
    public string[]? Environment { get; set; }

    /// <summary>
    /// Windows paths to mount into the container, in "windowsPath:containerPath[:ro]" form.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default)]
    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject)]
    [ValidateNotNullOrEmpty]
    public string[]? Volume { get; set; }

    /// <summary>
    /// The networking mode for the container. Defaults to Bridged, matching the wslc
    /// CLI and docker; the SDK's own default of None would reject published ports and
    /// leave the container without network access.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default)]
    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject)]
    public ContainerNetworkingMode NetworkingMode { get; set; } = ContainerNetworkingMode.Bridged;

    /// <summary>
    /// The container's hostname, like `wslc container create --hostname`.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default)]
    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject)]
    [ValidateNotNullOrEmpty]
    public string? Hostname { get; set; }

    /// <summary>
    /// The container's NIS domain name, like `wslc container create --domainname`.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default)]
    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject)]
    [ValidateNotNullOrEmpty]
    public string? Domainname { get; set; }

    /// <summary>
    /// Give the container GPU access, like `wslc container create --gpus`.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default)]
    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject)]
    public SwitchParameter EnableGpu { get; set; }

    /// <summary>
    /// The working directory of the container's init process, like
    /// `wslc container create --workdir`.
    /// </summary>
    [Parameter(ParameterSetName = CommonParameterSetNames.Default)]
    [Parameter(ParameterSetName = CommonParameterSetNames.ImageObject)]
    [ValidateNotNullOrEmpty]
    public string? WorkingDirectory { get; set; }

    #endregion

    /// <summary>
    /// Builds the WSLC container settings from the common creation parameters.
    /// </summary>
    protected ContainerSettings BuildContainerSettings(string imageName)
    {
        var settings = new ContainerSettings(imageName)
        {
            NetworkingMode = NetworkingMode
        };

        if (!string.IsNullOrEmpty(Name))
        {
            settings.Name = Name;
        }

        if (!string.IsNullOrEmpty(Hostname))
        {
            settings.HostName = Hostname;
        }

        if (!string.IsNullOrEmpty(Domainname))
        {
            settings.DomainName = Domainname;
        }

        if (EnableGpu)
        {
            settings.EnableGpu = true;
        }

        var initProcess = new ProcessSettings
        {
            OutputMode = ProcessOutputMode.Event
        };

        if (Command is { Length: > 0 })
        {
            initProcess.CommandLine = Command;
        }

        if (!string.IsNullOrEmpty(WorkingDirectory))
        {
            initProcess.WorkingDirectory = WorkingDirectory;
        }

        if (Environment is { Length: > 0 })
        {
            var variables = new Dictionary<string, string>();
            foreach (var entry in Environment)
            {
                var separator = entry.IndexOf('=');
                if (separator <= 0)
                {
                    throw new ArgumentException($"Invalid environment variable '{entry}'. Expected NAME=VALUE.");
                }

                variables[entry[..separator]] = entry[(separator + 1)..];
            }

            initProcess.EnvironmentVariables = variables;
        }

        settings.InitProcess = initProcess;

        if (Publish is { Length: > 0 })
        {
            settings.PortMappings = Publish.Select(ParsePortMapping).ToList();
        }

        if (Volume is { Length: > 0 })
        {
            var volumes = new List<ContainerVolume>();
            var namedVolumes = new List<ContainerNamedVolume>();
            foreach (var volume in Volume)
            {
                ParseVolume(volume, volumes, namedVolumes);
            }

            if (volumes.Count > 0)
            {
                settings.Volumes = volumes;
            }

            if (namedVolumes.Count > 0)
            {
                settings.NamedVolumes = namedVolumes;
            }
        }

        return settings;
    }

    private static ContainerPortMapping ParsePortMapping(string mapping)
    {
        var protocol = PortProtocol.TCP;
        var spec = mapping;

        var protocolSeparator = spec.IndexOf('/');
        if (protocolSeparator >= 0)
        {
            protocol = spec[(protocolSeparator + 1)..].ToUpperInvariant() switch
            {
                "TCP" => PortProtocol.TCP,
                "UDP" => PortProtocol.UDP,
                _ => throw new ArgumentException($"Invalid protocol in port mapping '{mapping}'.")
            };
            spec = spec[..protocolSeparator];
        }

        var parts = spec.Split(':');
        switch (parts.Length)
        {
            case 1:
                return new ContainerPortMapping(ushort.Parse(parts[0]), ushort.Parse(parts[0]), protocol);
            case 2:
                return new ContainerPortMapping(ushort.Parse(parts[0]), ushort.Parse(parts[1]), protocol);
            case 3:
                // address:windowsPort:containerPort — bind to a specific host address.
                return new ContainerPortMapping(ushort.Parse(parts[1]), ushort.Parse(parts[2]), protocol)
                {
                    WindowsAddress = new HostName(parts[0])
                };
            default:
                throw new ArgumentException(
                    $"Invalid port mapping '{mapping}'. Expected [address:]windowsPort:containerPort[/tcp|/udp].");
        }
    }

    private static void ParseVolume(string volume, List<ContainerVolume> volumes, List<ContainerNamedVolume> namedVolumes)
    {
        var readOnly = false;
        var spec = volume;

        if (spec.EndsWith(":ro", StringComparison.OrdinalIgnoreCase))
        {
            readOnly = true;
            spec = spec[..^3];
        }

        // The Windows path may itself contain a drive-letter colon (e.g. C:\data),
        // so split on the last separator.
        var separator = spec.LastIndexOf(':');
        if (separator <= 0 || separator == spec.Length - 1)
        {
            throw new ArgumentException($"Invalid volume '{volume}'. Expected windowsPath:containerPath[:ro] or name:containerPath[:ro].");
        }

        var source = spec[..separator];
        var containerPath = spec[(separator + 1)..];

        // Like docker: a source without path separators or a drive designator is a
        // named volume, anything path-like is a Windows bind mount.
        if (source.Contains('\\') || source.Contains('/') || (source.Length >= 2 && source[1] == ':') || source.StartsWith('.'))
        {
            volumes.Add(new ContainerVolume(source, containerPath, readOnly));
        }
        else
        {
            namedVolumes.Add(new ContainerNamedVolume(source, containerPath, readOnly));
        }
    }
}
