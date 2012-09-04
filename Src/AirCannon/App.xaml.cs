using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using AirCannon.Framework.Services;
using AirCannon.Framework.WPF;
using AirCannon.Properties;
using AirCannon.Services;
using AirCannon.ViewModels;

namespace AirCannon
{
    /// <summary>
    ///   Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static string mCopyright;
        private static string mVersion;
        private static readonly Assembly ASSEMBLY = Assembly.GetEntryAssembly();
        private static DelegateCommand mExitCommand;

        /// <summary>
        ///   Gets the copyright message.
        /// </summary>
        public static string Copyright
        {
            get
            {
                if (mCopyright == null)
                {
                    var copyrightAttrib = (AssemblyCopyrightAttribute)
                                          ASSEMBLY.GetCustomAttributes(typeof (AssemblyCopyrightAttribute), false)
                                              .FirstOrDefault();
                    mCopyright = copyrightAttrib != null ? copyrightAttrib.Copyright : string.Empty;
                }
                return mCopyright;
            }
        }

        /// <summary>
        ///   Gets the current <see cref = "App" />.
        /// </summary>
        public new static App Current
        {
            get { return (App) Application.Current; }
        }

        /// <summary>
        ///   Gets the command to exit the application.
        /// </summary>
        public static DelegateCommand ExitCommand
        {
            get
            {
                if (mExitCommand == null)
                {
                    mExitCommand = new DelegateCommand(_Exit);
                }
                return mExitCommand;
            }
        }

        /// <summary>
        ///   Gets or sets the shell window.
        /// </summary>
        public Shell Shell
        {
            get { return MainWindow as Shell; }
            private set { MainWindow = value; }
        }

        /// <summary>
        ///   Gets the version of the application.
        /// </summary>
        public static string Version
        {
            get
            {
                if (mVersion == null)
                {
                    mVersion = ASSEMBLY.GetName().Version.ToString();
                }
                return mVersion;
            }
        }

        /// <summary>
        ///   Raises the <see cref = "E:System.Windows.Application.Exit" /> event.
        /// </summary>
        /// <param name = "e">An <see cref = "T:System.Windows.ExitEventArgs" /> that contains the event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            Settings.Default.Save();
            base.OnExit(e);
        }

        /// <summary>
        ///   Raises the <see cref = "E:System.Windows.Application.Startup" /> event.
        /// </summary>
        /// <param name = "e">A <see cref = "T:System.Windows.StartupEventArgs" /> that contains the event data.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            _RegisterServices();
            Shell = new Shell();

            base.OnStartup(e);

            Shell.Show();
        }

        /// <summary>
        ///   Prompts the user to save and exits.
        /// </summary>
        private static void _Exit()
        {
            if (Current.Shell.ViewModel.SaveCommand.CanExecute())
            {
                const string SAVE = "Save and Exit";
                const string DISCARD_CHANGES = "Discard Changes and Exit";
                const string DONT_EXIT = "Don't Exit";
                var result = Service<IUserInteraction>.Instance.Prompt(
                    "You currently have unsaved changes that will be lost unless you save before exiting.", "Save?",
                    SAVE, DISCARD_CHANGES, DONT_EXIT);

                if(result == SAVE)
                {
                    Current.Shell.ViewModel.SaveCommand.Execute();
                }
                else if(result != DISCARD_CHANGES)
                {
                    return;
                }
            }
            Current.Shutdown();
        }

        /// <summary>
        ///   Registers all known services.
        /// </summary>
        private static void _RegisterServices()
        {
            Service<IUserInteraction>.Register(new UserInteractionService());
        }
    }
}