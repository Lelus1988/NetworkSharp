namespace NetworkSharp.Data.Models
{
    /// <summary>
    /// Represents a DNS test result
    /// </summary>
    public class DnsTestResult
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Server { get; set; } = string.Empty; // DNS server address
        public string Query { get; set; } = string.Empty; // Domain queried
        public int ResponseTime { get; set; } // milliseconds
        public string Result { get; set; } = string.Empty; // Resolved IP or error
    }
}
