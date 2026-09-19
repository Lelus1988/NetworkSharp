using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using NetworkSharp.Data;
using NetworkSharp.Data.Repositories;
using NetworkSharp.Services;

namespace NetworkSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Setup dependency injection
            var services = new ServiceCollection();
            
            // Register database
            services.AddDbContext<NetworkSharpDbContext>();
            services.AddScoped<INetworkDataRepository, NetworkDataRepository>();
            
            // Register services
            services.AddTransient<INetworkMonitorService, NetworkMonitorService>();
            services.AddTransient<IPortScannerService, PortScannerService>();
            services.AddTransient<ILanDeviceService, LanDeviceService>();
            services.AddTransient<IPingService, PingService>();
            services.AddTransient<IDnsTesterService, DnsTesterService>();
            services.AddTransient<IWlanProfileService, WlanProfileService>();
            services.AddTransient<IIpGeoIpService, IpGeoIpService>();
            services.AddTransient<ITracerouteService, TracerouteService>();
            services.AddTransient<IWhoisService, WhoisService>();
            services.AddTransient<ISpeedTestService, SpeedTestService>();
            
            _serviceProvider = services.BuildServiceProvider();
            
            // Initialize database
            var dbInitializer = _serviceProvider.GetRequiredService<INetworkDataRepository>();
            Task.Run(async () => await dbInitializer.InitializeDatabaseAsync()).Wait();
            
            // Create and show the main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
        
        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
