using Neptuo;
using Neptuo.Logging;
using NuGet.Common;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using PackageManager.Logging;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    public class NuGetPackageVersionService
    {
        private readonly INuGetPackageFilter filter;
        private readonly NuGetPackageContent.IFrameworkFilter frameworkFilter;
        private readonly ILog log;
        private readonly ILogger nuGetLog;

        public NuGetPackageVersionService(ILog log, INuGetPackageFilter filter = null, NuGetPackageContent.IFrameworkFilter frameworkFilter = null)
        {
            Ensure.NotNull(log, "log");
            this.log = log;
            this.nuGetLog = new NuGetLogger(log);

            if (filter == null)
                filter = OkNuGetPackageFilter.Instance;

            this.filter = filter;
            this.frameworkFilter = frameworkFilter;
        }

        public async Task<IReadOnlyList<IPackage>> GetListAsync(int resultCount, IPackageSearchMetadata package, SourceRepository repository, Func<IPackageSearchMetadata, IPackageSearchMetadata, bool> versionFilter = null, CancellationToken cancellationToken = default)
        {
            if (versionFilter == null)
                versionFilter = (source, target) => true;

            try
            {
                List<IPackage> result = new List<IPackage>();
                if (await SearchOlderVersionsDirectly(result, resultCount, package, repository, versionFilter))
                    return result;

                if (await SearchOlderVersionsUsingMetadataResource(result, resultCount, package, repository, versionFilter, cancellationToken))
                    return result;

                return new List<IPackage>();
            }
            catch (FatalProtocolException e) when (e.InnerException is TaskCanceledException)
            {
                cancellationToken.ThrowIfCancellationRequested();
                throw e;
            }
        }

        private async Task<bool> SearchOlderVersionsDirectly(List<IPackage> result, int resultCount, IPackageSearchMetadata package, SourceRepository repository, Func<IPackageSearchMetadata, IPackageSearchMetadata, bool> versionFilter)
        {
            bool isSuccess = false;
            IEnumerable<VersionInfo> versions = await package.GetVersionsAsync();
            foreach (VersionInfo version in versions)
            {
                if (version.PackageSearchMetadata != null && versionFilter(package, version.PackageSearchMetadata))
                {
                    IPackage item = ProcessOlderVersion(repository, version.PackageSearchMetadata);
                    if (item != null)
                    {
                        result.Add(item);
                        if (result.Count == resultCount)
                            return true;

                        isSuccess = true;
                    }
                }
            }

            return isSuccess;
        }

        private async Task<bool> SearchOlderVersionsUsingMetadataResource(List<IPackage> result, int resultCount, IPackageSearchMetadata package, SourceRepository repository, Func<IPackageSearchMetadata, IPackageSearchMetadata, bool> versionFilter, CancellationToken cancellationToken)
        {
            PackageMetadataResource metadataResource = await repository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
            if (metadataResource == null)
                return false;

            using (var sourceCacheContext = new SourceCacheContext())
            {
                IEnumerable<IPackageSearchMetadata> versions = await metadataResource?.GetMetadataAsync(
                    package.Identity.Id,
                    false,
                    false,
                    sourceCacheContext,
                    nuGetLog,
                    cancellationToken
                );

                versions = versions.OrderByDescending(p => p.Identity.Version, VersionComparer.Default);
                foreach (IPackageSearchMetadata version in versions)
                {
                    if (versionFilter(package, version))
                    {
                        IPackage item = ProcessOlderVersion(repository, version);
                        if (item != null)
                        {
                            result.Add(item);
                            if (result.Count == resultCount)
                                return true;
                        }
                    }
                }
            }

            return true;
        }

        private IPackage ProcessOlderVersion(SourceRepository repository, IPackageSearchMetadata version)
        {
            log.Debug($"Found '{version.Identity}'.");

            NuGetPackageFilterResult filterResult = filter.IsPassed(version);
            switch (filterResult)
            {
                case NuGetPackageFilterResult.Ok:
                    log.Debug("Package added.");
                    return new NuGetPackage(version, repository, log, this, frameworkFilter);

                default:
                    log.Debug("Package skipped.");
                    break;
            }

            return null;
        }
    }
}
