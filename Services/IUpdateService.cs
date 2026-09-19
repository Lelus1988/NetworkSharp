using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkSharp.Services
{
    public interface IUpdateService
    {
        Task<UpdateInfo?> CheckForUpdateAsync(Version currentVersion, CancellationToken cancellationToken = default);

        Task<string> DownloadInstallerAsync(UpdateInfo update, CancellationToken cancellationToken = default);
    }

    public sealed record UpdateInfo(Version Version, string ReleaseName, string InstallerUrl);
}