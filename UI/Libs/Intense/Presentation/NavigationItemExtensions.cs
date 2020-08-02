using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intense.Presentation
{
    /// <summary>
    /// Extension helpers for the <see cref="NavigationItem"/> class.
    /// </summary>
    public static class NavigationItemExtensions
    {
        private static NavigationItemHierarchyService service = new NavigationItemHierarchyService();

        /// <summary>
        /// Returns a collection of ancestors of specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<NavigationItem> GetAncestors(this NavigationItem item)
        {
            return service.GetAncestors(item);
        }

        /// <summary>
        /// Returns a collection of items that contain specified item, and the ancestors of specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<NavigationItem> GetAncestorsAndSelf(this NavigationItem item)
        {
            return service.GetAncestorsAndSelf(item);
        }

        /// <summary>
        /// Retrieves the descendant child items of specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<NavigationItem> GetDescendants(this NavigationItem item)
        {
            return service.GetDescendants(item);
        }

        /// <summary>
        /// Returns a collection of items that contain the specified item and all descendant items.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<NavigationItem> GetDescendantsAndSelf(this NavigationItem item)
        {
            return service.GetDescendantsAndSelf(item);
        }

        /// <summary>
        /// Returns a collection of the siblings before specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<NavigationItem> GetObjectsBeforeSelf(this NavigationItem item)
        {
            return service.GetObjectsBeforeSelf(item);
        }

        /// <summary>
        /// Returns a collection of the siblings after specified item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<NavigationItem> GetObjectsAfterSelf(this NavigationItem item)
        {
            return service.GetObjectsAfterSelf(item);
        }

        /// <summary>
        /// Determines whether specified item is the root item, that is it has no parent.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsRoot(this NavigationItem item)
        {
            return service.IsRoot(item);
        }

        /// <summary>
        /// Determines whether the item is a leaf item, meaning it has no children.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsLeaf(this NavigationItem item)
        {
            return service.IsLeaf(item);
        }

        /// <summary>
        /// Determines whether the item has grandchildren.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool HasGrandchildren(this NavigationItem item)
        {
            return service.HasGrandchildren(item);
        }
    }
}
