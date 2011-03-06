using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AirCannon
{
    /// <summary>
    /// Interaction logic for Shell.xaml
    /// </summary>
    public partial class Shell : Window
    {
        public Shell()
        {
            InitializeComponent();

            _DebugHook();
        }

        [Conditional("DEBUG")]
        private void _DebugHook()
        {
            Mouse.AddMouseDownHandler(this, _DebugHandleMouseDown);
        }

        /// <summary>
        /// When a debugger is attached and control is held, clicking executes a breakpoint.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void _DebugHandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Debugger.IsAttached && 
                Keyboard.Modifiers == ModifierKeys.Control &&
                e.LeftButton == MouseButtonState.Pressed)
            {
                var elementOver = Mouse.DirectlyOver;
                Debugger.Break();
            }
        }
    }
}
