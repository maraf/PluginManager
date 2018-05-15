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
    public class UpdateCommand : AsyncCommand<IPackage>
    {
        private readonly IInstallService installService;

        public event Action Completed;

        public UpdateCommand(IInstallService installService)
        {
            Ensure.NotNull(installService, "service");
            this.installService = installService;
        }

        protected override bool CanExecuteOverride(IPackage package)
            => package != null && !installService.IsInstalled(package);

        protected override async Task ExecuteAsync(IPackage package, CancellationToken cancellationToken)
        {
            IPackageContent packageContent = await package.DownloadAsync(cancellationToken);
            await packageContent.RemoveFromAsync(installService.Path, cancellationToken);
            await packageContent.ExtractToAsync(installService.Path, cancellationToken);

            installService.Install(package);
            Completed?.Invoke();
        }

        public new void RaiseCanExecuteChanged()
            => base.RaiseCanExecuteChanged();
    }
}
