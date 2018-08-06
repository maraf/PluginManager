using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    public interface ISelfUpdateService
    {
        bool IsSelfUpdate { get; }

        void Update(IPackage latest);

        void RunNewInstance(IPackage package);
    }
}
