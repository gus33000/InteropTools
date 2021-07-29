using System.Collections.Generic;
using Windows.UI.Xaml;

namespace Intense.UI
{
    /// <summary>
    /// Provides extension methods for <see cref="DependencyObject"/>.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        private static readonly DependencyObjectHierarchyService service = new();

        /// <summary>
        /// Returns a collection of ancestors of specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject o)
        {
            return service.GetAncestors(o);
        }

        /// <summary>
        /// Returns a collection of objects that contain specified object, and the ancestors of specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetAncestorsAndSelf(this DependencyObject o)
        {
            return service.GetAncestorsAndSelf(o);
        }

        /// <summary>
        /// Retrieves the descendant children of specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject o)
        {
            return service.GetDescendants(o);
        }

        /// <summary>
        /// Returns a collection of objects that contain the specified object and all descendant objects.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetDescendantsAndSelf(this DependencyObject o)
        {
            return service.GetDescendantsAndSelf(o);
        }

        /// <summary>
        /// Returns a collection of the siblings after specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetObjectsAfterSelf(this DependencyObject o)
        {
            return service.GetObjectsAfterSelf(o);
        }

        /// <summary>
        /// Returns a collection of the siblings before specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> GetObjectsBeforeSelf(this DependencyObject o)
        {
            return service.GetObjectsBeforeSelf(o);
        }

        /// <summary>
        /// Determines whether the object has grandchildren.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool HasGrandchildren(this DependencyObject o)
        {
            return service.HasGrandchildren(o);
        }

        /// <summary>
        /// Determines whether the object is a leaf object, meaning it has no children.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsLeaf(this DependencyObject o)
        {
            return service.IsLeaf(o);
        }

        /// <summary>
        /// Determines whether specified object is the root object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsRoot(this DependencyObject o)
        {
            return service.IsRoot(o);
        }
    }
}