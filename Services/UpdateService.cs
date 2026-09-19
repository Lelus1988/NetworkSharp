using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    public sealed class UpdateService : IUpdateService, IDisposable
    {
        private const string LatestReleaseUrl = "https://api.github.com/repos/Lelus1988/NetworkSharp/releases/latest";
        private readonly HttpClient _httpClient = new();

        public UpdateService()
        {
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("NetworkSharp", "1.0"));
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<UpdateInfo?> CheckForUpdateAsync(Version currentVersion, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync(LatestReleaseUrl, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var release = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
            var root = release.RootElement;
            var tagName = root.GetProperty("tag_name").GetString()?.TrimStart('v');
            var releaseName = root.GetProperty("name").GetString() ?? $"NetworkSharp {tagName}";

            if (!Version.TryParse(tagName, out var latestVersion) || latestVersion <= currentVersion)
                return null;

            foreach (var asset in root.GetProperty("assets").EnumerateArray())
            {
                var assetName = asset.GetProperty("name").GetString() ?? string.Empty;
                if (!assetName.Equals($"NetworkSharp-Setup-{latestVersion}.exe", StringComparison.OrdinalIgnoreCase))
                    continue;

                return new UpdateInfo(
                    latestVersion,
                    releaseName,
                    asset.GetProperty("browser_download_url").GetString() ?? string.Empty);
            }

            return null;
        }

        public async Task<string> DownloadInstallerAsync(UpdateInfo update, CancellationToken cancellationToken = default)
        {
            var installerPath = Path.Combine(Path.GetTempPath(), $"NetworkSharp-Setup-{update.Version}.exe");
            await using var source = await _httpClient.GetStreamAsync(update.InstallerUrl, cancellationToken);
            await using var destination = File.Create(installerPath);
            await source.CopyToAsync(destination, cancellationToken);
            return installerPath;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}