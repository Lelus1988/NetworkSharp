using System;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Service for IP address and GeoIP information
    /// </summary>
    public interface IIpGeoIpService
    {
        /// <summary>
        /// Get public IP address information
        /// </summary>
        Task<PublicIpInfo> GetPublicIpInfoAsync();
        
        /// <summary>
        /// Get local network information
        /// </summary>
        Task<LocalNetworkInfo> GetLocalNetworkInfoAsync();
        
        /// <summary>
        /// Get geographical location for an IP address
        /// </summary>
        Task<GeoIpInfo> GetGeoIpInfoAsync(string ipAddress);
        
        /// <summary>
        /// Refresh all IP information
        /// </summary>
        Task RefreshAllAsync();
    }

    /// <summary>
    /// Public IP address information
    /// </summary>
    public class PublicIpInfo
    {
        public string IPv4 { get; set; } = string.Empty;
        public string IPv6 { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Local network information
    /// </summary>
    public class LocalNetworkInfo
    {
        public string LocalIP { get; set; } = string.Empty;
        public string Gateway { get; set; } = string.Empty;
        public string Subnet { get; set; } = string.Empty;
        public string DnsServer { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string Hostname { get; set; } = string.Empty;
    }

    /// <summary>
    /// GeoIP information
    /// </summary>
    public class GeoIpInfo
    {
        public string IpAddress { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
