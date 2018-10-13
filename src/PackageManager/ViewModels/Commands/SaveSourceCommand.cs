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
    public class SaveSourceCommand : Command
    {
        private readonly ICollection<IPackageSource> sources;
        private readonly IPackageSourceCollection service;

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                if (name != value)
                {
                    name = value;
                    RaiseCanExecuteChanged();
                }
            }
        }

        private string url;
        public string Url
        {
            get { return url; }
            set
            {
                if (url != value)
                {
                    url = value;
                    RaiseCanExecuteChanged();
                }
            }
        }

        public event Action Executed;

        public SaveSourceCommand(ICollection<IPackageSource> sources, IPackageSourceCollection service)
        {
            Ensure.NotNull(sources, "sources");
            Ensure.NotNull(service, "service");
            this.sources = sources;
            this.service = service;
        }

        public override bool CanExecute()
            => !string.IsNullOrEmpty(Url) && Uri.TryCreate(Url, UriKind.RelativeOrAbsolute, out _);

        public override void Execute()
        {
            if (CanExecute() && Uri.TryCreate(Url, UriKind.RelativeOrAbsolute, out var uri))
            {
                sources.Add(service.Add(Name, uri));
                Name = name;
                Url = null;

                Executed?.Invoke();
            }
        }
    }
}
