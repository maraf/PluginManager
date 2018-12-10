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
        private readonly IPackageSourceSelector packageSource;
        private readonly ISearchService search;

        private string lastSearchText;

        public event Action Completed;

        public SearchCommand(BrowserViewModel viewModel, IPackageSourceSelector packageSource, ISearchService search)
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
            if (lastSearchText != viewModel.SearchText)
            {
                lastSearchText = viewModel.SearchText;
                viewModel.Paging.CurrentIndex = 0;
            }

            (List<IPackage> packages, int pageSize) = await Task.Run(() => SearchAsync(cancellationToken));
            if (packages.Count > 0 || viewModel.Paging.CurrentIndex == 0)
            {
                viewModel.Packages.Clear();
                viewModel.Packages.AddRange(packages);

                viewModel.Paging.IsNextAvailable = viewModel.Packages.Count == pageSize;
            }
            else
            {
                viewModel.Paging.CurrentIndex--;
                viewModel.Paging.IsNextAvailable = false;
            }

            Completed?.Invoke();
        }

        private async Task<(List<IPackage> packages, int pageSize)> SearchAsync(CancellationToken cancellationToken)
        {
            SearchOptions options = new SearchOptions(viewModel.Paging.CurrentIndex);
            IEnumerable<IPackage> packages = await search.SearchAsync(packageSource.Sources, viewModel.SearchText, options, cancellationToken);
            return (packages.ToList(), options.PageSize);
        }
    }
}
