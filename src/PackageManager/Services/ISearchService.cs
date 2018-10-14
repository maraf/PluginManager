using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<IPackage>> SearchAsync(IEnumerable<IPackageSource> packageSources, string searchText, SearchOptions options = default, CancellationToken cancellationToken = default);

        Task<IPackage> FindLatestVersionAsync(IEnumerable<IPackageSource> packageSources, IPackage package, CancellationToken cancellationToken = default);
    }
}
