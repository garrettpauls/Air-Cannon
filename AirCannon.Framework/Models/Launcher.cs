using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using AirCannon.Framework.WPF;
using Newtonsoft.Json;

namespace AirCannon.Framework.Models
{
    /// <summary>
    ///   Settings for launching a given application.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class Launcher : NotifyPropertyChangedBase
    {
        private string mArguments;
        private EnvironmentVariableDictionary mEnvironmentVariables;
        private string mFile;
        private bool mHasChanges;
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
            EnvironmentVariables = new EnvironmentVariableDictionary();
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
        public EnvironmentVariableDictionary EnvironmentVariables
        {
            get { return mEnvironmentVariables; }
            set
            {
                if (mEnvironmentVariables != value)
                {
                    if (mEnvironmentVariables != null)
                    {
                        mEnvironmentVariables.CollectionChanged -= _HandleEnvironmentVariablesChanged;
                    }

                    mEnvironmentVariables = value;

                    if (mEnvironmentVariables != null)
                    {
                        mEnvironmentVariables.CollectionChanged += _HandleEnvironmentVariablesChanged;
                    }

                    RaisePropertyChanged(() => EnvironmentVariables);
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
            set { HasChanges |= SetPropertyValue(ref mFile, value, () => File); }
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
            set { HasChanges |= SetPropertyValue(ref mWorkingDirectory, value, () => WorkingDirectory); }
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

        /// <summary>
        ///   Determines whether the specified <see cref = "Launcher" /> is equal to the current <see cref = "Launcher" />.
        /// </summary>
        /// <returns>
        ///   true if the specified <see cref = "Launcher" /> is equal to the current <see cref = "Launcher" />; otherwise, false.
        /// </returns>
        /// <param name = "other">The <see cref = "Launcher" /> to compare with the current <see cref = "Launcher" />. </param>
        /// <filterpriority>2</filterpriority>
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
            return Equals(other.mEnvironmentVariables, mEnvironmentVariables) &&
                   Equals(other.mArguments, mArguments) &&
                   Equals(other.mFile, mFile) &&
                   Equals(other.mName, mName) &&
                   Equals(other.mWorkingDirectory, mWorkingDirectory);
        }

        /// <summary>
        ///   Determines whether the specified <see cref = "T:System.Object" /> is equal to the current <see cref = "T:System.Object" />.
        /// </summary>
        /// <returns>
        ///   true if the specified <see cref = "T:System.Object" /> is equal to the current <see cref = "T:System.Object" />; otherwise, false.
        /// </returns>
        /// <param name = "obj">The <see cref = "T:System.Object" /> to compare with the current <see cref = "T:System.Object" />. </param>
        /// <filterpriority>2</filterpriority>
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
        ///   Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        ///   A hash code for the current <see cref = "T:System.Object" />.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = (mEnvironmentVariables != null ? mEnvironmentVariables.GetHashCode() : 0);
                result = (result*397) ^ (mArguments != null ? mArguments.GetHashCode() : 0);
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
            var startInfo = new ProcessStartInfo(File, Arguments);
            startInfo.WorkingDirectory = WorkingDirectory;

            foreach (var envVar in AggregateEnvironmentVariables())
            {
                startInfo.EnvironmentVariables.Add(envVar.Key, envVar.Value);
            }

            return Process.Start(startInfo);
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
    }
}