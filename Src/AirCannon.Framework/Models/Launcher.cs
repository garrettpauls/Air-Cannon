using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using AirCannon.Framework.Services;
using AirCannon.Framework.Utilities;
using AirCannon.Framework.WPF;
using Newtonsoft.Json;
using IOFile=System.IO.File;

namespace AirCannon.Framework.Models
{
    /// <summary>
    ///   Settings for launching a given application.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
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
            get
            {
                OnPropertyChanged(() => File);
                OnPropertyChanged(() => WorkingDirectory);
                return mIsValid;
            }
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
                    if (!IOFile.Exists(AggregateEnvironmentVariables().Expand(File)))
                    {
                        error = "File must exist.";
                    }
                }
                else if (propertyName == Property.Name(() => WorkingDirectory))
                {
                	var wd = AggregateEnvironmentVariables().Expand(WorkingDirectory);
                    if (!string.IsNullOrEmpty(wd) &&
                        !Directory.Exists(wd))
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
            var vars = new Stack<EnvironmentVariableCollection>();
            vars.Push(EnvironmentVariables);
            var parent = mParent;
            while(parent != null)
            {
                vars.Push(parent.EnvironmentVariables);
                parent = parent.Parent;
            }

            var aggregatedEnvVars = new EnvironmentVariableCollection();

            while(vars.Any())
            {
                var v = vars.Pop();
                foreach(EnvironmentVariable envVar in v)
                {
                    var value = aggregatedEnvVars.Expand(envVar.Value);
                    aggregatedEnvVars.UpdateWith(new EnvironmentVariable(envVar.Key, value));
                }
            }

            return aggregatedEnvVars;
        }

        /// <summary>
        ///   Creates a copy of this launcher.
        /// </summary>
        public Launcher Clone()
        {
            var serialized = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<Launcher>(serialized);
        }

        /// <summary>
        ///   Copies this launcher from its current group to the destination group.
        /// </summary>
        public void CopyTo(LaunchGroup destination)
        {
            destination.Launchers.Add(Clone());
        }

        public bool Equals(Launcher other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Equals(other.mArguments, mArguments) && Equals(other.mEnvironmentVariables, mEnvironmentVariables) &&
                   Equals(other.mFile, mFile) && Equals(other.mName, mName) &&
                   Equals(other.mWorkingDirectory, mWorkingDirectory);
        }

        /// <summary>
        ///   Determines whether the specified <see cref = "System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name = "obj">The <see cref = "System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref = "System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != typeof (Launcher))
            {
                return false;
            }
            return Equals((Launcher) obj);
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = (mArguments != null ? mArguments.GetHashCode() : 0);
                result = (result*397) ^ (mEnvironmentVariables != null ? mEnvironmentVariables.GetHashCode() : 0);
                result = (result*397) ^ (mFile != null ? mFile.GetHashCode() : 0);
                result = (result*397) ^ (mName != null ? mName.GetHashCode() : 0);
                result = (result*397) ^ (mWorkingDirectory != null ? mWorkingDirectory.GetHashCode() : 0);
                return result;
            }
        }

        /// <summary>
        ///   Launches the application and returns the running process.
        /// </summary>
        public Process Launch()
        {
            var aggregateVars = AggregateEnvironmentVariables();

            var startInfo = new ProcessStartInfo(aggregateVars.Expand(File), aggregateVars.Expand(Arguments));
            startInfo.WorkingDirectory = aggregateVars.Expand(WorkingDirectory);
            startInfo.UseShellExecute = false;

            foreach (var envVar in aggregateVars)
            {
                if (string.IsNullOrEmpty(envVar.Key) ||
                    string.IsNullOrEmpty(envVar.Value))
                {
                    continue;
                }
                startInfo.EnvironmentVariables[envVar.Key] = envVar.Value;
            }

            Process process = null;

            try
            {
                process = Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                string message =
                    string.Format("The launcher '{0}' could not be launched. The following error occurred: {2}{2}{1}",
                                  Name, ex.Message, Environment.NewLine);
                Service<IUserInteraction>.Instance.ShowErrorMessage(message, "Launch Error");
            }

            return process;
        }

        /// <summary>
        ///   Moves this launcher from its current group to the destination group.
        /// </summary>
        public void MoveTo(LaunchGroup destination)
        {
            Parent.Launchers.Remove(this);
            destination.Launchers.Add(this);
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