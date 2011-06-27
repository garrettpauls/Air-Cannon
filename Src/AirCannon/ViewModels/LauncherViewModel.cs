using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Windows.Interop;
using AirCannon.Framework.Models;
using AirCannon.Framework.Utilities;
using AirCannon.Framework.WPF;
using IOFile=System.IO.File;
using OpenFileDialog=Microsoft.Win32.OpenFileDialog;

namespace AirCannon.ViewModels
{
    /// <summary>
    ///   A view model for the <see cref = "Launcher" /> class.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
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

        private DelegateCommand mDeleteCommand;
        private bool mIsSelected;
        private DelegateCommand mLaunchCommand;
        private DelegateCommand mLookupFileCommand;
        private DelegateCommand mLookupWorkingDirectoryCommand;
        private LaunchGroupViewModel mParent;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "LauncherViewModel" /> class.
        /// </summary>
        /// <param name = "launcher">The <see cref = "Launcher" /> instance to wrap.</param>
        public LauncherViewModel(Launcher launcher)
        {
            Model = launcher;
            Parent = new LaunchGroupViewModel(Model.Parent);
            ComponentDispatcher.ThreadIdle += _HandleThreadIdle;
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
        ///   Gets the command to prompt the user for a file.
        /// </summary>
        public DelegateCommand LookupFileCommand
        {
            get
            {
                if (mLookupFileCommand == null)
                {
                    mLookupFileCommand = new DelegateCommand(_LookupFile);
                }
                return mLookupFileCommand;
            }
        }

        /// <summary>
        ///   Gets the command to prompt the user for a directory.
        /// </summary>
        public DelegateCommand LookupWorkingDirectoryCommand
        {
            get
            {
                if (mLookupWorkingDirectoryCommand == null)
                {
                    mLookupWorkingDirectoryCommand =
                        new DelegateCommand(_LookupWorkingDirectory);
                }
                return mLookupWorkingDirectoryCommand;
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
        ///   Determines whether the specified <see cref = "LauncherViewModel" /> is equal to this instance.
        /// </summary>
        /// <param name = "other">The <see cref = "LauncherViewModel" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref = "LauncherViewModel" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(LauncherViewModel other)
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
            if (obj.GetType() != typeof (LauncherViewModel))
            {
                return false;
            }
            return Equals((LauncherViewModel) obj);
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
        ///   Deletes this launcher from its parent.
        /// </summary>
        private void _Delete()
        {
            Model.Parent.Delete(Model);
        }

        /// <summary>
        ///   Handles the Idle event of the Application to update the launch command.
        /// </summary>
        private void _HandleThreadIdle(object sender, EventArgs e)
        {
            if (!LaunchCommand.LastCanExecute)
            {
                // Check if the file or working dir now exists.
                LaunchCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        ///   Launches this instance.
        /// </summary>
        private void _Launch()
        {
            Model.Launch();
        }

        /// <summary>
        ///   Prompts the user to select a file.
        /// </summary>
        private void _LookupFile()
        {
            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.RestoreDirectory = true;
            if (!string.IsNullOrEmpty(File) &&
                Directory.Exists(Path.GetDirectoryName(File)))
            {
                dialog.InitialDirectory = Path.GetDirectoryName(File);
            }

            if (dialog.ShowDialog().Value)
            {
                File = dialog.FileName;
            }
        }

        /// <summary>
        ///   Prompts the user for a working directory.
        /// </summary>
        private void _LookupWorkingDirectory()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (Directory.Exists(WorkingDirectory))
                {
                    dialog.SelectedPath = WorkingDirectory;
                }
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    WorkingDirectory = dialog.SelectedPath;
                }
            }
        }
    }
}