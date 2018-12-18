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
        private readonly SelfPackageConfiguration selfPackageConfiguration;

        public event Func<Task<bool>> Executing;
        public event Action Completed;

        public UninstallCommand(IInstallService service, SelfPackageConfiguration selfPackageConfiguration)
        {
            Ensure.NotNull(service, "service");
            Ensure.NotNull(selfPackageConfiguration, "selfPackageConfiguration");
            this.service = service;
            this.selfPackageConfiguration = selfPackageConfiguration;
        }

        protected override bool CanExecuteOverride(IPackage package)
            => package != null && service.IsInstalled(package) && !selfPackageConfiguration.Equals(package.Id);

        protected override async Task ExecuteAsync(IPackage package, CancellationToken cancellationToken)
        {
            bool execute = true;

            if (Executing != null)
                execute = await Executing();

            if (execute)
            {
                IPackageContent packageContent = await package.GetContentAsync(cancellationToken);
                await packageContent.RemoveFromAsync(service.Path, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                service.Uninstall(package);
            }

            Completed?.Invoke();
        }

        public new void RaiseCanExecuteChanged()
            => base.RaiseCanExecuteChanged();
    }
}
