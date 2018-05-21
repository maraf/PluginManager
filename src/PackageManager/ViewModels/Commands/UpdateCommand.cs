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
        private readonly IInstallService installService;

        public event Action Completed;

        public UpdateCommand(IInstallService installService)
        {
            Ensure.NotNull(installService, "service");
            this.installService = installService;
        }

        protected override bool CanExecuteOverride(PackageUpdateViewModel package)
            => package != null && !installService.IsInstalled(package.Current);

        protected override async Task ExecuteAsync(PackageUpdateViewModel package, CancellationToken cancellationToken)
        {
            IPackageContent packageContent = await package.Current.GetContentAsync(cancellationToken);
            await packageContent.RemoveFromAsync(installService.Path, cancellationToken);
            installService.Uninstall(package.Current);

            packageContent = await package.Latest.GetContentAsync(cancellationToken);
            await packageContent.ExtractToAsync(installService.Path, cancellationToken);
            installService.Install(package.Latest);

            Completed?.Invoke();
        }

        public new void RaiseCanExecuteChanged()
            => base.RaiseCanExecuteChanged();
    }
}
