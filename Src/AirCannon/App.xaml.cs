using System.Linq;
using System.Reflection;
using System.Windows;
using AirCannon.Framework.Services;
using AirCannon.Properties;
using AirCannon.Services;

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
        ///   Handles the Exit event of the application.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.ExitEventArgs" /> instance containing the event data.</param>
        private void _HandleExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }

        /// <summary>
        ///   Handles the Startup event of the application.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Windows.StartupEventArgs" /> instance containing the event data.</param>
        private void _HandleStartup(object sender, StartupEventArgs e)
        {
            Service<IUserInteraction>.Register(new UserInteractionService());
        }
    }
}