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
            DefaultStyleKey = typeof(ContentFrame);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the page title should be visible.
        /// </summary>
        public Visibility PageTitleVisibility
        {
            get => (Visibility)GetValue(PageTitleVisibilityProperty);
            set => SetValue(PageTitleVisibilityProperty, value);
        }
    }
}