using System.IO;
using System.Linq;
using System.Windows;
using AirCannon.Framework.Models;
using AirCannon.Framework.Services;
using AirCannon.Framework.WPF;
using AirCannon.Properties;
using AirCannon.Views;

namespace AirCannon.ViewModels
{
    /// <summary>
    ///   The <see cref = "MainViewModel" /> contains all global commands and the root launch group.
    /// </summary>
    public class MainViewModel : NotifyPropertyChangedBase
    {
        private const string JSON_FILE_FILTER = "JSON files (*.json)|*.json|All files (*.*)|*.*";
        private DelegateCommand mExitCommand;
        private DelegateCommand mNewCommand;
        private DelegateCommand mOpenCommand;
        private LaunchGroupViewModel mRoot;
        private DelegateCommand mSaveAsCommand;
        private DelegateCommand mSaveCommand;
        private DelegateCommand mShowAboutCommand;
        private DelegateCommand mToggleMainWindowCommand;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            _Open(Settings.Default.CurrentFile);
        }

        /// <summary>
        ///   Exits the application.
        /// </summary>
        public DelegateCommand ExitCommand
        {
            get
            {
                if (mExitCommand == null)
                {
                    mExitCommand = new DelegateCommand(Application.Current.Shutdown);
                }
                return mExitCommand;
            }
        }

        /// <summary>
        ///   Gets the new command.
        /// </summary>
        public DelegateCommand NewCommand
        {
            get
            {
                if (mNewCommand == null)
                {
                    mNewCommand = new DelegateCommand(_New);
                }
                return mNewCommand;
            }
        }

        /// <summary>
        ///   Gets the open command.
        /// </summary>
        public DelegateCommand OpenCommand
        {
            get
            {
                if (mOpenCommand == null)
                {
                    mOpenCommand = new DelegateCommand(_Open);
                }
                return mOpenCommand;
            }
        }

        /// <summary>
        ///   Gets the root <see cref = "LaunchGroupViewModel" /> that is used to collect all launchable items.
        /// </summary>
        public LaunchGroupViewModel Root
        {
            get { return mRoot; }
            private set
            {
                if (SetPropertyValue(ref mRoot, value, () => Root))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                    SaveAsCommand.RaiseCanExecuteChanged();
                    if (Root != null)
                    {
                        var firstItem = Root.Children.FirstOrDefault();
                        if (firstItem is LauncherViewModel)
                        {
                            ((LauncherViewModel) firstItem).IsSelected = true;
                        }
                        else if (firstItem is LaunchGroupViewModel)
                        {
                            ((LaunchGroupViewModel) firstItem).IsSelected = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///   Gets the save as command.
        /// </summary>
        public DelegateCommand SaveAsCommand
        {
            get
            {
                if (mSaveAsCommand == null)
                {
                    mSaveAsCommand = new DelegateCommand(() => _SaveAs(), _CanSave);
                }
                return mSaveAsCommand;
            }
        }

        /// <summary>
        ///   Gets the save command.
        /// </summary>
        public DelegateCommand SaveCommand
        {
            get
            {
                if (mSaveCommand == null)
                {
                    mSaveCommand = new DelegateCommand(() => _Save(), _CanSave);
                }
                return mSaveCommand;
            }
        }

        /// <summary>
        ///   Gets a command to show the about dialog.
        /// </summary>
        public DelegateCommand ShowAboutCommand
        {
            get
            {
                if (mShowAboutCommand == null)
                {
                    return mShowAboutCommand = new DelegateCommand(_ShowAbout);
                }
                return mShowAboutCommand;
            }
        }

        public DelegateCommand ToggleMainWindowCommand
        {
            get
            {
                if (mToggleMainWindowCommand == null)
                {
                    mToggleMainWindowCommand = new DelegateCommand(_ToggleMainWindow, _CanToggleMainWindow);
                }
                return mToggleMainWindowCommand;
            }
        }

        /// <summary>
        ///   Determines if the user can save.
        /// </summary>
        private bool _CanSave()
        {
            return Root != null;
        }

        /// <summary>
        ///   Determines if the main window's visiblity can be toggled.
        /// </summary>
        /// <returns></returns>
        private bool _CanToggleMainWindow()
        {
            return Application.Current.MainWindow != null;
        }

        /// <summary>
        ///   Creates a new root launch group. Prompts user to save if they have changes.
        /// </summary>
        private void _New()
        {
            if (_PromptForContinueIfRootIsChanged())
            {
                Root = new LaunchGroupViewModel(new LaunchGroup());
                Settings.Default.CurrentFile = null;
            }
        }

        /// <summary>
        ///   Prompts the user for a file and attempts to load it as a launch group.
        /// </summary>
        private void _Open()
        {
            var file = Service<IUserInteraction>.Instance.OpenFilePrompt(JSON_FILE_FILTER);
            if (file != null && _PromptForContinueIfRootIsChanged())
            {
                _Open(file);
            }
        }

        /// <summary>
        ///   Opens the given file as the root node.
        /// </summary>
        /// <param name = "file">The file to open.</param>
        private void _Open(string file)
        {
            if (File.Exists(file))
            {
                Root = new LaunchGroupViewModel(LaunchGroup.LoadFrom(file));
                Settings.Default.CurrentFile = file;
            }
            else
            {
                Root = new LaunchGroupViewModel(new LaunchGroup());
            }
        }


        /// <summary>
        ///   If the root model has changes this prompts the user if they want to save their 
        ///   current changes, discard them, or cancel the current operation.
        /// </summary>
        /// <returns><c>true</c> if the operation should continue, otherwise <c>false</c>.</returns>
        private bool _PromptForContinueIfRootIsChanged()
        {
            if (Root.Model.HasChanges)
            {
                var options = new[]
                                  {
                                      "Save",
                                      "Discard",
                                      "Cancel"
                                  };
                var selected = Service<IUserInteraction>.Instance.Prompt(
                    @"Your current launch group has changes that have not been saved.
You can save your changes, discard them, or cancel the current operation.",
                    @"Save changes?", options);

                if (selected == options[0])
                {
                    return _Save();
                }

                if (selected == options[2] || selected == null)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///   Saves the root model to a file. 
        ///   If no file has been previously specified, the user is prompted for one.
        /// </summary>
        /// <returns><c>true</c> if the save was successful, otherwise <c>false</c>.</returns>
        private bool _Save()
        {
            if (Settings.Default.CurrentFile == null)
            {
                return _SaveAs();
            }

            Root.Model.SaveTo(Settings.Default.CurrentFile);
            Root.Model.ClearAllHasChanges();

            return true;
        }

        /// <summary>
        ///   Prompts the user for a file and then saves the root model to that file.
        /// </summary>
        /// <returns><c>true</c> if the save was successful, otherwise <c>false</c>.</returns>
        private bool _SaveAs()
        {
            var file = Service<IUserInteraction>.Instance.SaveFilePrompt(JSON_FILE_FILTER);

            if (file == null)
            {
                return false;
            }

            Settings.Default.CurrentFile = file;
            return _Save();
        }

        /// <summary>
        ///   Shows the about dialog.
        /// </summary>
        private static void _ShowAbout()
        {
            Window window = new Window();
            window.Title = "About";
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Content = new AboutView();
            window.ResizeMode = ResizeMode.NoResize;
            window.Icon = Application.Current.MainWindow.Icon;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.Owner = Application.Current.MainWindow;
            window.ShowDialog();
        }

        /// <summary>
        ///   Toggles the visibility of the main window.
        /// </summary>
        private void _ToggleMainWindow()
        {
            if (Application.Current.MainWindow.Visibility == Visibility.Visible)
            {
                Application.Current.MainWindow.Hide();
            }
            else
            {
                Application.Current.MainWindow.Show();
            }
        }
    }
}