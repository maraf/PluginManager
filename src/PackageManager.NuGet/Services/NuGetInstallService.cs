using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Neptuo;
using Neptuo.Activators;
using NuGet.Common;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using PackageManager.Models;

namespace PackageManager.Services
{
    public class NuGetInstallService : IInstallService
    {
        private readonly IFactory<SourceRepository, string> repositoryFactory;

        public string Path { get; }
        public string ConfigFilePath => System.IO.Path.Combine(Path, "packages.config");

        public NuGetInstallService(IFactory<SourceRepository, string> repositoryFactory, string path)
        {
            Ensure.NotNull(repositoryFactory, "repositoryFactory");
            Ensure.NotNull(path, "path");
            this.repositoryFactory = repositoryFactory;
            Path = path;
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

        public async Task<IReadOnlyCollection<IPackage>> GetInstalledAsync(string packageSourceUrl, CancellationToken cancellationToken)
        {
            if (!File.Exists(ConfigFilePath))
                return new List<IPackage>(0);

            SourceRepository repository = repositoryFactory.Create(packageSourceUrl);

            using (Stream fileContent = new FileStream(ConfigFilePath, FileMode.Open))
            using (SourceCacheContext context = new SourceCacheContext())
            {
                List<IPackage> result = new List<IPackage>();

                PackagesConfigReader reader = new PackagesConfigReader(fileContent);
                foreach (PackageReference package in reader.GetPackages())
                {
                    PackageMetadataResource metadataResource = await repository.GetResourceAsync<PackageMetadataResource>(cancellationToken);
                    if (metadataResource != null)
                    {
                        var metadata = await metadataResource.GetMetadataAsync(package.PackageIdentity, context, NullLogger.Instance, cancellationToken);
                        if (metadata != null)
                        {
                            result.Add(new NuGetPackage(metadata, repository));
                            continue;
                        }
                    }
                }

                return result;
            }
        }
    }
}
