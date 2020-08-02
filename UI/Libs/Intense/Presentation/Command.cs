using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Intense.Presentation
{
    /// <summary>
    /// The base implementation of a command.
    /// </summary>
    public abstract class Command
        : ICommand
    {
        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged"/> event.
        /// </summary>
        public virtual void OnCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Determines whether the command can execute in its current state.
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public abstract bool CanExecute(object parameter);

        /// <summary>
        /// Performs the command execution.
        /// </summary>
        /// <param name="parameter"></param>
        public abstract void Execute(object parameter);
    }
}
