using NuGet.Frameworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager
{
    public class Args
    {
        public string Path { get; set; }
        public IReadOnlyCollection<NuGetFramework> Monikers { get; set; }
        public (string id, string version)[] Dependencies { get; set; }
        public string PackageSourceUrl { get; set; }

        public Args()
        {
            Monikers = Array.Empty<NuGetFramework>();
            Dependencies = Array.Empty<(string id, string version)>();
        }
    }
}
