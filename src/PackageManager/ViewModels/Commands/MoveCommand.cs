using Neptuo;
using Neptuo.Observables.Collections;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    public class MoveCommand<T> : Command<T>, IDisposable
    {
        private readonly ObservableCollection<T> sources;
        private readonly Func<T, int> execute;
        private readonly Func<T, bool> canExecute;

        public MoveCommand(ObservableCollection<T> sources, Func<T, int> execute, Func<T, bool> canExecute)
        {
            Ensure.NotNull(sources, "sources");
            Ensure.NotNull(execute, "execute");
            Ensure.NotNull(canExecute, "canExecute");
            this.sources = sources;
            this.execute = execute;
            this.canExecute = canExecute;

            sources.CollectionChanged += OnSourcesChanged;
        }

        private void OnSourcesChanged(object sender, NotifyCollectionChangedEventArgs e)
            => RaiseCanExecuteChanged();

        public override bool CanExecute(T item)
            => item != null && canExecute(item);

        public override void Execute(T item)
        {
            if (CanExecute(item))
            {
                int oldIndex = sources.IndexOf(item);
                int newIndex = execute(item);
                sources.Move(oldIndex, newIndex);
            }
        }

        public new void RaiseCanExecuteChanged()
            => base.RaiseCanExecuteChanged();

        public void Dispose()
            => sources.CollectionChanged -= OnSourcesChanged;
    }
}
