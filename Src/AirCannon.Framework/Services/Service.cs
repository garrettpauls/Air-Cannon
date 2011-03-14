namespace AirCannon.Framework.Services
{
    /// <summary>
    ///   A service locator for the service of type <see cref = "TService" />.
    /// </summary>
    /// <typeparam name = "TService">The type of the service.</typeparam>
    public static class Service<TService>
    {
        private static TService mInstance;

        /// <summary>
        ///   Gets the registered instance of the service implementing <typeparamref name = "TService" />.
        /// </summary>
        public static TService Instance
        {
            get { return mInstance; }
        }

        /// <summary>
        ///   Registers an instance of <typeparamref name = "TService" /> to use.
        /// </summary>
        /// <param name = "instance">The instance of <typeparamref name = "TService" />.</param>
        public static void Register(TService instance)
        {
            mInstance = instance;
        }
    }
}