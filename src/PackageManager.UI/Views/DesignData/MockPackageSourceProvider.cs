using PackageManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Views.DesignData
{
    public class MockPackageSourceProvider : IPackageSourceProvider
    {
        public string Url => "https://api.nuget.org/v3/index.json";
    }
}
