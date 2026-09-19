namespace NetworkSharp.Data.Models
{
    /// <summary>
    /// Represents a LAN device (database model)
    /// </summary>
    public class LanDeviceDb
    {
        public int Id { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public string MacAddress { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // PC, Router, Phone, etc.
        public DateTime LastSeen { get; set; }
        public int ResponseTime { get; set; } // milliseconds
        public string Status { get; set; } = string.Empty; // Online, Offline, Standby
    }
}
