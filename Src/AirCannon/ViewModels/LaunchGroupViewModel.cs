using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using AirCannon.Framework.Models;
using AirCannon.Framework.Utilities;
using AirCannon.Framework.WPF;

namespace AirCannon.ViewModels
{
    /// <summary>
    ///   A view model for the <see cref = "LaunchGroup" /> class.
    /// </summary>
    public class LaunchGroupViewModel : ViewModelBase<LaunchGroup>
    {
        private static readonly string[] mPassthroughPropertyNames =
            new[]
                {
                    Property<LaunchGroupViewModel>.Name(p => p.EnvironmentVariables),
                    Property<LaunchGroupViewModel>.Name(p => p.HasChanges),
                    Property<LaunchGroupViewModel>.Name(p => p.Launchers),
                    Property<LaunchGroupViewModel>.Name(p => p.LaunchGroups),
                    Property<LaunchGroupViewModel>.Name(p => p.Name),
                };

        private DelegateCommand mAddLaunchGroupCommand;
        private DelegateCommand mAddLauncherCommand;
        private DelegateCommand mDeleteCommand;
        private bool mHasChildren;
        private bool mIsExpanded;
        private bool mIsSelected;
        private ReadOnlyWrappingCollection<LaunchGroupViewModel, LaunchGroup> mLaunchGroups;
        private ReadOnlyWrappingCollection<LauncherViewModel, Launcher> mLaunchers;
        private LaunchGroupViewModel mParent;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LaunchGroupViewModel" /> class.
        /// </summary>
        /// <param name = "model">The <see cref = "LaunchGroup" /> instance to wrap.</param>
        public LaunchGroupViewModel(LaunchGroup model)
        {
            Model = model;
            HasChildren = Model.Launchers.Count > 0 ||
                          Model.LaunchGroups.Count > 0;
        }

        /// <summary>
        ///   Gets the command to add a new launch group.
        /// </summary>
        public DelegateCommand AddLaunchGroupCommand
        {
            get
            {
                if (mAddLaunchGroupCommand == null)
                {
                    mAddLaunchGroupCommand = new DelegateCommand(_AddLaunchGroup);
                }
                return mAddLaunchGroupCommand;
            }
        }

        /// <summary>
        ///   Gets the command to add a new launcher.
        /// </summary>
        public DelegateCommand AddLauncherCommand
        {
            get
            {
                if (mAddLauncherCommand == null)
                {
                    mAddLauncherCommand = new DelegateCommand(_AddLauncher);
                }
                return mAddLauncherCommand;
            }
        }

        /// <summary>
        ///   Gets all child <see cref = "LaunchGroups" /> and <see cref = "Launchers" />.
        /// </summary>
        public IEnumerable<object> Children
        {
            get { return LaunchGroups.Cast<object>().Concat(Launchers); }
        }

        /// <summary>
        ///   Gets the command to delete this group from its parent.
        /// </summary>
        public DelegateCommand DeleteCommand
        {
            get
            {
                if (mDeleteCommand == null)
                {
                    mDeleteCommand = new DelegateCommand(_Delete, () => Model.Parent != null);
                }
                return mDeleteCommand;
            }
        }

        /// <summary>
        ///   Gets the environment variables used to launch any child apps.
        /// </summary>
        public EnvironmentVariableCollection EnvironmentVariables
        {
            get { return Model.EnvironmentVariables; }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance has changes.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance has changes; otherwise, <c>false</c>.
        /// </value>
        public bool HasChanges
        {
            get { return Model.HasChanges; }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance has children.
        /// </summary>
        public bool HasChildren
        {
            get { return mHasChildren; }
            private set { SetPropertyValue(ref mHasChildren, value, () => HasChildren); }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get { return mIsExpanded; }
            set
            {
                if (SetPropertyValue(ref mIsExpanded, value, () => IsExpanded) &&
                    mIsExpanded && Parent != null)
                {
                    Parent.IsExpanded = true;
                }
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        public bool IsSelected
        {
            get { return mIsSelected; }
            set
            {
                if (SetPropertyValue(ref mIsSelected, value, () => IsSelected) &&
                    mIsSelected && Parent != null)
                {
                    Parent.IsExpanded = true;
                }
            }
        }

        /// <summary>
        ///   Gets the child launch groups.
        /// </summary>
        public ICollection<LaunchGroupViewModel> LaunchGroups
        {
            get
            {
                if (mLaunchGroups == null)
                {
                    mLaunchGroups = new ReadOnlyWrappingCollection<LaunchGroupViewModel, LaunchGroup>(
                        Model.LaunchGroups, group => new LaunchGroupViewModel(group), vm => vm.Model);
                    mLaunchGroups.CollectionChanged += _HandleChildCollectionChanged;
                }

                return mLaunchGroups;
            }
        }

        /// <summary>
        ///   Gets the child launchers.
        /// </summary>
        public ICollection<LauncherViewModel> Launchers
        {
            get
            {
                if (mLaunchers == null)
                {
                    mLaunchers = new ReadOnlyWrappingCollection<LauncherViewModel, Launcher>(
                        Model.Launchers, launcher => new LauncherViewModel(launcher), vm => vm.Model);
                    mLaunchers.CollectionChanged += _HandleChildCollectionChanged;
                }
                return mLaunchers;
            }
        }

        /// <summary>
        ///   Gets or sets the name.
        /// </summary>
        public string Name
        {
            get { return Model.Name; }
            set { Model.Name = value; }
        }

        /// <summary>
        ///   Gets the parent wrapped in a view model.
        /// </summary>
        public LaunchGroupViewModel Parent
        {
            get { return mParent; }
            private set { SetPropertyValue(ref mParent, value, () => Parent); }
        }

        /// <summary>
        ///   Gets the passthrough property names.
        /// </summary>
        protected override IEnumerable<string> PassthroughPropertyNames
        {
            get { return mPassthroughPropertyNames; }
        }

        /// <summary>
        ///   Determines whether the specified <see cref = "LaunchGroupViewModel" /> is equal to this instance.
        /// </summary>
        /// <param name = "other">The <see cref = "LaunchGroupViewModel" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref = "LaunchGroupViewModel" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(LaunchGroupViewModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return other.Model.Equals(Model);
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
            if (obj.GetType() != typeof (LaunchGroupViewModel))
            {
                return false;
            }
            return Equals((LaunchGroupViewModel) obj);
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Model.GetHashCode();
        }

        /// <summary>
        ///   Called when a model property is changed. Used to pass through property changed events.
        /// </summary>
        protected override void OnBasePropertyChanged(string propertyName)
        {
            if (propertyName == Property<LaunchGroup>.Name(p => p.Parent))
            {
                Parent = new LaunchGroupViewModel(Model.Parent);
            }

            base.OnBasePropertyChanged(propertyName);
        }

        /// <summary>
        ///   Adds a new launch group.
        /// </summary>
        private void _AddLaunchGroup()
        {
            Model.LaunchGroups.Add(new LaunchGroup(Model)
                                 {
                                     Name = "New launch group"
                                 });
        }

        /// <summary>
        ///   Adds a launcher to this group.
        /// </summary>
        private void _AddLauncher()
        {
            Model.Launchers.Add(new Launcher(Model)
                                    {
                                        Name = "New launcher"
                                    });
        }

        /// <summary>
        ///   Deletes this launch group from its parent.
        /// </summary>
        private void _Delete()
        {
            Model.Parent.Delete(Model);
        }

        /// <summary>
        ///   Whenever one of the collections that is included in <see cref = "Children" /> changes,
        ///   we have to raise the Children property changed event.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void _HandleChildCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(() => Children);
            HasChildren = Launchers.Count > 0 ||
                          LaunchGroups.Count > 0;
        }
    }
}