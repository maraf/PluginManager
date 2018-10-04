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

        public bool IsInstalled(IPackage package)
            => false;

        public void Install(IPackage package)
        { }

        public void Uninstall(IPackage package)
        { }

        public Task<IReadOnlyCollection<IInstalledPackage>> GetInstalledAsync(string packageSourceUrl, CancellationToken cancellationToken)
        {
            return Task.FromResult<IReadOnlyCollection<IInstalledPackage>>(
                new List<IInstalledPackage>()
                {
                    ViewModelLocator.IncompatiblePackage,
                    ViewModelLocator.CompatiblePackage
                }
            );
        }
    }
}
