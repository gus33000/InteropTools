using System.Collections.Generic;

namespace Intense
{
    /// <summary>
    /// Defines a uniform contract for hierarchical data structures.
    /// </summary>
    public interface IHierarchyService<T>
    {
        /// <summary>
        /// Retrieves the children of specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        IEnumerable<T> GetChildren(T o);

        /// <summary>
        /// Retrieves the parent of specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        T GetParent(T o);
    }
}