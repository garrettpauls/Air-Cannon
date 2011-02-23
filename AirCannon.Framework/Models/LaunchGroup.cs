using System.Collections.Generic;
using System.Collections.ObjectModel;
using AirCannon.Framework.WPF;

namespace AirCannon.Framework.Models
{
    /// <summary>
    ///   Represents a group of <see cref = "Launcher" />s that share common settings.
    /// </summary>
    public class LaunchGroup : NotifyPropertyChangedBase
    {
        private readonly EnvironmentVariableDictionary mEnvironmentVariables;
        private readonly ObservableCollection<LaunchGroup> mGroups;
        private readonly ObservableCollection<Launcher> mLaunchers;
        private string mName;
        private LaunchGroup mParent;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LaunchGroup" /> class.
        /// </summary>
        /// <param name = "parent">The parent of this group.</param>
        /// <param name = "groups">The child groups of this group.</param>
        /// <param name = "launchers">The child launchers of this group.</param>
        public LaunchGroup(LaunchGroup parent = null,
                           IEnumerable<LaunchGroup> groups = null,
                           IEnumerable<Launcher> launchers = null)
        {
            mGroups = new ObservableCollection<LaunchGroup>();
            mLaunchers = new ObservableCollection<Launcher>();
            mEnvironmentVariables = new EnvironmentVariableDictionary();
            mParent = parent;

            if (groups != null)
            {
                foreach (var group in groups)
                {
                    mGroups.Add(group);
                    group.Parent = this;
                }
            }

            if (launchers != null)
            {
                foreach (var launcher in launchers)
                {
                    mLaunchers.Add(launcher);
                    launcher.Parent = this;
                }
            }
        }

        /// <summary>
        ///   Gets the environment variables that will be used when 
        ///   launching any child <see cref = "Launcher" />s.
        /// </summary>
        public EnvironmentVariableDictionary EnvironmentVariables
        {
            get { return mEnvironmentVariables; }
        }

        /// <summary>
        ///   Gets the child <see cref = "LaunchGroup" />s.
        /// </summary>
        public ObservableCollection<LaunchGroup> Groups
        {
            get { return mGroups; }
        }

        /// <summary>
        ///   Gets the child <see cref = "Launcher" />s.
        /// </summary>
        public ObservableCollection<Launcher> Launchers
        {
            get { return mLaunchers; }
        }

        /// <summary>
        ///   Gets or sets the name of the <see cref = "LaunchGroup" />.
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { SetPropertyValue(ref mName, value, () => Name); }
        }

        /// <summary>
        ///   Gets the <see cref = "LaunchGroup" /> that contains this one, or <c>null</c> if none exists.
        /// </summary>
        public LaunchGroup Parent
        {
            get { return mParent; }
            private set { SetPropertyValue(ref mParent, value, () => Parent); }
        }
    }
}