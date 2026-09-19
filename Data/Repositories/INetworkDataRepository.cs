using NetworkSharp.Data.Models;

namespace NetworkSharp.Data.Repositories
{
    /// <summary>
    /// Interface for network data repository
    /// </summary>
    public interface INetworkDataRepository
    {
        // Network Statistics
        Task AddNetworkStatisticsAsync(NetworkStatistics statistics);
        Task<List<NetworkStatistics>> GetNetworkStatisticsAsync(DateTime from, DateTime to);
        Task<List<NetworkStatistics>> GetRecentNetworkStatisticsAsync(int count);

        // Ping History
        Task AddPingHistoryAsync(PingHistory pingHistory);
        Task<List<PingHistory>> GetPingHistoryAsync(string target, DateTime from, DateTime to);
        Task<List<PingHistory>> GetRecentPingHistoryAsync(string target, int count);

        // LAN Devices
        Task AddOrUpdateLanDeviceAsync(LanDeviceDb device);
        Task<List<LanDeviceDb>> GetLanDevicesAsync();
        Task<LanDeviceDb?> GetLanDeviceByIpAsync(string ipAddress);
        Task CleanupOldDevicesAsync(TimeSpan olderThan);

        // DNS Test Results
        Task AddDnsTestResultAsync(DnsTestResult result);
        Task<List<DnsTestResult>> GetDnsTestResultsAsync(DateTime from, DateTime to);

        // User Settings
        Task SetSettingAsync(string key, string value);
        Task<string?> GetSettingAsync(string key);
        Task<List<UserSetting>> GetAllSettingsAsync();

        // Database Maintenance
        Task InitializeDatabaseAsync();
        Task CleanupOldDataAsync(TimeSpan olderThan);
    }
}
