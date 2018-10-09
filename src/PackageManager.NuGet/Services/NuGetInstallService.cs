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
        private readonly IFactory<SourceRepository, string> repositoryFactory;
        private readonly ILog log;
        private readonly ILogger nuGetLog;
        private readonly INuGetPackageFilter packageFilter;
        private readonly NuGetPackageContent.IFrameworkFilter frameworkFilter;

        public string Path { get; }
        public string ConfigFilePath => System.IO.Path.Combine(Path, "packages.config");

        public NuGetInstallService(IFactory<SourceRepository, string> repositoryFactory, ILog log, string path, INuGetPackageFilter packageFilter = null, NuGetPackageContent.IFrameworkFilter frameworkFilter = null)
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
                    writer.RemovePackageEntry(package.Id, new NuGetVersion(package.Version), NuGetFramework.AnyFramework);

                writer.AddPackageEntry(package.Id, new NuGetVersion(package.Version), NuGetFramework.AnyFramework);
            }
        }

        public void Uninstall(IPackage package)
        {
            Ensure.NotNull(package, "package");

            using (PackagesConfigWriter writer = new PackagesConfigWriter(ConfigFilePath, !File.Exists(ConfigFilePath)))
                writer.RemovePackageEntry(package.Id, new NuGetVersion(package.Version), NuGetFramework.AnyFramework);
        }

        public async Task<IReadOnlyCollection<IInstalledPackage>> GetInstalledAsync(string packageSourceUrl, CancellationToken cancellationToken)
        {
            if (!File.Exists(ConfigFilePath))
                return new List<IInstalledPackage>(0);

            SourceRepository repository = repositoryFactory.Create(packageSourceUrl);

            using (Stream fileContent = new FileStream(ConfigFilePath, FileMode.Open))
            using (SourceCacheContext context = new SourceCacheContext())
            {
                List<IInstalledPackage> result = new List<IInstalledPackage>();

                PackagesConfigReader reader = new PackagesConfigReader(fileContent);
                foreach (PackageReference package in reader.GetPackages())
                {
                    PackageMetadataResource metadataResource = await repository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
                    if (metadataResource != null)
                    {
                        var metadata = await metadataResource.GetMetadataAsync(package.PackageIdentity, context, nuGetLog, cancellationToken);
                        if (metadata != null)
                        {
                            NuGetPackageFilterResult filterResult = packageFilter.IsPassed(metadata);
                            result.Add(new NuGetInstalledPackage(
                                new NuGetPackage(metadata, repository, log, frameworkFilter),
                                filterResult == NuGetPackageFilterResult.Ok
                            ));
                        }
                    }
                }

                return result;
            }
        }
    }
}
