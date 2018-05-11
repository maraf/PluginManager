using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Services
{
    public interface IInstallService
    {
        bool IsInstalled(IPackage package);
    }
}
