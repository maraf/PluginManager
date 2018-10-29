using Neptuo;
using Neptuo.Observables.Collections;
using Neptuo.Observables.Commands;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public class MoveUpCommand : Command<IPackageSource>
    {
        private readonly ObservableCollection<IPackageSource> sources;
        private readonly IPackageSourceCollection service;

        public MoveUpCommand(ObservableCollection<IPackageSource> sources, IPackageSourceCollection service)
        {
            Ensure.NotNull(sources, "sources");
            Ensure.NotNull(service, "service");
            this.sources = sources;
            this.service = service;
        }

        public override bool CanExecute(IPackageSource source)
            => source != null && sources.IndexOf(source) != 0;

        public override void Execute(IPackageSource source)
        {
            if (CanExecute(source))
            {
                int oldIndex = sources.IndexOf(source);
                int newIndex = service.MoveUp(source);
                sources.Move(oldIndex, newIndex);

                RaiseCanExecuteChanged();
            }
        }

        public new void RaiseCanExecuteChanged()
            => base.RaiseCanExecuteChanged();
    }
}
