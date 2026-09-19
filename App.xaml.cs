using System.Windows;
using System.Diagnostics;
using System.Reflection;
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
            services.AddSingleton<IUpdateService, UpdateService>();
            
            _serviceProvider = services.BuildServiceProvider();
            
            // Initialize database
            var dbInitializer = _serviceProvider.GetRequiredService<INetworkDataRepository>();
            Task.Run(async () => await dbInitializer.InitializeDatabaseAsync()).Wait();
            
            // Create and show the main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
            _ = CheckForUpdatesAsync(mainWindow);
        }

        private async Task CheckForUpdatesAsync(Window owner)
        {
            try
            {
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);
                var updateService = _serviceProvider?.GetRequiredService<IUpdateService>();
                if (updateService == null)
                    return;

                var update = await updateService.CheckForUpdateAsync(currentVersion);
                if (update == null || !owner.IsVisible)
                    return;

                var message = $"Eine neue NetworkSharp-Version ist verfügbar: {update.ReleaseName}.\n\nJetzt ohne Administratorrechte aktualisieren?";
                var answer = MessageBox.Show(owner, message, "NetworkSharp-Update", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (answer != MessageBoxResult.Yes)
                    return;

                var installerPath = await updateService.DownloadInstallerAsync(update);
                Process.Start(new ProcessStartInfo
                {
                    FileName = installerPath,
                    UseShellExecute = true
                });
                Shutdown();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update check failed: {ex.Message}");
            }
        }
        
        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
