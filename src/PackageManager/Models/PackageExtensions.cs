using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    public static class PackageExtensions
    {
        /// <summary>
        /// Gets package identity in form "{id}-v{version}" for <paramref name="package"/>.
        /// </summary>
        /// <param name="package">A package to identity for.</param>
        /// <returns>A package identity string.</returns>
        public static string ToIdentityString(this IPackage package)
        {
            Ensure.NotNull(package, "package");
            return $"{package.Id}-v{package.Version}";
        }
    }
}
