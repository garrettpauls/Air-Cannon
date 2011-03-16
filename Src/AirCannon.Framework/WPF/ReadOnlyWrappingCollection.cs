using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace AirCannon.Framework.WPF
{
    /// <summary>
    ///   A read only collection that syncs with an observable backing collection and wraps the values
    ///   into new types. It updates with the backing collection.
    /// </summary>
    /// <typeparam name = "TWrapped">
    ///   The type the backing objects should be wrapped into.
    ///   Two different instances of <see cref = "TWrapped" /> should be equal 
    ///   as long as they wrap the same <see cref = "TBacking" /> object.</typeparam>
    /// <typeparam name = "TBacking">The type of the backing objects.</typeparam>
    /// <typeparam name = "TBackingCollection">The type of the backing collection.</typeparam>
    public class ReadOnlyWrappingCollection<TWrapped, TBacking>
        : ICollection<TWrapped>, INotifyCollectionChanged
    {
        private readonly ObservableCollection<TBacking> mBackingCollection;
        private readonly Func<TWrapped, TBacking> mUnwrap;
        private readonly Func<TBacking, TWrapped> mWrap;
        private readonly IList<TWrapped> mWrappedItems;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ReadOnlyWrappingCollection&lt;TWrapped, TBacking, TBackingCollection&gt;" /> class.
        /// </summary>
        /// <param name = "backingCollection">The collection to wrap and sync with.</param>
        /// <param name = "wrap">A function that wraps a <see cref = "TBacking" /> into a <see cref = "TWrapped" />.</param>
        /// <param name = "unwrap">A function that unwraps a <see cref = "TWrapped" /> into a <see cref = "TBacking" />.</param>
        public ReadOnlyWrappingCollection(ObservableCollection<TBacking> backingCollection,
                                          Func<TBacking, TWrapped> wrap,
                                          Func<TWrapped, TBacking> unwrap)
        {
            mWrappedItems = new List<TWrapped>();
            mBackingCollection = backingCollection;
            mWrap = wrap;
            mUnwrap = unwrap;
            mBackingCollection.CollectionChanged += _HandleBackingCollectionChanged;

            foreach (var item in backingCollection)
            {
                mWrappedItems.Add(mWrap(item));
            }
        }

        /// <summary>
        ///   Handles the <see cref = "INotifyCollectionChanged.CollectionChanged" /> event of the backing collection.
        ///   Used to sync with the backing collection.
        /// </summary>
        /// <param name = "sender">The source of the event.</param>
        /// <param name = "e">The <see cref = "System.Collections.Specialized.NotifyCollectionChangedEventArgs" /> instance containing the event data.</param>
        private void _HandleBackingCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    int addIndex = e.NewStartingIndex > -1
                                       ? e.NewStartingIndex
                                       : 0;
                    var addedItems = new List<TWrapped>();

                    foreach (TBacking item in e.NewItems)
                    {
                        var wrappedItem = mWrap(item);
                        mWrappedItems.Insert(addIndex, wrappedItem);
                        addedItems.Add(wrappedItem);
                    }

                    _RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Add,
                                                addedItems, e.NewStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Move:
                    var moveItem = mWrappedItems[e.OldStartingIndex];
                    mWrappedItems.RemoveAt(e.OldStartingIndex);
                    mWrappedItems.Insert(e.NewStartingIndex, moveItem);

                    _RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Move, moveItem,
                                                e.NewStartingIndex, e.OldStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var removedItems = mWrappedItems
                        .Where(item => e.OldItems.Contains(mUnwrap(item)))
                        .ToList();

                    foreach (var item in removedItems)
                    {
                        mWrappedItems.Remove(item);
                    }

                    _RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Remove,
                                                removedItems, e.OldStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Replace:
                    int replaceIndex = e.NewStartingIndex > -1
                                           ? e.NewStartingIndex
                                           : 0;
                    var newReplacedItems = new List<TWrapped>();
                    var oldReplacedItems = new List<TWrapped>();

                    foreach (TBacking item in e.NewItems)
                    {
                        oldReplacedItems.Add(mWrappedItems[replaceIndex]);
                        mWrappedItems[replaceIndex] = mWrap(item);
                        newReplacedItems.Add(mWrappedItems[replaceIndex]);

                        replaceIndex++;
                    }

                    _RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(
                                                NotifyCollectionChangedAction.Replace,
                                                newReplacedItems, oldReplacedItems,
                                                e.OldStartingIndex));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    mWrappedItems.Clear();
                    _RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    break;
            }
        }

        /// <summary>
        ///   Raises the <see cref = "INotifyCollectionChanged.CollectionChanged" /> event.
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

        #region Implementation of INotifyCollectionChanged

        /// <summary>
        ///   Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        #region Implementation of ICollection<TWrapped>

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// <param name = "item">The object to add to the <see cref = "T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref = "T:System.NotSupportedException">
        ///   The <see cref = "T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public void Add(TWrapped item)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// <exception cref = "T:System.NotSupportedException">
        ///   The <see cref = "T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public void Clear()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///   Determines whether the <see cref = "T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name = "item">The object to locate in the <see cref = "T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        ///   true if <paramref name = "item" /> is found in the <see cref = "T:System.Collections.Generic.ICollection`1" />; otherwise, false.
        /// </returns>
        public bool Contains(TWrapped item)
        {
            return mWrappedItems.Contains(item);
        }

        /// <summary>
        ///   Copies the values of the collection into an array starting at the given index.
        /// </summary>
        /// <param name = "array">The array.</param>
        /// <param name = "arrayIndex">The index in the array to start copying at.</param>
        public void CopyTo(TWrapped[] array, int arrayIndex)
        {
            mWrappedItems.CopyTo(array, arrayIndex);
        }

        /// <summary>
        ///   Gets the number of elements contained in the <see cref = "T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <returns>
        ///   The number of elements contained in the <see cref = "T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        public int Count
        {
            get { return mWrappedItems.Count; }
        }

        /// <summary>
        ///   Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        ///   A <see cref = "T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<TWrapped> GetEnumerator()
        {
            return mWrappedItems.GetEnumerator();
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
            get { return true; }
        }

        /// <summary>
        ///   Not supported.
        /// </summary>
        /// <param name = "item">The object to remove from the <see cref = "T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>
        ///   true if <paramref name = "item" /> was successfully removed from the <see cref = "T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name = "item" /> is not found in the original <see cref = "T:System.Collections.Generic.ICollection`1" />.
        /// </returns>
        /// <exception cref = "T:System.NotSupportedException">
        ///   The <see cref = "T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </exception>
        public bool Remove(TWrapped item)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}