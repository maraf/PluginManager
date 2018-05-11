using PackageManager.Models;
using PackageManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.Views.DesignData
{
    public static class ViewModelLocator
    {
        private static BrowserViewModel browser;
        private static IPackage package;

        public static BrowserViewModel Browser
        {
            get
            {
                if (browser == null)
                {
                    browser = new BrowserViewModel(new MockSearchService())
                    {
                        SearchText = "GitExtensions",
                        Source = "https://api.nuget.org/v3/index.json",
                    };
                    browser.Search.Execute(null);
                }

                return browser;
            }
        }

        public static IPackage Package
        {
            get
            {
                if (package == null)
                {
                    package = new MockPackage()
                    {
                        Id = "GitExtensions.BundleBackuper",
                        Version = "1.0.0",
                        Description = $"Branch backuping plugin for GitExtensions. {Environment.NewLine}GIT bundles is a great way to create backups of local branches. This extension for GitExtensions creates item in top menu containg all bundles at specified path. Clicking bundle item maps this bundle as remote. Beside this restore operation, it also contains button to create bundle/backup between current branch head and last commit pushed commit.",
                        Dependecies = new List<IPackage>(),
                        Authors = "maraf",
                        Tags = "GitExtensions",
                        ProjectUrl = new Uri("https://github.com/maraf/GitExtensions.BundleBackuper", UriKind.Absolute),
                        LicenseUrl = new Uri("https://raw.githubusercontent.com/maraf/GitExtensions.BundleBackuper/master/LICENSE", UriKind.Absolute),
                        Published = DateTime.Today
                    };
                }

                return package;
            }
        }
    }
}
