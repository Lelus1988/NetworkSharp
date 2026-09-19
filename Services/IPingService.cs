using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Service for ping functionality with graphing capabilities
    /// </summary>
    public interface IPingService
    {
        /// <summary>
        /// Event raised when a ping result is received
        /// </summary>
        event EventHandler<PingResultEventArgs> PingResult;
        
        /// <summary>
        /// Current ping statistics
        /// </summary>
        PingStatistics Statistics { get; }
        
        /// <summary>
        /// Start continuous ping to a target
        /// </summary>
        Task StartPingAsync(string target, int intervalMs = 1000);
        
        /// <summary>
        /// Stop the current ping operation
        /// </summary>
        Task StopPingAsync();
        
        /// <summary>
        /// Get ping history for graphing
        /// </summary>
        List<PingDataPoint> GetPingHistory(TimeSpan timeRange);
        
        /// <summary>
        /// Reset ping statistics
        /// </summary>
        void ResetStatistics();
    }

    /// <summary>
    /// Event arguments for ping results
    /// </summary>
    public class PingResultEventArgs : EventArgs
    {
        public string Target { get; set; } = string.Empty;
        public int ResponseTime { get; set; }
        public bool Success { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Ping statistics
    /// </summary>
    public class PingStatistics
    {
        public int CurrentPing { get; set; }
        public int AveragePing { get; set; }
        public int MinimumPing { get; set; }
        public int MaximumPing { get; set; }
        public double PacketLoss { get; set; }
        public int TotalPackets { get; set; }
        public int LostPackets { get; set; }
        public DateTime StartTime { get; set; }
    }

    /// <summary>
    /// Data point for ping history
    /// </summary>
    public class PingDataPoint
    {
        public DateTime Timestamp { get; set; }
        public int ResponseTime { get; set; }
        public bool Success { get; set; }
    }
}
