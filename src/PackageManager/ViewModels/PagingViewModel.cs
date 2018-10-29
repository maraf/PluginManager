using Neptuo;
using Neptuo.Observables;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PackageManager.ViewModels
{
    public class PagingViewModel : ObservableModel
    {
        private readonly Command searchCommand;
        private readonly DelegateCommand prev;
        private readonly DelegateCommand next;

        private int currentIndex;
        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                Ensure.PositiveOrZero(value, "CurrentIndex");
                if (currentIndex != value)
                {
                    currentIndex = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(CurrentNumber));

                    RaisePropertyChanged(nameof(IsPrevAvailable));
                    prev.RaiseCanExecuteChanged();
                }
            }
        }

        public int CurrentNumber
            => CurrentIndex + 1;

        public bool IsPrevAvailable
            => CurrentIndex > 0;

        private bool isNextAvailable;
        public bool IsNextAvailable
        {
            get { return isNextAvailable; }
            set
            {
                if (isNextAvailable != value)
                {
                    isNextAvailable = value;
                    RaisePropertyChanged();
                    next.RaiseCanExecuteChanged();
                }
            }
        }

        public Command Prev => prev;
        public Command Next => next;

        public PagingViewModel(Command searchCommand)
        {
            Ensure.NotNull(searchCommand, "searchCommand");
            this.searchCommand = searchCommand;

            next = new DelegateCommand(OnNext, () => IsNextAvailable);
            prev = new DelegateCommand(OnPrev, () => IsPrevAvailable);
        }

        private void OnNext()
        {
            CurrentIndex++;
            searchCommand.Execute();
        }

        private void OnPrev()
        {
            CurrentIndex--;
            searchCommand.Execute();
        }

        // TODO: Fix with update of Neptuo.Observables.
        private class DelegateCommand : Neptuo.Observables.Commands.DelegateCommand
        {
            public DelegateCommand(Action execute, Func<bool> canExecute) 
                : base(execute, canExecute)
            { }

            public new void RaiseCanExecuteChanged()
                => base.RaiseCanExecuteChanged();
        }
    }
}
