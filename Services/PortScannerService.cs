using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Implementation of port scanner service
    /// </summary>
    public class PortScannerService : IPortScannerService
    {
        private readonly object _lock = new object();
        private readonly List<PortScanResult> _results = new List<PortScanResult>();
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isScanning;
        
        public event EventHandler<PortScanProgressEventArgs>? ScanProgress;
        public event EventHandler<PortScanResultEventArgs>? PortScanned;

        public PortScannerService()
        {
        }

        public Dictionary<int, string> GetCommonPorts()
        {
            return new Dictionary<int, string>
            {
                { 21, "FTP" },
                { 22, "SSH" },
                { 23, "Telnet" },
                { 25, "SMTP" },
                { 53, "DNS" },
                { 80, "HTTP" },
                { 110, "POP3" },
                { 143, "IMAP" },
                { 443, "HTTPS" },
                { 445, "SMB" },
                { 993, "IMAPS" },
                { 995, "POP3S" },
                { 1433, "MSSQL" },
                { 3306, "MySQL" },
                { 3389, "RDP" },
                { 5432, "PostgreSQL" },
                { 5900, "VNC" },
                { 6379, "Redis" },
                { 8080, "HTTP-Alt" },
                { 8443, "HTTPS-Alt" },
                { 27017, "MongoDB" }
            };
        }

        public async Task StartScanAsync(string target, int startPort, int endPort)
        {
            if (_isScanning)
                return;
                
            _isScanning = true;
            _cancellationTokenSource = new CancellationTokenSource();
            
            lock (_lock)
            {
                _results.Clear();
            }
            
            try
            {
                var commonPorts = GetCommonPorts();
                int totalPorts = endPort - startPort + 1;
                int currentPort = startPort;
                int openPorts = 0;
                int closedPorts = 0;
                int filteredPorts = 0;
                
                // Use parallel scanning for better performance
                var options = new ParallelOptions
                {
                    CancellationToken = _cancellationTokenSource.Token,
                    MaxDegreeOfParallelism = 50 // Limit concurrent connections
                };
                
                await Task.Run(() =>
                {
                    Parallel.For(startPort, endPort + 1, options, (port, state) =>
                    {
                        if (_cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            state.Stop();
                            return;
                        }
                        
                        var result = ScanPort(target, port, commonPorts);
                        
                        lock (_lock)
                        {
                            _results.Add(result);
                            
                            switch (result.Status)
                            {
                                case PortStatus.Open:
                                    openPorts++;
                                    break;
                                case PortStatus.Closed:
                                    closedPorts++;
                                    break;
                                case PortStatus.Filtered:
                                    filteredPorts++;
                                    break;
                            }
                            
                            currentPort = port;
                        }
                        
                        // Raise event for this port
                        PortScanned?.Invoke(this, new PortScanResultEventArgs
                        {
                            Port = result.Port,
                            Service = result.Service,
                            Status = result.Status,
                            ResponseTime = result.ResponseTime
                        });
                        
                        // Raise progress event
                        ScanProgress?.Invoke(this, new PortScanProgressEventArgs
                        {
                            CurrentPort = port,
                            TotalPorts = totalPorts,
                            OpenPorts = openPorts,
                            ClosedPorts = closedPorts,
                            FilteredPorts = filteredPorts
                        });
                    });
                }, _cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
                // Scan was cancelled
            }
            finally
            {
                _isScanning = false;
                _cancellationTokenSource?.Dispose();
                _cancellationTokenSource = null;
            }
        }

        public async Task StopScanAsync()
        {
            if (_isScanning && _cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                await Task.CompletedTask;
            }
        }

        private PortScanResult ScanPort(string target, int port, Dictionary<int, string> commonPorts)
        {
            var result = new PortScanResult
            {
                Port = port,
                Service = commonPorts.TryGetValue(port, out var service) ? service : "Unknown"
            };
            
            try
            {
                using var tcpClient = new TcpClient();
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                var connectTask = tcpClient.ConnectAsync(target, port);
                var timeoutTask = Task.Delay(2000); // 2 second timeout
                
                var completedTask = Task.WhenAny(connectTask, timeoutTask).Result;
                
                stopwatch.Stop();
                result.ResponseTime = (int)stopwatch.ElapsedMilliseconds;
                
                if (completedTask == connectTask)
                {
                    if (tcpClient.Connected)
                    {
                        result.Status = PortStatus.Open;
                    }
                    else
                    {
                        result.Status = PortStatus.Closed;
                    }
                }
                else
                {
                    result.Status = PortStatus.Filtered;
                    result.ResponseTime = -1; // Timeout
                }
            }
            catch (Exception)
            {
                result.Status = PortStatus.Closed;
                result.ResponseTime = -1;
            }
            
            return result;
        }

        public List<PortScanResult> GetResults()
        {
            lock (_lock)
            {
                return new List<PortScanResult>(_results);
            }
        }
    }
}
