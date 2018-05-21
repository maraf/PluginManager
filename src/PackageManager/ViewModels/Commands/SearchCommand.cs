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
    public class SearchCommand : AsyncCommand
    {
        private readonly BrowserViewModel viewModel;
        private readonly IPackageSourceProvider packageSource;
        private readonly ISearchService search;

        public SearchCommand(BrowserViewModel viewModel, IPackageSourceProvider packageSource, ISearchService search)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(packageSource, "packageSource");
            Ensure.NotNull(search, "search");
            this.viewModel = viewModel;
            this.packageSource = packageSource;
            this.search = search;
        }

        protected override bool CanExecuteOverride()
            => true;

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            IEnumerable<IPackage> packages = await search.SearchAsync(packageSource.Url, viewModel.SearchText, cancellationToken: cancellationToken);
            viewModel.Packages.Clear();
            viewModel.Packages.AddRange(packages);
        }
    }
}
