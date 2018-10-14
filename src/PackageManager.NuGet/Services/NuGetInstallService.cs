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
        private readonly ILogger nuGetLog;
        private readonly INuGetPackageFilter packageFilter;
        private readonly NuGetPackageContent.IFrameworkFilter frameworkFilter;

        public string Path { get; }
        public string ConfigFilePath => System.IO.Path.Combine(Path, "packages.config");

        public NuGetInstallService(IFactory<SourceRepository, IPackageSource> repositoryFactory, ILog log, string path, INuGetPackageFilter packageFilter = null, NuGetPackageContent.IFrameworkFilter frameworkFilter = null)
        {
            Ensure.NotNull(repositoryFactory, "repositoryFactory");
            Ensure.NotNull(log, "log");
            Ensure.NotNull(path, "path");
            this.repositoryFactory = repositoryFactory;
            this.log = log;
            this.nuGetLog = new NuGetLogger(log);
            Path = path;
            this.frameworkFilter = frameworkFilter;

            if (packageFilter == null)
                packageFilter = OkNuGetPackageFilter.Instance;

            this.packageFilter = packageFilter;
        }

        public bool IsInstalled(IPackage package)
        {
            Ensure.NotNull(package, "package");

            if (!File.Exists(ConfigFilePath))
                return false;

            using (Stream fileContent = new FileStream(ConfigFilePath, FileMode.Open))
            {
                PackagesConfigReader reader = new PackagesConfigReader(fileContent);
                return reader.GetPackages().Any(p => p.PackageIdentity.Id == package.Id);
            }
        }

        public void Install(IPackage package)
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

        public void Uninstall(IPackage package)
        {
            Ensure.NotNull(package, "package");

            using (PackagesConfigWriter writer = new PackagesConfigWriter(ConfigFilePath, !File.Exists(ConfigFilePath)))
                writer.RemovePackageEntry(package.Id, new NuGetVersion(package.Version), NuGetFramework.AnyFramework);
        }

        public async Task<IReadOnlyCollection<IInstalledPackage>> GetInstalledAsync(IEnumerable<IPackageSource> packageSources, CancellationToken cancellationToken)
        {
            log.Debug($"Getting list of installed packages from '{ConfigFilePath}'.");

            if (!File.Exists(ConfigFilePath))
                return new List<IInstalledPackage>(0);

            List<IInstalledPackage> result = new List<IInstalledPackage>();

            using (Stream fileContent = new FileStream(ConfigFilePath, FileMode.Open))
            using (SourceCacheContext context = new SourceCacheContext())
            {
                PackagesConfigReader reader = new PackagesConfigReader(fileContent);
                foreach (PackageReference package in reader.GetPackages())
                {
                    log.Debug($"Installed package '{package.PackageIdentity}'.");

                    foreach (IPackageSource packageSource in packageSources)
                    {
                        log.Debug($"Lookin in repository '{packageSource.Uri}'.");
                        SourceRepository repository = repositoryFactory.Create(packageSource);

                        PackageMetadataResource metadataResource = await repository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
                        if (metadataResource != null)
                        {
                            var metadata = await metadataResource.GetMetadataAsync(package.PackageIdentity, context, nuGetLog, cancellationToken);
                            if (metadata != null)
                            {
                                log.Debug($"Package '{package.PackageIdentity}' was found.");

                                NuGetPackageFilterResult filterResult = packageFilter.IsPassed(metadata);
                                result.Add(new NuGetInstalledPackage(
                                    new NuGetPackage(metadata, repository, log, frameworkFilter),
                                    filterResult == NuGetPackageFilterResult.Ok
                                ));

                                break;
                            }
                        }
                    }
                }
            }

            log.Debug($"Returning '{result.Count}' installed packages.");
            return result;
        }
    }
}
