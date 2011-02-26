using System.Collections.Generic;
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
        }

        /// <summary>
        ///   Gets all child <see cref = "LaunchGroups" /> and <see cref = "Launchers" />.
        /// </summary>
        public IEnumerable<object> Children
        {
            get { return LaunchGroups.Cast<object>().Concat(Launchers); }
        }

        /// <summary>
        ///   Gets the environment variables used to launch any child apps.
        /// </summary>
        public EnvironmentVariableDictionary EnvironmentVariables
        {
            get { return Model.EnvironmentVariables; }
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
                        Model.Groups, group => new LaunchGroupViewModel(group), vm => vm.Model);
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

        #region Overrides of ViewModelBase<LaunchGroup>

        /// <summary>
        ///   Called when a model property is changed. Used to pass through property changed events.
        /// </summary>
        protected override void OnBasePropertyChanged(string propertyName)
        {
            if (propertyName == Property<LaunchGroup>.Name(p => p.Parent))
            {
                Parent = new LaunchGroupViewModel(Model.Parent);
            }
            else
            {
                RaisePropertyChanged(propertyName);
            }
        }

        #endregion
    }
}