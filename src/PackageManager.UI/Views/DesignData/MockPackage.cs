using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Views.DesignData
{
    public class MockPackage : IPackage
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public Uri IconUrl { get; set; }
        public IReadOnlyCollection<IPackage> Dependecies { get; set; }
    }
}
