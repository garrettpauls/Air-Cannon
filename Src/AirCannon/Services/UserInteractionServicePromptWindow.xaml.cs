using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace AirCannon.Services
{
    /// <summary>
    ///   Interaction logic for UserInteractionServicePromptWindow.xaml
    /// </summary>
    public partial class UserInteractionServicePromptWindow : Window
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "UserInteractionServicePromptWindow" /> class.
        /// </summary>
        public UserInteractionServicePromptWindow()
        {
            InitializeComponent();

            Options = new ObservableCollection<string>();
            DataContext = this;
        }

        /// <summary>
        ///   Gets or sets the message.
        /// </summary>
        public string Message
        {
            get { return mMessage.Text; }
            set { mMessage.Text = value; }
        }

        /// <summary>
        ///   Gets the options.
        /// </summary>
        public ObservableCollection<string> Options { get; private set; }

        /// <summary>
        ///   Gets the selected option.
        /// </summary>
        public string SelectedOption { get; private set; }

        /// <summary>
        ///   Handles the Click event of an option button.
        ///   Used to set the selected option and close the window.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.RoutedEventArgs" /> instance containing the event data.</param>
        private void _HandleOptionButtonClick(object sender, RoutedEventArgs e)
        {
            SelectedOption = ((Button) sender).Content as string;
            Close();
        }
    }
}