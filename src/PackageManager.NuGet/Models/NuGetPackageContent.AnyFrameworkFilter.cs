using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuGet.Packaging;

namespace PackageManager.Models
{
    partial class NuGetPackageContent
    {
        public class AnyFrameworkFilter : IFrameworkFilter
        {
            public bool IsPassed(FrameworkSpecificGroup group)
                => group.TargetFramework.IsAny;

            private static AnyFrameworkFilter instance;
            private static object instanceLock = new object();

            public static AnyFrameworkFilter Instance
            {
                get
                {
                    if (instance == null)
                    {
                        lock (instanceLock)
                        {
                            if (instance == null)
                                instance = new AnyFrameworkFilter();
                        }
                    }

                    return instance;
                }
            }
        }
    }
}
