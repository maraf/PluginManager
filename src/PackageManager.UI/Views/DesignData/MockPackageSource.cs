using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Views.DesignData
{
    public class MockPackageSource : IPackageSource
    {
        public string Name { get; }
        public Uri Uri { get; }

        public MockPackageSource(string name, Uri uri)
        {
            Name = name;
            Uri = uri;
        }
    }
}
