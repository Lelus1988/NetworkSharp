using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Service for discovering and managing LAN devices
    /// </summary>
    public interface ILanDeviceService
    {
        /// <summary>
        /// Event raised when device scan progress updates
        /// </summary>
        event EventHandler<DeviceScanProgressEventArgs> ScanProgress;
        
        /// <summary>
        /// Event raised when a device is discovered
        /// </summary>
        event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered;
        
        /// <summary>
        /// Start scanning for LAN devices
        /// </summary>
        Task StartScanAsync(string subnet);
        
        /// <summary>
        /// Stop the current device scan
        /// </summary>
        Task StopScanAsync();
        
        /// <summary>
        /// Get discovered devices
        /// </summary>
        List<LanDevice> GetDevices();
        
        /// <summary>
        /// Get network gateway information
        /// </summary>
        Task<NetworkGatewayInfo> GetGatewayInfoAsync();
        
        /// <summary>
        /// Ping a specific device
        /// </summary>
        Task<int> PingDeviceAsync(string ipAddress);
        
        /// <summary>
        /// Ping a specific device with custom timeout
        /// </summary>
        Task<int> PingDeviceAsync(string ipAddress, int timeoutMs);
    }

    /// <summary>
    /// Event arguments for device scan progress
    /// </summary>
    public class DeviceScanProgressEventArgs : EventArgs
    {
        public int CurrentIP { get; set; }
        public int TotalIPs { get; set; }
        public int DevicesFound { get; set; }
        public int CurrentPort { get; set; }
    }

    /// <summary>
    /// Event arguments for device discovery
    /// </summary>
    public class DeviceDiscoveredEventArgs : EventArgs
    {
        public LanDevice Device { get; set; } = null!;
    }

    /// <summary>
    /// Represents a LAN device
    /// </summary>
    public class LanDevice
    {
        public string Name { get; set; } = string.Empty;
        public DeviceType Type { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public DeviceStatus Status { get; set; }
        public int ResponseTime { get; set; }
        public bool IsThisPC { get; set; }
        public DateTime LastSeen { get; set; }
    }

    /// <summary>
    /// Device type enumeration
    /// </summary>
    public enum DeviceType
    {
        Router,
        PC,
        Smartphone,
        Tablet,
        SmartTV,
        Printer,
        Server,
        Console,
        Speaker,
        Unknown
    }

    /// <summary>
    /// Device status enumeration
    /// </summary>
    public enum DeviceStatus
    {
        Online,
        Offline,
        Standby,
        New
    }

    /// <summary>
    /// Network gateway information
    /// </summary>
    public class NetworkGatewayInfo
    {
        public string GatewayIP { get; set; } = string.Empty;
        public string Subnet { get; set; } = string.Empty;
        public string LocalIP { get; set; } = string.Empty;
        public string DnsServer { get; set; } = string.Empty;
    }
}
