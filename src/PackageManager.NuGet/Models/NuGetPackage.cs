﻿using Neptuo;
using Neptuo.Logging;
using NuGet.Common;
using NuGet.Packaging;
using NuGet.Protocol.Core.Types;
using PackageManager.Logging;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    public class NuGetPackage : NuGetPackageIdentity, IPackage
    {
        private readonly IPackageSearchMetadata source;
        private readonly ILog log;
        private readonly ILogger nuGetLog;
        private readonly SourceRepository repository;
        private readonly NuGetPackageVersionService versionService;
        private readonly NuGetPackageContent.IFrameworkFilter frameworkFilter;

        public string Description => source.Description;

        public string Authors => source.Authors;
        public DateTime? Published => source.Published?.DateTime;
        public string Tags => source.Tags;

        public Uri IconUrl => source.IconUrl;
        public Uri ProjectUrl => source.ProjectUrl;
        public Uri LicenseUrl => source.LicenseUrl;

        public NuGetPackage(IPackageSearchMetadata source, SourceRepository repository, ILog log, NuGetPackageVersionService versionService, NuGetPackageContent.IFrameworkFilter frameworkFilter = null)
            : base(source?.Identity)
        {
            Ensure.NotNull(source, "source");
            Ensure.NotNull(repository, "repository");
            Ensure.NotNull(log, "log");
            Ensure.NotNull(versionService, "versionService");
            this.source = source;
            this.repository = repository;
            this.versionService = versionService;
            this.log = log.Factory.Scope("Package");
            this.nuGetLog = new NuGetLogger(this.log);
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
                var result = await download.GetDownloadResourceResultAsync(source.Identity, context, String.Empty, nuGetLog, cancellationToken);
                if (result.Status == DownloadResourceResultStatus.Cancelled)
                    throw new OperationCanceledException();
                else if (result.Status == DownloadResourceResultStatus.NotFound)
                    throw Ensure.Exception.InvalidOperation($"Package '{source.Identity.Id}-v{source.Identity.Version}' not found");

                return new NuGetPackageContent(new PackageArchiveReader(result.PackageStream), log, frameworkFilter);
            }
        }

        public async Task<IEnumerable<IPackage>> GetVersionsAsync(CancellationToken cancellationToken)
            => await versionService.GetListAsync(Int32.MaxValue, source, repository, cancellationToken: cancellationToken);
    }
}
