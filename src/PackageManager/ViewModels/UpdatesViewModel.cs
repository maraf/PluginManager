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
    public class UpdatesViewModel : ObservableObject
    {
        public ObservableCollection<IPackage> Packages { get; }
        public RefreshUpdatesCommand Refresh { get; }
        public UpdateCommand Update { get; }

        public UpdatesViewModel(IInstallService installService, ISearchService searchService)
        {
            Ensure.NotNull(installService, "service");
            Ensure.NotNull(searchService, "searchService");

            Packages = new ObservableCollection<IPackage>();
            Refresh = new RefreshUpdatesCommand(this, installService, searchService);
            Update = new UpdateCommand(installService);
        }
    }
}
