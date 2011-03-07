using System.Collections.Generic;
using AirCannon.Framework.Models;
using AirCannon.Framework.Utilities;
using AirCannon.Framework.WPF;

namespace AirCannon.ViewModels
{
    /// <summary>
    ///   A view model for the <see cref = "Launcher" /> class.
    /// </summary>
    public class LauncherViewModel : ViewModelBase<Launcher>
    {
        private static readonly string[] mPassthroughPropertyNames =
            new[]
                {
                    Property<LauncherViewModel>.Name(p => p.Arguments),
                    Property<LauncherViewModel>.Name(p => p.EnvironmentVariables),
                    Property<LauncherViewModel>.Name(p => p.File),
                    Property<LauncherViewModel>.Name(p => p.HasChanges),
                    Property<LauncherViewModel>.Name(p => p.Name),
                    Property<LauncherViewModel>.Name(p => p.WorkingDirectory),
                };

        private LaunchGroupViewModel mParent;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LauncherViewModel" /> class.
        /// </summary>
        /// <param name = "launcher">The <see cref = "Launcher" /> instance to wrap.</param>
        public LauncherViewModel(Launcher launcher)
        {
            Model = launcher;
            Parent = new LaunchGroupViewModel(Model.Parent);
        }

        /// <summary>
        ///   Gets or sets the arguments used to launch the app.
        /// </summary>
        public string Arguments
        {
            get { return Model.Arguments; }
            set { Model.Arguments = value; }
        }

        /// <summary>
        ///   Gets the environment variable collection used when launching the app.
        /// </summary>
        public EnvironmentVariableCollection EnvironmentVariables
        {
            get { return Model.EnvironmentVariables; }
        }

        /// <summary>
        ///   Gets or sets the file to launch.
        /// </summary>
        public string File
        {
            get { return Model.File; }
            set { Model.File = value; }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance has changes.
        /// </summary>
        public bool HasChanges
        {
            get { return Model.HasChanges; }
        }

        /// <summary>
        ///   Gets or sets the name to describe this launcher.
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
        ///   Gets or sets the working directory.
        /// </summary>
        public string WorkingDirectory
        {
            get { return Model.WorkingDirectory; }
            set { Model.WorkingDirectory = value; }
        }

        #region Overrides of ViewModelBase<Launcher>

        /// <summary>
        ///   Called when a property on the model is changed. 
        ///   Used to pass through property changed events and
        ///   update the parent view model instance.
        /// </summary>
        protected override void OnBasePropertyChanged(string propertyName)
        {
            if (propertyName == Property<Launcher>.Name(p => p.Parent))
            {
                Parent = new LaunchGroupViewModel(Model.Parent);
            }

            base.OnBasePropertyChanged(propertyName);
        }

        #endregion
    }
}