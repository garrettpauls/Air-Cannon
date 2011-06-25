using System.Windows;
using AirCannon.Framework.Services;
using System.Linq;
using Microsoft.Win32;

namespace AirCannon.Services
{
    /// <summary>
    ///   An implementation of the <see cref = "IUserInteraction" /> service.
    /// </summary>
    public class UserInteractionService : IUserInteraction
    {
        #region IUserInteraction Members

        /// <summary>
        ///   Prompts the user for a file to open.
        /// </summary>
        /// <param name = "fileFilter">The file filter to use.</param>
        /// <returns>
        ///   The full path to the file or null if no file was selected.
        /// </returns>
        public string OpenFilePrompt(string fileFilter)
        {
            return _ShowFilePrompt(new OpenFileDialog
                                       {
                                           CheckFileExists = true
                                       }, fileFilter);
        }

        /// <summary>
        ///   Prompts the user to select an option.
        /// </summary>
        /// <param name = "message">The message to present to the user.</param>
        /// <param name = "caption">The caption to present to the user.</param>
        /// <param name = "options">The options the user can pick from.</param>
        /// <returns>
        ///   The option the user picked.
        /// </returns>
        public string Prompt(string message, string caption, params string[] options)
        {
            var window = new UserInteractionServicePromptWindow();
            window.Title = caption;
            window.Message = message;
            // We want the option buttons pushed to the right, which requires
            // a flow direction of Right-to-Left, so we add the options
            // in reverse order to still get them ordered correctly.
            foreach (var option in options.Reverse())
            {
                window.Options.Add(option);
            }

            window.ShowDialog();

            return window.SelectedOption;
        }

        /// <summary>
        ///   Prompts the user for a file to save to.
        /// </summary>
        /// <param name = "fileFilter">The file filter to use.</param>
        /// <returns>
        ///   The full path to the file or null if no file was selected.
        /// </returns>
        public string SaveFilePrompt(string fileFilter)
        {
            return _ShowFilePrompt(new SaveFileDialog(), fileFilter);
        }

        /// <summary>
        ///   Shows an error message to the user.
        /// </summary>
        /// <param name = "message">The message to present to the user.</param>
        /// <param name = "caption">The caption to present to the user.</param>
        public void ShowErrorMessage(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        #endregion

        /// <summary>
        ///   Configures a <see cref = "FileDialog" /> and shows it to the user.
        /// </summary>
        /// <returns>
        ///   The full path to the file or null if no file was selected.
        /// </returns>
        private static string _ShowFilePrompt(FileDialog dialog, string filter)
        {
            dialog.AddExtension = true;
            dialog.CheckPathExists = true;
            dialog.DereferenceLinks = true;
            dialog.Filter = filter;
            dialog.RestoreDirectory = true;

            if (dialog.ShowDialog() ?? false)
            {
                return dialog.FileName;
            }

            return null;
        }
    }
}