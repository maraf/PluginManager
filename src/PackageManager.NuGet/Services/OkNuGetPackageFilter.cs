using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Protocol.Core.Types;

namespace PackageManager.Services
{
    public class OkNuGetPackageFilter : INuGetPackageFilter
    {
        public NuGetPackageFilterResult IsPassed(IPackageSearchMetadata package)
            => NuGetPackageFilterResult.Ok;

        #region Singleton

        private static OkNuGetPackageFilter instance;
        private static object instanceLock = new object();

        public static OkNuGetPackageFilter Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (instanceLock)
                    {
                        if (instance == null)
                            instance = new OkNuGetPackageFilter();
                    }
                }

                return instance;
            }
        }

        #endregion
    }
}
