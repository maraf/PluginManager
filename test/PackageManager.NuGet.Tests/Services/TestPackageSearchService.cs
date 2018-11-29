using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestInitialize]
        public void Initialize()
        {
            (search, sources) = SearchService.Create(ConfigFilePath);
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
