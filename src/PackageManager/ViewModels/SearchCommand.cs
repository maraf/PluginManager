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

namespace PackageManager.ViewModels
{
    public class SearchCommand : AsyncCommand
    {
        private readonly BrowserViewModel viewModel;
        private readonly ISearchService search;

        public SearchCommand(BrowserViewModel viewModel, ISearchService search)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(search, "search");
            this.viewModel = viewModel;
            this.search = search;
        }

        protected override bool CanExecuteOverride()
            => !String.IsNullOrEmpty(viewModel.SearchText) && !String.IsNullOrWhiteSpace(viewModel.SearchText);

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            IEnumerable<IPackage> packages = await search.SearchAsync(viewModel.Source, viewModel.SearchText);
            viewModel.Packages.Clear();
            viewModel.Packages.AddRange(packages);
        }
    }
}
