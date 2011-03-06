using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace AirCannon.Framework.WPF
{
    /// <summary>
    ///   A base class for a view model.
    /// </summary>
    /// <typeparam name = "TModel">The type of model the view model is for.</typeparam>
    public abstract class ViewModelBase<TModel> : NotifyPropertyChangedBase
    {
        private TModel mModel;

        /// <summary>
        ///   Gets or sets the model this view model is wrapping.
        /// </summary>
        public TModel Model
        {
            get { return mModel; }
            protected set
            {
                if (!ReferenceEquals(mModel, value))
                {
                    if (mModel is INotifyPropertyChanged)
                    {
                        ((INotifyPropertyChanged) mModel).PropertyChanged -= _HandleBasePropertyChanged;
                    }

                    mModel = value;

                    if (mModel is INotifyPropertyChanged)
                    {
                        ((INotifyPropertyChanged) mModel).PropertyChanged += _HandleBasePropertyChanged;
                    }
                }
            }
        }

        /// <summary>
        ///   A list of property names that, when changes in the model, will be used to
        ///   fire the <see cref = "INotifyPropertyChanged.PropertyChanged" /> event on this object.
        /// </summary>
        protected virtual IEnumerable<string> PassthroughPropertyNames
        {
            get { return new string[0]; }
        }

        /// <summary>
        ///   Called when a property on the Model is called.
        /// </summary>
        /// <param name = "propertyName">Name of the property that was changed.</param>
        protected virtual void OnBasePropertyChanged(string propertyName)
        {
            if (PassthroughPropertyNames.Contains(propertyName))
            {
                RaisePropertyChanged(propertyName);
            }
        }

        /// <summary>
        ///   Handles the PropertyChanged event of the Model. 
        ///   Used for passing property changed events to <see cref = "OnBasePropertyChanged" />.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        private void _HandleBasePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnBasePropertyChanged(e.PropertyName);
        }
    }
}