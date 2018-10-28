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

namespace PackageManager.Services
{
    public class DependencyNuGetPackageFilter : INuGetPackageFilter
    {
        private readonly IReadOnlyCollection<(string id, string version)> dependencies;
        private readonly IReadOnlyCollection<NuGetFramework> frameworks;

        public DependencyNuGetPackageFilter(IReadOnlyCollection<(string id, string version)> dependencies, IReadOnlyCollection<NuGetFramework> frameworks)
        {
            Ensure.NotNull(dependencies, "dependencies");
            Ensure.NotNull(frameworks, "frameworks");
            this.dependencies = dependencies;
            this.frameworks = frameworks;
        }

        public NuGetPackageFilterResult IsPassed(IPackageSearchMetadata package)
        {
            if (!dependencies.Any())
                return NuGetPackageFilterResult.Ok;

            foreach (var group in package.DependencySets)
            {
                if (frameworks.Contains(group.TargetFramework))
                {
                    NuGetPackageFilterResult result = NuGetPackageFilterResult.Ok;

                    // Dependency filtering:
                    // - When incompatible dependency version is found there is a chance that previous version has the right one.
                    // - When all dependencies are missing, don't even try previous versions.
                    foreach (var dependency in dependencies)
                    {
                        PackageDependency packageDependency = group.Packages.FirstOrDefault(p => p.Id == dependency.id);
                        if (packageDependency == null)
                            result = NuGetPackageFilterResult.NotCompatible;

                        if (dependency.version != null && !packageDependency.VersionRange.Satisfies(new NuGetVersion(dependency.version)))
                            return NuGetPackageFilterResult.NotCompatibleVersion;
                    }

                    return result;
                }
            }

            return NuGetPackageFilterResult.NotCompatible;
        }
    }
}
