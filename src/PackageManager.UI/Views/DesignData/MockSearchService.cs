using PackageManager.Models;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Views.DesignData
{
    public class MockSearchService : ISearchService
    {
        public Task<IEnumerable<IPackage>> SearchAsync(string packageSourceUrl, string searchText, SearchOptions options = null)
        {
            return Task.FromResult<IEnumerable<IPackage>>(new List<IPackage>()
            {
                ViewModelLocator.Package
            });
        }
    }
}
