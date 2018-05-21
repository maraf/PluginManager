using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Protocol.Core.Types;

namespace PackageManager.Services
{
    partial class NuGetSearchService
    {
        public class NullFilter : IFilter
        {
            public bool IsPassed(IPackageSearchMetadata package)
                => true;
        }
    }
}
