using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AirCannon.Framework.Utilities;

namespace AirCannon.Views
{
    /// <summary>
    ///   Interaction logic for LauncherTreeView.xaml
    /// </summary>
    public partial class LauncherTreeView : UserControl
    {
        /// <summary>
        ///   Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(Property<LauncherTreeView>.Name(p => p.SelectedItem),
                                        typeof (object), typeof (LauncherTreeView),
                                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="LauncherTreeView"/> class.
        /// </summary>
        public LauncherTreeView()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Gets the currently selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set {  }
        }

        /// <summary>
        ///   Handles the SelectedItemChanged event of the tree view.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "object" /> instance containing the event data.</param>
        private void _HandleSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            SetValue(SelectedItemProperty, e.NewValue);
        }
    }
}