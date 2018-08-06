using PackageManager.Models;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Views.DesignData
{
    public class MockSelfUpdateService : ISelfUpdateService
    {
        public bool IsSelfUpdate { get; set; }

        public void RunNewInstance(IPackage package)
        { }

        public void Update(IPackage latest)
        { }
    }
}
