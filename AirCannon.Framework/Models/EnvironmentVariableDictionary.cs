using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace AirCannon.Framework.Models
{
    /// <summary>
    ///   A dictionary mapping a string to a string. Keys are case-insensitive.
    /// </summary>
    public class EnvironmentVariableDictionary : IDictionary<string, string>, INotifyCollectionChanged
    {
        private readonly Dictionary<string, string> mBackingDictionary;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "EnvironmentVariableDictionary" /> class.
        /// </summary>
        public EnvironmentVariableDictionary()
            : this(new KeyValuePair<string, string>[] {})
        {
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref = "EnvironmentVariableDictionary" /> class.
        /// </summary>
        /// <param name = "initialValues">The initial values to use to populate the collection.</param>
        public EnvironmentVariableDictionary(IEnumerable<KeyValuePair<string, string>> initialValues)
        {
            mBackingDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var value in initialValues)
            {
                Add(value);
            }
        }

        #region IDictionary<string,string> Members

        /// <summary>
        ///   Adds an item to the <see cref = "T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name = "item">The object to add to the <see cref = "T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref = "T:System.NotSupportedException">
        ///   The <see cref = "T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public void Add(KeyValuePair<string, string> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        ///   Adds the specified key and value pair.
        /// </summary>
        /// <param name = "key">The key.</param>
        /// <param name = "value">The value.</param>
        public void Add(string key, string value)
        {
            this[key] = value;
        }

        /// <summary>
        ///   Removes all items from the <see cref = "T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <exception cref = "T:System.NotSupportedException">
        ///   The <see cref = "T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public void Clear()
        {
            mBackingDictionary.Clear();
            _RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>
        ///   Determines whether the <see cref = "T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name = "item">The object to locate in the <see cref = "T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        ///   true if <paramref name = "item" /> is found in the <see cref = "T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<string, string> item)
        {
            return mBackingDictionary.ContainsKey(item.Key) &&
                   Equals(mBackingDictionary[item.Key], item.Value);
        }

        /// <summary>
        ///   Determines whether the <see cref = "T:System.Collections.Generic.IDictionary`2" /> contains an element with the specified key.
        /// </summary>
        /// <param name = "key">The key to locate in the <see cref = "T:System.Collections.Generic.IDictionary`2" />.</param>
        /// <returns>
        ///   true if the <see cref = "T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise, false.
        /// </returns>
        /// <exception cref = "T:System.ArgumentNullException"><paramref name = "key" /> is null.
        /// </exception>
        public bool ContainsKey(string key)
        {
            return mBackingDictionary.ContainsKey(key);
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Gets the number of elements contained in the <see cref = "T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <returns>
        ///   The number of elements contained in the <see cref = "T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public int Count
        {
            get { return mBackingDictionary.Count; }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return mBackingDictionary.GetEnumerator();
        }

        /// <summary>
        ///   Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        ///   An <see cref = "T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///   Gets a value indicating whether the <see cref = "T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <returns>true if the <see cref = "T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.
        /// </returns>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        ///   Gets or sets the element with the specified key.
        /// </summary>
        /// <returns>
        ///   The element with the specified key.
        /// </returns>
        /// <exception cref = "T:System.ArgumentNullException"><paramref name = "key" /> is null.
        /// </exception>
        /// <exception cref = "T:System.Collections.Generic.KeyNotFoundException">
        ///   The property is retrieved and <paramref name = "key" /> is not found.
        /// </exception>
        /// <exception cref = "T:System.NotSupportedException">
        ///   The property is set and the <see cref = "T:System.Collections.Generic.IDictionary`2" /> is read-only.
        /// </exception>
        public string this[string key]
        {
            get { return mBackingDictionary[key]; }
            set
            {
                if (mBackingDictionary.ContainsKey(key))
                {
                    var oldItem = new KeyValuePair<string, string>(key, mBackingDictionary[key]);
                    mBackingDictionary[key] = value;
                    var newItem = new KeyValuePair<string, string>(key, value);

                    _RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Replace,
                                                newItem, oldItem));
                }
                else
                {
                    mBackingDictionary[key] = value;
                    _RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Add,
                                                new KeyValuePair<string, string>(key, value)));
                }
            }
        }

        /// <summary>
        ///   Gets an <see cref = "T:System.Collections.Generic.ICollection`1" /> containing the keys of the <see cref = "T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <returns>
        ///   An <see cref = "T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements <see cref = "T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        public ICollection<string> Keys
        {
            get { return mBackingDictionary.Keys; }
        }

        /// <summary>
        ///   Removes the first occurrence of a specific object from the <see cref = "T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <param name = "item">The object to remove from the <see cref = "T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        ///   true if <paramref name = "item" /> was successfully removed from the <see cref = "T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name = "item" /> is not found in the original <see cref = "T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        /// <exception cref = "T:System.NotSupportedException">
        ///   The <see cref = "T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public bool Remove(KeyValuePair<string, string> item)
        {
            if (Contains(item))
            {
                Remove(item.Key);
                return true;
            }
            return false;
        }

        /// <summary>
        ///   Removes the element with the specified key from the <see cref = "T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <param name = "key">The key of the element to remove.</param>
        /// <returns>
        ///   true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name = "key" /> was not found in the original <see cref = "T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        /// <exception cref = "T:System.ArgumentNullException"><paramref name = "key" /> is null.
        /// </exception>
        /// <exception cref = "T:System.NotSupportedException">
        ///   The <see cref = "T:System.Collections.Generic.IDictionary`2" /> is read-only.
        /// </exception>
        public bool Remove(string key)
        {
            if (mBackingDictionary.ContainsKey(key))
            {
                var item = new KeyValuePair<string, string>(key, mBackingDictionary[key]);
                mBackingDictionary.Remove(key);
                _RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(
                                            NotifyCollectionChangedAction.Remove, item));

                return true;
            }
            return false;
        }

        /// <summary>
        ///   Gets the value associated with the specified key.
        /// </summary>
        /// <param name = "key">The key.</param>
        /// <param name = "value">The value.</param>
        /// <returns><c>true</c> if the key is in the collection, otherwise <c>false</c></returns>
        public bool TryGetValue(string key, out string value)
        {
            return mBackingDictionary.TryGetValue(key, out value);
        }

        /// <summary>
        ///   Gets an <see cref = "T:System.Collections.Generic.ICollection`1" /> containing the values in the <see cref = "T:System.Collections.Generic.IDictionary`2" />.
        /// </summary>
        /// <returns>
        ///   An <see cref = "T:System.Collections.Generic.ICollection`1" /> containing the values in the object that implements <see cref = "T:System.Collections.Generic.IDictionary`2" />.
        /// </returns>
        public ICollection<string> Values
        {
            get { return mBackingDictionary.Values; }
        }

        #endregion

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        /// <summary>
        ///   Determines whether the specified <see cref = "EnvironmentVariableDictionary" /> is equal to this instance.
        /// </summary>
        /// <param name = "other">The <see cref = "EnvironmentVariableDictionary" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref = "EnvironmentVariableDictionary" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(EnvironmentVariableDictionary other)
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

            foreach (var key in Keys)
            {
                if (!other.ContainsKey(key))
                {
                    return false;
                }
                if (!Equals(this[key], other[key]))
                {
                    return false;
                }
            }

            return true;
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
            if (obj.GetType() != typeof (EnvironmentVariableDictionary))
            {
                return false;
            }
            return Equals((EnvironmentVariableDictionary) obj);
        }

        /// <summary>
        ///   Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///   A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            int result = 397;

            foreach (var item in this)
            {
                result = (result*397) ^ (item.Key != null ? item.Key.GetHashCode() : 0);
                result = (result*397) ^ (item.Value != null ? item.Value.GetHashCode() : 0);
            }

            return result;
        }

        /// <summary>
        ///   Updates the dictionary with the given <see cref = "KeyValuePair{TKey,TValue}" />s, 
        ///   overwriting any existing values and adding any new values.
        /// </summary>
        /// <param name = "environmentVariables">The environment variables to add to the dictionary.</param>
        /// <returns>The dictionary.</returns>
        public EnvironmentVariableDictionary UpdateWith(params KeyValuePair<string, string>[] environmentVariables)
        {
            return UpdateWith(environmentVariables.AsEnumerable());
        }

        /// <summary>
        ///   Updates the dictionary with the given <see cref = "KeyValuePair{TKey,TValue}" />s, 
        ///   overwriting any existing values and adding any new values.
        /// </summary>
        /// <param name = "environmentVariables">The environment variables to add to the dictionary.</param>
        /// <returns>The dictionary.</returns>
        public EnvironmentVariableDictionary UpdateWith(IEnumerable<KeyValuePair<string, string>> environmentVariables)
        {
            foreach (var variable in environmentVariables)
            {
                this[variable.Key] = variable.Value;
            }

            return this;
        }

        /// <summary>
        ///   Raises the <see cref = "CollectionChanged" /> event.
        /// </summary>
        /// <param name = "args">The <see cref = "System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void _RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            var temp = CollectionChanged;
            if (temp != null)
            {
                temp(this, args);
            }
        }
    }
}