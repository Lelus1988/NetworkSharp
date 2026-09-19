using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Implementation of ping service with graphing capabilities
    /// </summary>
    public class PingService : IPingService, IDisposable
    {
        private readonly object _lock = new object();
        private readonly List<PingDataPoint> _pingHistory = new List<PingDataPoint>();
        private readonly Random _random = new Random();
        
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isPinging;
        private string _currentTarget = string.Empty;


        public PingStatistics Statistics { get; private set; } = new PingStatistics
        {
            CurrentPing = 0,
            AveragePing = 0,
            MinimumPing = int.MaxValue,
            MaximumPing = 0,
            PacketLoss = 0,
            TotalPackets = 0,
            LostPackets = 0,
            StartTime = DateTime.Now
        };


        public event EventHandler<PingResultEventArgs>? PingResult;

        public PingService()
        {
        }

        public async Task StartPingAsync(string target, int intervalMs = 1000)
        {
            if (_isPinging)
                return;
                
            _isPinging = true;
            _currentTarget = target;
            _cancellationTokenSource = new CancellationTokenSource();
            
            // Reset statistics
            ResetStatistics();
            
            try
            {
                while (_isPinging && !_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    var result = await PerformPingAsync(target);
                    
                    lock (_lock)
                    {
                        UpdateStatistics(result);
                        
                        var dataPoint = new PingDataPoint
                        {
                            Timestamp = DateTime.Now,
                            ResponseTime = result.ResponseTime,
                            Success = result.Success
                        };
                        
                        _pingHistory.Add(dataPoint);
                        
                        // Keep only last 2 hours of data
                        var cutoff = DateTime.Now.AddHours(-2);
                        _pingHistory.RemoveAll(dp => dp.Timestamp < cutoff);
                    }
                    
                    PingResult?.Invoke(this, result);
                    
                    await Task.Delay(intervalMs, _cancellationTokenSource.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Ping was cancelled
            }
            finally
            {
                _isPinging = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public async Task StopPingAsync()
        {
            if (_isPinging && _cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                await Task.CompletedTask;
            }
        }

        private async Task<PingResultEventArgs> PerformPingAsync(string target)
        {
            var cleanedTarget = target?.Trim() ?? string.Empty;
            var result = new PingResultEventArgs
            {
                Target = cleanedTarget,
                Timestamp = DateTime.Now
            };

            if (string.IsNullOrWhiteSpace(cleanedTarget))
            {
                result.ResponseTime = -1;
                result.Success = false;
                return result;
            }

            try
            {
                if (cleanedTarget.Contains("//", StringComparison.Ordinal))
                {
                    if (Uri.TryCreate(cleanedTarget, UriKind.Absolute, out var uri))
                    {
                        cleanedTarget = uri.Host;
                        result.Target = cleanedTarget;
                    }
                }

                using var ping = new Ping();
                var reply = await ping.SendPingAsync(cleanedTarget, 2000); // 2 second timeout

                if (reply.Status == IPStatus.Success)
                {
                    result.ResponseTime = (int)reply.RoundtripTime;
                    result.Success = true;
                }
                else
                {
                    result.ResponseTime = -1;
                    result.Success = false;
                }
            }
            catch (Exception)
            {
                result.ResponseTime = -1;
                result.Success = false;
            }

            return result;
        }

        private void UpdateStatistics(PingResultEventArgs result)
        {
            Statistics.TotalPackets++;
            
            if (result.Success)
            {
                Statistics.CurrentPing = result.ResponseTime;
                
                // Update average
                var totalResponseTime = Statistics.AveragePing * (Statistics.TotalPackets - 1 - Statistics.LostPackets) + result.ResponseTime;
                var successfulPackets = Statistics.TotalPackets - Statistics.LostPackets;
                Statistics.AveragePing = successfulPackets > 0 ? totalResponseTime / successfulPackets : 0;
                
                // Update min/max
                if (result.ResponseTime < Statistics.MinimumPing)
                    Statistics.MinimumPing = result.ResponseTime;
                    
                if (result.ResponseTime > Statistics.MaximumPing)
                    Statistics.MaximumPing = result.ResponseTime;
            }
            else
            {
                Statistics.LostPackets++;
                Statistics.CurrentPing = -1;
            }
            
            // Calculate packet loss percentage
            Statistics.PacketLoss = Statistics.TotalPackets > 0 
                ? (double)Statistics.LostPackets / Statistics.TotalPackets * 100 
                : 0;
        }

        public List<PingDataPoint> GetPingHistory(TimeSpan timeRange)
        {
            lock (_lock)
            {
                var cutoff = DateTime.Now - timeRange;
                return _pingHistory
                    .Where(dp => dp.Timestamp >= cutoff)
                    .OrderBy(dp => dp.Timestamp)
                    .ToList();
            }
        }

        public void ResetStatistics()
        {
            lock (_lock)
            {
                Statistics = new PingStatistics
                {
                    CurrentPing = 0,
                    AveragePing = 0,
                    MinimumPing = int.MaxValue,
                    MaximumPing = 0,
                    PacketLoss = 0,
                    TotalPackets = 0,
                    LostPackets = 0,
                    StartTime = DateTime.Now
                };
                
                _pingHistory.Clear();
            }
        }

        public void Dispose()
        {
            _isPinging = false;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
