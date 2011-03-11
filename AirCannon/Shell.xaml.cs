using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace AirCannon
{
    /// <summary>
    ///   Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();

            _DebugHook();
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
        ///   Handles the NotifyIconDoubleClick event of the notify icon.
        ///   Used to toggle window visibility.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void _HandleNotifyIconDoubleClick(object sender, RoutedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        /// <summary>
        ///   Handles the NotifyIconLeftClick event of the notify icon.
        ///   Used to show the launch context menu.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void _HandleNotifyIconLeftClick(object sender, RoutedEventArgs e)
        {
        }
    }
}