namespace NetworkSharp.Services
{
    /// <summary>
    /// Interface for speed test service
    /// </summary>
    public interface ISpeedTestService
    {
        /// <summary>
        /// Test download speed
        /// </summary>
        Task<double> TestDownloadSpeedAsync(int durationSeconds = 10, IProgress<double>? progress = null);

        /// <summary>
        /// Test upload speed
        /// </summary>
        Task<double> TestUploadSpeedAsync(int durationSeconds = 10, IProgress<double>? progress = null);

        /// <summary>
        /// Test ping and jitter
        /// </summary>
        Task<(double ping, double jitter)> TestPingJitterAsync(int sampleCount = 10);

        /// <summary>
        /// Get list of available test servers
        /// </summary>
        Task<List<SpeedTestServer>> GetServersAsync();

        /// <summary>
        /// Select best server based on ping
        /// </summary>
        Task<SpeedTestServer?> SelectBestServerAsync();
    }

    /// <summary>
    /// Represents a speed test server
    /// </summary>
    public class SpeedTestServer
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Ping { get; set; }
    }

    /// <summary>
    /// Speed test results
    /// </summary>
    public class SpeedTestResult
    {
        public double DownloadSpeed { get; set; } // Mbps
        public double UploadSpeed { get; set; } // Mbps
        public double Ping { get; set; } // ms
        public double Jitter { get; set; } // ms
        public string ServerName { get; set; } = string.Empty;
        public DateTime TestTime { get; set; }
    }
}
