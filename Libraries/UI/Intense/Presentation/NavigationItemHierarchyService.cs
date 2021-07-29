using System;
using System.Collections.Generic;

namespace Intense.Presentation
{
    /// <summary>
    /// Provide navigation item hierarchy services.
    /// </summary>
    public class NavigationItemHierarchyService
        : IHierarchyService<NavigationItem>
    {
        /// <summary>
        /// Retrieves the children of specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public IEnumerable<NavigationItem> GetChildren(NavigationItem o)
        {
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }
            return o.Items;
        }

        /// <summary>
        /// Retrieves the parent of specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public NavigationItem GetParent(NavigationItem o)
        {
            if (o == null)
            {
                throw new ArgumentNullException(nameof(o));
            }
            return o.Parent;
        }
    }
}