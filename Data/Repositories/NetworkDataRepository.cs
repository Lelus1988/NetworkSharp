using Microsoft.EntityFrameworkCore;
using NetworkSharp.Data.Models;

namespace NetworkSharp.Data.Repositories
{
    /// <summary>
    /// Implementation of network data repository
    /// </summary>
    public class NetworkDataRepository : INetworkDataRepository
    {
        private readonly NetworkSharpDbContext _context;

        public NetworkDataRepository(NetworkSharpDbContext context)
        {
            _context = context;
        }

        public async Task InitializeDatabaseAsync()
        {
            await _context.Database.EnsureCreatedAsync();
        }

        // Network Statistics
        public async Task AddNetworkStatisticsAsync(NetworkStatistics statistics)
        {
            _context.NetworkStatistics.Add(statistics);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NetworkStatistics>> GetNetworkStatisticsAsync(DateTime from, DateTime to)
        {
            return await _context.NetworkStatistics
                .Where(s => s.Timestamp >= from && s.Timestamp <= to)
                .OrderBy(s => s.Timestamp)
                .ToListAsync();
        }

        public async Task<List<NetworkStatistics>> GetRecentNetworkStatisticsAsync(int count)
        {
            return await _context.NetworkStatistics
                .OrderByDescending(s => s.Timestamp)
                .Take(count)
                .OrderBy(s => s.Timestamp)
                .ToListAsync();
        }

        // Ping History
        public async Task AddPingHistoryAsync(PingHistory pingHistory)
        {
            _context.PingHistory.Add(pingHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PingHistory>> GetPingHistoryAsync(string target, DateTime from, DateTime to)
        {
            return await _context.PingHistory
                .Where(p => p.Target == target && p.Timestamp >= from && p.Timestamp <= to)
                .OrderBy(p => p.Timestamp)
                .ToListAsync();
        }

        public async Task<List<PingHistory>> GetRecentPingHistoryAsync(string target, int count)
        {
            return await _context.PingHistory
                .Where(p => p.Target == target)
                .OrderByDescending(p => p.Timestamp)
                .Take(count)
                .OrderBy(p => p.Timestamp)
                .ToListAsync();
        }

        // LAN Devices
        public async Task AddOrUpdateLanDeviceAsync(LanDeviceDb device)
        {
            var existing = await _context.LanDevices
                .FirstOrDefaultAsync(d => d.IPAddress == device.IPAddress);

            if (existing != null)
            {
                existing.Name = device.Name;
                existing.MacAddress = device.MacAddress;
                existing.Manufacturer = device.Manufacturer;
                existing.Type = device.Type;
                existing.LastSeen = device.LastSeen;
                existing.ResponseTime = device.ResponseTime;
                existing.Status = device.Status;
            }
            else
            {
                _context.LanDevices.Add(device);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<LanDeviceDb>> GetLanDevicesAsync()
        {
            return await _context.LanDevices
                .OrderBy(d => d.IPAddress)
                .ToListAsync();
        }

        public async Task<LanDeviceDb?> GetLanDeviceByIpAsync(string ipAddress)
        {
            return await _context.LanDevices
                .FirstOrDefaultAsync(d => d.IPAddress == ipAddress);
        }

        public async Task CleanupOldDevicesAsync(TimeSpan olderThan)
        {
            var cutoffDate = DateTime.Now - olderThan;
            var oldDevices = await _context.LanDevices
                .Where(d => d.LastSeen < cutoffDate)
                .ToListAsync();

            _context.LanDevices.RemoveRange(oldDevices);
            await _context.SaveChangesAsync();
        }

        // DNS Test Results
        public async Task AddDnsTestResultAsync(DnsTestResult result)
        {
            _context.DnsTestResults.Add(result);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DnsTestResult>> GetDnsTestResultsAsync(DateTime from, DateTime to)
        {
            return await _context.DnsTestResults
                .Where(d => d.Timestamp >= from && d.Timestamp <= to)
                .OrderBy(d => d.Timestamp)
                .ToListAsync();
        }

        // User Settings
        public async Task SetSettingAsync(string key, string value)
        {
            var existing = await _context.UserSettings
                .FirstOrDefaultAsync(s => s.Key == key);

            if (existing != null)
            {
                existing.Value = value;
            }
            else
            {
                _context.UserSettings.Add(new UserSetting { Key = key, Value = value });
            }

            await _context.SaveChangesAsync();
        }

        public async Task<string?> GetSettingAsync(string key)
        {
            var setting = await _context.UserSettings
                .FirstOrDefaultAsync(s => s.Key == key);
            return setting?.Value;
        }

        public async Task<List<UserSetting>> GetAllSettingsAsync()
        {
            return await _context.UserSettings.ToListAsync();
        }

        // Database Maintenance
        public async Task CleanupOldDataAsync(TimeSpan olderThan)
        {
            var cutoffDate = DateTime.Now - olderThan;

            // Cleanup old network statistics
            var oldStats = await _context.NetworkStatistics
                .Where(s => s.Timestamp < cutoffDate)
                .ToListAsync();
            _context.NetworkStatistics.RemoveRange(oldStats);

            // Cleanup old ping history
            var oldPings = await _context.PingHistory
                .Where(p => p.Timestamp < cutoffDate)
                .ToListAsync();
            _context.PingHistory.RemoveRange(oldPings);

            // Cleanup old DNS results
            var oldDns = await _context.DnsTestResults
                .Where(d => d.Timestamp < cutoffDate)
                .ToListAsync();
            _context.DnsTestResults.RemoveRange(oldDns);

            await _context.SaveChangesAsync();
        }
    }
}
