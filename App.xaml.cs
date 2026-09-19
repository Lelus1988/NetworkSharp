using System.Windows;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NetworkSharp.Data;
using NetworkSharp.Data.Repositories;
using NetworkSharp.Services;
using NetworkSharp.Views;

namespace NetworkSharp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;

        private async void Application_Startup(object sender, StartupEventArgs e)
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

            if (!AcceptTerms())
            {
                Shutdown();
                return;
            }

            if (!await OfferUpdateAsync())
            {
                Shutdown();
                return;
            }
            
            // Initialize database
            var dbInitializer = _serviceProvider.GetRequiredService<INetworkDataRepository>();
            Task.Run(async () => await dbInitializer.InitializeDatabaseAsync()).Wait();
            
            // Create and show the main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private bool AcceptTerms()
        {
            var termsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NetworkSharp",
                "terms-accepted.txt");

            if (File.Exists(termsPath))
                return true;

            var dialog = new StartupGateWindow(
                "Nutzungsbedingungen",
                "NetworkSharp ist ein Diagnosewerkzeug für deine eigenen Geräte und Netzwerke. Verwende Scans und Tests nur dort, wo du dazu berechtigt bist. Die Ergebnisse können abhängig von Windows, Netzwerk und Firewall unvollständig sein.\n\nMit \"Ja, akzeptieren\" bestätigst du, dass du diese Hinweise gelesen hast und NetworkSharp auf eigene Verantwortung verwendest.",
                "Ja, akzeptieren",
                "Schließen");

            if (dialog.ShowDialog() != true)
                return false;

            Directory.CreateDirectory(Path.GetDirectoryName(termsPath)!);
            File.WriteAllText(termsPath, DateTime.UtcNow.ToString("O"));
            return true;
        }

        private async Task<bool> OfferUpdateAsync()
        {
            try
            {
                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0);
                var updateService = _serviceProvider!.GetRequiredService<IUpdateService>();
                var update = await updateService.CheckForUpdateAsync(currentVersion);
                if (update == null)
                    return true;

                var dialog = new StartupGateWindow(
                    "Neue Version ist verfügbar",
                    $"NetworkSharp {update.Version} steht bereit.\n\nMöchtest du jetzt aktualisieren? Die aktuelle App wird danach geschlossen und der Installer gestartet.",
                    "Ja, jetzt aktualisieren",
                    "Schließen");

                if (dialog.ShowDialog() != true)
                    return false;

                var installerPath = await updateService.DownloadInstallerAsync(update);
                Process.Start(new ProcessStartInfo
                {
                    FileName = installerPath,
                    UseShellExecute = true
                });
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Update check failed: {ex.Message}");
                return true;
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
