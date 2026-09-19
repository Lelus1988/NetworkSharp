using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using LiveCharts;
using NetworkSharp.Services;

namespace NetworkSharp.ViewModels
{
    /// <summary>
    /// ViewModel for the Ping feature
    /// </summary>
    public partial class PingViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly IPingService _pingService;
        private PingStatistics _statistics;

        public System.Collections.ObjectModel.ObservableCollection<string> Intervals { get; set; }

        public PingViewModel(IPingService pingService)
        {
            _pingService = pingService;

            Intervals = new ObservableCollection<string> { "1 s", "2 s", "5 s", "10 s" };

            _statistics = _pingService.Statistics;

            InitChart();

            _pingService.PingResult += OnPingResult;
        }

        private void OnPingResult(object? sender, PingResultEventArgs e)
        {
            try
            {
                var dispatcher = Application.Current?.Dispatcher;
                if (dispatcher == null)
                    return;

                if (!dispatcher.CheckAccess())
                {
                    dispatcher.BeginInvoke(new Action(() => OnPingResult(sender, e)));
                    return;
                }

                try
                {
                    _statistics = _pingService.Statistics;

                    OnPingResult(e.Success ? e.ResponseTime : null);

                    OnPropertyChanged(nameof(CurrentPing));
                    OnPropertyChanged(nameof(AveragePing));
                    OnPropertyChanged(nameof(MinimumPing));
                    OnPropertyChanged(nameof(MaximumPing));
                    OnPropertyChanged(nameof(PacketLoss));
                    OnPropertyChanged(nameof(TotalPackets));
                    OnPropertyChanged(nameof(LostPackets));
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error updating ping UI: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OnPingResult: {ex.Message}");
            }
        }

        public async Task StartPingAsync()
        {
            if (IsPinging)
                return;

            if (string.IsNullOrWhiteSpace(Target))
            {
                Target = "1.1.1.1";
            }

            try
            {
                IsPinging = true;
                ResetChart(Interval / 1000d);
                await _pingService.StartPingAsync(Target, Interval);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error starting ping: {ex.Message}");
                IsPinging = false;
            }
        }

        public async Task StopPingAsync()
        {
            try
            {
                await _pingService.StopPingAsync();
                IsPinging = false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping ping: {ex.Message}");
            }
        }

        public void ResetStatistics()
        {
            _pingService.ResetStatistics();
            _statistics = _pingService.Statistics;
            
            ResetChart(Interval / 1000d);
            
            OnPropertyChanged(nameof(CurrentPing));
            OnPropertyChanged(nameof(AveragePing));
            OnPropertyChanged(nameof(MinimumPing));
            OnPropertyChanged(nameof(MaximumPing));
            OnPropertyChanged(nameof(PacketLoss));
            OnPropertyChanged(nameof(TotalPackets));
            OnPropertyChanged(nameof(LostPackets));
        }

        public string Target
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = "1.1.1.1";

        public int Interval
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = 1000;

        public string IntervalDisplay
        {
            get;
            set
            {
                field = value;
                Interval = value switch
                {
                    "2 s" => 2000,
                    "5 s" => 5000,
                    "10 s" => 10000,
                    _ => 1000
                };
                OnPropertyChanged();
            }
        } = "1 s";

        public bool IsPinging
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int CurrentPing
        {
            get => _statistics.CurrentPing;
            set
            {
                if (_statistics.CurrentPing == value) return;
                _statistics.CurrentPing = value;
                OnPropertyChanged();
            }
        }

        public int AveragePing
        {
            get => _statistics.AveragePing;
            set
            {
                if (_statistics.AveragePing == value) return;
                _statistics.AveragePing = value;
                OnPropertyChanged();
            }
        }

        public int MinimumPing
        {
            get => _statistics.MinimumPing == int.MaxValue ? 0 : _statistics.MinimumPing;
            set
            {
                if (_statistics.MinimumPing == value) return;
                _statistics.MinimumPing = value;
                OnPropertyChanged();
            }
        }

        public int MaximumPing
        {
            get => _statistics.MaximumPing;
            set
            {
                if (_statistics.MaximumPing == value) return;
                _statistics.MaximumPing = value;
                OnPropertyChanged();
            }
        }

        public double PacketLoss
        {
            get => _statistics.PacketLoss;
            set
            {
                if (Math.Abs(_statistics.PacketLoss - value) < 0.0001) return;
                _statistics.PacketLoss = value;
                OnPropertyChanged();
            }
        }

        public int TotalPackets
        {
            get => _statistics.TotalPackets;
            set
            {
                if (_statistics.TotalPackets == value) return;
                _statistics.TotalPackets = value;
                OnPropertyChanged();
            }
        }

        public int LostPackets
        {
            get => _statistics.LostPackets;
            set
            {
                if (_statistics.LostPackets == value) return;
                _statistics.LostPackets = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            try
            {
                _pingService.PingResult -= OnPingResult;
            }
            catch
            {
                // ignore disposal race
            }
        }
    }
}
