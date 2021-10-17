// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using Intense.Resources;

namespace Intense.Presentation
{
    /// <summary>
    /// Represents an observable collection of navigation items.
    /// </summary>
    public class NavigationItemCollection
        : ObservableCollection<NavigationItem>
    {
        private readonly NavigationItem parent;

        /// <summary>
        /// Initializes a default instance of the <see cref="NavigationItemCollection"/>.
        /// </summary>
        public NavigationItemCollection()
        {
        }

        internal NavigationItemCollection(NavigationItem parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        protected override void ClearItems()
        {
            NavigationItem[] items = Items.ToArray();
            base.ClearItems();

            // clear parent from items
            foreach (NavigationItem item in items)
            {
                item.Parent = null;
            }
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert</param>
        protected override void InsertItem(int index, NavigationItem item)
        {
            VerifyNewItem(item);

            base.InsertItem(index, item);

            item.Parent = parent;
        }

        /// <summary>
        /// Removes the item at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        protected override void RemoveItem(int index)
        {
            NavigationItem oldItem = Items[index];

            base.RemoveItem(index);

            oldItem.Parent = null;
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="item">The new value for the element at the specified index.</param>
        protected override void SetItem(int index, NavigationItem item)
        {
            VerifyNewItem(item);
            NavigationItem oldItem = Items[index];

            base.SetItem(index, item);
            oldItem.Parent = null;
            item.Parent = parent;
        }

        private void VerifyNewItem(NavigationItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.Parent != null)
            {
                throw new InvalidOperationException(ResourceHelper.GetString("ItemAlreadyInCollection"));
            }
        }
    }
}