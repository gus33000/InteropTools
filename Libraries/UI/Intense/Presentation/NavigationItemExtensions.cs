// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System.Collections.Generic;

namespace Intense.Presentation
{
    /// <summary>
    /// Extension helpers for the <see cref="NavigationItem"/> class.
    /// </summary>
    public static class NavigationItemExtensions
    {
        private static readonly NavigationItemHierarchyService service = new();

        /// <summary>
        /// Returns a collection of ancestors of specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<NavigationItem> GetAncestors(this NavigationItem item) => service.GetAncestors(item);

        /// <summary>
        /// Returns a collection of items that contain specified item, and the ancestors of specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<NavigationItem> GetAncestorsAndSelf(this NavigationItem item) =>
            service.GetAncestorsAndSelf(item);

        /// <summary>
        /// Retrieves the descendant child items of specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<NavigationItem> GetDescendants(this NavigationItem item) =>
            service.GetDescendants(item);

        /// <summary>
        /// Returns a collection of items that contain the specified item and all descendant items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<NavigationItem> GetDescendantsAndSelf(this NavigationItem item) =>
            service.GetDescendantsAndSelf(item);

        /// <summary>
        /// Returns a collection of the siblings after specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<NavigationItem> GetObjectsAfterSelf(this NavigationItem item) =>
            service.GetObjectsAfterSelf(item);

        /// <summary>
        /// Returns a collection of the siblings before specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<NavigationItem> GetObjectsBeforeSelf(this NavigationItem item) =>
            service.GetObjectsBeforeSelf(item);

        /// <summary>
        /// Determines whether the item has grandchildren.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool HasGrandchildren(this NavigationItem item) => service.HasGrandchildren(item);

        /// <summary>
        /// Determines whether the item is a leaf item, meaning it has no children.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsLeaf(this NavigationItem item) => service.IsLeaf(item);

        /// <summary>
        /// Determines whether specified item is the root item, that is it has no parent.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsRoot(this NavigationItem item) => service.IsRoot(item);
    }
}
