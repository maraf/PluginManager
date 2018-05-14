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
    public class InstalledViewModel : ObservableObject
    {
        private readonly IInstallService service;

        public ObservableCollection<IPackage> Packages { get; }
        public ICommand Refresh { get; }
        public UninstallCommand Uninstall { get; }

        public InstalledViewModel(IInstallService service)
        {
            Ensure.NotNull(service, "service");
            this.service = service;

            Packages = new ObservableCollection<IPackage>();
            Refresh = new RefreshCommand(this, service);
            Uninstall = new UninstallCommand(service);
        }
    }
}
