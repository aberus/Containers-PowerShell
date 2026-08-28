using System.Collections.Generic;
using System.Linq;
using Docker.DotNet.Models;

namespace Docker.PowerShell.Objects
{
    /// <summary>
    /// Formatting helpers used by the module's format file to render containers and images.
    /// </summary>
    public static class Formatter
    {
        /// <summary>
        /// Renders a container's port bindings the way the docker CLI does, collapsing runs of
        /// consecutive ports into ranges.
        /// </summary>
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

        /// <summary>
        /// Orders port bindings by private port, address, public port, and protocol, so that
        /// consecutive ports end up adjacent.
        /// </summary>
        public class PortComparer : IComparer<PortSummary>
        {
            /// <summary>
            /// The shared instance of the comparer.
            /// </summary>
            public static PortComparer Instance { get; } = new PortComparer();

            /// <summary>
            /// Compares two port bindings.
            /// </summary>
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

        /// <summary>
        /// Shortens an id to the 12 characters the docker CLI shows, dropping any algorithm prefix.
        /// </summary>
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
