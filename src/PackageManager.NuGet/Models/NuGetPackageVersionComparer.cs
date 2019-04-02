using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    public class NuGetPackageVersionComparer : IComparer<IPackageIdentity>
    {
        public static readonly NuGetPackageVersionComparer Instance = new NuGetPackageVersionComparer();

        public int Compare(IPackageIdentity x, IPackageIdentity y)
        {
            NuGetVersion xVersion = new NuGetVersion(x.Version);
            NuGetVersion yVersion = new NuGetVersion(y.Version);
            return xVersion.CompareTo(yVersion);
        }
    }
}
