using System;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Service for WHOIS domain information lookup
    /// </summary>
    public interface IWhoisService
    {
        /// <summary>
        /// Get WHOIS information for a domain
        /// </summary>
        Task<WhoisInfo> GetWhoisInfoAsync(string domain);
        
        /// <summary>
        /// Get WHOIS information for an IP address
        /// </summary>
        Task<WhoisInfo> GetWhoisInfoByIpAsync(string ipAddress);
    }

    /// <summary>
    /// WHOIS information
    /// </summary>
    public class WhoisInfo
    {
        public string Domain { get; set; } = string.Empty;
        public string Registrar { get; set; } = string.Empty;
        public string CreatedDate { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
        public string UpdatedDate { get; set; } = string.Empty;
        public string NameServers { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Dnssec { get; set; } = string.Empty;
        public string Organization { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
