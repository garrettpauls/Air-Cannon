using System;
using System.Windows.Input;

namespace AirCannon.Framework.WPF
{
    /// <summary>
    ///   An implementation of <see cref = "ICommand" /> which uses delegates to define functionality.
    /// </summary>
    public class DelegateCommand : DelegateCommand<object>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "DelegateCommand" /> class.
        /// </summary>
        /// <param name = "execute">The method to call when the command is executed.</param>
        /// <param name = "canExecute">
        ///   The method that determines if the command can execute. 
        ///   If <c>null</c>, the command can always be executed.
        /// </param>
        public DelegateCommand(Action execute, Func<bool> canExecute = null)
            : base(obj => execute(), canExecute != null
                                         ? new Func<object, bool>(obj => canExecute())
                                         : null)
        {
        }

        /// <summary>
        ///   Determines if the command can execute.
        /// </summary>
        public bool CanExecute()
        {
            return CanExecute(null);
        }

        /// <summary>
        ///   Determines if the command can execute.
        /// </summary>
        public void Execute()
        {
            Execute(null);
        }
    }

    /// <summary>
    ///   An implementation of <see cref = "ICommand" /> which uses delegates to define functionality.
    /// </summary>
    /// <typeparam name = "TParameter">The type of the parameter.</typeparam>
    public class DelegateCommand<TParameter> : ICommand
    {
        private readonly Func<TParameter, bool> mCanExecute;
        private readonly Action<TParameter> mExecute;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "DelegateCommand&lt;TParameter&gt;" /> class.
        /// </summary>
        /// <param name = "execute">The method to call when the command is executed.</param>
        /// <param name = "canExecute">
        ///   The method that determines if the command can execute. 
        ///   If <c>null</c>, the command can always be executed.
        /// </param>
        public DelegateCommand(Action<TParameter> execute, Func<TParameter, bool> canExecute = null)
        {
            mExecute = execute;
            mCanExecute = canExecute;
        }

        /// <summary>
        ///   Gets the value of CanExecute as of the last time it was called.
        /// </summary>
        public bool LastCanExecute { get; private set; }

        #region ICommand Members

        /// <summary>
        ///   Determines if the command can execute.
        /// </summary>
        /// <param name = "parameter">
        ///   Data used by the command. It should be of type <typeparamref name = "TParameter" />.
        /// </param>
        /// <returns>
        ///   true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return CanExecute((TParameter) parameter);
        }

        /// <summary>
        ///   Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        ///   Executes the command.
        /// </summary>
        /// <param name = "parameter">
        ///   Data used by the command. It should be of type <typeparamref name = "TParameter" />.
        /// </param>
        public void Execute(object parameter)
        {
            Execute((TParameter) parameter);
        }

        #endregion

        /// <summary>
        ///   Determines if the command can execute.
        /// </summary>
        /// <param name = "parameter">
        ///   Data used by the command.
        /// </param>
        /// <returns>
        ///   true if this command can be executed; otherwise, false.
        /// </returns>
        public bool CanExecute(TParameter parameter)
        {
            if (mCanExecute == null)
            {
                LastCanExecute = true;
                return true;
            }
            LastCanExecute = mCanExecute(parameter);
            return LastCanExecute;
        }

        /// <summary>
        ///   Executes the command.
        /// </summary>
        /// <param name = "parameter">
        ///   Data used by the command.
        /// </param>
        public void Execute(TParameter parameter)
        {
            if (CanExecute(parameter))
            {
                mExecute(parameter);
            }
        }

        /// <summary>
        ///   Raises the <see cref = "CanExecuteChanged" /> event.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            var temp = CanExecuteChanged;
            if (temp != null)
            {
                temp(this, EventArgs.Empty);
            }
        }
    }
}