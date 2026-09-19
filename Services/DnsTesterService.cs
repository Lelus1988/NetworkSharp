using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Implementation of DNS tester service
    /// </summary>
    public class DnsTesterService : IDnsTesterService
    {
        private readonly List<DnsServer> _predefinedServers = new List<DnsServer>();
        private readonly Random _random = new Random();
        
        public event EventHandler<DnsTestResultEventArgs>? TestCompleted;

        public DnsTesterService()
        {
            InitializePredefinedServers();
        }

        private void InitializePredefinedServers()
        {
            _predefinedServers.AddRange(new List<DnsServer>
            {
                new DnsServer { Name = "Router", Address = "192.168.178.1", IsCustom = false },
                new DnsServer { Name = "Cloudflare", Address = "1.1.1.1", IsCustom = false },
                new DnsServer { Name = "Google", Address = "8.8.8.8", IsCustom = false },
                new DnsServer { Name = "Quad9", Address = "9.9.9.9", IsCustom = false },
                new DnsServer { Name = "OpenDNS", Address = "208.67.222.222", IsCustom = false }
            });
        }

        public List<DnsServer> GetPredefinedServers()
        {
            return new List<DnsServer>(_predefinedServers);
        }

        public void AddCustomServer(DnsServer server)
        {
            server.IsCustom = true;
            _predefinedServers.Add(server);
        }

        public async Task<List<DnsTestResult>> TestDnsServersAsync(string domain, string recordType = "A")
        {
            var results = new List<DnsTestResult>();
            var tasks = new List<Task<DnsTestResult>>();
            
            // Test each DNS server
            foreach (var server in _predefinedServers)
            {
                tasks.Add(TestSingleServerAsync(server, domain, recordType, results.Count + 1));
            }
            
            // Wait for all tests to complete
            var testResults = await Task.WhenAll(tasks);
            results.AddRange(testResults);
            
            // Sort by response time
            results = results.OrderBy(r => r.ResponseTime).ToList();
            
            // Update ranks
            for (int i = 0; i < results.Count; i++)
            {
                results[i].Rank = i + 1;
            }
            
            TestCompleted?.Invoke(this, new DnsTestResultEventArgs { Results = results });
            
            return results;
        }

        private async Task<DnsTestResult> TestSingleServerAsync(DnsServer server, string domain, string recordType, int rank)
        {
            var result = new DnsTestResult
            {
                Server = server,
                Rank = rank
            };
            
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // For demonstration, we'll simulate DNS resolution
                // In a real implementation, this would use custom DNS queries
                
                // Simulate network delay
                await Task.Delay(_random.Next(3, 50));
                
                stopwatch.Stop();
                result.ResponseTime = (int)stopwatch.ElapsedMilliseconds;
                
                // Simulate result based on server
                if (server.Name == "Router")
                {
                    result.IsCached = true;
                    result.Result = "93.184.216.34"; // Example result
                    result.Success = true;
                }
                else
                {
                    result.IsCached = false;
                    result.Result = GetSimulatedIpAddress(domain);
                    result.Success = true;
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                result.ResponseTime = (int)stopwatch.ElapsedMilliseconds;
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            
            return result;
        }

        private string GetSimulatedIpAddress(string domain)
        {
            // Return a consistent IP address for the same domain
            var hash = domain.GetHashCode();
            var parts = new[]
            {
                Math.Abs(hash % 256),
                Math.Abs((hash >> 8) % 256),
                Math.Abs((hash >> 16) % 256),
                Math.Abs((hash >> 24) % 256)
            };
            
            return string.Join(".", parts);
        }
    }
}
