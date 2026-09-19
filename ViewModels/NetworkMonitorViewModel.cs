using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using NetworkSharp.Services;

namespace NetworkSharp.ViewModels
{
    /// <summary>
    /// ViewModel for the Network Monitor feature
    /// </summary>
    public partial class NetworkMonitorViewModel : INotifyPropertyChanged
    {
        private readonly INetworkMonitorService _networkMonitorService;
        
        private double _currentDownloadSpeed;
        private double _currentUploadSpeed;
        private double _averageDownloadSpeedToday;
        private double _averageUploadSpeedToday;
        private double _totalDataTransferred7Days;
        private double _totalDownload7Days;
        private double _totalUpload7Days;
        private double _peakDownloadSpeed;
        private DateTime _peakSpeedTimestamp;
        private bool _isMonitoring;
        
        public NetworkMonitorViewModel(INetworkMonitorService networkMonitorService)
        {
            _networkMonitorService = networkMonitorService;
            
            InitializeData();
            InitChart();
            
            _networkMonitorService.StatsUpdated += OnNetworkStatsUpdated;
        }

        private void InitializeData()
        {
            _currentDownloadSpeed = _networkMonitorService.CurrentDownloadSpeed;
            _currentUploadSpeed = _networkMonitorService.CurrentUploadSpeed;
            _averageDownloadSpeedToday = _networkMonitorService.AverageDownloadSpeedToday;
            _averageUploadSpeedToday = _networkMonitorService.AverageUploadSpeedToday;
            _totalDataTransferred7Days = _networkMonitorService.TotalDataTransferred7Days;
            _totalDownload7Days = _networkMonitorService.TotalDownload7Days;
            _totalUpload7Days = _networkMonitorService.TotalUpload7Days;
            _peakDownloadSpeed = _networkMonitorService.PeakDownloadSpeed;
            _peakSpeedTimestamp = _networkMonitorService.PeakSpeedTimestamp;
        }

        private async void OnNetworkStatsUpdated(object? sender, NetworkStatsUpdatedEventArgs e)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                CurrentDownloadSpeed = e.DownloadSpeed;
                CurrentUploadSpeed = e.UploadSpeed;
            });
        }

        public async Task StartMonitoringAsync()
        {
            if (_isMonitoring)
                return;
                
            _isMonitoring = true;
            await _networkMonitorService.StartMonitoringAsync();
        }

        public async Task StopMonitoringAsync()
        {
            _isMonitoring = false;
            await _networkMonitorService.StopMonitoringAsync();
        }

        public double CurrentDownloadSpeed
        {
            get => _currentDownloadSpeed;
            set
            {
                _currentDownloadSpeed = value;
                OnPropertyChanged();
            }
        }

        public double CurrentUploadSpeed
        {
            get => _currentUploadSpeed;
            set
            {
                _currentUploadSpeed = value;
                OnPropertyChanged();
            }
        }

        public double AverageDownloadSpeedToday
        {
            get => _averageDownloadSpeedToday;
            set
            {
                _averageDownloadSpeedToday = value;
                OnPropertyChanged();
            }
        }

        public double AverageUploadSpeedToday
        {
            get => _averageUploadSpeedToday;
            set
            {
                _averageUploadSpeedToday = value;
                OnPropertyChanged();
            }
        }

        public double TotalDataTransferred7Days
        {
            get => _totalDataTransferred7Days;
            set
            {
                _totalDataTransferred7Days = value;
                OnPropertyChanged();
            }
        }

        public double TotalDownload7Days
        {
            get => _totalDownload7Days;
            set
            {
                _totalDownload7Days = value;
                OnPropertyChanged();
            }
        }

        public double TotalUpload7Days
        {
            get => _totalUpload7Days;
            set
            {
                _totalUpload7Days = value;
                OnPropertyChanged();
            }
        }

        public double PeakDownloadSpeed
        {
            get => _peakDownloadSpeed;
            set
            {
                _peakDownloadSpeed = value;
                OnPropertyChanged();
            }
        }

        public DateTime PeakSpeedTimestamp
        {
            get => _peakSpeedTimestamp;
            set
            {
                _peakSpeedTimestamp = value;
                OnPropertyChanged();
            }
        }

        public string SelectedTimeRange
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = "7 Tage";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
