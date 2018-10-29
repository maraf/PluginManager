using Neptuo;
using Neptuo.Observables.Collections;
using Neptuo.Observables.Commands;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public class SaveSourceCommand : Command, INotifyPropertyChanged
    {
        private readonly ObservableCollection<IPackageSource> sources;
        private readonly IPackageSourceCollection service;

        private IPackageSource edit;

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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
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
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Url)));
                }
            }
        }

        public event Action Executed;
        public event PropertyChangedEventHandler PropertyChanged;

        public SaveSourceCommand(ObservableCollection<IPackageSource> sources, IPackageSourceCollection service)
        {
            Ensure.NotNull(sources, "sources");
            Ensure.NotNull(service, "service");
            this.sources = sources;
            this.service = service;
        }

        public override bool CanExecute()
        {
            if (string.IsNullOrEmpty(Name))
                return false;

            if (sources.Any(s => s.Name == Name && !(edit == null || edit == s)))
                return false;

            if (string.IsNullOrEmpty(Url))
                return false;

            if (!Uri.TryCreate(Url, UriKind.RelativeOrAbsolute, out _))
                return false;

            return true;
        }

        public override void Execute()
        {
            if (CanExecute() && Uri.TryCreate(Url, UriKind.RelativeOrAbsolute, out var uri))
            {
                if (edit == null)
                {
                    sources.Add(service.Add().Name(Name).Uri(uri).Save());
                }
                else
                {
                    int index = sources.IndexOf(edit);
                    sources.RemoveAt(index);
                    sources.Insert(index, service.Edit(edit).Name(Name).Uri(uri).Save());
                }

                edit = null;
                Name = null;
                Url = null;

                Executed?.Invoke();
            }
        }

        public void Edit(IPackageSource source)
        {
            edit = source;
            Name = source.Name;
            Url = source.Uri.ToString();
        }

        public void New()
        {
            edit = null;
            Name = null;
            Url = null;
        }
    }
}
