namespace NetworkSharp.Data.Models
{
    /// <summary>
    /// Represents network speed statistics data point
    /// </summary>
    public class NetworkStatistics
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public double DownloadSpeed { get; set; } // MB/s
        public double UploadSpeed { get; set; } // MB/s
    }
}
