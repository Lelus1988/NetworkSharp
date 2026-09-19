using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Implementation of traceroute service
    /// </summary>
    public class TracerouteService : ITracerouteService
    {
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isTracing;
        
        public event EventHandler<TracerouteHopEventArgs>? HopCompleted;

        public TracerouteService()
        {
        }

        public async Task<List<TracerouteHop>> StartTracerouteAsync(string target, int maxHops = 30)
        {
            if (_isTracing)
                return new List<TracerouteHop>();
                
            _isTracing = true;
            _cancellationTokenSource = new CancellationTokenSource();
            
            var hops = new List<TracerouteHop>();
            
            try
            {
                for (int ttl = 1; ttl <= maxHops; ttl++)
                {
                    if (_cancellationTokenSource.Token.IsCancellationRequested)
                        break;
                    
                    var hop = await TraceHopAsync(target, ttl);
                    hops.Add(hop);
                    
                    HopCompleted?.Invoke(this, new TracerouteHopEventArgs { Hop = hop });
                    
                    // Stop if we reached the target
                    if (hop.Success && !string.IsNullOrEmpty(hop.IPAddress) && 
                        (hop.IPAddress == target || hop.Hostname == target))
                    {
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Traceroute was cancelled
            }
            finally
            {
                _isTracing = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
            
            return hops;
        }

        public async Task StopTracerouteAsync()
        {
            if (_isTracing && _cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                await Task.CompletedTask;
            }
        }

        private async Task<TracerouteHop> TraceHopAsync(string target, int ttl)
        {
            var hop = new TracerouteHop
            {
                HopNumber = ttl,
                Status = "Timeout"
            };
            
            try
            {
                using var ping = new Ping();
                
                // Set TTL
                var pingOptions = new PingOptions(ttl, true);
                
                // Send 3 packets for better accuracy
                var reply1 = await ping.SendPingAsync(target, 2000, new byte[32], pingOptions);
                hop.ResponseTime1 = (int)reply1.RoundtripTime;
                
                if (reply1.Status == IPStatus.Success || reply1.Status == IPStatus.TtlExpired)
                {
                    hop.IPAddress = reply1.Address?.ToString() ?? "Unknown";
                    hop.Success = reply1.Status == IPStatus.Success;
                    hop.Status = reply1.Status.ToString();
                    
                    try
                    {
                        if (reply1.Address != null)
                        {
                            var hostEntry = System.Net.Dns.GetHostEntry(reply1.Address);
                            hop.Hostname = hostEntry.HostName;
                        }
                    }
                    catch
                    {
                        hop.Hostname = hop.IPAddress;
                    }
                    
                    // Send additional packets
                    var reply2 = await ping.SendPingAsync(target, 2000, new byte[32], pingOptions);
                    hop.ResponseTime2 = (int)reply2.RoundtripTime;
                    
                    var reply3 = await ping.SendPingAsync(target, 2000, new byte[32], pingOptions);
                    hop.ResponseTime3 = (int)reply3.RoundtripTime;
                }
            }
            catch (Exception ex)
            {
                hop.Status = $"Error: {ex.Message}";
            }
            
            return hop;
        }
    }
}
