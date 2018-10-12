using Neptuo;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Observables.Commands;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels
{
    public class PackageSourceViewModel : ObservableModel
    {
        private readonly IPackageSourceCollection service;

        public ObservableCollection<IPackageSource> Sources { get; }
        public Command<IPackageSource> Remove { get; }
        public Command Add { get; }

        public PackageSourceViewModel(IPackageSourceCollection service)
        {
            Ensure.NotNull(service, "service");
            this.service = service;

            Sources = new ObservableCollection<IPackageSource>(service.All);
        }
    }
}
