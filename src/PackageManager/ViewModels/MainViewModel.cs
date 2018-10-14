using Neptuo.Observables;
using PackageManager.Models;
using PackageManager.Services;
using PackageManager.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels
{
    public class MainViewModel : ObservableModel
    {
        public PackageSourceSelectorViewModel SourceSelector { get; }

        public BrowserViewModel Browser { get; }
        public InstalledViewModel Installed { get; }
        public UpdatesViewModel Updates { get; }

        public CancelCommand Cancel { get; }

        private bool isLoading;
        public bool IsLoading
        {
            get { return isLoading; }
            set
            {
                if (isLoading != value)
                {
                    isLoading = value;
                    RaisePropertyChanged();
                }
            }
        }

        public MainViewModel(IPackageSourceProvider sources, ISearchService search, IInstallService install, SelfPackageConfiguration selfPackageConfiguration, ISelfUpdateService selfUpdate)
        {
            SourceSelector = new PackageSourceSelectorViewModel(sources);

            Browser = new BrowserViewModel(SourceSelector, search, install, selfPackageConfiguration);
            Installed = new InstalledViewModel(SourceSelector, install, selfPackageConfiguration);
            Updates = new UpdatesViewModel(SourceSelector, install, search, selfPackageConfiguration, selfUpdate);

            Cancel = new CancelCommand(
                Browser.Search, 
                Browser.Install, 
                Browser.Uninstall,
                Browser.Reinstall,
                Installed.Uninstall,
                Installed.UninstallAll,
                Installed.Reinstall,
                Installed.Refresh,
                Updates.Update,
                Updates.UpdateAll,
                Updates.Refresh
            );
            Cancel.CanExecuteChanged += OnCancelCanExecuteChanged;

            Installed.Uninstall.Completed += OnInstalledChanged;
            Updates.Update.Completed += OnInstalledChanged;
        }

        private void OnCancelCanExecuteChanged(object sender, EventArgs e)
            => IsLoading = Cancel.CanExecute();

        private void OnInstalledChanged()
            => Browser.Packages.Clear();
    }
}
