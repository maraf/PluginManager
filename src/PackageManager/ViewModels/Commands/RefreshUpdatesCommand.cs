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
        private readonly IPackageSourceSelector packageSource;
        private readonly IInstallService installService;
        private readonly ISearchService searchService;
        private readonly SelfPackageConfiguration selfPackageConfiguration;
        private readonly IComparer<IPackageIdentity> packageVersionComparer;

        public event Action Completed;

        public RefreshUpdatesCommand(UpdatesViewModel viewModel, IPackageSourceSelector packageSource, IInstallService installService, ISearchService searchService, SelfPackageConfiguration selfPackageConfiguration, IComparer<IPackageIdentity> packageVersionComparer)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(packageSource, "packageSource");
            Ensure.NotNull(installService, "installService");
            Ensure.NotNull(searchService, "searchService");
            Ensure.NotNull(selfPackageConfiguration, "selfPackageConfiguration");
            Ensure.NotNull(packageVersionComparer, "packageVersionComparer");
            this.viewModel = viewModel;
            this.packageSource = packageSource;
            this.installService = installService;
            this.searchService = searchService;
            this.selfPackageConfiguration = selfPackageConfiguration;
            this.packageVersionComparer = packageVersionComparer;
        }

        protected override bool CanExecuteOverride()
            => true;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            viewModel.Packages.Clear();

            foreach (IInstalledPackage current in await installService.GetInstalledAsync(packageSource.Sources, cancellationToken))
            {
                IPackage latest = await searchService.FindLatestVersionAsync(packageSource.Sources, current.Definition, viewModel.IsPrereleaseIncluded, cancellationToken);

                if (latest != null && packageVersionComparer.Compare(latest, current.Definition) > 0)
                {
                    viewModel.Packages.Add(new PackageUpdateViewModel(
                        current.Definition,
                        latest,
                        viewModel,
                        selfPackageConfiguration.Equals(current.Definition)
                    ));
                }
            }

            Completed?.Invoke();
        }
    }
}
