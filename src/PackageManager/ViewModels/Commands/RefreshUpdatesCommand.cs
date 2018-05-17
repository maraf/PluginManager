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
    public class RefreshUpdatesCommand : AsyncCommand
    {
        private readonly UpdatesViewModel viewModel;
        private readonly IPackageSourceProvider packageSource;
        private readonly IInstallService installService;
        private readonly ISearchService searchService;

        public RefreshUpdatesCommand(UpdatesViewModel viewModel, IPackageSourceProvider packageSource, IInstallService installService, ISearchService searchService)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(packageSource, "packageSource");
            Ensure.NotNull(installService, "installService");
            Ensure.NotNull(searchService, "searchService");
            this.viewModel = viewModel;
            this.packageSource = packageSource;
            this.installService = installService;
            this.searchService = searchService;
        }

        protected override bool CanExecuteOverride()
            => true;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            viewModel.Packages.Clear();

            foreach (IPackage current in await installService.GetInstalledAsync(packageSource.Url, cancellationToken))
            {
                IPackage latest = await searchService.FindLatestVersionAsync(packageSource.Url, current, cancellationToken);

                // TODO: Compare versions.
                if (latest.Version != current.Version)
                    viewModel.Packages.Add(latest);
            }
        }
    }
}
