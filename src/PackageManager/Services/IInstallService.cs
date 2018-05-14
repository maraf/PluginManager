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
        string Path { get; }

        bool IsInstalled(IPackage package);
    }
}
