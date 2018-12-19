using Neptuo;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Observables.Commands;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PackageManager.ViewModels
{
    public class PackageViewModel : ObservableModel
    {
        private readonly IPackage model;

        // TODO: Remove...
        public IPackage Model => model;

        public string Id => model.Id;
        public string Version => model.Version;
        public string Description => model.Description;
        public string Authors => model.Authors;
        public DateTime? Published => model.Published;
        public string Tags => model.Tags;
        public Uri IconUrl => model.IconUrl;
        public Uri ProjectUrl => model.ProjectUrl;
        public Uri LicenseUrl => model.LicenseUrl;

        public ObservableCollection<PackageViewModel> Versions { get; } = new ObservableCollection<PackageViewModel>();

        public Command LoadVersions { get; }

        private bool areVersionsLoaded;
        public bool AreVersionsLoaded
        {
            get { return areVersionsLoaded; }
            set
            {
                if (areVersionsLoaded != value)
                {
                    areVersionsLoaded = value;
                    RaisePropertyChanged();
                }
            }
        }

        public PackageViewModel(IPackage model)
        {
            Ensure.NotNull(model, "model");
            this.model = model;

            LoadVersions = new AsyncDelegateCommand(OnLoadVersionsAsync, CanLoadVersions);
        }

        private async Task OnLoadVersionsAsync(CancellationToken cancellationToken)
        {
            Versions.Clear();

            IEnumerable<IPackage> versions = await model.GetVersionsAsync(cancellationToken);
            foreach (IPackage version in versions)
                Versions.Add(new PackageViewModel(version));

            AreVersionsLoaded = true;
        }

        private bool CanLoadVersions()
            => !AreVersionsLoaded;

        public override bool Equals(object obj)
        {
            PackageViewModel other = obj as PackageViewModel;
            if (other == null)
                return false;

            return Id == other.Id && Version == other.Version;
        }

        public override int GetHashCode()
        {
            int hash = 13 * 2;
            hash += 7 * Model.GetHashCode();
            return hash;
        }
    }
}
