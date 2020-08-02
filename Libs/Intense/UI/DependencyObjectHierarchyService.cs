using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Intense.UI
{
    /// <summary>
    /// Implements a <see cref="IHierarchyService{T}"/> for <see cref="DependencyObject"/>.
    /// </summary>
    public class DependencyObjectHierarchyService
        : IHierarchyService<DependencyObject>
    {
        /// <summary>
        /// Retrieves the children of specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>

        public IEnumerable<DependencyObject> GetChildren(DependencyObject o)
        {
            var count = VisualTreeHelper.GetChildrenCount(o);
            for (var i = 0; i < count; i++) {
                yield return VisualTreeHelper.GetChild(o, i);
            }
        }

        /// <summary>
        /// Retrieves the parent of specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public DependencyObject GetParent(DependencyObject o)
        {
            return VisualTreeHelper.GetParent(o);
        }
    }
}
