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
    /// ViewModel for the Port Scanner feature
    /// </summary>
    public class PortScannerViewModel : INotifyPropertyChanged
    {
        private readonly IPortScannerService _portScannerService;
        private int _startPort = 1;
        private int _endPort = 1024;


        public ObservableCollection<PortScanResult> ScanResults { get; set; }
        public ObservableCollection<string> PortRanges { get; set; }
        
        public PortScannerViewModel(IPortScannerService portScannerService)
        {
            _portScannerService = portScannerService;
            
            ScanResults = new ObservableCollection<PortScanResult>();
            PortRanges = new ObservableCollection<string> { "Häufige", "1-1024", "Alle" };
            
            _portScannerService.ScanProgress += OnScanProgress;
            _portScannerService.PortScanned += OnPortScanned;
        }

        private void OnScanProgress(object? sender, PortScanProgressEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ScanProgress = e.CurrentPort;
                OpenPorts = e.OpenPorts;
                ClosedPorts = e.ClosedPorts;
                FilteredPorts = e.FilteredPorts;
            });
        }

        private void OnPortScanned(object? sender, PortScanResultEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                // Add or update result
                var existingResult = ScanResults.FirstOrDefault(r => r.Port == e.Port);
                if (existingResult != null)
                {
                    existingResult.Status = e.Status;
                    existingResult.ResponseTime = e.ResponseTime;
                }
                else
                {
                    ScanResults.Add(new PortScanResult
                    {
                        Port = e.Port,
                        Service = e.Service,
                        Status = e.Status,
                        ResponseTime = e.ResponseTime
                    });
                }
            });
        }

        public async Task StartScanAsync()
        {
            if (IsScanning)
                return;
                
            IsScanning = true;
            ScanResults.Clear();
            ScanProgress = 0;
            OpenPorts = 0;
            ClosedPorts = 0;
            FilteredPorts = 0;
            
            // Set port range based on selection
            switch (SelectedPortRange)
            {
                case "Häufige":
                    var commonPorts = _portScannerService.GetCommonPorts();
                    if (commonPorts.Any())
                    {
                        _startPort = commonPorts.Keys.Min();
                        _endPort = commonPorts.Keys.Max();
                    }
                    break;
                case "1-1024":
                    _startPort = 1;
                    _endPort = 1024;
                    break;
                case "Alle":
                    _startPort = 1;
                    _endPort = 65535;
                    break;
            }
            
            await _portScannerService.StartScanAsync(Target, _startPort, _endPort);
            IsScanning = false;
        }

        public async Task StopScanAsync()
        {
            await _portScannerService.StopScanAsync();
            IsScanning = false;
        }

        public string Target
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = "192.168.178.1";

        public int StartPort
        {
            get => _startPort;
            set
            {
                _startPort = value;
                OnPropertyChanged();
            }
        }

        public int EndPort
        {
            get => _endPort;
            set
            {
                _endPort = value;
                OnPropertyChanged();
            }
        }

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

        public int OpenPorts
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int ClosedPorts
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public int FilteredPorts
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public string SelectedPortRange
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = "1-1024";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
