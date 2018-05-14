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
    public class RefreshCommand : Command
    {
        private readonly InstalledViewModel viewModel;
        private readonly IInstallService service;

        public RefreshCommand(InstalledViewModel viewModel, IInstallService service)
        {
            Ensure.NotNull(viewModel, "viewModel");
            Ensure.NotNull(service, "service");
            this.viewModel = viewModel;
            this.service = service;
        }

        public override bool CanExecute()
            => true;

        public override void Execute()
        {
            viewModel.Items.Clear();
            viewModel.Items.AddRange(service.GetInstalled());
        }
    }
}
