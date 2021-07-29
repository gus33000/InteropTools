using System;

namespace Intense.UI
{
    /// <summary>
    /// The <see cref="AppearanceManager"/> event sink.
    /// </summary>
    public interface IAppearanceManagerEventSink
    {
        /// <summary>
        /// Occurs when the accent color has changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void OnAccentColorChanged(object source, EventArgs e);

        /// <summary>
        /// Occurs when the system accent color has changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void OnSystemAccentColorChanged(object source, EventArgs e);

        /// <summary>
        /// Occurs when the theme has changed.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        void OnThemeChanged(object source, EventArgs e);
    }
}