using Neptuo.Observables.Commands;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public class InstallCommand : AsyncCommand<IPackage>
    {
        protected override bool CanExecuteOverride(IPackage package)
        {
            return false;
        }

        protected override Task ExecuteAsync(IPackage package, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
