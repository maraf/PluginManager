using NuGet.Packaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Models
{
    partial class NuGetPackageContent
    {
        public interface IFrameworkFilter
        {
            bool IsPassed(FrameworkSpecificGroup group);
        }
    }
}
