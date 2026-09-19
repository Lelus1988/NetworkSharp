using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    /// <summary>
    /// Service for managing WLAN profiles and passwords
    /// </summary>
    public interface IWlanProfileService
    {
        /// <summary>
        /// Get all WLAN profiles on this PC
        /// </summary>
        Task<List<WlanProfile>> GetProfilesAsync();
        
        /// <summary>
        /// Get password for a specific WLAN profile
        /// </summary>
        Task<string> GetProfilePasswordAsync(string profileName);
        
        /// <summary>
        /// Delete a WLAN profile
        /// </summary>
        Task<bool> DeleteProfileAsync(string profileName);
        
        /// <summary>
        /// Get signal strength for a profile
        /// </summary>
        Task<int> GetSignalStrengthAsync(string profileName);
        
        /// <summary>
        /// Check if the application has administrator privileges
        /// </summary>
        bool HasAdministratorPrivileges();
    }

    /// <summary>
    /// WLAN profile information
    /// </summary>
    public class WlanProfile
    {
        public string Name { get; set; } = string.Empty;
        public string SecurityType { get; set; } = string.Empty;
        public string Authentication { get; set; } = string.Empty;
        public string Encryption { get; set; } = string.Empty;
        public bool AutoConnect { get; set; }
        public DateTime LastConnected { get; set; }
        public int SignalStrength { get; set; }
        public bool IsConnected { get; set; }
    }
}
