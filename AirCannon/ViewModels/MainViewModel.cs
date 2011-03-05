using AirCannon.Framework.Models;
using AirCannon.Framework.WPF;

namespace AirCannon.ViewModels
{
    /// <summary>
    ///   The <see cref = "MainViewModel" /> contains all global commands and the root launch group.
    /// </summary>
    public class MainViewModel : NotifyPropertyChangedBase
    {
        private LaunchGroupViewModel mRoot;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "MainViewModel" /> class.
        /// </summary>
        public MainViewModel()
        {
            Root = new LaunchGroupViewModel(new LaunchGroup());
        }

        /// <summary>
        ///   Gets the root <see cref = "LaunchGroupViewModel" /> that is used to collect all launchable items.
        /// </summary>
        public LaunchGroupViewModel Root
        {
            get { return mRoot; }
            private set { SetPropertyValue(ref mRoot, value, () => Root); }
        }
    }
}