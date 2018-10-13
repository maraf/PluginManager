using Neptuo;
using Neptuo.Observables;
using Neptuo.Observables.Collections;
using Neptuo.Observables.Commands;
using PackageManager.Models;
using PackageManager.ViewModels.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels
{
    public class PackageSourceViewModel : ObservableModel
    {
        private readonly IPackageSourceCollection service;

        public ObservableCollection<IPackageSource> Sources { get; }
        public RemoveSourceCommand Remove { get; }
        public Command Add { get; }
        public SaveSourceCommand Save { get; }
        public Command Cancel { get; }

        private bool isEditActive;
        public bool IsEditActive
        {
            get { return isEditActive; }
            set
            {
                if (isEditActive != value)
                {
                    isEditActive = value;
                    RaisePropertyChanged();
                }
            }
        }

        public PackageSourceViewModel(IPackageSourceCollection service)
        {
            Ensure.NotNull(service, "service");
            this.service = service;

            Sources = new ObservableCollection<IPackageSource>(service.All);

            Add = new DelegateCommand(() => IsEditActive = true);
            Remove = new RemoveSourceCommand(Sources, service);
            Save = new SaveSourceCommand(Sources, service);
            Save.Executed += () => IsEditActive = false;
            Cancel = new DelegateCommand(() => IsEditActive = false);
        }
    }
}
