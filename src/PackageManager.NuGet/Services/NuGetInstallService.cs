using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Versioning;
using PackageManager.Models;

namespace PackageManager.Services
{
    public class NuGetInstallService : IInstallService
    {
        public string Path { get; }
        public string FrameworkMoniker { get; }
        public string ConfigFilePath => System.IO.Path.Combine(Path, "packages.config");

        public NuGetInstallService(string path, string frameworkMoniker)
        {
            Ensure.NotNull(path, "path");
            Ensure.NotNull(frameworkMoniker, "frameworkMoniker");
            Path = path;
            FrameworkMoniker = frameworkMoniker;
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
                writer.AddPackageEntry(package.Id, new NuGetVersion(package.Version), new NuGetFramework(FrameworkMoniker));
        }

        public void Uninstall(IPackage package)
        {
            Ensure.NotNull(package, "package");

            using (PackagesConfigWriter writer = new PackagesConfigWriter(ConfigFilePath, !File.Exists(ConfigFilePath)))
                writer.RemovePackageEntry(package.Id, new NuGetVersion(package.Version), new NuGetFramework(FrameworkMoniker));
        }
    }
}
