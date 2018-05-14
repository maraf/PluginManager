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
        public string[] Monikers { get; set; }
        public (string id, string version)[] Dependencies { get; set; }

        public Args()
        {
            Monikers = Array.Empty<string>();
            Dependencies = Array.Empty<(string id, string version)>();
        }
    }
}
