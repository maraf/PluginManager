using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Logging;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using PackageManager.Logging;
using PackageManager.Models;

namespace PackageManager.Services
{
    public partial class NuGetSearchService : ISearchService
    {
        public const int PageCountToProbe = 10;

        private readonly IFactory<SourceRepository, string> repositoryFactory;
        private readonly ILog log;
        private readonly ILogger nuGetLog;
        private readonly INuGetPackageFilter filter;
        private readonly NuGetPackageContent.IFrameworkFilter frameworkFilter;

        public NuGetSearchService(IFactory<SourceRepository, string> repositoryFactory, ILog log, INuGetPackageFilter filter = null, NuGetPackageContent.IFrameworkFilter frameworkFilter = null)
        {
            Ensure.NotNull(repositoryFactory, "repositoryFactory");
            Ensure.NotNull(log, "log");

            if (filter == null)
                filter = OkNuGetPackageFilter.Instance;

            this.repositoryFactory = repositoryFactory;
            this.log = log;
            this.nuGetLog = new NuGetLogger(log);
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
            => search.SearchAsync(searchText, new SearchFilter(false), options.PageIndex * options.PageSize, options.PageSize, nuGetLog, cancellationToken);

        public async Task<IEnumerable<IPackage>> SearchAsync(string packageSourceUrl, string searchText, SearchOptions options = default, CancellationToken cancellationToken = default)
        {
            log.Debug($"Searching '{searchText}' in '{packageSourceUrl}'.");

            options = EnsureOptions(options);

            SourceRepository repository = repositoryFactory.Create(packageSourceUrl);
            PackageSearchResource search = await repository.GetResourceAsync<PackageSearchResource>(cancellationToken);
            if (search == null)
                return Enumerable.Empty<IPackage>();

            List<IPackage> result = new List<IPackage>();

            // Try to find N results passing filter (until zero results is returned).
            while (result.Count < options.PageSize && options.PageIndex < PageCountToProbe)
            {
                log.Debug($"Loading page '{options.PageIndex}'.");

                bool hasItems = false;
                foreach (IPackageSearchMetadata package in await SearchAsync(search, searchText, options, cancellationToken))
                {
                    log.Debug($"Found '{package.Identity}'.");

                    hasItems = true;
                    if (result.Count >= options.PageSize)
                        break;

                    NuGetPackageFilterResult filterResult = filter.IsPassed(package);
                    switch (filterResult)
                    {
                        case NuGetPackageFilterResult.Ok:
                            log.Debug("Package added.");
                            result.Add(new NuGetPackage(package, repository, log, frameworkFilter));
                            break;

                        case NuGetPackageFilterResult.NotCompatibleVersion:
                            log.Debug("Loading order versions.");
                            await TryAddOlderVersionOfPackageAsync(result, package, repository);
                            break;

                        default:
                            log.Debug("Package skipped.");
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

            log.Debug($"Search completed. Found '{result.Count}' items.");
            return result;
        }

        private async Task TryAddOlderVersionOfPackageAsync(List<IPackage> result, IPackageSearchMetadata package, SourceRepository repository)
        {
            IEnumerable<VersionInfo> versions = await package.GetVersionsAsync();
            foreach (VersionInfo version in versions)
            {
                if (version.Version != package.Identity.Version)
                {
                    log.Debug($"Found '{version.PackageSearchMetadata.Identity}'.");

                    NuGetPackageFilterResult filterResult = filter.IsPassed(version.PackageSearchMetadata);
                    switch (filterResult)
                    {
                        case NuGetPackageFilterResult.Ok:
                            log.Debug("Package added.");
                            result.Add(new NuGetPackage(version.PackageSearchMetadata, repository, log, frameworkFilter));
                            return;

                        default:
                            log.Debug("Package skipped.");
                            break;
                    }
                }
            }
        }

        public async Task<IPackage> FindLatestVersionAsync(string packageSourceUrl, IPackage package, CancellationToken cancellationToken = default)
        {
            Ensure.NotNull(package, "package");

            log.Debug($"Finding latest version of '{package.Id}' in '{packageSourceUrl}'.");

            IEnumerable<IPackage> packages = await SearchAsync(packageSourceUrl, package.Id, new SearchOptions() { PageSize = 1 }, cancellationToken);
            IPackage latest = packages.FirstOrDefault();
            if (latest != null && latest.Id == package.Id)
            {
                log.Debug($"Found version '{latest.Version}'.");
                return latest;
            }

            return null;
        }
    }
}
