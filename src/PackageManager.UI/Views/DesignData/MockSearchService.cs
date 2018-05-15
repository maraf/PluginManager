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
        public Task<IEnumerable<IPackage>> SearchAsync(string packageSourceUrl, string searchText, SearchOptions options = default, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<IPackage>>(new List<IPackage>()
            {
                ViewModelLocator.Package
            });
        }

        public Task<IPackage> FindLatestVersionAsync(string packageSourceUrl, IPackage package, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(ViewModelLocator.Package);
        }
    }
}
