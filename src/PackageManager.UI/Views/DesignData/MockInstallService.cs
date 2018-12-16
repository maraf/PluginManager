using PackageManager.Models;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Views.DesignData
{
    internal class MockInstallService : IInstallService
    {
        public string Path => @"C:\Temp";

        public bool IsInstalled(string packageId)
            => false;

        public bool IsInstalled(IPackageIdentity package)
            => false;

        public void Install(IPackageIdentity package)
        { }

        public void Uninstall(IPackageIdentity package)
        { }

        public Task<IReadOnlyCollection<IInstalledPackage>> GetInstalledAsync(IEnumerable<IPackageSource> packageSources, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<IInstalledPackage>>(
                new List<IInstalledPackage>()
                {
                    ViewModelLocator.IncompatiblePackage,
                    ViewModelLocator.CompatiblePackage
                }
            );
        }

        public Task<IPackageIdentity> FindInstalledAsync(string packageId, CancellationToken cancellationToken)
        {
            return null;
        }
    }
}
