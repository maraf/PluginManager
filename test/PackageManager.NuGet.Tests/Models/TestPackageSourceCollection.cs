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

        [TestMethod]
        public void Crud()
        {
            EnsureConfigDeleted();

            var sources = new NuGetPackageSourceCollection(new PackageSourceProvider(new Settings(Environment.CurrentDirectory, ConfigFilePath)));
        }

        private static void EnsureConfigDeleted()
        {
            if (File.Exists(ConfigFilePath))
                File.Delete(ConfigFilePath);
        }
    }
}
