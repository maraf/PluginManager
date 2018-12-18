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
    internal class MockSearchService : ISearchService
    {
        public Task<IEnumerable<IPackage>> SearchAsync(IEnumerable<IPackageSource> packageSources, string searchText, SearchOptions options = default, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<IPackage>>(new List<IPackage>()
            {
                ViewModelLocator.Package
            });
        }

        public Task<IPackage> FindLatestVersionAsync(IEnumerable<IPackageSource> packageSources, IPackage package, bool isPrereleaseIncluded, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ViewModelLocator.Package);
        }
    }
}
