using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    [TestClass]
    public class TestPackageContent
    {
        public const string ConfigFilePath = "PackageSource_Content.config";
        public static string ExtractPath => Path.Combine(Environment.CurrentDirectory, "UserPlugins");

        private ISearchService search;
        private IPackageSourceCollection sources;
        private IPackage package;

        [TestInitialize]
        public void Initialize()
        {
            (search, sources) = SearchService.Create(ConfigFilePath);
            package = search.SearchAsync(sources.All, "PluginA").Result.FirstOrDefault();
            Assert.IsNotNull(package);

            if (!Directory.Exists(ExtractPath))
                Directory.CreateDirectory(ExtractPath);
        }

        [TestMethod]
        public void ExtractToAsync()
        {
            IPackageContent packageContent = package.GetContentAsync(default).Result;
            packageContent.ExtractToAsync(ExtractPath, default).Wait();

            Assert.IsTrue(File.Exists(Path.Combine(ExtractPath, "PluginA-2.txt")));
        }

        [TestMethod]
        public void RemoveFromAsync()
        {
            ExtractToAsync();

            IPackageContent packageContent = package.GetContentAsync(default).Result;
            packageContent.RemoveFromAsync(ExtractPath, default).Wait();

            Assert.IsFalse(File.Exists(Path.Combine(ExtractPath, "PluginA-2.txt")));
        }
    }
}
