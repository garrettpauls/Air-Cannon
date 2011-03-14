using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace AirCannon.Views
{
    /// <summary>
    ///   Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        public AboutView()
        {
            InitializeComponent();
        }

        /// <summary>
        ///   Handles the RequestNavigate event of a hyperlink by launching
        ///   the system browser with the given link.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.Navigation.RequestNavigateEventArgs" /> instance containing the event data.</param>
        private void _HandleRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var hyperlink = (Hyperlink) sender;
            Process.Start(hyperlink.NavigateUri.ToString());
            e.Handled = true;
        }
    }
}