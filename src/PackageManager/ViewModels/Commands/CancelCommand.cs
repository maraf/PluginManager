using Neptuo;
using Neptuo.Observables.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackageManager.ViewModels.Commands
{
    /// <summary>
    /// A command that cancels a command implementing <see cref="ICancellableCommand"/>.
    /// </summary>
    public class CancelCommand : Command
    {
        private readonly IEnumerable<ICancellableCommand> commands;

        /// <summary>
        /// Creates a new instance that can cancel <paramref name="commands"/>.
        /// </summary>
        /// <param name="commands">An enumeration of commands that can be cancelled by this object.</param>
        public CancelCommand(params ICancellableCommand[] commands)
            : this((IEnumerable<ICancellableCommand>)commands)
        { }

        /// <summary>
        /// Creates a new instance that can cancel <paramref name="commands"/>.
        /// </summary>
        /// <param name="commands">An enumeration of commands that can be cancelled by this object.</param>
        public CancelCommand(IEnumerable<ICancellableCommand> commands)
        {
            Ensure.NotNull(commands, "commands");
            this.commands = commands;

            foreach (ICancellableCommand command in commands)
                command.CanExecuteChanged += OnCanExecuteChanged;
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
            => RaiseCanExecuteChanged();

        public override bool CanExecute()
            => commands.Any(c => c.IsRunning);

        public override void Execute()
        {
            foreach (ICancellableCommand command in commands)
            {
                if (command.IsRunning)
                    command.Cancel();
            }
        }
    }
}
