using Neptuo;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    public class NuGetPackage : IPackage
    {
        private readonly IPackageSearchMetadata source;
        private readonly SourceRepository repository;
        private readonly NuGetPackageContent.IFrameworkFilter frameworkFilter;

        public string Id => source.Identity.Id;
        public string Version => source.Identity.Version.ToFullString();
        public string Description => source.Description;

        public string Authors => source.Authors;
        public DateTime? Published => source.Published?.DateTime;
        public string Tags => source.Tags;

        public Uri IconUrl => source.IconUrl;
        public Uri ProjectUrl => source.ProjectUrl;
        public Uri LicenseUrl => source.LicenseUrl;

        public NuGetPackage(IPackageSearchMetadata source, SourceRepository repository, NuGetPackageContent.IFrameworkFilter frameworkFilter = null)
        {
            Ensure.NotNull(source, "source");
            Ensure.NotNull(repository, "repository");
            this.source = source;
            this.repository = repository;
            this.frameworkFilter = frameworkFilter;
        }

        public async Task<IPackageContent> GetContentAsync(CancellationToken cancellationToken)
        {
            DownloadResource download = await repository.GetResourceAsync<DownloadResource>();
            if (download == null)
                throw Ensure.Exception.InvalidOperation($"Unnable to resolve '{nameof(DownloadResource)}'.");

            using (var sourceCacheContext = new SourceCacheContext())
            {
                var context = new PackageDownloadContext(sourceCacheContext, Path.GetTempPath(), true);
                var result = await download.GetDownloadResourceResultAsync(source.Identity, context, String.Empty, NullLogger.Instance, cancellationToken);
                if (result.Status == DownloadResourceResultStatus.Cancelled)
                    throw new OperationCanceledException();
                else if (result.Status == DownloadResourceResultStatus.NotFound)
                    throw Ensure.Exception.InvalidOperation($"Package '{source.Identity.Id}-v{source.Identity.Version}' not found");
                
                return new NuGetPackageContent(new PackageArchiveReader(result.PackageStream), frameworkFilter);
            }
        }
    }
}
