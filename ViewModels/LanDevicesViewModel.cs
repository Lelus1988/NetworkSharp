using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using NetworkSharp.Services;

namespace NetworkSharp.ViewModels
{
    /// <summary>
    /// ViewModel for the LAN Devices feature
    /// </summary>
    public class LanDevicesViewModel : INotifyPropertyChanged
    {
        private readonly ILanDeviceService _lanDeviceService;
        private NetworkGatewayInfo _gatewayInfo;


        public ObservableCollection<LanDevice> Devices { get; set; }
        
        public LanDevicesViewModel(ILanDeviceService lanDeviceService)
        {
            _lanDeviceService = lanDeviceService;
            
            Devices = new ObservableCollection<LanDevice>();
            _gatewayInfo = new NetworkGatewayInfo();
            
            _lanDeviceService.ScanProgress += OnScanProgress;
            _lanDeviceService.DeviceDiscovered += OnDeviceDiscovered;
            
            // Load initial data
            LoadInitialData();
        }

        private async void LoadInitialData()
        {
            var devices = _lanDeviceService.GetDevices();
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                Devices.Clear();
                // Only show actually connected devices
                foreach (var device in devices.Where(d => d.Status == DeviceStatus.Online || d.Status == DeviceStatus.Standby))
                {
                    Devices.Add(device);
                }
                
                DevicesFound = devices.Count(d => d.Status == DeviceStatus.Online);
            });
            
            _gatewayInfo = await _lanDeviceService.GetGatewayInfoAsync();
            OnPropertyChanged(nameof(GatewayIP));
            OnPropertyChanged(nameof(Subnet));
        }

        private void OnScanProgress(object? sender, DeviceScanProgressEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ScanProgress = e.CurrentIP;
                DevicesFound = e.DevicesFound;
            });
        }

        private void OnDeviceDiscovered(object? sender, DeviceDiscoveredEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var existingDevice = Devices.FirstOrDefault(d => d.IPAddress == e.Device.IPAddress);
                if (existingDevice != null)
                {
                    // Update existing device
                    existingDevice.ResponseTime = e.Device.ResponseTime;
                    existingDevice.Status = e.Device.Status;
                    existingDevice.LastSeen = e.Device.LastSeen;
                }
                else
                {
                    // Add new device
                    Devices.Add(e.Device);
                }
            });
        }

        public async Task StartScanAsync()
        {
            if (IsScanning)
                return;
                
            IsScanning = true;
            ScanProgress = 0;
            
            var gatewayInfo = await _lanDeviceService.GetGatewayInfoAsync();
            await _lanDeviceService.StartScanAsync(gatewayInfo.Subnet);
            
            IsScanning = false;
        }

        public async Task StopScanAsync()
        {
            await _lanDeviceService.StopScanAsync();
            IsScanning = false;
        }

        public async Task RefreshGatewayInfoAsync()
        {
            _gatewayInfo = await _lanDeviceService.GetGatewayInfoAsync();
            OnPropertyChanged(nameof(GatewayIP));
            OnPropertyChanged(nameof(Subnet));
        }

        public string GatewayIP => _gatewayInfo.GatewayIP;
        public string Subnet => _gatewayInfo.Subnet;
        public string LocalIP => _gatewayInfo.LocalIP;
        public string DnsServer => _gatewayInfo.DnsServer;

        public bool IsScanning
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int ScanProgress
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int DevicesFound
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public LanDevice? SelectedDevice
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
