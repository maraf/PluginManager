using Neptuo;
using Neptuo.Observables.Commands;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public class RemoveSourceCommand : Command<IPackageSource>
    {
        private readonly ICollection<IPackageSource> sources;
        private readonly IPackageSourceCollection service;

        public RemoveSourceCommand(ICollection<IPackageSource> sources, IPackageSourceCollection service)
        {
            Ensure.NotNull(sources, "sources");
            Ensure.NotNull(service, "service");
            this.sources = sources;
            this.service = service;
        }

        public override bool CanExecute(IPackageSource source)
            => source != null;

        public override void Execute(IPackageSource source)
        {
            if (CanExecute(source))
            {
                sources.Remove(source);
                service.Remove(source);
            }
        }

        public new void RaiseCanExecuteChanged()
            => base.RaiseCanExecuteChanged();
    }
}
