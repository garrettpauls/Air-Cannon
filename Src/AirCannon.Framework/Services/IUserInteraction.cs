namespace AirCannon.Framework.Services
{
    /// <summary>
    /// A service for interacting with the user.
    /// </summary>
    public interface IUserInteraction
    {
        /// <summary>
        ///   Prompts the user to select an option.
        /// </summary>
        /// <param name = "message">The message to present to the user.</param>
        /// <param name = "caption">The caption to present to the user.</param>
        /// <param name = "options">The options the user can pick from.</param>
        /// <returns>The option the user picked.</returns>
        string Prompt(string message, string caption, params string[] options);

        /// <summary>
        /// Prompts the user for a file to save to.
        /// </summary>
        /// <param name="fileFilter">The file filter to use.</param>
        /// <returns>
        /// The full path to the file or null if no file was selected.
        /// </returns>
        string SaveFilePrompt(string fileFilter);

        /// <summary>
        /// Prompts the user for a file to open.
        /// </summary>
        /// <param name="fileFilter">The file filter to use.</param>
        /// <returns>The full path to the file or null if no file was selected.</returns>
        string OpenFilePrompt(string fileFilter);
    }
}