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
    /// ViewModel for the DNS Tester feature
    /// </summary>
    public class DnsTesterViewModel : INotifyPropertyChanged
    {
        private readonly IDnsTesterService _dnsTesterService;


        public ObservableCollection<DnsTestResult> TestResults { get; set; }
        public ObservableCollection<string> RecordTypes { get; set; }
        public ObservableCollection<DnsServer> DnsServers { get; set; }
        
        public DnsTesterViewModel(IDnsTesterService dnsTesterService)
        {
            _dnsTesterService = dnsTesterService;
            
            TestResults = new ObservableCollection<DnsTestResult>();
            RecordTypes = new ObservableCollection<string> { "A", "AAAA", "MX", "TXT", "NS" };
            DnsServers = new ObservableCollection<DnsServer>(_dnsTesterService.GetPredefinedServers());
            
            _dnsTesterService.TestCompleted += OnTestCompleted;
        }

        private void OnTestCompleted(object? sender, DnsTestResultEventArgs e)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null)
            {
                return;
            }

            dispatcher.Invoke(() =>
            {
                TestResults.Clear();
                foreach (var result in e.Results)
                {
                    TestResults.Add(result);
                }

                if (e.Results.Any(r => r.Success))
                {
                    var fastest = e.Results.Where(r => r.Success).OrderBy(r => r.ResponseTime).First();
                    FastestServer = fastest.Server.Name;
                    FastestTime = $"{fastest.ResponseTime} ms";

                    if (fastest.IsCached)
                    {
                        SummaryMessage = $"Schnellster Server: {fastest.Server.Name}, {fastest.ResponseTime} ms (aus Cache)";

                        var fastestNonCached = e.Results.Where(r => r.Success && !r.IsCached).OrderBy(r => r.ResponseTime).FirstOrDefault();
                        if (fastestNonCached != null)
                        {
                            SummaryMessage += $"\nBei nicht zwischengespeicherten Abfragen liegt {fastestNonCached.Server.Name} mit {fastestNonCached.ResponseTime} ms vorn.";
                        }
                    }
                    else
                    {
                        SummaryMessage = $"Schnellster Server: {fastest.Server.Name}, {fastest.ResponseTime} ms";
                    }
                }
                else
                {
                    SummaryMessage = "Kein Server konnte eine erfolgreiche Abfrage durchführen.";
                }

                IsTesting = false;
            });
        }

        public async Task StartTestAsync()
        {
            if (IsTesting)
                return;
                
            IsTesting = true;
            TestResults.Clear();
            SummaryMessage = "";
            
            await _dnsTesterService.TestDnsServersAsync(Domain, RecordType);
        }

        public void AddCustomServer(string name, string address)
        {
            var server = new DnsServer
            {
                Name = name,
                Address = address,
                IsCustom = true
            };
            
            _dnsTesterService.AddCustomServer(server);
            DnsServers.Add(server);
        }

        public string Domain
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = "example.com";

        public string RecordType
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = "A";

        public bool IsTesting
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        }

        public string FastestServer
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = "";

        public string FastestTime
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = "";

        public string SummaryMessage
        {
            get;
            set
            {
                field = value;
                OnPropertyChanged();
            }
        } = "";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
