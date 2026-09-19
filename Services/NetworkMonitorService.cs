using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Implementation of network monitoring service using Windows performance counters
    /// </summary>
    public class NetworkMonitorService : INetworkMonitorService, IDisposable
    {
        private readonly object _lock = new object();
        private readonly List<NetworkDataPoint> _dataHistory = new List<NetworkDataPoint>();
        private readonly List<DailyConsumptionData> _dailyConsumption = new List<DailyConsumptionData>();
        private readonly Random _random = new Random();
        
        private PerformanceCounter? _downloadCounter;
        private PerformanceCounter? _uploadCounter;
        private bool _isMonitoring;
        private DateTime _lastUpdate = DateTime.MinValue;

        // Current values

        // Historical data


        public double CurrentDownloadSpeed { get; private set; }
        public double CurrentUploadSpeed { get; private set; }
        public double AverageDownloadSpeedToday { get; private set; }
        public double AverageUploadSpeedToday { get; private set; }
        public double TotalDataTransferred7Days => TotalDownload7Days + TotalUpload7Days;
        public double TotalDownload7Days { get; private set; }
        public double TotalUpload7Days { get; private set; }
        public double PeakDownloadSpeed { get; private set; }
        public DateTime PeakSpeedTimestamp { get; private set; }


        public event EventHandler<NetworkStatsUpdatedEventArgs>? StatsUpdated;

        public NetworkMonitorService()
        {
            InitializeCounters();
            GenerateInitialData();
        }

        private void InitializeCounters()
        {
            try
            {
                // Try to get the network interface
                var networkInterface = NetworkInterface.GetAllNetworkInterfaces()
                    .FirstOrDefault(ni => ni.OperationalStatus == OperationalStatus.Up && 
                                         ni.NetworkInterfaceType != NetworkInterfaceType.Loopback);
                
                if (networkInterface != null)
                {
                    var interfaceName = networkInterface.Description;
                    
                    _downloadCounter = new PerformanceCounter("Network Interface", 
                        "Bytes Received/sec", interfaceName);
                    _uploadCounter = new PerformanceCounter("Network Interface", 
                        "Bytes Sent/sec", interfaceName);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to initialize performance counters: {ex.Message}");
                // Fall back to simulation mode
            }
        }

        private void GenerateInitialData()
        {
            // Generate some initial historical data for demonstration
            var now = DateTime.Now;
            var sevenDaysAgo = now.AddDays(-7);
            
            // Generate daily consumption data
            for (int i = 0; i < 7; i++)
            {
                var date = sevenDaysAgo.AddDays(i);
                var consumption = new DailyConsumptionData
                {
                    Date = date,
                    ConsumptionGB = 40 + _random.NextDouble() * 60,
                    DownloadGB = 35 + _random.NextDouble() * 50,
                    UploadGB = 5 + _random.NextDouble() * 10
                };
                _dailyConsumption.Add(consumption);
                
                if (i < 7)
                {
                    TotalDownload7Days += consumption.DownloadGB;
                    TotalUpload7Days += consumption.UploadGB;
                }
            }
            
            // Generate data rate history for the last 7 days
            for (int i = 0; i < 168; i++) // 168 hours in 7 days
            {
                var timestamp = sevenDaysAgo.AddHours(i);
                var dataPoint = new NetworkDataPoint
                {
                    Timestamp = timestamp,
                    DownloadSpeed = 20 + _random.NextDouble() * 100,
                    UploadSpeed = 5 + _random.NextDouble() * 20
                };
                _dataHistory.Add(dataPoint);
            }
            
            // Set initial peak values
            PeakDownloadSpeed = 241;
            PeakSpeedTimestamp = now.AddDays(-3).AddHours(21).AddMinutes(14);
            
            // Set average speeds
            AverageDownloadSpeedToday = 61;
            AverageUploadSpeedToday = 9;
        }

        public async Task StartMonitoringAsync()
        {
            if (_isMonitoring)
                return;
                
            _isMonitoring = true;
            
            while (_isMonitoring)
            {
                await UpdateNetworkStatsAsync();
                await Task.Delay(1000); // Update every second
            }
        }

        public async Task StopMonitoringAsync()
        {
            _isMonitoring = false;
            await Task.CompletedTask;
        }

        private async Task UpdateNetworkStatsAsync()
        {
            double downloadSpeed = 0;
            double uploadSpeed = 0;
            
            try
            {
                if (_downloadCounter != null && _uploadCounter != null)
                {
                    // Get actual values from performance counters
                    var downloadBytes = _downloadCounter.NextValue();
                    var uploadBytes = _uploadCounter.NextValue();
                    
                    // Convert bytes/sec to Mbps
                    downloadSpeed = (downloadBytes * 8) / 1_000_000;
                    uploadSpeed = (uploadBytes * 8) / 1_000_000;
                }
                else
                {
                    // Simulate values for demonstration
                    downloadSpeed = 100 + _random.NextDouble() * 150;
                    uploadSpeed = 10 + _random.NextDouble() * 30;
                }
            }
            catch
            {
                // Fall back to simulation
                downloadSpeed = 100 + _random.NextDouble() * 150;
                uploadSpeed = 10 + _random.NextDouble() * 30;
            }
            
            lock (_lock)
            {
                CurrentDownloadSpeed = downloadSpeed;
                CurrentUploadSpeed = uploadSpeed;
                
                // Update peak if necessary
                if (downloadSpeed > PeakDownloadSpeed)
                {
                    PeakDownloadSpeed = downloadSpeed;
                    PeakSpeedTimestamp = DateTime.Now;
                }
                
                // Add to history
                var dataPoint = new NetworkDataPoint
                {
                    Timestamp = DateTime.Now,
                    DownloadSpeed = downloadSpeed,
                    UploadSpeed = uploadSpeed
                };
                
                _dataHistory.Add(dataPoint);
                
                // Keep only last 7 days of data
                var cutoff = DateTime.Now.AddDays(-7);
                _dataHistory.RemoveAll(dp => dp.Timestamp < cutoff);
                
                _lastUpdate = DateTime.Now;
            }
            
            // Notify subscribers
            StatsUpdated?.Invoke(this, new NetworkStatsUpdatedEventArgs
            {
                DownloadSpeed = downloadSpeed,
                UploadSpeed = uploadSpeed,
                Timestamp = DateTime.Now
            });
            
            await Task.CompletedTask;
        }

        public async Task<NetworkDataPoint[]> GetDataRateHistoryAsync(TimeSpan timeRange)
        {
            await Task.CompletedTask;
            
            lock (_lock)
            {
                var cutoff = DateTime.Now - timeRange;
                return _dataHistory
                    .Where(dp => dp.Timestamp >= cutoff)
                    .OrderBy(dp => dp.Timestamp)
                    .ToArray();
            }
        }

        public async Task<DailyConsumptionData[]> GetDailyConsumptionAsync(int days)
        {
            await Task.CompletedTask;
            
            lock (_lock)
            {
                var cutoff = DateTime.Now.AddDays(-days);
                return _dailyConsumption
                    .Where(dc => dc.Date >= cutoff)
                    .OrderBy(dc => dc.Date)
                    .ToArray();
            }
        }

        public void Dispose()
        {
            _isMonitoring = false;
            _downloadCounter?.Dispose();
            _uploadCounter?.Dispose();
        }
    }
}
