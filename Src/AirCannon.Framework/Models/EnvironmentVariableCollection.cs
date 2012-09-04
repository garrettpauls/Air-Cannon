using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using AirCannon.Framework.Utilities;
using Newtonsoft.Json;

namespace AirCannon.Framework.Models
{
    /// <summary>
    ///   A dictionary mapping a string to a string. Keys are case-insensitive.
    /// </summary>
    [JsonConverter(typeof (EnvironmentVariableCollectionJsonConverter))]
    public class EnvironmentVariableCollection : ObservableCollection<EnvironmentVariable>
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref = "EnvironmentVariableCollection" /> class.
        /// </summary>
        public EnvironmentVariableCollection()
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "EnvironmentVariableCollection" /> class.
        /// </summary>
        /// <param name = "existing">Adds these environment variables to the collection.</param>
        public EnvironmentVariableCollection(IEnumerable<EnvironmentVariable> existing)
        {
            foreach (var item in existing)
            {
                Add(new EnvironmentVariable(item.Key, item.Value));
            }
        }

        /// <summary>
        ///   Gets or sets the element at the specified index.
        /// </summary>
        /// <returns>
        ///   The element at the specified index.
        /// </returns>
        /// <exception cref = "T:System.ArgumentOutOfRangeException"><paramref name = "index" /> is less than zero.
        ///   -or-
        ///   <paramref name = "index" /> is equal to or greater than <see cref = "P:System.Collections.ObjectModel.Collection`1.Count" />.
        /// </exception>
        public string this[string key]
        {
            get
            {
                var item = _FindByKey(key);
                if (item == null)
                {
                    throw new KeyNotFoundException();
                }
                return item.Value;
            }
            set
            {
                var item = _FindByKey(key);
                if (item == null)
                {
                    item = new EnvironmentVariable(key, value);
                    Add(item);
                }
                else
                {
                    item.Value = value;
                }
            }
        }

        /// <summary>
        ///   Adds an environment variable with the given key and value.
        /// </summary>
        /// <param name = "key">The key.</param>
        /// <param name = "value">The value.</param>
        public void Add(string key, string value)
        {
            Add(new EnvironmentVariable(key, value));
        }

        /// <summary>
        ///   Determines whether this collection contains an item with the given key.
        /// </summary>
        /// <param name = "key">The key.</param>
        /// <returns>
        ///   <c>true</c> if this contains the key; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsKey(string key)
        {
            return Items.Any(envar => envar.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///   Determines whether the specified <see cref = "EnvironmentVariableCollection" /> is equal to this instance.
        /// </summary>
        /// <param name = "other">The <see cref = "EnvironmentVariableCollection" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref = "EnvironmentVariableCollection" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(EnvironmentVariableCollection other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (Count != other.Count)
            {
                return false;
            }

            return Items.All(other.Contains);
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
            if (obj.GetType() != typeof (EnvironmentVariableCollection))
            {
                return false;
            }
            return Equals((EnvironmentVariableCollection) obj);
        }

        public string Expand(string value)
        {
            if(string.IsNullOrEmpty(value))
            {
                return value;
            }

            return mExpandRegex.Replace(value, _HandleExpandMatch);
        }

        private string _HandleExpandMatch(Match match)
        {
            var key = match.Groups["var"].Value;
            string result;

            if(ContainsKey(key))
            {
                result = this[key];
            }
            else
            {
                result = Environment.GetEnvironmentVariable(key) ?? "%" + key + "%";
            }

            return result;
        }

        private static readonly Regex mExpandRegex = new Regex(@"%(?<var>[^%]+)%");

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int result = 397;

                foreach (var item in this)
                {
                    result = (result*397) ^ (item.Key != null ? item.Key.GetHashCode() : 0);
                    result = (result*397) ^ (item.Value != null ? item.Value.GetHashCode() : 0);
                }

                return result;
            }
        }

        /// <summary>
        ///   Occurs when an item is changed.
        /// </summary>
        public event EventHandler<ItemChangedEventArgs<EnvironmentVariable>> ItemChanged;

        /// <summary>
        ///   Updates the dictionary with the given <see cref = "KeyValuePair{TKey,TValue}" />s, 
        ///   overwriting any existing values and adding any new values.
        /// </summary>
        /// <param name = "environmentVariables">The environment variables to add to the dictionary.</param>
        /// <returns>The dictionary.</returns>
        public EnvironmentVariableCollection UpdateWith(params EnvironmentVariable[] environmentVariables)
        {
            return UpdateWith(environmentVariables.AsEnumerable());
        }

        /// <summary>
        ///   Updates the dictionary with the given <see cref = "KeyValuePair{TKey,TValue}" />s, 
        ///   overwriting any existing values and adding any new values.
        /// </summary>
        /// <param name = "environmentVariables">The environment variables to add to the dictionary.</param>
        /// <returns>The dictionary.</returns>
        public EnvironmentVariableCollection UpdateWith(IEnumerable<EnvironmentVariable> environmentVariables)
        {
            foreach (var variable in environmentVariables)
            {
                this[variable.Key] = variable.Value;
            }

            return this;
        }

        /// <summary>
        ///   Raises the <see cref = "ObservableCollection{T}.CollectionChanged" /> event.
        /// </summary>
        /// <param name = "e">The <see cref = "System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (EnvironmentVariable item in e.NewItems)
                {
                    item.PropertyChanged += _HandleItemPropertyChanged;
                }
            }
            if (e.OldItems != null)
            {
                foreach (EnvironmentVariable item in e.OldItems)
                {
                    item.PropertyChanged -= _HandleItemPropertyChanged;
                }
            }
            base.OnCollectionChanged(e);
        }

        /// <summary>
        ///   Finds an item by key.
        /// </summary>
        /// <param name = "key">The key.</param>
        /// <returns>The item with the given key, or null if there is none.</returns>
        private EnvironmentVariable _FindByKey(string key)
        {
            return Items.FirstOrDefault(envar => envar.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///   Handles the PropertyChanged event of each item.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        private void _HandleItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _RaiseItemChanged(sender as EnvironmentVariable, e.PropertyName);
        }

        /// <summary>
        ///   Raises the <see cref = "ItemChanged" /> event.
        /// </summary>
        /// <param name = "item">The item that was changed.</param>
        /// <param name = "property">The property that was changed.</param>
        private void _RaiseItemChanged(EnvironmentVariable item, string property)
        {
            var temp = ItemChanged;
            if (temp != null)
            {
                temp(this, new ItemChangedEventArgs<EnvironmentVariable>(item, property));
            }
        }
    }
}