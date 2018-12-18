using Neptuo;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    public class SelfPackageConfiguration : IEquatable<IPackageIdentity>
    {
        public string PackageId { get; }

        public SelfPackageConfiguration(string packageId)
        {
            PackageId = packageId;
        }

        public bool Equals(IPackageIdentity packageIdentity)
        {
            if (packageIdentity == null)
                return false;

            return Equals(packageIdentity.Id);
        }

        public bool Equals(string packageId)
        {
            if (PackageId == null || packageId == null)
                return false;

            return PackageId == packageId;
        }
    }
}
