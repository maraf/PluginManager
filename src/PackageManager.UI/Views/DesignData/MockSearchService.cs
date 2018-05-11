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
                new MockPackage()
                {
                    Id = "GitExtensions.BundleBackuper",
                    Version = "1.0.0",
                    Description ="Branch backuping plugin for GitExtensions. GIT bundles is a great way to create backups of local branches. This extension for GitExtensions creates item in top menu containg all bundles at specified path. Clicking bundle item maps this bundle as remote. Beside this restore operation, it also contains button to create bundle/backup between current branch head and last commit pushed commit.",
                    Dependecies = new List<IPackage>()
                }
            });
        }
    }
}
