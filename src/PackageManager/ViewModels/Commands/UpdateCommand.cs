using Neptuo;
using Neptuo.Observables.Commands;
using PackageManager.Models;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public class UpdateCommand : AsyncCommand<PackageUpdateViewModel>
    {
        private readonly IInstallService install;
        private readonly ISelfUpdateService selfUpdate;

        public event Func<Task<bool>> Executing;
        public event Action Completed;

        public UpdateCommand(IInstallService installService, ISelfUpdateService selfUpdate)
        {
            Ensure.NotNull(installService, "service");
            Ensure.NotNull(selfUpdate, "selfUpdate");
            this.install = installService;
            this.selfUpdate = selfUpdate;
        }

        protected override bool CanExecuteOverride(PackageUpdateViewModel package)
            => package != null && install.IsInstalled(package.Current.Model) && package.Target != null;

        protected override async Task ExecuteAsync(PackageUpdateViewModel package, CancellationToken cancellationToken)
        {
            bool execute = true;

            if (Executing != null)
                execute = await Executing();

            if (execute)
            {
                if (package.IsSelf && !selfUpdate.IsSelfUpdate)
                {
                    selfUpdate.Update(package.Target);
                    return;
                }

                IPackageContent packageContent = await package.Current.Model.GetContentAsync(cancellationToken);
                await packageContent.RemoveFromAsync(install.Path, cancellationToken);
                install.Uninstall(package.Current.Model);

                packageContent = await package.Target.GetContentAsync(cancellationToken);
                await packageContent.ExtractToAsync(install.Path, cancellationToken);
                install.Install(package.Target);

                if (package.IsSelf)
                    selfUpdate.RunNewInstance(package.Target);
            }

            Completed?.Invoke();
        }

        public new void RaiseCanExecuteChanged()
            => base.RaiseCanExecuteChanged();
    }
}
