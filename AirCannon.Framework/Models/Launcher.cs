using System.Collections.Generic;
using AirCannon.Framework.WPF;

namespace AirCannon.Framework.Models
{
    /// <summary>
    ///   Settings for launching a given application.
    /// </summary>
    public class Launcher : NotifyPropertyChangedBase
    {
        private readonly EnvironmentVariableDictionary mEnvironmentVariables;
        private string mFile;
        private string mName;
        private LaunchGroup mParent;
        private string mWorkingDirectory;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Launcher" /> class.
        /// </summary>
        /// <param name = "parent">The <see cref = "LaunchGroup" /> that contains this Launcher.</param>
        public Launcher(LaunchGroup parent = null)
        {
            mParent = parent;
            mEnvironmentVariables = new EnvironmentVariableDictionary();
            mFile = string.Empty;
            mName = string.Empty;
            mWorkingDirectory = string.Empty;
        }

        /// <summary>
        ///   Gets the environment variables that are used when launching.
        /// </summary>
        public EnvironmentVariableDictionary EnvironmentVariables
        {
            get { return mEnvironmentVariables; }
        }

        /// <summary>
        ///   Gets or sets the file to launch.
        /// </summary>
        public string File
        {
            get { return mFile; }
            set { SetPropertyValue(ref mFile, value, () => File); }
        }

        /// <summary>
        ///   Gets or sets the alias.
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { SetPropertyValue(ref mName, value, () => Name); }
        }

        /// <summary>
        ///   Gets or sets the parent.
        /// </summary>
        public LaunchGroup Parent
        {
            get { return mParent; }
            set { SetPropertyValue(ref mParent, value, () => Parent); }
        }

        /// <summary>
        ///   Gets or sets the working directory for the launched app.
        /// </summary>
        public string WorkingDirectory
        {
            get { return mWorkingDirectory; }
            set { SetPropertyValue(ref mWorkingDirectory, value, () => WorkingDirectory); }
        }

        /// <summary>
        ///   Gets all the environment variables to be used when launching this application, starting
        ///   with the topmost parent and overriding values down to the launcher's settings.
        /// </summary>
        public EnvironmentVariableDictionary AggregateEnvironmentVariables()
        {
            var envVars = new Stack<EnvironmentVariableDictionary>();
            envVars.Push(EnvironmentVariables);

            LaunchGroup parent = mParent;

            while (parent != null)
            {
                envVars.Push(parent.EnvironmentVariables);
                parent = parent.Parent;
            }

            var aggregatedEnvVars = new EnvironmentVariableDictionary(envVars.Pop());

            while (envVars.Count > 0)
            {
                aggregatedEnvVars.UpdateWith(envVars.Pop());
            }

            return aggregatedEnvVars;
        }
    }
}