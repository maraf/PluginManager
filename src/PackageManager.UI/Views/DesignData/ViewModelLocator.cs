using PackageManager.Models;
using PackageManager.Services;
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
        private static SelfPackageConfiguration selfPackageConfiguration;
        private static MainViewModel main;
        private static BrowserViewModel browser;
        private static InstalledViewModel installed;
        private static UpdatesViewModel updates;
        private static IPackage package;
        private static IInstalledPackage compatiblePackage;
        private static IInstalledPackage incompatiblePackage;
        private static PackageSourceViewModel packageSources;
        private static IPackageSourceCollection packageSourceCollection;

        public static SelfPackageConfiguration SelfPackageConfiguration
        {
            get
            {
                if (selfPackageConfiguration == null)
                    selfPackageConfiguration = new SelfPackageConfiguration(Package.Id);

                return selfPackageConfiguration;
            }
        }

        public static MainViewModel Main
        {
            get
            {
                if (main == null)
                {
                    main = new MainViewModel(PackageSourceCollection, new MockSearchService(), new MockInstallService(), SelfPackageConfiguration, new MockSelfUpdateService());
                    main.Browser.Search.Execute();
                }

                return main;
            }
        }

        public static BrowserViewModel Browser
        {
            get
            {
                if (browser == null)
                {
                    browser = new BrowserViewModel(new MockPackageSourceProvider(), new MockSearchService(), new MockInstallService(), SelfPackageConfiguration)
                    {
                        SearchText = "GitExtensions"
                    };
                    browser.Search.Execute();
                }

                return browser;
            }
        }

        public static InstalledViewModel Installed
        {
            get
            {
                if (installed == null)
                {
                    installed = new InstalledViewModel(new MockPackageSourceProvider(), new MockInstallService(), SelfPackageConfiguration);
                    installed.Refresh.Execute();
                }

                return installed;
            }
        }

        public static UpdatesViewModel Updates
        {
            get
            {
                if (updates == null)
                {
                    updates = new UpdatesViewModel(new MockPackageSourceProvider(), new MockInstallService(), new MockSearchService(), SelfPackageConfiguration, new MockSelfUpdateService());
                    updates.Refresh.Execute();
                }

                return updates;
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

        public static IInstalledPackage CompatiblePackage
        {
            get
            {
                if (compatiblePackage == null)
                    compatiblePackage = new MockInstalledPackage(Package, true);

                return compatiblePackage;
            }
        }

        public static IInstalledPackage IncompatiblePackage
        {
            get
            {
                if (incompatiblePackage == null)
                    incompatiblePackage = new MockInstalledPackage(Package, true);

                return incompatiblePackage;
            }
        }

        public static PackageSourceViewModel PackageSources
        {
            get
            {
                if (packageSources == null)
                {
                    packageSources = new PackageSourceViewModel(PackageSourceCollection);
                    //packageSources.IsEditActive = true;
                }

                return packageSources;
            }
        }

        public static IPackageSourceCollection PackageSourceCollection
        {
            get
            {
                if (packageSourceCollection == null)
                {
                    packageSourceCollection = new MockPackageSourceCollection();
                    packageSourceCollection.Add("NuGet.org", new Uri("https://www.nuget.org/api/v2", UriKind.Absolute));
                    packageSourceCollection.Add("Neptuo GitExtensions Plugins", new Uri("https://www.myget.org/F/neptuo-gitextensions/api/v2", UriKind.Absolute));
                }

                return packageSourceCollection;
            }
        }
    }
}
