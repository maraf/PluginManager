using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    public interface IInstallService
    {
        string Path { get; }

        bool IsInstalled(IPackageIdentity package);

        void Install(IPackageIdentity package);
        void Uninstall(IPackageIdentity package);

        Task<IReadOnlyCollection<IInstalledPackage>> GetInstalledAsync(IEnumerable<IPackageSource> packageSources, CancellationToken cancellationToken);
    }
}
