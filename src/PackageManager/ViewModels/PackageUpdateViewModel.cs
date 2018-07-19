using Neptuo;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels
{
    public class PackageUpdateViewModel
    {
        public IPackage Current { get; private set; }
        public IPackage Latest { get; private set; }
        public bool IsSelf { get; private set; }

        public PackageUpdateViewModel(IPackage current, IPackage latest, bool isSelf)
        {
            Ensure.NotNull(current, "current");
            Ensure.NotNull(latest, "latest");
            Current = current;
            Latest = latest;
            IsSelf = isSelf;
        }
    }
}
