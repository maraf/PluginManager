using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Views.DesignData
{
    internal class MockInstalledPackage : IInstalledPackage
    {
        public IPackage Definition { get; }
        public bool IsCompatible { get; }

        public MockInstalledPackage(IPackage definition, bool isCompatible)
        {
            Definition = definition;
            IsCompatible = isCompatible;
        }
    }
}
