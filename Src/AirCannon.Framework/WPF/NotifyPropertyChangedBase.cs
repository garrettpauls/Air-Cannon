using System;
using System.ComponentModel;
using System.Linq.Expressions;
using AirCannon.Framework.Utilities;

namespace AirCannon.Framework.WPF
{
    /// <summary>
    ///   A base class implementation of <see cref = "INotifyPropertyChanged" />
    /// </summary>
    public abstract class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        /// <summary>
        ///   Raises the property changed event with the given property.
        /// </summary>
        /// <typeparam name = "TProp">The type of the property.</typeparam>
        /// <param name = "propertySelector">
        ///   A member expression which contains the property that was changed as its body.<br />
        ///   () => SomeProperty
        /// </param>
        /// <example>
        ///   this.OnPropertyChanged(() => ChangedProperty);
        /// </example>
        protected void OnPropertyChanged<TProp>(Expression<Func<TProp>> propertySelector)
        {
            OnPropertyChanged(Property.Name(propertySelector));
        }

        /// <summary>
        ///   Raises the property changed event with the given property.
        /// </summary>
        protected virtual void OnPropertyChanged(string property)
        {
            var temp = PropertyChanged;
            if (temp != null)
            {
                temp(this, new PropertyChangedEventArgs(property));
            }
        }

        /// <summary>
        ///   Sets the backing property to the new value if necessary and raises the property changed event.
        /// </summary>
        /// <typeparam name = "TProp">The type of the property.</typeparam>
        /// <param name = "backingField">
        ///   The field which backs the property. It will be updated with the new value.
        /// </param>
        /// <param name = "value">The new value for the property.</param>
        /// <param name = "propertySelector">
        ///   A member expression which contains the property that was changed as its body.<br />
        ///   () => SomeProperty
        /// </param>
        /// <returns>
        ///   <c>true</c> if the <paramref name = "backingField" /> 
        ///   is not equal to <paramref name = "value" />, <c>false</c> otherwise.
        /// </returns>
        protected bool SetPropertyValue<TProp>(ref TProp backingField, TProp value,
                                               Expression<Func<TProp>> propertySelector)
        {
            if (!Equals(backingField, value))
            {
                backingField = value;
                OnPropertyChanged(propertySelector);
                return true;
            }

            return false;
        }

        #region Implementation of INotifyPropertyChanged

        /// <summary>
        ///   Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}