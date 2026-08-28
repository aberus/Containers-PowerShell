using System.Management.Automation;
using Docker.DotNet.Models;
using System.Threading.Tasks;
using System;

namespace Docker.PowerShell.Cmdlets;

[Cmdlet(VerbsCommon.New, "ContainerHostConfig",
        DefaultParameterSetName = CommonParameterSetNames.Default)]
[OutputType(typeof(HostConfig))]
public class NewContainerHostConfig : DkrCmdlet
{
    #region Parameters

    [Parameter(Position = 0)]
    public string[] Binds { get; set; }

    [Parameter]
    public string NetworkMode { get; set; }

    [Parameter]
    public SwitchParameter PublishAllPorts { get; set; }

    [Parameter]
    public SwitchParameter Privileged { get; set; }

    // Dns/DnsSearch are not present on this HostConfig version; omit to keep compatibility

    [Parameter]
    public string[] CapAdd { get; set; }

    [Parameter]
    public string[] CapDrop { get; set; }

    [Parameter]
    public string[] ExtraHosts { get; set; }

    [Parameter]
    public string[] VolumesFrom { get; set; }

    [Parameter]
    public string RestartPolicyName { get; set; }

    [Parameter]
    public int RestartPolicyMaximumRetryCount { get; set; }

    [Parameter]
    public double Cpus { get; set; }

    [Parameter]
    public string Memory { get; set; }

    [Parameter]
    public string MemorySwap { get; set; }

    [Parameter]
    public SwitchParameter AutoRemove { get; set; }

    [Parameter]
    public string[] Devices { get; set; }

    #endregion

    #region Overrides

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
