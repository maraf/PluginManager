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

        public event Action Completed;

        public UpdateCommand(IInstallService installService, ISelfUpdateService selfUpdate)
        {
            Ensure.NotNull(installService, "service");
            Ensure.NotNull(selfUpdate, "selfUpdate");
            this.install = installService;
            this.selfUpdate = selfUpdate;
        }

        protected override bool CanExecuteOverride(PackageUpdateViewModel package)
            => package != null && install.IsInstalled(package.Current);

        protected override async Task ExecuteAsync(PackageUpdateViewModel package, CancellationToken cancellationToken)
        {
            if (package.IsSelf && !selfUpdate.IsSelfUpdate)
            {
                selfUpdate.Update(package.Latest);
                return;
            }

            IPackageContent packageContent = await package.Current.GetContentAsync(cancellationToken);
            await packageContent.RemoveFromAsync(install.Path, cancellationToken);
            install.Uninstall(package.Current);

            packageContent = await package.Latest.GetContentAsync(cancellationToken);
            await packageContent.ExtractToAsync(install.Path, cancellationToken);
            install.Install(package.Latest);

            if (package.IsSelf)
                selfUpdate.RunNewInstance(package.Latest);

            Completed?.Invoke();
        }

        public new void RaiseCanExecuteChanged()
            => base.RaiseCanExecuteChanged();
    }
}
