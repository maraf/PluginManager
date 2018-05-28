using Neptuo.Observables.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    partial class UpdateAllCommand
    {
        public interface IViewModel
        {
            ObservableCollection<PackageUpdateViewModel> Packages { get; }
            UpdateCommand Update { get; }
        }
    }
}
