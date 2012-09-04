using System;
using System.Windows.Input;
using System.Windows.Markup;
#pragma warning disable 67

namespace AirCannon.Framework.WPF
{
    /// <summary>
    ///   This command does nothing an can never execute.
    /// </summary>
    public class DisableCommand : MarkupExtension, ICommand
    {
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return false;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}