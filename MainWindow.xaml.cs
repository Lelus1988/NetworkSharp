using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using NetworkSharp.ViewModels;
using NetworkSharp.Views;
using NetworkSharp.Services;

namespace NetworkSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static IServiceProvider? _serviceProvider;
        private readonly ObservableCollection<NavigationItem> _navigationItems;


        public MainWindow()
        {
            InitializeComponent();
            
            // Initialize service provider if not already done
            if (_serviceProvider == null)
            {
                _serviceProvider = ConfigureServices();
            }
            
            _navigationItems = new ObservableCollection<NavigationItem>
            {
                new NavigationItem { Name = "NetworkMonitor", DisplayName = "Netzwerk-Monitor", Icon = "📊", IsSelected = true },
                new NavigationItem { Name = "PortScanner", DisplayName = "Port-Scanner", Icon = "🔍", IsSelected = false },
                new NavigationItem { Name = "LanDevices", DisplayName = "LAN-Geräte", Icon = "🖥️", IsSelected = false },
                new NavigationItem { Name = "Ping", DisplayName = "Ping", Icon = "📡", IsSelected = false },
                new NavigationItem { Name = "DnsTester", DisplayName = "DNS-Tester", Icon = "🌐", IsSelected = false },
                new NavigationItem { Name = "WlanProfiles", DisplayName = "WLAN-Profile", Icon = "📶", IsSelected = false },
                new NavigationItem { Name = "IpGeoIp", DisplayName = "IP & Standort", Icon = "📍", IsSelected = false }
            };
            
            NavigationItems.ItemsSource = _navigationItems;
            
            // Load initial view
            LoadView("NetworkMonitor");
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string viewName)
            {
                // Update selection
                foreach (var item in _navigationItems)
                {
                    item.IsSelected = item.Name == viewName;
                }
                
                SelectedItem = _navigationItems.FirstOrDefault(i => i.Name == viewName);
                LoadView(viewName);
            }
        }

        private void LoadView(string viewName)
        {
            try
            {
                UserControl? view = null;
                
                if (_serviceProvider == null)
                {
                    _serviceProvider = ConfigureServices();
                }
                
                switch (viewName)
                {
                    case "NetworkMonitor":
                        var networkMonitorViewModel = _serviceProvider.GetRequiredService<NetworkMonitorViewModel>();
                        view = new NetworkMonitorView { DataContext = networkMonitorViewModel };
                        break;
                        
                    case "PortScanner":
                        var portScannerViewModel = _serviceProvider.GetRequiredService<PortScannerViewModel>();
                        view = new PortScannerView { DataContext = portScannerViewModel };
                        break;
                        
                    case "LanDevices":
                        var lanDevicesViewModel = _serviceProvider.GetRequiredService<LanDevicesViewModel>();
                        view = new LanDevicesView { DataContext = lanDevicesViewModel };
                        break;
                        
                    case "Ping":
                        var pingViewModel = _serviceProvider.GetRequiredService<PingViewModel>();
                        view = new PingView { DataContext = pingViewModel };
                        break;
                        
                    case "DnsTester":
                        var dnsTesterViewModel = _serviceProvider.GetRequiredService<DnsTesterViewModel>();
                        view = new DnsTesterView { DataContext = dnsTesterViewModel };
                        break;
                        
                    case "WlanProfiles":
                        var wlanProfilesViewModel = _serviceProvider.GetRequiredService<WlanProfilesViewModel>();
                        view = new WlanProfilesView { DataContext = wlanProfilesViewModel };
                        break;
                        
                    case "IpGeoIp":
                        var ipGeoIpViewModel = _serviceProvider.GetRequiredService<IpGeoIpViewModel>();
                        view = new IpGeoIpView { DataContext = ipGeoIpViewModel };
                        break;

                }
                
                if (view != null)
                {
                    MainContent.Content = view;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading view: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // Register services
            services.AddSingleton<INetworkMonitorService, NetworkMonitorService>();
            services.AddSingleton<IPortScannerService, PortScannerService>();
            services.AddSingleton<ILanDeviceService, LanDeviceService>();
            services.AddSingleton<IPingService, PingService>();
            services.AddSingleton<IDnsTesterService, DnsTesterService>();
            services.AddSingleton<IWlanProfileService, WlanProfileService>();
            services.AddSingleton<IIpGeoIpService, IpGeoIpService>();
            services.AddSingleton<ITracerouteService, TracerouteService>();
            services.AddSingleton<IWhoisService, WhoisService>();
            
            // Register ViewModels
            services.AddTransient<NetworkMonitorViewModel>();
            services.AddTransient<PortScannerViewModel>();
            services.AddTransient<LanDevicesViewModel>();
            services.AddTransient<PingViewModel>();
            services.AddTransient<DnsTesterViewModel>();
            services.AddTransient<WlanProfilesViewModel>();
            services.AddTransient<IpGeoIpViewModel>();
            
            return services.BuildServiceProvider();
        }

        public NavigationItem? SelectedItem
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

    /// <summary>
    /// Navigation item for the sidebar
    /// </summary>
    public class NavigationItem : INotifyPropertyChanged
    {

        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        
        public bool IsSelected
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
