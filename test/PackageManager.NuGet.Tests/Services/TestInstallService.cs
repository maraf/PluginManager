using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neptuo.Logging;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Versioning;
using PackageManager.Models;
using PackageManager.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    [TestClass]
    public class TestInstallService
    {
        public static string ExtractPath => Path.Combine(Environment.CurrentDirectory, "Plugins");
        public static string ExtractFilePath => Path.Combine(ExtractPath, "packages.config");

        private IInstallService install;
        private Package package = new Package(ExtractPath, "PluginA", "2.0.0");

        private void Reader(Action<PackagesConfigReader> handler)
        {
            using (Stream fileContent = new FileStream(ExtractFilePath, FileMode.Open))
            {
                PackagesConfigReader reader = new PackagesConfigReader(fileContent);
                handler(reader);
            }
        }

        private void Writer(Action<PackagesConfigWriter> handler)
        {
            using (PackagesConfigWriter writer = new PackagesConfigWriter(ExtractFilePath, !File.Exists(ExtractFilePath)))
            {
                handler(writer);
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            install = InstallService.Create(ExtractPath);

            if (File.Exists(ExtractFilePath))
                File.Delete(ExtractFilePath);
        }

        [TestMethod]
        public void Install()
        {
            install.Install(package.Object);

            Reader(reader =>
            {
                Assert.IsTrue(reader.GetPackages().Any(p => p.PackageIdentity.Id == package.Object.Id && p.PackageIdentity.Version.ToFullString() == package.Object.Version));
            });
        }

        [TestMethod]
        public void UnInstall()
        {
            Writer(writer => writer.AddPackageEntry("PluginA", new NuGetVersion("2.0.0"), NuGetFramework.AnyFramework));

            install.Uninstall(package.Object);

            Reader(reader =>
            {
                Assert.IsFalse(reader.GetPackages().Any(p => p.PackageIdentity.Id == package.Object.Id && p.PackageIdentity.Version.ToFullString() == package.Object.Version));
            });
        }

        [TestMethod]
        public void IsInstalled()
        {
            Assert.IsFalse(install.IsInstalled(package.Object));

            Writer(writer => writer.AddPackageEntry("PluginA", new NuGetVersion("2.0.0"), NuGetFramework.AnyFramework));

            Assert.IsTrue(install.IsInstalled(package.Object));
        }
    }
}
