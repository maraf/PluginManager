using PackageManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager
{
    public partial class Args : IPackageSourceProvider
    {
        public string Path { get; set; }
        public string PackageSourceUrl { get; set; }

        public bool IsUpdateCount { get; set; }

        string IPackageSourceProvider.Url => PackageSourceUrl;
    }
}
