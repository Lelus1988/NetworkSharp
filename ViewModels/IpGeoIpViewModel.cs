using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using NetworkSharp.Services;

namespace NetworkSharp.ViewModels
{
    /// <summary>
    /// ViewModel for the IP & GeoIP feature
    /// </summary>
    public class IpGeoIpViewModel : INotifyPropertyChanged
    {
        private readonly IIpGeoIpService _ipGeoIpService;
        private PublicIpInfo _publicIpInfo;
        private LocalNetworkInfo _localNetworkInfo;
        private GeoIpInfo _geoIpInfo;
        
        public IpGeoIpViewModel(IIpGeoIpService ipGeoIpService)
        {
            _ipGeoIpService = ipGeoIpService;
            
            _publicIpInfo = new PublicIpInfo();
            _localNetworkInfo = new LocalNetworkInfo();
            _geoIpInfo = new GeoIpInfo();
            
            // Load data asynchronously
            _ = LoadAllDataAsync();
        }

        public async Task LoadAllDataAsync()
        {
            IsLoading = true;
            
            try
            {
                await _ipGeoIpService.RefreshAllAsync();
                
                _publicIpInfo = await _ipGeoIpService.GetPublicIpInfoAsync();
                _localNetworkInfo = await _ipGeoIpService.GetLocalNetworkInfoAsync();
                
                if (!string.IsNullOrEmpty(_publicIpInfo.IPv4))
                {
                    _geoIpInfo = await _ipGeoIpService.GetGeoIpInfoAsync(_publicIpInfo.IPv4);
                }
                
                // Notify all properties changed
                OnPropertyChanged(nameof(PublicIPv4));
                OnPropertyChanged(nameof(PublicIPv6));
                OnPropertyChanged(nameof(LocalIP));
                OnPropertyChanged(nameof(Gateway));
                OnPropertyChanged(nameof(Subnet));
                OnPropertyChanged(nameof(DnsServer));
                OnPropertyChanged(nameof(Country));
                OnPropertyChanged(nameof(City));
                OnPropertyChanged(nameof(Provider));
                OnPropertyChanged(nameof(Timezone));
                OnPropertyChanged(nameof(Latitude));
                OnPropertyChanged(nameof(Longitude));
            }
            catch
            {
                // Handle error
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task RefreshAsync()
        {
            await LoadAllDataAsync();
        }

        public string PublicIPv4
        {
            get => _publicIpInfo.IPv4;
            set
            {
                if (_publicIpInfo.IPv4 == value) return;
                _publicIpInfo.IPv4 = value;
                OnPropertyChanged();
            }
        }

        public string PublicIPv6
        {
            get => _publicIpInfo.IPv6;
            set
            {
                if (_publicIpInfo.IPv6 == value) return;
                _publicIpInfo.IPv6 = value;
                OnPropertyChanged();
            }
        }

        public string LocalIP
        {
            get => _localNetworkInfo.LocalIP;
            set
            {
                if (_localNetworkInfo.LocalIP == value) return;
                _localNetworkInfo.LocalIP = value;
                OnPropertyChanged();
            }
        }

        public string Gateway
        {
            get => _localNetworkInfo.Gateway;
            set
            {
                if (_localNetworkInfo.Gateway == value) return;
                _localNetworkInfo.Gateway = value;
                OnPropertyChanged();
            }
        }

        public string Subnet
        {
            get => _localNetworkInfo.Subnet;
            set
            {
                if (_localNetworkInfo.Subnet == value) return;
                _localNetworkInfo.Subnet = value;
                OnPropertyChanged();
            }
        }

        public string DnsServer
        {
            get => _localNetworkInfo.DnsServer;
            set
            {
                if (_localNetworkInfo.DnsServer == value) return;
                _localNetworkInfo.DnsServer = value;
                OnPropertyChanged();
            }
        }

        public string Country
        {
            get => _geoIpInfo.Country;
            set
            {
                if (_geoIpInfo.Country == value) return;
                _geoIpInfo.Country = value;
                OnPropertyChanged();
            }
        }

        public string City
        {
            get => _geoIpInfo.City;
            set
            {
                if (_geoIpInfo.City == value) return;
                _geoIpInfo.City = value;
                OnPropertyChanged();
            }
        }

        public string Provider
        {
            get => _geoIpInfo.Provider;
            set
            {
                if (_geoIpInfo.Provider == value) return;
                _geoIpInfo.Provider = value;
                OnPropertyChanged();
            }
        }

        public string Timezone
        {
            get => _geoIpInfo.Timezone;
            set
            {
                if (_geoIpInfo.Timezone == value) return;
                _geoIpInfo.Timezone = value;
                OnPropertyChanged();
            }
        }

        public double Latitude
        {
            get => _geoIpInfo.Latitude;
            set
            {
                if (Math.Abs(_geoIpInfo.Latitude - value) < 0.0000001) return;
                _geoIpInfo.Latitude = value;
                OnPropertyChanged();
            }
        }

        public double Longitude
        {
            get => _geoIpInfo.Longitude;
            set
            {
                if (Math.Abs(_geoIpInfo.Longitude - value) < 0.0000001) return;
                _geoIpInfo.Longitude = value;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
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
