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
    public class SearchService
    {
        private static void EnsureConfigDeleted(string configFilePath)
        {
            string path = Path.Combine(Environment.CurrentDirectory, configFilePath);

            if (File.Exists(path))
                File.Delete(path);
        }

        public static (ISearchService search, IPackageSourceCollection sources) Create(string configFilePath)
        {
            var frameworks = new List<NuGetFramework>() { NuGetFramework.AnyFramework };

            var frameworkFilter = new NuGetFrameworkFilter(frameworks);
            var packageFilter = new DependencyNuGetPackageFilter(
                new List<Args.Dependency>()
                {
                    new Args.Dependency("GitExtensions.Plugins", null)
                },
                frameworks
            );
            var search = new NuGetSearchService(
                new NuGetSourceRepositoryFactory(),
                new DefaultLog(),
                new NuGetPackageContentService(new DefaultLog()),
                new NuGetPackageVersionService(
                    new NuGetPackageContentService(new DefaultLog()),
                    new DefaultLog(),
                    packageFilter,
                    frameworkFilter
                ),
                new DependencyNuGetPackageFilter(
                    new List<Args.Dependency>() { new Args.Dependency("GitExtensions.Plugins", null) },
                    frameworks
                ),
                new NuGetFrameworkFilter(frameworks)
            );

            EnsureConfigDeleted(configFilePath);
            var sources = new NuGetPackageSourceCollection(
                new PackageSourceProvider(new Settings(Environment.CurrentDirectory, configFilePath))
            );

            sources.Remove(sources.All.First());
            sources.Add().Name("Local").Uri(new Uri(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\data\NuGetFeed"), UriKind.Absolute)).Save();

            Assert.IsTrue(Directory.Exists(sources.All.First().Uri.AbsolutePath));

            return (search, sources);
        }
    }
}
