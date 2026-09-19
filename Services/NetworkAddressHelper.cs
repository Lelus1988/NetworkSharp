using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace NetworkSharp.Services
{
    public static class NetworkAddressHelper
    {
        public static List<string> GetCandidateAddresses(string subnet)
        {
            if (string.IsNullOrWhiteSpace(subnet))
            {
                return new List<string>();
            }

            var trimmed = subnet.Trim();
            var separatorIndex = trimmed.IndexOf('/');
            if (separatorIndex <= 0 || separatorIndex == trimmed.Length - 1)
            {
                return new List<string>();
            }

            var baseIpText = trimmed.Substring(0, separatorIndex);
            var prefixText = trimmed.Substring(separatorIndex + 1);

            if (!IPAddress.TryParse(baseIpText, out var baseAddress) ||
                baseAddress.AddressFamily != AddressFamily.InterNetwork ||
                !int.TryParse(prefixText, out var prefixLength) ||
                prefixLength < 0 || prefixLength > 32)
            {
                return new List<string>();
            }

            var addressBytes = baseAddress.GetAddressBytes();
            if (addressBytes.Length != 4)
            {
                return new List<string>();
            }

            var baseValue = BitConverter.ToUInt32(addressBytes.Reverse().ToArray(), 0);
            uint maskValue = prefixLength == 0 ? 0u : ((uint)0xFFFFFFFF << (32 - prefixLength));
            if (prefixLength == 0)
            {
                maskValue = 0u;
            }

            var network = baseValue & maskValue;
            var broadcast = prefixLength == 32 ? network : network | (~maskValue);
            var firstUsable = prefixLength <= 30 ? network + 1u : network;
            var lastUsable = prefixLength <= 30 ? broadcast - 1u : broadcast;

            var result = new List<string>();
            for (var current = firstUsable; current <= lastUsable; current++)
            {
                if (current == 0)
                {
                    continue;
                }

                var bytes = BitConverter.GetBytes(current).Reverse().ToArray();
                result.Add(new IPAddress(bytes).ToString());
            }

            if (result.Count == 0)
            {
                return new List<string> { baseAddress.ToString() };
            }

            return result;
        }
    }
}
