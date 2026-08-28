using System.Collections.Generic;
using System.Linq;
using Docker.DotNet.Models;

namespace Docker.PowerShell.Objects
{
    public static class Formatter
    {
        public static string PortsToString(this IList<PortSummary> ports)
        {
            if (ports.Count == 0)
            {
                return string.Empty;
            }

            var groupMap = new Dictionary<string, (ushort First, ushort Last)>();
            var result = new List<string>();
            var hostMappings = new List<string>();
            var groupMapKeys = new List<string>();

            // Sort ports using a custom comparison
            ports.ToList().Sort(PortComparer.Instance);

            foreach (var port in ports)
            {
                ushort current = port.PrivatePort;
                string portKey = port.Type;

                if (!string.IsNullOrEmpty(port.IP))
                {
                    if (port.PublicPort != current)
                    {
                        string hAddrPort = $"{port.IP}:{port.PublicPort}";
                        hostMappings.Add($"{hAddrPort}->{port.PrivatePort}/{port.Type}");
                        continue;
                    }
                    portKey = $"{port.IP}/{port.Type}";
                }

                if (!groupMap.TryGetValue(portKey, out (ushort First, ushort Last) group))
                {
                    groupMap[portKey] = (current, current);
                    groupMapKeys.Add(portKey);
                    continue;
                }

                if (current == (group.Last + 1))
                {
                    group.Last = current;
                    continue;
                }

                result.Add(FormGroup(portKey, group.First, group.Last));
                groupMap[portKey] = (current, current);
            }

            foreach (var portKey in groupMapKeys)
            {
                var g = groupMap[portKey];
                result.Add(FormGroup(portKey, g.First, g.Last));
            }

            result.AddRange(hostMappings);
            return string.Join(", ", result);
        }


        private static string FormGroup(string key, ushort start, ushort last)
        {
            var parts = key.Split('/');
            string groupType = parts[0];
            string ip = parts.Length > 1 ? parts[0] : "";

            string group = start.ToString();
            if (start != last)
            {
                group = $"{group}-{last}";
            }

            if (!string.IsNullOrEmpty(ip))
            {
                group = $"{ip}:{group}->{group}";
            }

            return $"{group}/{(parts.Length > 1 ? parts[1] : groupType)}";
        }

        public class PortComparer : IComparer<PortSummary>
        {
            public static PortComparer Instance { get; } = new PortComparer();

            public int Compare(PortSummary x, PortSummary y)
            {
                if (x.PrivatePort != y.PrivatePort)
                {
                    return x.PrivatePort.CompareTo(y.PrivatePort);
                }

                if (x.IP != y.IP)
                {
                    return string.Compare(x.IP, y.IP);
                }

                if (x.PublicPort != y.PublicPort)
                {
                    return x.PublicPort.Value.CompareTo(y.PublicPort);
                }

                return string.Compare(x.Type, y.Type);
            }
        }

        private const int shortLen = 12;

        public static string TruncateId(string id)
        {
            int index = id.IndexOf(':');
            if (index >= 0)
            {
                id = id.Substring(index + 1);
            }
            if (id.Length > shortLen)
            {
                id = id.Substring(0, shortLen);
            }
            return id;
        }
    }
}
