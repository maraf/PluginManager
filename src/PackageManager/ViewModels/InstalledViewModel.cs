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
    public class InstalledViewModel : ObservableModel, UninstallAllCommand.IViewModel
    {
        private readonly IInstallService service;

        public ObservableCollection<IInstalledPackage> Packages { get; }
        public RefreshInstalledCommand Refresh { get; }
        public ReinstallCommand Reinstall { get; }
        public UninstallCommand Uninstall { get; }
        public UninstallAllCommand UninstallAll { get; }

        public InstalledViewModel(IPackageSourceSelector packageSource, IInstallService service, SelfPackageConfiguration selfPackageConfiguration)
        {
            Ensure.NotNull(service, "service");
            this.service = service;

            Packages = new ObservableCollection<IInstalledPackage>();
            Refresh = new RefreshInstalledCommand(this, packageSource, service);
            Reinstall = new ReinstallCommand(service, selfPackageConfiguration);
            Uninstall = new UninstallCommand(service, selfPackageConfiguration);
            UninstallAll = new UninstallAllCommand(this);
        }
    }
}
