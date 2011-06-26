using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using AirCannon.Framework.Utilities;
using AirCannon.Framework.WPF;
using Newtonsoft.Json;

namespace AirCannon.Framework.Models
{
    /// <summary>
    ///   Represents a group of <see cref = "Launcher" />s that share common settings.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public class LaunchGroup : NotifyPropertyChangedBase
    {
        private EnvironmentVariableCollection mEnvironmentVariables;
        private bool mHasChanges;
        private ObservableCollection<LaunchGroup> mLaunchGroups;
        private ObservableCollection<Launcher> mLaunchers;
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
            LaunchGroups = new ObservableCollection<LaunchGroup>();
            Launchers = new ObservableCollection<Launcher>();
            EnvironmentVariables = new EnvironmentVariableCollection();
            Parent = parent;

            HasChanges = false;

            if (groups != null)
            {
                foreach (var group in groups)
                {
                    LaunchGroups.Add(group);
                }
            }

            if (launchers != null)
            {
                foreach (var launcher in launchers)
                {
                    Launchers.Add(launcher);
                }
            }
        }

        /// <summary>
        ///   Gets the environment variables that will be used when 
        ///   launching any child <see cref = "Launcher" />s.
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
        ///   Gets or sets a value indicating whether this instance has changes.
        /// </summary>
        [JsonIgnore]
        public bool HasChanges
        {
            get { return mHasChanges; }
            set { SetPropertyValue(ref mHasChanges, value, () => HasChanges); }
        }

        /// <summary>
        ///   Gets the child <see cref = "LaunchGroup" />s.
        /// </summary>
        public ObservableCollection<LaunchGroup> LaunchGroups
        {
            get { return mLaunchGroups; }
            set
            {
                if (mLaunchGroups != null)
                {
                    mLaunchGroups.Clear();
                    mLaunchGroups.CollectionChanged -= _HandleLaunchGroupCollectionChanged;
                }

                if (SetPropertyValue(ref mLaunchGroups, value, () => LaunchGroups))
                {
                    foreach (var launchGroup in mLaunchGroups)
                    {
                        launchGroup.Parent = this;
                    }
                    _HandleLaunchGroupCollectionChanged(mLaunchGroups,
                                                        new NotifyCollectionChangedEventArgs(
                                                            NotifyCollectionChangedAction.Add, mLaunchGroups));

                    HasChanges = true;
                }

                if (mLaunchGroups != null)
                {
                    mLaunchGroups.CollectionChanged += _HandleLaunchGroupCollectionChanged;
                }
            }
        }

        /// <summary>
        ///   Gets the child <see cref = "Launcher" />s.
        /// </summary>
        public ObservableCollection<Launcher> Launchers
        {
            get { return mLaunchers; }
            set
            {
                if (mLaunchers != null)
                {
                    mLaunchers.Clear();
                    mLaunchers.CollectionChanged -= _HandleLauncherCollectionChanged;
                }

                if (SetPropertyValue(ref mLaunchers, value, () => Launchers))
                {
                    foreach (var launcher in mLaunchers)
                    {
                        launcher.Parent = this;
                    }
                    _HandleLauncherCollectionChanged(mLaunchers,
                                                     new NotifyCollectionChangedEventArgs(
                                                         NotifyCollectionChangedAction.Add,
                                                         mLaunchers));

                    HasChanges = true;
                }

                if (mLaunchers != null)
                {
                    mLaunchers.CollectionChanged += _HandleLauncherCollectionChanged;
                }
            }
        }

        /// <summary>
        ///   Gets or sets the name of the <see cref = "LaunchGroup" />.
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { HasChanges |= SetPropertyValue(ref mName, value, () => Name); }
        }

        /// <summary>
        ///   Gets the <see cref = "LaunchGroup" /> that contains this one, or <c>null</c> if none exists.
        /// </summary>
        [JsonIgnore]
        public LaunchGroup Parent
        {
            get { return mParent; }
            private set { HasChanges |= SetPropertyValue(ref mParent, value, () => Parent); }
        }

        /// <summary>
        ///   Sets <see cref = "HasChanges" /> to false on this group and all children groups and launchers.
        /// </summary>
        public void ClearAllHasChanges()
        {
            HasChanges = false;

            foreach (var group in LaunchGroups)
            {
                group.ClearAllHasChanges();
            }

            foreach (var launcher in Launchers)
            {
                launcher.HasChanges = false;
            }
        }

        /// <summary>
        ///   Creates a clone of this instance. All children are also cloned.
        /// </summary>
        public LaunchGroup Clone()
        {
            var serialized = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<LaunchGroup>(serialized);
        }

        /// <summary>
        ///   Copies this launch group from its current group to the destination group.
        /// </summary>
        public void CopyTo(LaunchGroup destination)
        {
            destination.LaunchGroups.Add(Clone());
        }

        /// <summary>
        ///   Deletes the specified child group.
        /// </summary>
        public void Delete(LaunchGroup childGroup)
        {
            LaunchGroups.Remove(childGroup);
        }

        /// <summary>
        ///   Deletes the specified child launcher.
        /// </summary>
        public void Delete(Launcher childLauncher)
        {
            Launchers.Remove(childLauncher);
        }

        /// <summary>
        ///   Creates a new <see cref = "LaunchGroup" /> from the JSON in the given file.
        /// </summary>
        /// <param name = "file">The path of the file to load from.</param>
        /// <returns>The <see cref = "LaunchGroup" /> represented by the JSON in the file.</returns>
        public static LaunchGroup LoadFrom(string file)
        {
            var group = JsonConvert.DeserializeObject<LaunchGroup>(File.ReadAllText(file));

            group.ClearAllHasChanges();

            return group;
        }

        /// <summary>
        ///   Moves this launch group from its current group to the destination group.
        /// </summary>
        public void MoveTo(LaunchGroup destination)
        {
            Parent.LaunchGroups.Remove(this);
            destination.LaunchGroups.Add(this);
        }

        /// <summary>
        ///   Saves the <see cref = "LaunchGroup" /> to a file as JSON.
        /// </summary>
        /// <param name = "file">The file to save to.</param>
        public void SaveTo(string file)
        {
            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        /// <summary>
        ///   Handles the PropertyChanged event of a child <see cref = "LaunchGroup" />.
        ///   Used to update <see cref = "HasChanges" />.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        private void _HandleChildGroupPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Property<LaunchGroup>.Name(p => p.HasChanges) &&
                ((LaunchGroup) sender).HasChanges)
            {
                HasChanges = true;
            }
        }

        /// <summary>
        ///   Handles the PropertyChanged event of a child <see cref = "Launcher" />.
        ///   Used to update <see cref = "HasChanges" />.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        private void _HandleChildLauncherPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Property<Launcher>.Name(p => p.HasChanges) &&
                ((Launcher) sender).HasChanges)
            {
                HasChanges = true;
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
        ///   Marks the launch group as having changes when the environment variables are changed.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void _HandleEnvironmentVariablesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            HasChanges = true;
        }

        /// <summary>
        ///   Handles the CollectionChanged event of the Groups collection.
        ///   Whenever a group is added, updates its parent to this launch group.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void _HandleLaunchGroupCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (LaunchGroup group in e.NewItems)
                {
                    group.Parent = this;
                    group.PropertyChanged += _HandleChildGroupPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (LaunchGroup group in e.OldItems)
                {
                    group.PropertyChanged -= _HandleChildGroupPropertyChanged;
                }
            }

            HasChanges = true;
        }

        /// <summary>
        ///   Handles the CollectionChanged event of the launchers collection.
        ///   When a launcher is added, updates its parent to this launch group.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void _HandleLauncherCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Launcher launcher in e.NewItems)
                {
                    launcher.Parent = this;
                    launcher.PropertyChanged += _HandleChildLauncherPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (Launcher launcher in e.OldItems)
                {
                    launcher.PropertyChanged -= _HandleChildLauncherPropertyChanged;
                }
            }

            HasChanges = true;
        }
    }
}