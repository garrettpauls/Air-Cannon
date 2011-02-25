using AirCannon.Framework.Models;
using AirCannon.Framework.WPF;

namespace AirCannon.ViewModels
{
    /// <summary>
    ///   A view model for the <see cref = "LaunchGroup" /> class.
    /// </summary>
    public class LaunchGroupViewModel : ViewModelBase<LaunchGroup>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "LaunchGroupViewModel" /> class.
        /// </summary>
        /// <param name = "model">The <see cref = "LaunchGroup" /> instance to wrap.</param>
        public LaunchGroupViewModel(LaunchGroup model)
        {
            Model = model;
        }

        #region Overrides of ViewModelBase<LaunchGroup>

        /// <summary>
        ///   Called when a model property is changed. Used to pass through property changed events.
        /// </summary>
        protected override void OnBasePropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);
        }

        #endregion
    }
}