using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Intense.UI
{
    /// <summary>
    /// Manages the app's theme.
    /// </summary>
    [Obsolete("Use Intense.UI.AppearanceManager")]
    public static class ThemeManager
    {
        /// <summary>
        /// Occurs when the theme has changed.
        /// </summary>
        public static event EventHandler ThemeChanged;

        private static FrameworkElement GetRoot()
        {
            return Window.Current.Content.GetAncestorsAndSelf().OfType<FrameworkElement>().Last();
        }

        private static void OnThemeChanged()
        {
            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Gets or sets the current theme.
        /// </summary>
        public static ApplicationTheme Theme
        {
            get
            {
                var root = GetRoot();
                if (root.RequestedTheme == ElementTheme.Default) {
                    return Application.Current.RequestedTheme;
                }
                if (root.RequestedTheme == ElementTheme.Dark) {
                    return ApplicationTheme.Dark;
                }
                return ApplicationTheme.Light;
            }
            set
            {
                var oldTheme = Theme;
                var elementTheme = ElementTheme.Default;

                if (Application.Current.RequestedTheme != value) {
                    elementTheme = ElementTheme.Light;
                    if (value == ApplicationTheme.Dark) {
                        elementTheme = ElementTheme.Dark;
                    }
                }
                var root = GetRoot();
                root.RequestedTheme = elementTheme;

                var newTheme = Theme;

                if (oldTheme != newTheme) {
                    // raise Theme changed event
                    OnThemeChanged();
                }
            }
        }
    }
}
