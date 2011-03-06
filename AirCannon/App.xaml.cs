using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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