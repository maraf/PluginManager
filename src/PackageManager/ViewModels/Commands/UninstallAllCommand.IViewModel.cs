using Neptuo.Observables.Collections;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    partial class UninstallAllCommand
    {
        public interface IViewModel
        {
            ObservableCollection<IInstalledPackage> Packages { get; }
            UninstallCommand Uninstall { get; }
        }
    }
}
