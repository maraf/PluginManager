using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager
{
    public static class VersionInfo
    {
        internal const string Version = "0.6.0";
        internal const string Preview = null; //-beta1

        public static Version GetVersion()
        {
            return new Version(Version);
        }
    }
}
