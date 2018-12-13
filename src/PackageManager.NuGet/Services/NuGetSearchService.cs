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
using NuGet.Versioning;
using PackageManager.Logging;
using PackageManager.Models;

namespace PackageManager.Services
{
    public partial class NuGetSearchService : ISearchService
    {
        public const int PageCountToProbe = 10;

        private readonly IFactory<SourceRepository, IPackageSource> repositoryFactory;
        private readonly ILog log;
        private readonly ILogger nuGetLog;
        private readonly NuGetPackageVersionService versionService;
        private readonly INuGetPackageFilter filter;
        private readonly NuGetPackageContent.IFrameworkFilter frameworkFilter;

        public NuGetSearchService(IFactory<SourceRepository, IPackageSource> repositoryFactory, ILog log, NuGetPackageVersionService versionService, INuGetPackageFilter filter = null, NuGetPackageContent.IFrameworkFilter frameworkFilter = null)
        {
            Ensure.NotNull(repositoryFactory, "repositoryFactory");
            Ensure.NotNull(log, "log");
            Ensure.NotNull(versionService, "versionService");

            if (filter == null)
                filter = OkNuGetPackageFilter.Instance;

            this.repositoryFactory = repositoryFactory;
            this.log = log;
            this.nuGetLog = new NuGetLogger(log);
            this.versionService = versionService;
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

        public async Task<IEnumerable<IPackage>> SearchAsync(IEnumerable<IPackageSource> packageSources, string searchText, SearchOptions options = default, CancellationToken cancellationToken = default)
        {
            log.Debug($"Searching '{searchText}'.");

            options = EnsureOptions(options);

            List<IPackage> result = new List<IPackage>();
            List<IPackageSource> sources = new List<IPackageSource>(packageSources);
            List<IPackageSource> sourcesToSkip = new List<IPackageSource>();

            // Try to find N results passing filter (until zero results is returned).
            while (result.Count < options.PageSize && options.PageIndex < PageCountToProbe)
            {
                log.Debug($"Loading page '{options.PageIndex}'.");

                bool hasItems = false;
                foreach (IPackageSource packageSource in sources)
                {
                    log.Debug($"Searching in '{packageSource.Uri}'.");

                    SourceRepository repository = repositoryFactory.Create(packageSource);
                    PackageSearchResource search = await repository.GetResourceAsync<PackageSearchResource>(cancellationToken);
                    if (search == null)
                    {
                        log.Debug($"Source skipped, because it doesn't provide '{nameof(PackageSearchResource)}'.");
                        continue;
                    }

                    int sourceSearchPackageCount = 0;
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
                                result.Add(new NuGetPackage(package, repository, log, versionService, frameworkFilter));
                                break;

                            case NuGetPackageFilterResult.NotCompatibleVersion:
                                log.Debug("Loading order versions.");
                                result.AddRange(await versionService.GetListAsync(1, package, repository, (source, target) => source.Identity.Version != target.Identity.Version));
                                break;

                            default:
                                log.Debug("Package skipped.");
                                break;
                        }

                        sourceSearchPackageCount++;
                    }

                    // If package source reached end, skip it from next probing.
                    if (sourceSearchPackageCount < options.PageSize)
                        sourcesToSkip.Add(packageSource);
                }

                if (!hasItems)
                    break;

                options = new SearchOptions()
                {
                    PageIndex = options.PageIndex + 1,
                    PageSize = options.PageSize
                };

                foreach (IPackageSource source in sourcesToSkip)
                    sources.Remove(source);
            }

            log.Debug($"Search completed. Found '{result.Count}' items.");
            return result;
        }

        public async Task<IPackage> FindLatestVersionAsync(IEnumerable<IPackageSource> packageSources, IPackage package, CancellationToken cancellationToken = default)
        {
            Ensure.NotNull(package, "package");

            log.Debug($"Finding latest version of '{package.Id}'.");

            IEnumerable<IPackage> packages = await SearchAsync(packageSources, package.Id, new SearchOptions() { PageSize = 1 }, cancellationToken);
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
