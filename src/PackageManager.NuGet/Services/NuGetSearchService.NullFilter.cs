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
            public FilterResult IsPassed(IPackageSearchMetadata package)
                => FilterResult.Ok;

            private static NullFilter instance;
            private static object instanceLock = new object();

            public static NullFilter Instance
            {
                get
                {
                    if (instance == null)
                    {
                        lock (instanceLock)
                        {
                            if (instance == null)
                                instance = new NullFilter();
                        }
                    }

                    return instance;
                }
            }
        }
    }
}
