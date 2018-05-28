using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public partial class UpdateAllCommand : AsyncCommand
    {
        private readonly IViewModel viewModel;
        private IEnumerator<PackageUpdateViewModel> current;
        private TaskCompletionSource<bool> currentCompletion;

        public UpdateAllCommand(IViewModel viewModel)
        {
            Ensure.NotNull(viewModel, "viewModel");
            this.viewModel = viewModel;
        }

        protected override bool CanExecuteOverride()
            => current == null;

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            current = viewModel.Packages.ToList().GetEnumerator();
            currentCompletion = new TaskCompletionSource<bool>();

            viewModel.Update.Completed += OnOneCompleted;
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
                if (viewModel.Update.CanExecute(current.Current))
                    viewModel.Update.Execute(current.Current);
                else
                    ExecuteNext();
            }
            else
            {
                current = null;
                currentCompletion.TrySetResult(true);
                viewModel.Update.Completed -= ExecuteNext;
                RaiseCanExecuteChanged();
            }
        }
    }
}
