using Neptuo;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using PackageManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels
{
    public class PackageSourceSelectorViewModel : ObservableModel, IPackageSourceSelector
    {
        public const string AllFeedName = "All Feeds";

        private readonly IPackageSourceProvider service;
        private IEnumerable<IPackageSource> selectedSources;

        IEnumerable<IPackageSource> IPackageSourceSelector.Sources
        {
            get
            {
                if (selectedSources == null)
                {
                    if (string.IsNullOrEmpty(SelectedName) || SelectedName == AllFeedName)
                        selectedSources = service.All;
                    else
                        selectedSources = new List<IPackageSource>(1) { service.All.First(s => s.Name == SelectedName) };
                }

                return selectedSources;
            }
        }

        public ObservableCollection<string> SourceNames { get; }

        private string selectedName;
        public string SelectedName
        {
            get { return selectedName; }
            set
            {
                if (selectedName != value)
                {
                    selectedName = value;
                    RaisePropertyChanged();

                    selectedSources = null;
                }
            }
        }

        public PackageSourceSelectorViewModel(IPackageSourceProvider service)
        {
            Ensure.NotNull(service, "service");
            this.service = service;

            SourceNames = new ObservableCollection<string>();
            if (service.All.Count > 1)
            {
                SourceNames.Add(AllFeedName);
                foreach (IPackageSource source in service.All)
                    SourceNames.Add(source.Name);
            }

            SelectedName = SourceNames.FirstOrDefault();
        }
    }
}
