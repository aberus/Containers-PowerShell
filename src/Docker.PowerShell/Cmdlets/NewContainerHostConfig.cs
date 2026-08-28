using System.Management.Automation;
using Docker.DotNet.Models;
using System.Threading.Tasks;
using System;

namespace Docker.PowerShell.Cmdlets;

/// <summary>
/// Builds the host side of a container's configuration, for passing to the
/// -HostConfiguration parameter of the cmdlets that create containers. It only assembles
/// an object; nothing is sent to the daemon.
/// </summary>
[Cmdlet(VerbsCommon.New, "ContainerHostConfig",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(HostConfig))]
public class NewContainerHostConfig : DkrCmdlet
{
    #region Parameters

    /// <summary>
    /// The volume and bind mounts, in the docker CLI's <c>source:destination[:options]</c> form.
    /// </summary>
    [Parameter(Position = 0)]
    public string[] Binds { get; set; }

    /// <summary>
    /// The network to attach the container to.
    /// </summary>
    [Parameter]
    public string NetworkMode { get; set; }

    /// <summary>
    /// Publishes every exposed port to a port the host picks.
    /// </summary>
    [Parameter]
    public SwitchParameter PublishAllPorts { get; set; }

    /// <summary>
    /// Gives the container extended privileges on the host.
    /// </summary>
    [Parameter]
    public SwitchParameter Privileged { get; set; }

    // Dns/DnsSearch are not present on this HostConfig version; omit to keep compatibility

    /// <summary>
    /// Linux capabilities to add to the container.
    /// </summary>
    [Parameter]
    public string[] CapAdd { get; set; }

    /// <summary>
    /// Linux capabilities to drop from the container.
    /// </summary>
    [Parameter]
    public string[] CapDrop { get; set; }

    /// <summary>
    /// Extra entries for the container's hosts file, as <c>hostname:ip</c>.
    /// </summary>
    [Parameter]
    public string[] ExtraHosts { get; set; }

    /// <summary>
    /// Containers whose volumes this container should mount.
    /// </summary>
    [Parameter]
    public string[] VolumesFrom { get; set; }

    /// <summary>
    /// When to restart the container: no, always, unless-stopped, or on-failure.
    /// </summary>
    [Parameter]
    public string RestartPolicyName { get; set; }

    /// <summary>
    /// How many times to retry under the on-failure restart policy.
    /// </summary>
    [Parameter]
    public int RestartPolicyMaximumRetryCount { get; set; }

    /// <summary>
    /// How many CPUs the container may use, as a fraction of the host's.
    /// </summary>
    [Parameter]
    public double Cpus { get; set; }

    /// <summary>
    /// The memory limit, with an optional unit suffix such as "512m" or "2g".
    /// </summary>
    [Parameter]
    public string Memory { get; set; }

    /// <summary>
    /// The combined memory and swap limit, in the same form as -Memory.
    /// </summary>
    [Parameter]
    public string MemorySwap { get; set; }

    /// <summary>
    /// Removes the container as soon as it exits.
    /// </summary>
    [Parameter]
    public SwitchParameter AutoRemove { get; set; }

    /// <summary>
    /// Host devices to expose, as <c>host[:container[:permissions]]</c>.
    /// </summary>
    [Parameter]
    public string[] Devices { get; set; }

    #endregion

    #region Overrides

    /// <summary>
    /// Assembles the host configuration and writes it.
    /// </summary>
    protected override Task ProcessRecordAsync()
    {
        RestartPolicy restartPolicy = null;
        if (!string.IsNullOrEmpty(RestartPolicyName))
        {
            if (!Enum.TryParse<RestartPolicyKind>(RestartPolicyName, true, out var kind))
            {
                // Try numeric parse as fallback
                restartPolicy = new RestartPolicy { Name = RestartPolicyKind.No, MaximumRetryCount = RestartPolicyMaximumRetryCount };
            }
            else
            {
                restartPolicy = new RestartPolicy { Name = kind, MaximumRetryCount = RestartPolicyMaximumRetryCount };
            }
        }

        var hostConfig = new HostConfig()
        {
            Binds = Binds,
            NetworkMode = NetworkMode,
            PublishAllPorts = PublishAllPorts,
            Privileged = Privileged,
            CapAdd = CapAdd,
            CapDrop = CapDrop,
            ExtraHosts = ExtraHosts,
            VolumesFrom = VolumesFrom,
            RestartPolicy = restartPolicy,
            AutoRemove = AutoRemove
        };

        if (Cpus > 0)
        {
            // Docker.DotNet uses NanoCPUs as long
            try
            {
                hostConfig.NanoCPUs = Convert.ToInt64(Cpus * 1000000000L);
            }
            catch { }
        }

        if (!string.IsNullOrEmpty(Memory))
        {
            if (TryParseByteSize(Memory, out var bytes))
            {
                hostConfig.Memory = bytes;
            }
        }

        if (!string.IsNullOrEmpty(MemorySwap))
        {
            if (TryParseByteSize(MemorySwap, out var bytes))
            {
                hostConfig.MemorySwap = bytes;
            }
        }

        if (Devices?.Length > 0)
        {
            // Map simple device strings into DeviceMapping objects if available on this HostConfig
            try
            {
                var devs = new System.Collections.Generic.List<DeviceMapping>();
                foreach (var d in Devices)
                {
                    // Expect form "/dev/snd:/dev/snd:rwm" or "/dev/snd"
                    var parts = d.Split(':');
                    var dm = new DeviceMapping();
                    if (parts.Length == 1)
                    {
                        dm.PathOnHost = parts[0];
                        dm.PathInContainer = parts[0];
                    }
                    else
                    {
                        dm.PathOnHost = parts[0];
                        dm.PathInContainer = parts[1];
                        if (parts.Length > 2) dm.CgroupPermissions = parts[2];
                    }
                    devs.Add(dm);
                }
                hostConfig.Devices = devs;
            }
            catch { }
        }

        WriteObject(hostConfig);

        return Task.CompletedTask;
    }

    private static bool TryParseByteSize(string input, out long bytes)
    {
        bytes = 0;
        if (string.IsNullOrEmpty(input)) return false;

        input = input.Trim();
        var last = input[input.Length - 1];
        long multiplier = 1;
        string numberPart = input;

        if (char.IsLetter(last))
        {
            numberPart = input.Substring(0, input.Length - 1);
            multiplier = char.ToLowerInvariant(last) switch
            {
                'k' => 1024L,
                'm' => 1024L * 1024L,
                'g' => 1024L * 1024L * 1024L,
                't' => 1024L * 1024L * 1024L * 1024L,
                _ => 1,
            };
        }

        if (double.TryParse(numberPart, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var n))
        {
            try
            {
                bytes = Convert.ToInt64(n * multiplier);
                return true;
            }
            catch { }
        }

        return false;
    }

    #endregion
}
