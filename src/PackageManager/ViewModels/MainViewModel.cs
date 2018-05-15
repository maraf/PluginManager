using Neptuo.Observables;
using PackageManager.Services;
using PackageManager.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        public BrowserViewModel Browser { get; }
        public InstalledViewModel Installed { get; }

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

        public MainViewModel(ISearchService search, IInstallService install)
        {
            Browser = new BrowserViewModel(search, install);
            Installed = new InstalledViewModel(install);

            Cancel = new CancelCommand(
                Browser.Search, 
                Browser.Install, 
                Browser.Uninstall,
                Installed.Uninstall
            );
            Cancel.CanExecuteChanged += OnCancelCanExecuteChanged;
        }

        private void OnCancelCanExecuteChanged(object sender, EventArgs e)
            => IsLoading = Cancel.CanExecute();
    }
}
