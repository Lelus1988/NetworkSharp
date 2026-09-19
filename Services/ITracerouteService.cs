using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Service for performing traceroute operations
    /// </summary>
    public interface ITracerouteService
    {
        /// <summary>
        /// Event raised when a hop is completed
        /// </summary>
        event EventHandler<TracerouteHopEventArgs> HopCompleted;
        
        /// <summary>
        /// Start traceroute to a target
        /// </summary>
        Task<List<TracerouteHop>> StartTracerouteAsync(string target, int maxHops = 30);
        
        /// <summary>
        /// Stop the current traceroute
        /// </summary>
        Task StopTracerouteAsync();
    }

    /// <summary>
    /// Event arguments for traceroute hop completion
    /// </summary>
    public class TracerouteHopEventArgs : EventArgs
    {
        public TracerouteHop Hop { get; set; } = null!;
    }

    /// <summary>
    /// Represents a hop in traceroute
    /// </summary>
    public class TracerouteHop
    {
        public int HopNumber { get; set; }
        public string Hostname { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public int ResponseTime1 { get; set; }
        public int ResponseTime2 { get; set; }
        public int ResponseTime3 { get; set; }
        public bool Success { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
