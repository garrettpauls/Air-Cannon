using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using AirCannon.Framework.Models;
using AirCannon.Framework.Utilities;
using AirCannon.Framework.WPF;
using IOFile=System.IO.File;

namespace AirCannon.ViewModels
{
    /// <summary>
    ///   A view model for the <see cref = "Launcher" /> class.
    /// </summary>
    public class LauncherViewModel : ViewModelBase<Launcher>, IDataErrorInfo
    {
        private static readonly string[] mPassthroughPropertyNames =
            new[]
                {
                    Property<LauncherViewModel>.Name(p => p.Arguments),
                    Property<LauncherViewModel>.Name(p => p.EnvironmentVariables),
                    Property<LauncherViewModel>.Name(p => p.File),
                    Property<LauncherViewModel>.Name(p => p.HasChanges),
                    Property<LauncherViewModel>.Name(p => p.IsValid),
                    Property<LauncherViewModel>.Name(p => p.Name),
                    Property<LauncherViewModel>.Name(p => p.WorkingDirectory),
                };

        private static readonly Image LAUNCHER_ICON;
        private static readonly Image DISABLED_LAUNCHER_ICON;
        private Image mIcon;
        private bool mIsSelected;
        private DelegateCommand mLaunchCommand;
        private LaunchGroupViewModel mParent;

        /// <summary>
        ///   Initializes the <see cref = "LauncherViewModel" /> class.
        /// </summary>
        static LauncherViewModel()
        {
            LAUNCHER_ICON =
                new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/bullet_go.png"))
                    };
            DISABLED_LAUNCHER_ICON =
                new Image
                    {
                        Source =
                            new BitmapImage(new Uri("pack://application:,,,/Resources/Icons/bullet_go_disabled.png"))
                    };
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LauncherViewModel" /> class.
        /// </summary>
        /// <param name = "launcher">The <see cref = "Launcher" /> instance to wrap.</param>
        public LauncherViewModel(Launcher launcher)
        {
            Model = launcher;
            Parent = new LaunchGroupViewModel(Model.Parent);
            _UpdateIcon();
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
            set
            {
                if (Model.File != value)
                {
                    Model.File = value;
                    _UpdateIcon();
                }
            }
        }

        /// <summary>
        ///   Gets a value indicating whether this instance has changes.
        /// </summary>
        public bool HasChanges
        {
            get { return Model.HasChanges; }
        }

        /// <summary>
        ///   Gets the icon representing this launcher.
        /// </summary>
        public Image Icon
        {
            get { return mIcon; }
            private set { SetPropertyValue(ref mIcon, value, () => Icon); }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <remarks>
        ///   This exists to avoid binding errors.
        /// </remarks>
        public bool IsExpanded
        {
            get { return false; }
            set
            {
                // This has no children and can't be expanded.
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
        ///   Gets a value indicating whether this instance is valid.
        /// </summary>
        public bool IsValid
        {
            get { return Model.IsValid; }
        }

        /// <summary>
        ///   Gets the launch command to launch the launcher.
        /// </summary>
        public DelegateCommand LaunchCommand
        {
            get
            {
                if (mLaunchCommand == null)
                {
                    mLaunchCommand = new DelegateCommand(_Launch, _CanLaunch);
                }
                return mLaunchCommand;
            }
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

        #region IDataErrorInfo Members

        /// <summary>
        ///   Gets an error message indicating what is wrong with this object.
        /// </summary>
        /// <returns>An error message indicating what is wrong with this object. The default is an empty string ("").</returns>
        public string Error
        {
            get { return Model.Error; }
        }

        /// <summary>
        ///   Gets the error message for the property with the given name.
        /// </summary>
        /// <returns>The error message for the property. The default is an empty string ("").</returns>
        public string this[string propertyName]
        {
            get { return Model[propertyName]; }
        }

        #endregion

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
            else if (propertyName == Property<Launcher>.Name(p => p.IsValid))
            {
                LaunchCommand.RaiseCanExecuteChanged();
            }

            base.OnBasePropertyChanged(propertyName);
        }

        /// <summary>
        ///   Determines if this instance can launch.
        /// </summary>
        private bool _CanLaunch()
        {
            return Model.IsValid;
        }

        /// <summary>
        ///   Launches this instance.
        /// </summary>
        private void _Launch()
        {
            Model.Launch();
        }

        /// <summary>
        ///   Updates the icon based on the file.
        /// </summary>
        private void _UpdateIcon()
        {
            if (IOFile.Exists(File))
            {
                Icon = LAUNCHER_ICON;
            }
            else
            {
                Icon = DISABLED_LAUNCHER_ICON;
            }
        }
    }
}