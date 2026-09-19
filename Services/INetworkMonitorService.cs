using System;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Service for monitoring network traffic and statistics
    /// </summary>
    public interface INetworkMonitorService
    {
        /// <summary>
        /// Current download speed in Mbps
        /// </summary>
        double CurrentDownloadSpeed { get; }
        
        /// <summary>
        /// Current upload speed in Mbps
        /// </summary>
        double CurrentUploadSpeed { get; }
        
        /// <summary>
        /// Average download speed today in Mbps
        /// </summary>
        double AverageDownloadSpeedToday { get; }
        
        /// <summary>
        /// Average upload speed today in Mbps
        /// </summary>
        double AverageUploadSpeedToday { get; }
        
        /// <summary>
        /// Total data transferred in the last 7 days in GB
        /// </summary>
        double TotalDataTransferred7Days { get; }
        
        /// <summary>
        /// Total download data in the last 7 days in GB
        /// </summary>
        double TotalDownload7Days { get; }
        
        /// <summary>
        /// Total upload data in the last 7 days in GB
        /// </summary>
        double TotalUpload7Days { get; }
        
        /// <summary>
        /// Peak download speed in Mbps
        /// </summary>
        double PeakDownloadSpeed { get; }
        
        /// <summary>
        /// Timestamp when peak speed was recorded
        /// </summary>
        DateTime PeakSpeedTimestamp { get; }
        
        /// <summary>
        /// Event raised when network statistics are updated
        /// </summary>
        event EventHandler<NetworkStatsUpdatedEventArgs> StatsUpdated;
        
        /// <summary>
        /// Start monitoring network traffic
        /// </summary>
        Task StartMonitoringAsync();
        
        /// <summary>
        /// Stop monitoring network traffic
        /// </summary>
        Task StopMonitoringAsync();
        
        /// <summary>
        /// Get historical data rate data for graphing
        /// </summary>
        Task<NetworkDataPoint[]> GetDataRateHistoryAsync(TimeSpan timeRange);
        
        /// <summary>
        /// Get daily consumption data for graphing
        /// </summary>
        Task<DailyConsumptionData[]> GetDailyConsumptionAsync(int days);
    }

    /// <summary>
    /// Event arguments for network statistics updates
    /// </summary>
    public class NetworkStatsUpdatedEventArgs : EventArgs
    {
        public double DownloadSpeed { get; set; }
        public double UploadSpeed { get; set; }
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Data point for network rate history
    /// </summary>
    public class NetworkDataPoint
    {
        public DateTime Timestamp { get; set; }
        public double DownloadSpeed { get; set; }
        public double UploadSpeed { get; set; }
    }

    /// <summary>
    /// Daily consumption data
    /// </summary>
    public class DailyConsumptionData
    {
        public DateTime Date { get; set; }
        public double ConsumptionGB { get; set; }
        public double DownloadGB { get; set; }
        public double UploadGB { get; set; }
    }
}
