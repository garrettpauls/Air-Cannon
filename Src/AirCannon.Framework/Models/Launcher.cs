using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using AirCannon.Framework.Utilities;
using AirCannon.Framework.WPF;
using Newtonsoft.Json;
using IOFile=System.IO.File;

namespace AirCannon.Framework.Models
{
    /// <summary>
    ///   Settings for launching a given application.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class Launcher : NotifyPropertyChangedBase, IDataErrorInfo
    {
        private string mArguments;
        private EnvironmentVariableCollection mEnvironmentVariables;
        private string mError;
        private string mFile;
        private bool mHasChanges;
        private bool mIsValid;
        private string mName;
        private LaunchGroup mParent;
        private string mWorkingDirectory;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "Launcher" /> class.
        /// </summary>
        /// <param name = "parent">The <see cref = "LaunchGroup" /> that contains this Launcher.</param>
        public Launcher(LaunchGroup parent = null)
        {
            Parent = parent;
            EnvironmentVariables = new EnvironmentVariableCollection();
            Arguments = string.Empty;
            File = string.Empty;
            Name = string.Empty;
            WorkingDirectory = string.Empty;
            HasChanges = false;
        }

        /// <summary>
        ///   Gets or sets the arguments used when launching the app.
        /// </summary>
        public string Arguments
        {
            get { return mArguments; }
            set { HasChanges |= SetPropertyValue(ref mArguments, value, () => Arguments); }
        }

        /// <summary>
        ///   Gets the environment variables that are used when launching.
        /// </summary>
        public EnvironmentVariableCollection EnvironmentVariables
        {
            get { return mEnvironmentVariables; }
            set
            {
                if (mEnvironmentVariables != value)
                {
                    if (mEnvironmentVariables != null)
                    {
                        mEnvironmentVariables.CollectionChanged -= _HandleEnvironmentVariablesChanged;
                        mEnvironmentVariables.ItemChanged -= _HandleEnvironmentVariableChanged;
                    }

                    mEnvironmentVariables = value;

                    if (mEnvironmentVariables != null)
                    {
                        mEnvironmentVariables.CollectionChanged += _HandleEnvironmentVariablesChanged;
                        mEnvironmentVariables.ItemChanged += _HandleEnvironmentVariableChanged;
                    }

                    OnPropertyChanged(() => EnvironmentVariables);
                    HasChanges = true;
                }
            }
        }

        /// <summary>
        ///   Gets or sets the file to launch.
        /// </summary>
        public string File
        {
            get { return mFile; }
            set
            {
                if (SetPropertyValue(ref mFile, value, () => File))
                {
                    HasChanges = true;
                    _UpdateError();
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this instance has changes.
        /// </summary>
        [JsonIgnore]
        public bool HasChanges
        {
            get { return mHasChanges; }
            set { SetPropertyValue(ref mHasChanges, value, () => HasChanges); }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance is valid and can launch.
        /// </summary>
        public bool IsValid
        {
            get { return mIsValid; }
            private set { SetPropertyValue(ref mIsValid, value, () => IsValid); }
        }

        /// <summary>
        ///   Gets or sets the alias.
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { HasChanges |= SetPropertyValue(ref mName, value, () => Name); }
        }

        /// <summary>
        ///   Gets or sets the parent.
        /// </summary>
        [JsonIgnore]
        public LaunchGroup Parent
        {
            get { return mParent; }
            set { HasChanges |= SetPropertyValue(ref mParent, value, () => Parent); }
        }

        /// <summary>
        ///   Gets or sets the working directory for the launched app.
        /// </summary>
        public string WorkingDirectory
        {
            get { return mWorkingDirectory; }
            set
            {
                if (SetPropertyValue(ref mWorkingDirectory, value, () => WorkingDirectory))
                {
                    HasChanges = true;
                    _UpdateError();
                }
            }
        }

        #region IDataErrorInfo Members

        /// <summary>
        ///   Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
        public string Error
        {
            get { return mError; }
            private set { SetPropertyValue(ref mError, value, () => Error); }
        }

        /// <summary>
        ///   Gets the error message for the property with the given name.
        /// </summary>
        /// <returns>The error message for the property. The default is an empty string ("").</returns>
        public string this[string propertyName]
        {
            get
            {
                string error = null;

                if (propertyName == Property.Name(() => File))
                {
                    if (!IOFile.Exists(File))
                    {
                        error = "File must exist.";
                    }
                }
                else if (propertyName == Property.Name(() => WorkingDirectory))
                {
                    if (!string.IsNullOrEmpty(WorkingDirectory) &&
                        !Directory.Exists(WorkingDirectory))
                    {
                        error = "Working directory must exist.";
                    }
                }

                return error;
            }
        }

        #endregion

        /// <summary>
        ///   Gets all the environment variables to be used when launching this application, starting
        ///   with the topmost parent and overriding values down to the launcher's settings.
        /// </summary>
        public EnvironmentVariableCollection AggregateEnvironmentVariables()
        {
            var envVars = new Stack<EnvironmentVariableCollection>();
            envVars.Push(EnvironmentVariables);

            LaunchGroup parent = mParent;

            while (parent != null)
            {
                envVars.Push(parent.EnvironmentVariables);
                parent = parent.Parent;
            }

            var aggregatedEnvVars = new EnvironmentVariableCollection(envVars.Pop());

            while (envVars.Count > 0)
            {
                aggregatedEnvVars.UpdateWith(envVars.Pop());
            }

            return aggregatedEnvVars;
        }

        /// <summary>
        ///   Launches the application and returns the running process.
        /// </summary>
        public Process Launch()
        {
            var startInfo = new ProcessStartInfo(File, Arguments);
            startInfo.WorkingDirectory = WorkingDirectory;
            startInfo.UseShellExecute = false;

            foreach (var envVar in AggregateEnvironmentVariables())
            {
                if (string.IsNullOrEmpty(envVar.Key) ||
                    string.IsNullOrEmpty(envVar.Value))
                {
                    continue;
                }
                startInfo.EnvironmentVariables[envVar.Key] = envVar.Value;
            }

            return Process.Start(startInfo);
        }

        /// <summary>
        ///   Raises the property changed event with the given property.
        /// </summary>
        protected override void OnPropertyChanged(string property)
        {
            base.OnPropertyChanged(property);

            if (property == Property.Name(() => File) ||
                property == Property.Name(() => WorkingDirectory))
            {
                IsValid = string.IsNullOrEmpty(this[Property.Name(() => File)]) &&
                          string.IsNullOrEmpty(this[Property.Name(() => WorkingDirectory)]);
            }
        }

        /// <summary>
        ///   Handles the ItemChanged event of the environment variable collection.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "AirCannon.Framework.Models.ItemChangedEventArgs&lt;AirCannon.Framework.Models.EnvironmentVariable&gt;" /> instance containing the event data.</param>
        private void _HandleEnvironmentVariableChanged(object sender, ItemChangedEventArgs<EnvironmentVariable> e)
        {
            HasChanges = true;
        }

        /// <summary>
        ///   Marks the launcher as having changes when the environment variable collection is changed.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void _HandleEnvironmentVariablesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasChanges = true;
        }

        /// <summary>
        ///   Updates the main error message.
        /// </summary>
        private void _UpdateError()
        {
            var sb = new StringBuilder();
            sb.AppendLine(this[Property.Name(() => File)]);
            sb.AppendLine(this[Property.Name(() => WorkingDirectory)]);

            string error = sb.ToString().Trim();
            if (error == string.Empty)
            {
                error = null;
            }

            Error = error;
        }
    }
}