using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Intense.UI.Controls
{
    /// <summary>
    /// Decorates a <see cref="Frame"/> with additional information.
    /// </summary>
    public class ContentFrame
        : Frame
    {
        /// <summary>
        /// Identifies the PageTitleVisibility property.
        /// </summary>
        public static readonly DependencyProperty PageTitleVisibilityProperty = DependencyProperty.Register("PageTitleVisibility", typeof(Visibility), typeof(ContentFrame), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentFrame"/>.
        /// </summary>
        public ContentFrame()
        {
            this.DefaultStyleKey = typeof(ContentFrame);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the page title should be visible.
        /// </summary>
        public Visibility PageTitleVisibility
        {
            get { return (Visibility)GetValue(PageTitleVisibilityProperty); }
            set { SetValue(PageTitleVisibilityProperty, value); }
        }
    }
}
