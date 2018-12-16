using Neptuo;
using NuGet.Packaging.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    public class NuGetPackageIdentity : IPackageIdentity
    {
        private readonly PackageIdentity identity;

        public string Id => identity.Id;
        public string Version => identity.Version.ToFullString();

        public NuGetPackageIdentity(PackageIdentity identity)
        {
            Ensure.NotNull(identity, "identity");
            this.identity = identity;
        }

        public bool Equals(IPackageIdentity other)
        {
            if (other == null)
                return false;

            return Id == other.Id && Version == other.Version;
        }
    }
}
