using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neptuo.Logging;
using NuGet.Configuration;
using NuGet.Frameworks;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    [TestClass]
    public class TestPackageSearchService
    {
        public const string ConfigFilePath = "PackageSource_Search.config";

        private ISearchService search;
        private IPackageSourceCollection sources;

        private static void EnsureConfigDeleted()
        {
            string path = Path.Combine(Environment.CurrentDirectory, ConfigFilePath);

            if (File.Exists(path))
                File.Delete(path);
        }

        [TestInitialize]
        public void Initialize()
        {
            var frameworks = new List<NuGetFramework>() { NuGetFramework.AnyFramework };

            search = new NuGetSearchService(
                new NuGetSourceRepositoryFactory(),
                new DefaultLog(),
                new DependencyNuGetPackageFilter(
                    new List<(string, string)>() { ("GitExtensions.Plugins", null) },
                    frameworks
                ),
                new NuGetFrameworkFilter(frameworks)
            );

            EnsureConfigDeleted();
            sources = new NuGetPackageSourceCollection(
                new PackageSourceProvider(new Settings(Environment.CurrentDirectory, ConfigFilePath))
            );

            sources.Remove(sources.All.First());
            sources.Add().Name("Local").Uri(new Uri(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\data\NuGetFeed"), UriKind.Absolute)).Save();

            Assert.IsTrue(Directory.Exists(sources.All.First().Uri.AbsolutePath));
        }

        [TestMethod]
        public void SearchAsync_Success()
        {
            List<IPackage> packages = search.SearchAsync(sources.All, "Plugin").Result.ToList();
            Assert.AreEqual(2, packages.Count);
            Assert.AreEqual("PluginA", packages[0].Id);
            Assert.AreEqual("PluginB", packages[1].Id);
        }

        [TestMethod]
        public void SearchAsync_Nothing()
        {
            List<IPackage> packages = search.SearchAsync(sources.All, "CustomPackage").Result.ToList();
            Assert.AreEqual(0, packages.Count);
        }

        [TestMethod]
        public void FindLatestVersionAsync()
        {
            IPackage package = search.FindLatestVersionAsync(sources.All, search.SearchAsync(sources.All, "PluginA").Result.First()).Result;
            Assert.IsNotNull(package);
        }
    }
}
