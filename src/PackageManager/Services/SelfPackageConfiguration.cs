using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    public class SelfPackageConfiguration
    {
        public string PackageId { get; }

        public SelfPackageConfiguration(string packageId)
        {
            PackageId = packageId;
        }
    }
}
