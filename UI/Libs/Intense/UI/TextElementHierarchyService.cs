using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Documents;

namespace Intense.UI
{
    /// <summary>
    /// Implements a <see cref="IHierarchyService{T}"/> for <see cref="TextElement"/>.
    /// </summary>
    public class TextElementHierarchyService
        : IHierarchyService<TextElement>
    {
        /// <summary>
        /// Retrieves the children of specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>

        public IEnumerable<TextElement> GetChildren(TextElement o)
        {
            if (o is Paragraph paragraph)
            {
                return paragraph.Inlines;
            }
            if (o is Span span)
            {
                return span.Inlines;
            }

            return Enumerable.Empty<TextElement>();
        }

        /// <summary>
        /// Retrieves the parent of specified object.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public TextElement GetParent(TextElement o)
        {
            throw new NotSupportedException();
        }
    }
}
