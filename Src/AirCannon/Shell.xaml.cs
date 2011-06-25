using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using AirCannon.ViewModels;

namespace AirCannon
{
    /// <summary>
    ///   Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        private WindowState mLastWindowState = WindowState.Normal;

        public Shell()
        {
            InitializeComponent();

            _DebugHook();
        }

        /// <summary>
        ///   Gets the view model associated with the shell.
        /// </summary>
        public MainViewModel ViewModel
        {
            get { return mMainViewModel; }
        }

        /// <summary>
        ///   Toggles the visibility of this window.
        /// </summary>
        public void ToggleVisibility()
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();
            }
            else
            {
                Show();
                WindowState = mLastWindowState;
            }
        }

        /// <summary>
        ///   When a debugger is attached and control is held, clicking executes a breakpoint.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.Input.MouseButtonEventArgs" /> instance containing the event data.</param>
        private static void _DebugHandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Debugger.IsAttached &&
                Keyboard.Modifiers == ModifierKeys.Control &&
                e.LeftButton == MouseButtonState.Pressed)
            {
                var elementOver = Mouse.DirectlyOver;
                Debugger.Break();
            }
        }

        [Conditional("DEBUG")]
        private void _DebugHook()
        {
            Mouse.AddMouseDownHandler(this, _DebugHandleMouseDown);
        }

        /// <summary>
        ///   Handles the Closing event of the window.
        ///   Prevents closing and calls the exit command.
        /// </summary>
        private void _HandleClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            App.ExitCommand.Execute();
        }

        /// <summary>
        ///   Handles the StateChanged event of the window.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.EventArgs" /> instance containing the event data.</param>
        private void _HandleWindowStateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
            else
            {
                mLastWindowState = WindowState;
            }
        }
    }
}