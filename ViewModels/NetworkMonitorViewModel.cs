using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using NetworkSharp.Services;

namespace NetworkSharp.ViewModels
{
    /// <summary>
    /// ViewModel for the Network Monitor feature
    /// </summary>
    public class NetworkMonitorViewModel : INotifyPropertyChanged
    {
        private readonly INetworkMonitorService _networkMonitorService;
        private readonly Random _random = new Random();
        
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
        
        public SeriesCollection DownloadUploadSeries { get; set; }
        public SeriesCollection DailyConsumptionSeries { get; set; }
        public ObservableCollection<string> TimeRanges { get; set; }
        
        public NetworkMonitorViewModel(INetworkMonitorService networkMonitorService)
        {
            _networkMonitorService = networkMonitorService;
            
            DownloadUploadSeries = new SeriesCollection();
            DailyConsumptionSeries = new SeriesCollection();
            TimeRanges = new ObservableCollection<string> { "Live", "24 Std", "7 Tage", "30 Tage" };
            
            InitializeData();
            InitializeCharts();
            
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

        private void InitializeCharts()
        {
            // Initialize download/upload chart
            DownloadUploadSeries.Clear();
            DownloadUploadSeries.Add(new LineSeries
            {
                Title = "Download",
                Values = new ChartValues<double> { 120, 135, 148, 142, 156, 149, 148 },
                PointGeometry = null,
                Fill = System.Windows.Media.Brushes.Transparent,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(137, 220, 235)),
                StrokeThickness = 2
            });
            
            DownloadUploadSeries.Add(new LineSeries
            {
                Title = "Upload",
                Values = new ChartValues<double> { 18, 22, 19, 25, 21, 23, 22 },
                PointGeometry = null,
                Fill = System.Windows.Media.Brushes.Transparent,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(203, 166, 247)),
                StrokeThickness = 2
            });
            
            // Initialize daily consumption chart
            DailyConsumptionSeries.Clear();
            DailyConsumptionSeries.Add(new ColumnSeries
            {
                Title = "Verbrauch",
                Values = new ChartValues<double> { 48, 55, 42, 96, 71, 52, 48 },
                Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(137, 220, 235))
            });
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

        public async Task UpdateChartDataAsync(string timeRange)
        {
            SelectedTimeRange = timeRange;
            
            TimeSpan timeSpan;
            switch (timeRange)
            {
                case "Live":
                    timeSpan = TimeSpan.FromMinutes(1);
                    break;
                case "24 Std":
                    timeSpan = TimeSpan.FromHours(24);
                    break;
                case "7 Tage":
                    timeSpan = TimeSpan.FromDays(7);
                    break;
                case "30 Tage":
                    timeSpan = TimeSpan.FromDays(30);
                    break;
                default:
                    timeSpan = TimeSpan.FromDays(7);
                    break;
            }
            
            var dataHistory = await _networkMonitorService.GetDataRateHistoryAsync(timeSpan);
            
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var downloadValues = new ChartValues<double>();
                var uploadValues = new ChartValues<double>();
                
                foreach (var dataPoint in dataHistory)
                {
                    downloadValues.Add(dataPoint.DownloadSpeed);
                    uploadValues.Add(dataPoint.UploadSpeed);
                }
                
                DownloadUploadSeries[0].Values = downloadValues;
                DownloadUploadSeries[1].Values = uploadValues;
            });
            
            // Update daily consumption for 30 days view
            if (timeSpan == TimeSpan.FromDays(30))
            {
                var dailyConsumption = await _networkMonitorService.GetDailyConsumptionAsync(30);
                
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    var consumptionValues = new ChartValues<double>();
                    foreach (var day in dailyConsumption)
                    {
                        consumptionValues.Add(day.ConsumptionGB);
                    }
                    
                    DailyConsumptionSeries[0].Values = consumptionValues;
                });
            }
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
