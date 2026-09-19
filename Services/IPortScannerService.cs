using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Service for scanning network ports
    /// </summary>
    public interface IPortScannerService
    {
        /// <summary>
        /// Event raised when scan progress updates
        /// </summary>
        event EventHandler<PortScanProgressEventArgs> ScanProgress;
        
        /// <summary>
        /// Event raised when a port scan is completed
        /// </summary>
        event EventHandler<PortScanResultEventArgs> PortScanned;
        
        /// <summary>
        /// Start scanning ports on a target host
        /// </summary>
        Task StartScanAsync(string target, int startPort, int endPort);
        
        /// <summary>
        /// Stop the current scan
        /// </summary>
        Task StopScanAsync();
        
        /// <summary>
        /// Get common ports to scan
        /// </summary>
        Dictionary<int, string> GetCommonPorts();
        
        /// <summary>
        /// Get scan results
        /// </summary>
        List<PortScanResult> GetResults();
    }

    /// <summary>
    /// Event arguments for scan progress
    /// </summary>
    public class PortScanProgressEventArgs : EventArgs
    {
        public int CurrentPort { get; set; }
        public int TotalPorts { get; set; }
        public int OpenPorts { get; set; }
        public int ClosedPorts { get; set; }
        public int FilteredPorts { get; set; }
    }

    /// <summary>
    /// Event arguments for individual port scan result
    /// </summary>
    public class PortScanResultEventArgs : EventArgs
    {
        public int Port { get; set; }
        public string Service { get; set; } = string.Empty;
        public PortStatus Status { get; set; }
        public int ResponseTime { get; set; }
    }

    /// <summary>
    /// Result of a port scan
    /// </summary>
    public class PortScanResult
    {
        public int Port { get; set; }
        public string Service { get; set; } = string.Empty;
        public PortStatus Status { get; set; }
        public int ResponseTime { get; set; }
    }

    /// <summary>
    /// Port status enumeration
    /// </summary>
    public enum PortStatus
    {
        Open,
        Closed,
        Filtered,
        Timeout
    }
}
