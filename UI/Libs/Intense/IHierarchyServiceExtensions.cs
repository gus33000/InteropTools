using System;
using System.Collections.Generic;
using System.Linq;

namespace Intense
{
    /// <summary>
    /// Provides extension methods for <see cref="IHierarchyService{T}"/>
    /// </summary>
    public static class IHierarchyServiceExtensions
    {
        private static void ThrowIfInvalidArgs<T>(IHierarchyService<T> service, T o)
        {
            if (service == null)
            {
                throw new ArgumentNullException(nameof(service));
            }
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }
        }

        /// <summary>
        /// Returns a collection of ancestors of specified object.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAncestors<T>(this IHierarchyService<T> service, T o)
        {
            ThrowIfInvalidArgs(service, o);

            while (true)
            {
                o = service.GetParent(o);
                if (o != null)
                {
                    yield return o;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Returns a collection of objects that contain specified object, and the ancestors of specified object.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetAncestorsAndSelf<T>(this IHierarchyService<T> service, T o)
        {
            ThrowIfInvalidArgs(service, o);

            while (o != null)
            {
                yield return o;

                o = service.GetParent(o);
            }
        }

        /// <summary>
        /// Retrieves the descendant children of specified object.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetDescendants<T>(this IHierarchyService<T> service, T o)
        {
            ThrowIfInvalidArgs(service, o);

            Stack<T> stack = new(service.GetChildren(o).Reverse());
            return GetDescendantsAndSelf(service, stack);
        }

        /// <summary>
        /// Returns a collection of objects that contain the specified object and all descendant objects.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetDescendantsAndSelf<T>(this IHierarchyService<T> service, T o)
        {
            ThrowIfInvalidArgs(service, o);

            Stack<T> stack = new();
            stack.Push(o);
            return GetDescendantsAndSelf(service, stack);
        }

        private static IEnumerable<T> GetDescendantsAndSelf<T>(this IHierarchyService<T> service, Stack<T> stack)
        {
            while (stack.Count > 0)
            {
                T o = stack.Pop();
                yield return o;

                foreach (T child in service.GetChildren(o).Reverse())
                {
                    stack.Push(child);
                }
            }
        }

        /// <summary>
        /// Returns a collection of the siblings before specified object.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetObjectsBeforeSelf<T>(this IHierarchyService<T> service, T o) where T : class
        {
            ThrowIfInvalidArgs(service, o);

            T parent = service.GetParent(o);
            if (parent != null)
            {
                return service.GetChildren(parent).TakeWhile(c => c != o);
            }
            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// Returns a collection of the siblings after specified object.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static IEnumerable<T> GetObjectsAfterSelf<T>(this IHierarchyService<T> service, T o) where T : class
        {
            ThrowIfInvalidArgs(service, o);

            T parent = service.GetParent(o);
            if (parent != null)
            {
                return service.GetChildren(parent).SkipWhile(c => c != o).Skip(1);
            }
            return Enumerable.Empty<T>();
        }

        /// <summary>
        /// Determines whether specified object is the root object.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsRoot<T>(this IHierarchyService<T> service, T o)
        {
            ThrowIfInvalidArgs(service, o);

            return service.GetParent(o) == null;
        }

        /// <summary>
        /// Determines whether the object is a leaf object, meaning it has no children.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool IsLeaf<T>(this IHierarchyService<T> service, T o)
        {
            ThrowIfInvalidArgs(service, o);

            return !service.GetChildren(o).Any();
        }

        /// <summary>
        /// Determines whether the object has grandchildren.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static bool HasGrandchildren<T>(this IHierarchyService<T> service, T o)
        {
            ThrowIfInvalidArgs(service, o);

            return service.GetChildren(o).Any(c => !service.IsLeaf(c));
        }
    }
}
