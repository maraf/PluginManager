using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neptuo;
using Neptuo.Activators;
using Neptuo.Logging;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using PackageManager.Logging;
using PackageManager.Models;

namespace PackageManager.Services
{
    public class NuGetInstallService : IInstallService
    {
        private readonly IFactory<SourceRepository, IPackageSource> repositoryFactory;
        private readonly ILog log;
        private readonly NuGetPackageContentService contentService;
        private readonly NuGetPackageVersionService versionService;
        private readonly ILogger nuGetLog;
        private readonly INuGetPackageFilter packageFilter;
        private readonly NuGetPackageContent.IFrameworkFilter frameworkFilter;

        public string Path { get; }
        public string ConfigFilePath => System.IO.Path.Combine(Path, "packages.config");

        public NuGetInstallService(IFactory<SourceRepository, IPackageSource> repositoryFactory, ILog log, string path, NuGetPackageContentService contentService, NuGetPackageVersionService versionService, INuGetPackageFilter packageFilter = null, NuGetPackageContent.IFrameworkFilter frameworkFilter = null)
        {
            Ensure.NotNull(repositoryFactory, "repositoryFactory");
            Ensure.NotNull(log, "log");
            Ensure.NotNull(path, "path");
            Ensure.NotNull(contentService, "contentService");
            Ensure.NotNull(versionService, "versionService");
            this.repositoryFactory = repositoryFactory;
            this.log = log;
            this.nuGetLog = new NuGetLogger(log);
            Path = path;
            this.contentService = contentService;
            this.versionService = versionService;
            this.frameworkFilter = frameworkFilter;

            if (packageFilter == null)
                packageFilter = OkNuGetPackageFilter.Instance;

            this.packageFilter = packageFilter;
        }

        public bool IsInstalled(string packageId)
        {
            Ensure.NotNull(packageId, "packageId");

            if (!File.Exists(ConfigFilePath))
                return false;

            using (Stream fileContent = new FileStream(ConfigFilePath, FileMode.Open))
            {
                PackagesConfigReader reader = new PackagesConfigReader(fileContent);
                return reader.GetPackages().Any(p => p.PackageIdentity.Id == packageId);
            }
        }

        public bool IsInstalled(IPackageIdentity package)
        {
            Ensure.NotNull(package, "package");

            if (!File.Exists(ConfigFilePath))
                return false;

            using (Stream fileContent = new FileStream(ConfigFilePath, FileMode.Open))
            {
                PackagesConfigReader reader = new PackagesConfigReader(fileContent);
                return reader.GetPackages().Any(p => p.PackageIdentity.Id == package.Id && p.PackageIdentity.Version.ToFullString() == package.Version);
            }
        }

        public void Install(IPackageIdentity package)
        {
            Ensure.NotNull(package, "package");

            using (PackagesConfigWriter writer = new PackagesConfigWriter(ConfigFilePath, !File.Exists(ConfigFilePath)))
            {
                if (IsInstalled(package))
                {
                    log.Debug($"Removing entry '{package.ToIdentityString()}' from packages.config.");
                    writer.RemovePackageEntry(package.Id, new NuGetVersion(package.Version), NuGetFramework.AnyFramework);
                }

                log.Debug($"Add entry '{package.ToIdentityString()}' to packages.config.");
                writer.AddPackageEntry(package.Id, new NuGetVersion(package.Version), NuGetFramework.AnyFramework);
            }
        }

        public void Uninstall(IPackageIdentity package)
        {
            Ensure.NotNull(package, "package");

            using (PackagesConfigWriter writer = new PackagesConfigWriter(ConfigFilePath, !File.Exists(ConfigFilePath)))
            {
                log.Debug($"Removing entry '{package.ToIdentityString()}' from packages.config.");
                writer.RemovePackageEntry(package.Id, new NuGetVersion(package.Version), NuGetFramework.AnyFramework);
            }
        }

        public void Uninstall(string packageId)
        {
            Ensure.NotNullOrEmpty(packageId, "packageId");

            using (PackagesConfigWriter writer = new PackagesConfigWriter(ConfigFilePath, !File.Exists(ConfigFilePath)))
            {
                log.Debug($"Removing entry '{packageId}' from packages.config.");
                writer.RemovePackageEntry(packageId, null, null);
            }
        }

        private async Task<bool> ReadPackageConfig(Func<PackageReference, SourceCacheContext, Task<bool>> handler, CancellationToken cancellationToken)
        {
            if (!File.Exists(ConfigFilePath))
                return false;

            using (Stream fileContent = new FileStream(ConfigFilePath, FileMode.Open))
            using (SourceCacheContext context = new SourceCacheContext())
            {
                PackagesConfigReader reader = new PackagesConfigReader(fileContent);
                foreach (PackageReference package in reader.GetPackages())
                {
                    log.Debug($"Installed package '{package.PackageIdentity}'.");

                    if (await handler(package, context))
                        break;
                }
            }

            return true;
        }

        public async Task<IReadOnlyCollection<IInstalledPackage>> GetInstalledAsync(IEnumerable<IPackageSource> packageSources, CancellationToken cancellationToken)
        {
            log.Debug($"Getting list of installed packages from '{ConfigFilePath}'.");

            List<IInstalledPackage> result = new List<IInstalledPackage>();
            await ReadPackageConfig(
                async (package, context) =>
                {
                    foreach (IPackageSource packageSource in packageSources)
                    {
                        log.Debug($"Looking in repository '{packageSource.Uri}'.");
                        SourceRepository repository = repositoryFactory.Create(packageSource);

                        PackageMetadataResource metadataResource = await repository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
                        if (metadataResource != null)
                        {
                            IPackageSearchMetadata metadata = await metadataResource.GetMetadataAsync(package.PackageIdentity, context, nuGetLog, cancellationToken);
                            if (metadata != null)
                            {
                                log.Debug($"Package '{package.PackageIdentity}' was found.");

                                NuGetPackageFilterResult filterResult = packageFilter.IsPassed(metadata);
                                result.Add(new NuGetInstalledPackage(
                                    new NuGetPackage(metadata, false, repository, contentService, versionService),
                                    filterResult == NuGetPackageFilterResult.Ok
                                ));
                                break;
                            }
                        }
                    }

                    return false;
                },
                cancellationToken
            );

            log.Debug($"Returning '{result.Count}' installed packages.");
            return result;
        }

        public async Task<IPackageIdentity> FindInstalledAsync(string packageId, CancellationToken cancellationToken)
        {
            log.Debug($"Finding installed packages with id '{packageId}'.");

            IPackageIdentity result = null;
            await ReadPackageConfig(
                (package, context) =>
                {
                    if (package.PackageIdentity.Id == packageId)
                    {
                        result = new NuGetPackageIdentity(package.PackageIdentity);
                        return Task.FromResult(true);
                    }

                    return Task.FromResult(false);
                },
                cancellationToken
            );

            return result;
        }
    }
}
