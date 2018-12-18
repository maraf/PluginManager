using Neptuo;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using PackageManager.Models;
using PackageManager.Services;
using PackageManager.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PackageManager.ViewModels
{
    public class UpdatesViewModel : ObservableModel, UpdateAllCommand.IViewModel
    {
        private bool isPrereleaseIncluded;
        public bool IsPrereleaseIncluded
        {
            get { return isPrereleaseIncluded; }
            set
            {
                if (isPrereleaseIncluded != value)
                {
                    isPrereleaseIncluded = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ObservableCollection<PackageUpdateViewModel> Packages { get; }
        public RefreshUpdatesCommand Refresh { get; }
        public UpdateCommand Update { get; }
        public UpdateAllCommand UpdateAll { get; }

        public UpdatesViewModel(IPackageSourceSelector packageSource, IInstallService installService, ISearchService searchService, SelfPackageConfiguration selfPackageConfiguration, ISelfUpdateService selfUpdate)
        {
            Ensure.NotNull(installService, "service");
            Ensure.NotNull(searchService, "searchService");

            Packages = new ObservableCollection<PackageUpdateViewModel>();
            Refresh = new RefreshUpdatesCommand(this, packageSource, installService, searchService, selfPackageConfiguration);
            Update = new UpdateCommand(installService, selfUpdate);
            UpdateAll = new UpdateAllCommand(this);
        }
    }
}
