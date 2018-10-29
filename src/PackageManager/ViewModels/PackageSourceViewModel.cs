﻿using Neptuo;
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
        public Command Add { get; }
        public DelegateCommand<IPackageSource> Edit { get; }
        public RemoveSourceCommand Remove { get; }

        public MoveCommand<IPackageSource> MoveUp { get; }
        public MoveCommand<IPackageSource> MoveDown { get; }

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

            Add = new DelegateCommand(OnAdd);
            Edit = new DelegateCommand<IPackageSource>(OnEdit, CanEdit);
            Remove = new RemoveSourceCommand(Sources, service);

            MoveUp = new MoveCommand<IPackageSource>(Sources, source => service.MoveUp(source), source => Sources.IndexOf(source) > 0);
            MoveDown = new MoveCommand<IPackageSource>(Sources, source => service.MoveDown(source), source => Sources.IndexOf(source) < Sources.Count - 1);

            Save = new SaveSourceCommand(Sources, service);
            Save.Executed += () => IsEditActive = false;
            Cancel = new DelegateCommand(() => IsEditActive = false);
        }

        private void OnAdd()
        {
            Save.New();
            IsEditActive = true;
        }

        private bool CanEdit(IPackageSource source)
            => source != null;

        private void OnEdit(IPackageSource source)
        {
            if (CanEdit(source))
            {
                Save.Edit(source);
                IsEditActive = true;
            }
        }


        // TODO: Fix with update of Neptuo.Observables.
        public class DelegateCommand<T> : Neptuo.Observables.Commands.DelegateCommand<T>
        {
            public DelegateCommand(Action<T> execute, Func<T, bool> canExecute) 
                : base(execute, canExecute)
            { }

            public new void RaiseCanExecuteChanged()
                => base.RaiseCanExecuteChanged();
        }
    }
}
