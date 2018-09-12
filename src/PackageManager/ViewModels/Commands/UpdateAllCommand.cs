using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public partial class UpdateAllCommand : AsyncCommand
    {
        private readonly IViewModel viewModel;

        public UpdateAllCommand(IViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            this.viewModel = viewModel;

            viewModel.Packages.CollectionChanged += OnPackagesChanged;
        }

        private void OnPackagesChanged(object sender, NotifyCollectionChangedEventArgs e)
            => RaiseCanExecuteChanged();

        protected override bool CanExecuteOverride()
            => viewModel.Packages.Count > 0;

        protected async override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            foreach (PackageUpdateViewModel package in viewModel.Packages.ToList())
            {
                if (viewModel.Update.CanExecute(package))
                    await viewModel.Update.ExecuteAsync(package);
            }
        }
    }
}
