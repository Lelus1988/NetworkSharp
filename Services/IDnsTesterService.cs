using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Service for testing DNS server performance
    /// </summary>
    public interface IDnsTesterService
    {
        /// <summary>
        /// Event raised when DNS test is completed
        /// </summary>
        event EventHandler<DnsTestResultEventArgs> TestCompleted;
        
        /// <summary>
        /// Test DNS resolution for a domain using multiple servers
        /// </summary>
        Task<List<DnsTestResult>> TestDnsServersAsync(string domain, string recordType = "A");
        
        /// <summary>
        /// Get predefined DNS servers to test
        /// </summary>
        List<DnsServer> GetPredefinedServers();
        
        /// <summary>
        /// Add a custom DNS server
        /// </summary>
        void AddCustomServer(DnsServer server);
    }

    /// <summary>
    /// Event arguments for DNS test results
    /// </summary>
    public class DnsTestResultEventArgs : EventArgs
    {
        public List<DnsTestResult> Results { get; set; } = new List<DnsTestResult>();
    }

    /// <summary>
    /// DNS server information
    /// </summary>
    public class DnsServer
    {
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsCustom { get; set; }
    }

    /// <summary>
    /// Result of DNS test
    /// </summary>
    public class DnsTestResult
    {
        public int Rank { get; set; }
        public DnsServer Server { get; set; } = null!;
        public string Result { get; set; } = string.Empty;
        public int ResponseTime { get; set; }
        public bool IsCached { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
