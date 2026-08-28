using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Docker.PowerShell.Support
{
    public class PortMapping
    {
        public string Port { get; set; }  // "80/tcp"
        public PortBinding Binding { get; set; }
    }

    public static class NatParser
    {

        public static Dictionary<string, IList<PortBinding>> ParsePortSpecs(IEnumerable<string> ports)
        {
            var bindings = new Dictionary<string, IList<PortBinding>>();

            foreach (var rawPort in ports)
            {
                foreach (var mapping in ParsePortSpec(rawPort))
                {
                    var port = mapping.Port;

                    if (!bindings.ContainsKey(port))
                    {
                        bindings[port] = new List<PortBinding>();
                    }

                    bindings[port].Add(mapping.Binding);
                }
            }

            return bindings;
        }

        public static List<PortMapping> ParsePortSpec(string rawPort)
        {
            var (ip, hostPort, containerPortRaw) = SplitParts(rawPort);
            var (portStr, proto) = SplitProtoPort(containerPortRaw);

            proto = proto.ToLower();
            ValidateProto(proto);

            if (!string.IsNullOrEmpty(ip) && ip.StartsWith("["))
            {
                ip = ip.Trim('[', ']');
            }

            if (!string.IsNullOrEmpty(ip) && !IPAddress.TryParse(ip, out _))
            {
                throw new Exception($"Invalid IP address: {ip}");
            }

            if (string.IsNullOrEmpty(portStr))
            {
                throw new Exception("No port specified");
            }

            var (startPort, endPort) = ParsePortRange(portStr);
            var (startHostPort, endHostPort) = string.IsNullOrEmpty(hostPort)
                ? (0, 0)
                : ParsePortRange(hostPort);

            if (!string.IsNullOrEmpty(hostPort) &&
                (endPort - startPort) != (endHostPort - startHostPort) &&
                endPort != startPort)
            {
                throw new Exception($"Invalid ranges specified for container and host Ports: {portStr} and {hostPort}");
            }

            var mappings = new List<PortMapping>();
            for (int i = 0; i <= endPort - startPort; i++)
            {
                var containerPort = (startPort + i).ToString();
                var mappedHostPort = string.IsNullOrEmpty(hostPort) ? "" : (startHostPort + i).ToString();

                if (startPort == endPort && startHostPort != endHostPort)
                {
                    mappedHostPort += "-" + endHostPort;
                }

                var fullPort = $"{containerPort}/{proto}";
                mappings.Add(new PortMapping
                {
                    Port = fullPort,
                    Binding = new PortBinding { HostIP = ip, HostPort = mappedHostPort }
                });
            }

            return mappings;
        }

        private static void ValidateProto(string proto)
        {
            if (proto != "tcp" && proto != "udp" && proto != "sctp")
            {
                throw new Exception("Invalid proto: " + proto);
            }
        }

        private static (string ip, string hostPort, string containerPort) SplitParts(string rawport)
        {
            var parts = rawport.Split(':');
            int n = parts.Length;

            var containerPort = parts[n - 1];
            if (n == 1) return ("", "", containerPort);
            if (n == 2) return ("", parts[0], containerPort);
            if (n == 3) return (parts[0], parts[1], containerPort);

            return (string.Join(":", parts.Take(n - 2)), parts[n - 2], containerPort);
        }

        private static (string port, string proto) SplitProtoPort(string rawPort)
        {
            var parts = rawPort.Split('/');
            string port = parts[0];
            string proto = parts.Length > 1 ? parts[1] : "tcp";
            return (port, proto);
        }

        private static (int, int) ParsePortRange(string rawPort)
        {
            var parts = rawPort.Split('-');
            if (parts.Length == 1)
            {
                int val = int.Parse(parts[0]);
                return (val, val);
            }
            if (parts.Length == 2)
            {
                int start = int.Parse(parts[0]);
                int end = int.Parse(parts[1]);
                return (start, end);
            }

            throw new ArgumentException("Invalid port range: " + rawPort);
        }
    }
}
