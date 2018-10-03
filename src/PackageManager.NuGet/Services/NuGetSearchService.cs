using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neptuo;
using Neptuo.Activators;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using PackageManager.Models;

namespace PackageManager.Services
{
    public partial class NuGetSearchService : ISearchService
    {
        private readonly IFactory<SourceRepository, string> repositoryFactory;
        private readonly IFilter filter;
        private readonly NuGetPackageContent.IFrameworkFilter frameworkFilter;

        public NuGetSearchService(IFactory<SourceRepository, string> repositoryFactory, IFilter filter = null, NuGetPackageContent.IFrameworkFilter frameworkFilter = null)
        {
            Ensure.NotNull(repositoryFactory, "repositoryFactory");

            if (filter == null)
                filter = NullFilter.Instance;

            this.repositoryFactory = repositoryFactory;
            this.filter = filter;
            this.frameworkFilter = frameworkFilter;
        }

        private SearchOptions EnsureOptions(SearchOptions options)
        {
            if (options == null)
            {
                options = new SearchOptions()
                {
                    PageIndex = 0,
                    PageSize = 10
                };
            }

            return options;
        }

        private Task<IEnumerable<IPackageSearchMetadata>> SearchAsync(PackageSearchResource search, string searchText, SearchOptions options, CancellationToken cancellationToken)
            => search.SearchAsync(searchText, new SearchFilter(false), options.PageIndex * options.PageSize, options.PageSize, NullLogger.Instance, cancellationToken);

        public async Task<IEnumerable<IPackage>> SearchAsync(string packageSourceUrl, string searchText, SearchOptions options = default, CancellationToken cancellationToken = default)
        {
            options = EnsureOptions(options);

            SourceRepository repository = repositoryFactory.Create(packageSourceUrl);
            PackageSearchResource search = await repository.GetResourceAsync<PackageSearchResource>(cancellationToken);
            if (search == null)
                return Enumerable.Empty<IPackage>();

            List<IPackage> result = new List<IPackage>();

            // Try to find N results passing filter (until zero results is returned).
            int i = 0;
            while (i < options.PageSize)
            {
                bool hasItems = false;
                foreach (IPackageSearchMetadata package in await SearchAsync(search, searchText, options, cancellationToken))
                {
                    hasItems = true;
                    if (i >= options.PageSize)
                        break;

                    FilterResult filterResult = filter.IsPassed(package);
                    switch (filterResult)
                    {
                        case FilterResult.Ok:
                            result.Add(new NuGetPackage(package, repository, frameworkFilter));
                            break;

                        case FilterResult.TryOlderVersion:
                            await TryAddOlderVersionOfPackageAsync(result, package, repository);
                            break;
                    }
                }

                if (!hasItems)
                    break;

                options = new SearchOptions()
                {
                    PageIndex = options.PageIndex + 1,
                    PageSize = options.PageSize
                };
            }

            return result;
        }

        private async Task TryAddOlderVersionOfPackageAsync(List<IPackage> result, IPackageSearchMetadata package, SourceRepository repository)
        {
            IEnumerable<VersionInfo> versions = await package.GetVersionsAsync();
            foreach (VersionInfo version in versions)
            {
                if (version.Version != package.Identity.Version)
                {
                    FilterResult filterResult = filter.IsPassed(version.PackageSearchMetadata);
                    if (filterResult == FilterResult.Ok)
                    {
                        result.Add(new NuGetPackage(version.PackageSearchMetadata, repository, frameworkFilter));
                        return;
                    }
                }
            }
        }

        public async Task<IPackage> FindLatestVersionAsync(string packageSourceUrl, IPackage package, CancellationToken cancellationToken = default)
        {
            Ensure.NotNull(package, "package");

            IEnumerable<IPackage> packages = await SearchAsync(packageSourceUrl, package.Id, new SearchOptions() { PageSize = 1 }, cancellationToken);
            IPackage latest = packages.FirstOrDefault();
            if (latest != null && latest.Id == package.Id)
                return latest;

            return null;
        }
    }
}
