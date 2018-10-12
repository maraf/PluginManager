using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    [TestClass]
    public class TestPackageSourceCollection
    {
        public const string ConfigFilePath = "PackageSource.config";

        private static NuGetPackageSourceCollection CreateSourceCollection() 
            => new NuGetPackageSourceCollection(new PackageSourceProvider(new Settings(Environment.CurrentDirectory, ConfigFilePath)));

        private static void EnsureConfigDeleted()
        {
            string path = Path.Combine(Environment.CurrentDirectory, ConfigFilePath);

            if (File.Exists(path))
                File.Delete(path);
        }

        [TestMethod]
        public void CreateNewConfig()
        {
            EnsureConfigDeleted();

            var sources = CreateSourceCollection();
            Assert.IsNull(sources.Primary);
            Assert.AreEqual(1, sources.All.Count);
        }

        [TestMethod]
        public void Crud()
        {
            EnsureConfigDeleted();

            var sources = CreateSourceCollection();
            Assert.IsNull(sources.Primary);
            Assert.AreEqual(1, sources.All.Count);

            // Because NuGet.Client by default creates a config with nuget.org source.
            sources.Remove(sources.All.First());
            Assert.AreEqual(0, sources.All.Count);

            var sourceUri = new Uri("https://wwww.nuget.org", UriKind.Absolute);
            var sourceName = "NuGet.org";
            var source = sources.Add(sourceName, sourceUri);
            Assert.AreEqual(sourceName, source.Name);
            Assert.AreEqual(sourceUri, source.Uri);
            Assert.AreEqual(1, sources.All.Count);

            sources.MarkAsPrimary(source);
            Assert.IsNotNull(sources.Primary);

            sources = CreateSourceCollection();
            Assert.IsNotNull(sources.Primary);
            Assert.AreEqual(1, sources.All.Count);

            source = sources.All.First();
            Assert.AreEqual(sourceName, source.Name);
            Assert.AreEqual(sourceUri, source.Uri);

            sources.MarkAsPrimary(null);
            Assert.IsNull(sources.Primary);

            sources.Remove(source);
            Assert.AreEqual(0, sources.All.Count);

            sources = CreateSourceCollection();
            Assert.IsNull(sources.Primary);
            Assert.AreEqual(0, sources.All.Count);
        }
    }
}
