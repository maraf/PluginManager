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
    public class UninstallCommand : AsyncCommand<IPackage>
    {
        private readonly IInstallService service;

        public event Action Completed;

        public UninstallCommand(IInstallService service)
        {
            Ensure.NotNull(service, "service");
            this.service = service;
        }

        protected override bool CanExecuteOverride(IPackage package)
            => package != null && service.IsInstalled(package);

        protected override async Task ExecuteAsync(IPackage package, CancellationToken cancellationToken)
        {
            IPackageContent packageContent = await package.DownloadAsync(cancellationToken);
            await packageContent.RemoveFromAsync(service.Path, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            service.Uninstall(package);
            Completed?.Invoke();
        }

        public new void RaiseCanExecuteChanged()
            => base.RaiseCanExecuteChanged();
    }
}
