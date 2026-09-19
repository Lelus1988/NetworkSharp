using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Implementation of LAN device discovery service
    /// </summary>
    public class LanDeviceService : ILanDeviceService
    {
        private readonly object _lock = new object();
        private readonly List<LanDevice> _devices = new List<LanDevice>();
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isScanning;
        
        public event EventHandler<DeviceScanProgressEventArgs>? ScanProgress;
        public event EventHandler<DeviceDiscoveredEventArgs>? DeviceDiscovered;

        public LanDeviceService()
        {
            InitializeLocalDevice();
        }

        private void InitializeLocalDevice()
        {
            try
            {
                var localIP = GetLocalIPAddress();
                if (string.IsNullOrWhiteSpace(localIP))
                {
                    return;
                }

                var localDevice = new LanDevice
                {
                    Name = Environment.MachineName,
                    Type = DeviceType.PC,
                    IPAddress = localIP,
                    MacAddress = GetLocalMacAddress(),
                    Manufacturer = "System",
                    Status = DeviceStatus.Online,
                    ResponseTime = 0,
                    IsThisPC = true,
                    LastSeen = DateTime.Now
                };

                lock (_lock)
                {
                    if (!_devices.Any(d => d.IPAddress == localDevice.IPAddress))
                    {
                        _devices.Add(localDevice);
                    }
                }
            }
            catch
            {
                // Ignore local device metadata errors and rely on live scan results.
            }
        }
        
        private string GetLocalIPAddress()
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
                return host.AddressList.FirstOrDefault(ip => 
                    ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }
        
        private string GetLocalMacAddress()
        {
            try
            {
                var nics = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
                var firstNic = nics.FirstOrDefault(ni => 
                    ni.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up && 
                    ni.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback);
                return firstNic?.GetPhysicalAddress().ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }

        public async Task StartScanAsync(string subnet)
        {
            if (_isScanning)
                return;

            _isScanning = true;
            _cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var gatewayInfo = await GetGatewayInfoAsync();
                var effectiveSubnet = !string.IsNullOrWhiteSpace(subnet) ? subnet : gatewayInfo.Subnet;
                if (string.IsNullOrWhiteSpace(effectiveSubnet))
                {
                    effectiveSubnet = "192.168.178.0/24";
                }

                var candidateAddresses = NetworkAddressHelper.GetCandidateAddresses(effectiveSubnet);
                if (candidateAddresses.Count == 0)
                {
                    candidateAddresses = NetworkAddressHelper.GetCandidateAddresses(gatewayInfo.Subnet);
                }

                if (candidateAddresses.Count == 0)
                {
                    candidateAddresses = NetworkAddressHelper.GetCandidateAddresses("192.168.178.0/24");
                }

                var knownAddresses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                lock (_lock)
                {
                    foreach (var device in _devices)
                    {
                        knownAddresses.Add(device.IPAddress);
                    }
                }

                var totalIPs = candidateAddresses.Count;
                var devicesFound = 0;

                for (var index = 0; index < candidateAddresses.Count; index++)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                        break;

                    var currentIP = candidateAddresses[index];
                    if (string.Equals(currentIP, gatewayInfo.GatewayIP, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(currentIP, gatewayInfo.LocalIP, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (knownAddresses.Contains(currentIP) && _devices.Any(d => d.IPAddress == currentIP && d.Status == DeviceStatus.Online))
                    {
                        continue;
                    }

                    var responseTime = await PingDeviceAsync(currentIP, 1000);
                    if (responseTime < 0)
                    {
                        ScanProgress?.Invoke(this, new DeviceScanProgressEventArgs
                        {
                            CurrentIP = index + 1,
                            TotalIPs = totalIPs,
                            DevicesFound = devicesFound
                        });

                        await Task.Delay(75, _cancellationTokenSource.Token);
                        continue;
                    }

                    var device = new LanDevice
                    {
                        Name = GetHostName(currentIP),
                        Type = DetectDeviceType(currentIP),
                        IPAddress = currentIP,
                        MacAddress = await GetMacAddressAsync(currentIP),
                        Manufacturer = GetManufacturer(currentIP),
                        Status = DeviceStatus.Online,
                        ResponseTime = responseTime,
                        IsThisPC = IsThisPC(currentIP),
                        LastSeen = DateTime.Now
                    };

                    lock (_lock)
                    {
                        var existingDevice = _devices.FirstOrDefault(d => d.IPAddress == currentIP);
                        if (existingDevice != null)
                        {
                            existingDevice.ResponseTime = responseTime;
                            existingDevice.LastSeen = DateTime.Now;
                            existingDevice.Status = DeviceStatus.Online;
                            existingDevice.Name = string.IsNullOrWhiteSpace(existingDevice.Name) || existingDevice.Name == "Unknown"
                                ? device.Name
                                : existingDevice.Name;
                        }
                        else
                        {
                            device.Status = DeviceStatus.New;
                            _devices.Add(device);
                            knownAddresses.Add(currentIP);
                        }

                        devicesFound = _devices.Count(d => d.Status == DeviceStatus.Online);
                    }

                    DeviceDiscovered?.Invoke(this, new DeviceDiscoveredEventArgs { Device = device });

                    ScanProgress?.Invoke(this, new DeviceScanProgressEventArgs
                    {
                        CurrentIP = index + 1,
                        TotalIPs = totalIPs,
                        DevicesFound = devicesFound
                    });

                    await Task.Delay(75, _cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Scan was cancelled.
            }
            finally
            {
                _isScanning = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public async Task StopScanAsync()
        {
            if (_isScanning && _cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                await Task.CompletedTask;
            }
        }

        public List<LanDevice> GetDevices()
        {
            lock (_lock)
            {
                return new List<LanDevice>(_devices);
            }
        }

        public async Task<NetworkGatewayInfo> GetGatewayInfoAsync()
        {
            // Get the actual network information
            var gatewayInfo = new NetworkGatewayInfo();
            
            try
            {
                // Get all network interfaces
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up && 
                                ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);
                
                foreach (var ni in networkInterfaces)
                {
                    var ipProperties = ni.GetIPProperties();
                    
                    // Get gateway
                    var gateway = ipProperties.GatewayAddresses.FirstOrDefault();
                    if (gateway != null)
                    {
                        gatewayInfo.GatewayIP = gateway.Address.ToString();
                    }
                    
                    // Get local IP
                    var ipv4Address = ipProperties.UnicastAddresses
                        .FirstOrDefault(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    if (ipv4Address != null)
                    {
                        gatewayInfo.LocalIP = ipv4Address.Address.ToString();
                        gatewayInfo.Subnet = $"{ipv4Address.Address}/{ipv4Address.PrefixLength}";
                    }
                    
                    // Get DNS server
                    var dnsServer = ipProperties.DnsAddresses.FirstOrDefault();
                    if (dnsServer != null)
                    {
                        gatewayInfo.DnsServer = dnsServer.ToString();
                    }
                    
                    if (!string.IsNullOrEmpty(gatewayInfo.GatewayIP))
                        break;
                }
                
                // Fallback to sample data if no real data found
                if (string.IsNullOrEmpty(gatewayInfo.GatewayIP))
                {
                    gatewayInfo.GatewayIP = "192.168.178.1";
                    gatewayInfo.Subnet = "192.168.178.0/24";
                    gatewayInfo.LocalIP = "192.168.178.20";
                    gatewayInfo.DnsServer = "192.168.178.1";
                }
            }
            catch
            {
                // Fallback to sample data
                gatewayInfo.GatewayIP = "192.168.178.1";
                gatewayInfo.Subnet = "192.168.178.0/24";
                gatewayInfo.LocalIP = "192.168.178.20";
                gatewayInfo.DnsServer = "192.168.178.1";
            }
            
            await Task.CompletedTask;
            return gatewayInfo;
        }

        public async Task<int> PingDeviceAsync(string ipAddress)
        {
            return await PingDeviceAsync(ipAddress, 2000);
        }
        
        public async Task<int> PingDeviceAsync(string ipAddress, int timeoutMs)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(ipAddress, timeoutMs);
                
                if (reply.Status == IPStatus.Success)
                {
                    return (int)reply.RoundtripTime;
                }
                
                return -1;
            }
            catch
            {
                return -1;
            }
        }

        private string GetHostName(string ipAddress)
        {
            try
            {
                var hostEntry = Dns.GetHostEntry(ipAddress);
                return hostEntry.HostName.Split('.')[0]; // Return just the hostname
            }
            catch
            {
                return "Unknown";
            }
        }

        private DeviceType DetectDeviceType(string ipAddress)
        {
            // Simple heuristic based on IP ranges and common patterns
            // In a real implementation, this would use MAC address OUI lookup
            var lastOctet = int.Parse(ipAddress.Split('.')[3]);
            
            if (lastOctet == 1)
                return DeviceType.Router;
            
            return DeviceType.Unknown;
        }

        private async Task<string> GetMacAddressAsync(string ipAddress)
        {
            try
            {
                // In a real implementation, this would use ARP table lookup
                // For now, return a placeholder
                await Task.CompletedTask;
                return "00:00:00:00:00:00";
            }
            catch
            {
                return "00:00:00:00:00:00";
            }
        }

        private string GetManufacturer(string ipAddress)
        {
            // In a real implementation, this would use MAC address OUI lookup
            return "Unknown";
        }

        private bool IsThisPC(string ipAddress)
        {
            try
            {
                var localIPs = Dns.GetHostAddresses(Dns.GetHostName())
                    .Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    .Select(ip => ip.ToString());
                
                return localIPs.Contains(ipAddress);
            }
            catch
            {
                return false;
            }
        }
    }
}
