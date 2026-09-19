using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using LiveCharts;
using LiveCharts.Wpf;
using NetworkSharp.Services;
using NetworkSharp.Common;

namespace NetworkSharp.ViewModels
{
    /// <summary>
    /// ViewModel for Speed Test feature
    /// </summary>
    public class SpeedTestViewModel : INotifyPropertyChanged
    {
        private readonly ISpeedTestService _speedTestService;


        public SeriesCollection SpeedSeries { get; set; }
        public ObservableCollection<string> Servers { get; set; }
        public ICommand StartTestCommand { get; private set; }
        
        public SpeedTestViewModel(ISpeedTestService speedTestService)
        {
            _speedTestService = speedTestService;
            
            SpeedSeries = new SeriesCollection();
            Servers = new ObservableCollection<string>();
            StartTestCommand = new AsyncRelayCommand(StartSpeedTestAsync, () => !IsTesting);
            
            InitializeCharts();
            LoadServersAsync();
        }

        private void InitializeCharts()
        {
            SpeedSeries.Clear();
            SpeedSeries.Add(new ColumnSeries
            {
                Title = "Download",
                Values = new ChartValues<double> { 0 },
                Fill = System.Windows.Media.Brushes.Cyan,
                Stroke = System.Windows.Media.Brushes.Transparent
            });
            SpeedSeries.Add(new ColumnSeries
            {
                Title = "Upload",
                Values = new ChartValues<double> { 0 },
                Fill = System.Windows.Media.Brushes.Orange,
                Stroke = System.Windows.Media.Brushes.Transparent
            });
        }

        private async void LoadServersAsync()
        {
            try
            {
                var servers = await _speedTestService.GetServersAsync();
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher == null)
                {
                    return;
                }

                await dispatcher.InvokeAsync(() =>
                {
                    Servers.Clear();
                    foreach (var server in servers)
                    {
                        Servers.Add($"{server.Name} ({server.Location}) - {server.Ping:F0}ms");
                    }

                    if (servers.Count > 0)
                    {
                        SelectedServer = $"{servers[0].Name} ({servers[0].Location})";
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading servers: {ex.Message}");
            }
        }

        public async Task StartSpeedTestAsync()
        {
            if (IsTesting)
                return;

            IsTesting = true;
            TestStatus = "Ping Test läuft...";
            TestProgress = 0;

            try
            {
                // Test Ping & Jitter
                var (ping, jitter) = await _speedTestService.TestPingJitterAsync();
                Ping = ping;
                Jitter = jitter;
                TestProgress = 20;

                // Test Download
                TestStatus = "Download Test läuft...";
                var downloadProgress = new Progress<double>(p =>
                {
                    TestProgress = 20 + (p * 30);
                });
                
                DownloadSpeed = await _speedTestService.TestDownloadSpeedAsync(10, downloadProgress);
                TestProgress = 50;

                // Test Upload
                TestStatus = "Upload Test läuft...";
                var uploadProgress = new Progress<double>(p =>
                {
                    TestProgress = 50 + (p * 50);
                });
                
                UploadSpeed = await _speedTestService.TestUploadSpeedAsync(10, uploadProgress);
                TestProgress = 100;

                // Update chart
                UpdateChart();
                
                TestStatus = "Test abgeschlossen";
            }
            catch (Exception ex)
            {
                TestStatus = $"Fehler: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Speed test error: {ex.Message}");
            }
            finally
            {
                IsTesting = false;
            }
        }

        private void UpdateChart()
        {
            var downloadValues = SpeedSeries[0].Values as ChartValues<double>;
            var uploadValues = SpeedSeries[1].Values as ChartValues<double>;
            
            if (downloadValues != null)
            {
                downloadValues.Clear();
                downloadValues.Add(DownloadSpeed);
            }
            
            if (uploadValues != null)
            {
                uploadValues.Clear();
                uploadValues.Add(UploadSpeed);
            }
        }

        public bool IsTesting
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
                (StartTestCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public double DownloadSpeed
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public double UploadSpeed
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public double Ping
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public double Jitter
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public string TestStatus
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = "Bereit";

        public string SelectedServer
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = "";

        public Func<double, string> YFormatter => value => $"{value:F0}";

        public double TestProgress
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
