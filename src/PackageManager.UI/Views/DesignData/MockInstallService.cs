using PackageManager.Models;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Views.DesignData
{
    internal class MockInstallService : IInstallService
    {
        public bool IsInstalled(IPackage package)
            => false;
    }
}
