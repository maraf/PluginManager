using Neptuo;
using Neptuo.Observables.Commands;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public partial class UninstallAllCommand : AsyncCommand
    {
        private readonly IViewModel viewModel;
        private IEnumerator<IPackage> current;
        private TaskCompletionSource<bool> currentCompletion;

        public UninstallAllCommand(IViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            this.viewModel = viewModel;

            viewModel.Packages.CollectionChanged += OnPackagesChanged;
        }

        private void OnPackagesChanged(object sender, NotifyCollectionChangedEventArgs e)
            => RaiseCanExecuteChanged();

        protected override bool CanExecuteOverride()
            => current == null && viewModel.Packages.Count > 0;

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            current = viewModel.Packages.ToList().GetEnumerator();
            currentCompletion = new TaskCompletionSource<bool>();

            viewModel.Uninstall.Completed += OnOneCompleted;
            ExecuteNext();
            RaiseCanExecuteChanged();

            return currentCompletion.Task;
        }

        private async void OnOneCompleted()
        {
            await Task.Delay(10);
            ExecuteNext();
        }

        private void ExecuteNext()
        {
            if (current.MoveNext())
            {
                if (viewModel.Uninstall.CanExecute(current.Current))
                    viewModel.Uninstall.Execute(current.Current);
                else
                    ExecuteNext();
            }
            else
            {
                current = null;
                currentCompletion.TrySetResult(true);
                viewModel.Uninstall.Completed -= ExecuteNext;
                RaiseCanExecuteChanged();
            }
        }
    }
}
