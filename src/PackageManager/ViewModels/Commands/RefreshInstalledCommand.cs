using Neptuo;
using Neptuo.Observables.Commands;
using PackageManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public class RefreshInstalledCommand : AsyncCommand
    {
        private readonly InstalledViewModel viewModel;
        private readonly IInstallService service;
        private readonly IPackageSourceProvider packageSource;

        public RefreshInstalledCommand(InstalledViewModel viewModel, IPackageSourceProvider packageSource, IInstallService service)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(packageSource, "packageSource");
            Ensure.NotNull(service, "service");
            this.viewModel = viewModel;
            this.packageSource = packageSource;
            this.service = service;
        }

        protected override bool CanExecuteOverride()
            => true;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            viewModel.Packages.Clear();
            viewModel.Packages.AddRange(await service.GetInstalledAsync(packageSource.Url, cancellationToken));
        }
    }
}
