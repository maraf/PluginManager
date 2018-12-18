using Neptuo;
using Neptuo.Logging;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using PackageManager.Logging;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    public class NuGetPackageContentService
    {
        private readonly ILog log;
        private readonly ILogger nuGetLog;
        private readonly NuGetPackageContent.IFrameworkFilter frameworkFilter;

        public NuGetPackageContentService(ILog log, NuGetPackageContent.IFrameworkFilter frameworkFilter = null)
        {
            Ensure.NotNull(log, "log");
            this.log = log.Factory.Scope("Package");
            this.nuGetLog = new NuGetLogger(this.log);
            this.frameworkFilter = frameworkFilter;
        }

        public async Task<IPackageContent> DownloadAsync(SourceRepository repository, IPackageSearchMetadata package, CancellationToken cancellationToken)
        {
            Ensure.NotNull(repository, "repository");
            Ensure.NotNull(package, "package");

            DownloadResource download = await repository.GetResourceAsync<DownloadResource>();
            if (download == null)
                throw Ensure.Exception.InvalidOperation($"Unnable to resolve '{nameof(DownloadResource)}'.");

            using (var sourceCacheContext = new SourceCacheContext())
            {
                var context = new PackageDownloadContext(sourceCacheContext, Path.GetTempPath(), true);
                var result = await download.GetDownloadResourceResultAsync(package.Identity, context, String.Empty, nuGetLog, cancellationToken);
                if (result.Status == DownloadResourceResultStatus.Cancelled)
                    throw new OperationCanceledException();
                else if (result.Status == DownloadResourceResultStatus.NotFound)
                    throw Ensure.Exception.InvalidOperation($"Package '{package.Identity.Id}-v{package.Identity.Version}' not found");

                return new NuGetPackageContent(new PackageArchiveReader(result.PackageStream), log, frameworkFilter);
            }
        }
    }
}
