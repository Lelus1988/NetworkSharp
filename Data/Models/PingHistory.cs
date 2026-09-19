namespace NetworkSharp.Data.Models
{
    /// <summary>
    /// Represents a ping test result
    /// </summary>
    public class PingHistory
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Target { get; set; } = string.Empty;
        public int ResponseTime { get; set; } // milliseconds
        public string Status { get; set; } = string.Empty; // Success, Timeout, Error
    }
}
