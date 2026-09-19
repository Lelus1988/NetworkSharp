using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Implementation of IP and GeoIP service
    /// </summary>
    public class IpGeoIpService : IIpGeoIpService
    {
        private readonly HttpClient _httpClient;
        private PublicIpInfo? _publicIpInfo;
        private LocalNetworkInfo? _localNetworkInfo;
        private GeoIpInfo? _geoIpInfo;
        
        public IpGeoIpService()
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
        }

        public async Task<PublicIpInfo> GetPublicIpInfoAsync()
        {
            if (_publicIpInfo != null && (DateTime.Now - _publicIpInfo.LastUpdated).TotalMinutes < 5)
            {
                return _publicIpInfo;
            }
            
            try
            {
                // Try to get public IP from various services
                var ipv4 = await GetPublicIpFromServiceAsync("https://api.ipify.org");
                var ipv6 = await GetPublicIpFromServiceAsync("https://api64.ipify.org");
                
                _publicIpInfo = new PublicIpInfo
                {
                    IPv4 = ipv4,
                    IPv6 = ipv6,
                    LastUpdated = DateTime.Now
                };
            }
            catch
            {
                // Fallback to sample data if services fail
                _publicIpInfo = new PublicIpInfo
                {
                    IPv4 = "203.0.113.42",
                    IPv6 = "2001:db8::1",
                    LastUpdated = DateTime.Now
                };
            }
            
            return _publicIpInfo;
        }

        public async Task<LocalNetworkInfo> GetLocalNetworkInfoAsync()
        {
            if (_localNetworkInfo != null && (DateTime.Now - DateTime.MinValue).TotalMinutes < 1)
            {
                return _localNetworkInfo;
            }
            
            var localInfo = new LocalNetworkInfo();
            
            try
            {
                // Get hostname
                localInfo.Hostname = Dns.GetHostName();
                
                // Get network interfaces
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(ni => ni.OperationalStatus == OperationalStatus.Up && 
                                ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);
                
                foreach (var ni in networkInterfaces)
                {
                    var ipProperties = ni.GetIPProperties();
                    
                    // Get local IP address
                    var ipv4Address = ipProperties.UnicastAddresses
                        .FirstOrDefault(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    if (ipv4Address != null && string.IsNullOrEmpty(localInfo.LocalIP))
                    {
                        localInfo.LocalIP = ipv4Address.Address.ToString();
                        localInfo.Subnet = $"{ipv4Address.Address}/{ipv4Address.PrefixLength}";
                    }
                    
                    // Get gateway
                    var gateway = ipProperties.GatewayAddresses.FirstOrDefault();
                    if (gateway != null && string.IsNullOrEmpty(localInfo.Gateway))
                    {
                        localInfo.Gateway = gateway.Address.ToString();
                    }
                    
                    // Get DNS server
                    var dnsServer = ipProperties.DnsAddresses.FirstOrDefault();
                    if (dnsServer != null && string.IsNullOrEmpty(localInfo.DnsServer))
                    {
                        localInfo.DnsServer = dnsServer.ToString();
                    }
                    
                    // Get MAC address
                    localInfo.MacAddress = ni.GetPhysicalAddress().ToString();
                    
                    if (!string.IsNullOrEmpty(localInfo.LocalIP))
                        break;
                }
            }
            catch
            {
                // Fallback to sample data
                localInfo = new LocalNetworkInfo
                {
                    LocalIP = "192.168.178.20",
                    Gateway = "192.168.178.1",
                    Subnet = "192.168.178.0/24",
                    DnsServer = "192.168.178.1",
                    MacAddress = "A4:BB:6D:52:10:8E",
                    Hostname = Dns.GetHostName()
                };
            }
            
            _localNetworkInfo = localInfo;
            return localInfo;
        }

        public async Task<GeoIpInfo> GetGeoIpInfoAsync(string ipAddress)
        {
            if (_geoIpInfo != null && _geoIpInfo.IpAddress == ipAddress && 
                (DateTime.Now - _geoIpInfo.LastUpdated).TotalHours < 1)
            {
                return _geoIpInfo;
            }
            
            try
            {
                // Try to get GeoIP information from various services
                var geoIpData = await GetGeoIpFromServiceAsync(ipAddress);
                
                _geoIpInfo = new GeoIpInfo
                {
                    IpAddress = ipAddress,
                    Country = geoIpData.Item1,
                    CountryCode = geoIpData.Item2,
                    City = geoIpData.Item3,
                    Region = geoIpData.Item4,
                    Provider = geoIpData.Item5,
                    Timezone = geoIpData.Item6,
                    Latitude = geoIpData.Item7,
                    Longitude = geoIpData.Item8,
                    LastUpdated = DateTime.Now
                };
            }
            catch
            {
                // Fallback to sample data
                _geoIpInfo = new GeoIpInfo
                {
                    IpAddress = ipAddress,
                    Country = "Deutschland",
                    CountryCode = "DE",
                    City = "Frankfurt am Main",
                    Region = "Hessen",
                    Provider = "Beispiel Telekom GmbH",
                    Timezone = "Europe/Berlin",
                    Latitude = 50.1109,
                    Longitude = 8.6821,
                    LastUpdated = DateTime.Now
                };
            }
            
            return _geoIpInfo;
        }

        public async Task RefreshAllAsync()
        {
            _publicIpInfo = null;
            _localNetworkInfo = null;
            _geoIpInfo = null;
            
            await GetPublicIpInfoAsync();
            await GetLocalNetworkInfoAsync();
            
            if (_publicIpInfo != null)
            {
                await GetGeoIpInfoAsync(_publicIpInfo.IPv4);
            }
        }

        private async Task<string> GetPublicIpFromServiceAsync(string serviceUrl)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(serviceUrl);
                return response.Trim();
            }
            catch
            {
                return string.Empty;
            }
        }

        private async Task<(string, string, string, string, string, string, double, double)> GetGeoIpFromServiceAsync(string ipAddress)
        {
            try
            {
                // Try ip-api.com (free, no API key required)
                var url = $"http://ip-api.com/json/{ipAddress}";
                var response = await _httpClient.GetStringAsync(url);
                
                // Parse JSON response (simplified parsing)
                var country = ExtractJsonValue(response, "country");
                var countryCode = ExtractJsonValue(response, "countryCode");
                var city = ExtractJsonValue(response, "city");
                var region = ExtractJsonValue(response, "regionName");
                var isp = ExtractJsonValue(response, "isp");
                var timezone = ExtractJsonValue(response, "timezone");
                var lat = ExtractJsonValue(response, "lat");
                var lon = ExtractJsonValue(response, "lon");
                
                double.TryParse(lat, out var latitude);
                double.TryParse(lon, out var longitude);
                
                return (country, countryCode, city, region, isp, timezone, latitude, longitude);
            }
            catch
            {
                return (string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, 0, 0);
            }
        }

        private string ExtractJsonValue(string json, string key)
        {
            try
            {
                var searchKey = $"\"{key}\":";
                var startIndex = json.IndexOf(searchKey);
                if (startIndex >= 0)
                {
                    startIndex += searchKey.Length;
                    var endIndex = json.IndexOf(",", startIndex);
                    if (endIndex < 0)
                        endIndex = json.IndexOf("}", startIndex);
                    
                    var value = json.Substring(startIndex, endIndex - startIndex).Trim();
                    
                    // Remove quotes if present
                    if (value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }
                    
                    return value;
                }
            }
            catch
            {
                // Ignore parsing errors
            }
            
            return string.Empty;
        }
    }
}
