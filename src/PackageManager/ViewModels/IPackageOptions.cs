using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels
{
    public interface IPackageOptions
    {
        bool IsPrereleaseIncluded { get; }
    }
}
