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
    public class PackageSourceSelectorViewModel : ObservableModel, IPackageSourceSelector, IDisposable
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

            service.Changed += OnServiceChanged;
            OnServiceChanged();
        }

        private void OnServiceChanged()
        {
            string selectedName = SelectedName;
            SourceNames.Clear();

            bool isSelectedNameContained = false;
            void Add(string name)
            {
                if (!isSelectedNameContained)
                    isSelectedNameContained = name == selectedName;

                SourceNames.Add(name);
            }

            if (service.All.Count > 1)
                Add(AllFeedName);

            foreach (IPackageSource source in service.All)
                Add(source.Name);

            if (isSelectedNameContained)
                SelectedName = selectedName;
            else
                SelectedName = SourceNames.FirstOrDefault();
        }

        public void Dispose()
        {
            service.Changed -= OnServiceChanged;
        }
    }
}
