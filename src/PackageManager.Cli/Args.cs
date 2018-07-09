using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager
{
    public partial class Args
    {
        public string Path { get; set; }
        public string PackageSourceUrl { get; set; }

        public bool IsUpdateCount { get; set; }
    }
}
