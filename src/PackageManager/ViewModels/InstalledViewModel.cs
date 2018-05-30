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
        public ReinstallCommand Reinstall { get; }
        public UninstallCommand Uninstall { get; }

        public InstalledViewModel(IPackageSourceProvider packageSource, IInstallService service)
        {
            Ensure.NotNull(service, "service");
            this.service = service;

            Packages = new ObservableCollection<IPackage>();
            Refresh = new RefreshInstalledCommand(this, packageSource, service);
            Reinstall = new ReinstallCommand(service);
            Uninstall = new UninstallCommand(service);
        }
    }
}
