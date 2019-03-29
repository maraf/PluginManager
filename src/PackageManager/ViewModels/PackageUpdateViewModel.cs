using Neptuo;
using Neptuo.Observables;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels
{
    public class PackageUpdateViewModel : ObservableModel
    {
        public PackageViewModel Current { get; }
        public bool IsSelf { get; }

        private IPackage target;
        public IPackage Target
        {
            get { return target; }
            set
            {
                if (target != value)
                {
                    target = value;
                    RaisePropertyChanged();
                }
            }
        }

        public PackageUpdateViewModel(IPackage current, IPackage latest, IPackageOptions packageOptions, bool isSelf)
        {
            Ensure.NotNull(current, "current");
            Ensure.NotNull(latest, "latest");
            Current = new PackageViewModel(current, packageOptions);
            Target = latest;
            IsSelf = isSelf;
        }
    }
}
