using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neptuo;
using NuGet.Frameworks;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using static PackageManager.Services.NuGetSearchService;

namespace PackageManager.Services
{
    public class DependencyNuGetSearchFilter : IFilter
    {
        private readonly (string id, string version)[] dependencies;
        private readonly IReadOnlyCollection<NuGetFramework> frameworks;

        public DependencyNuGetSearchFilter((string id, string version)[] dependencies, IReadOnlyCollection<NuGetFramework> frameworks)
        {
            Ensure.NotNull(dependencies, "dependencies");
            Ensure.NotNull(frameworks, "frameworks");
            this.dependencies = dependencies;
            this.frameworks = frameworks;
        }

        public FilterResult IsPassed(IPackageSearchMetadata package)
        {
            if (!dependencies.Any())
                return FilterResult.Ok;

            foreach (var group in package.DependencySets)
            {
                if (frameworks.Contains(group.TargetFramework))
                {
                    foreach (var dependency in dependencies)
                    {
                        PackageDependency packageDependency = group.Packages.FirstOrDefault(p => p.Id == dependency.id);
                        if (packageDependency == null)
                            return FilterResult.TryOlderVersion;

                        if (dependency.version != null && !packageDependency.VersionRange.Satisfies(new NuGetVersion(dependency.version)))
                            return FilterResult.TryOlderVersion;
                    }

                    return FilterResult.Ok;
                }
            }

            return FilterResult.NotPassed;
        }
    }
}
