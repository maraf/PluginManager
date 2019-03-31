using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    internal class SelfPackage : IPackageIdentity
    {
        public string Id { get; }
        public string Version { get; }

        public SelfPackage(string id)
        {
            Ensure.NotNull(id, "id");
            Id = id;
            Version = VersionInfo.Version;

            int indexOfPlus = Version.IndexOf('+');
            if (indexOfPlus > 0)
                Version = Version.Substring(0, indexOfPlus);
        }

        public bool Equals(IPackageIdentity other)
        {
            if (other == null)
                return false;

            return Id == other.Id && Version == other.Version;
        }
    }
}
