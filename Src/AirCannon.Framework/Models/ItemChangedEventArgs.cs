using System;

namespace AirCannon.Framework.Models
{
    /// <summary>
    ///   <see cref = "EventArgs" /> for when an item of a collection is changed.
    /// </summary>
    /// <typeparam name = "TItem">The type of the item.</typeparam>
    public class ItemChangedEventArgs<TItem> : EventArgs
    {
        /// <summary>
        ///   The item that was changed.
        /// </summary>
        public readonly TItem Item;

        /// <summary>
        ///   The property that was changed on the <see cref = "Item" />.
        /// </summary>
        public readonly string PropertyName;

        /// <summary>
        ///   Initializes a new instance of the <see cref = "ItemChangedEventArgs&lt;TItem&gt;" /> class.
        /// </summary>
        /// <param name = "item">The item that was changed.</param>
        /// <param name = "propertyName">The property that was changed on the item.</param>
        public ItemChangedEventArgs(TItem item, string propertyName)
        {
            PropertyName = propertyName;
            Item = item;
        }
    }
}