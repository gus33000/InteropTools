using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Intense.UI
{
    /// <summary>
    /// The <see cref="Window"/> event sink.
    /// </summary>
    public interface IWindowEventSink
    {
        /// <summary>
        /// Occurs when the window has successfully been activated.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnActivated(object sender, WindowActivatedEventArgs e);

        /// <summary>
        /// Occurs when the window has closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnClosed(object sender, CoreWindowEventArgs e);

        /// <summary>
        /// Occurs when the app window has first rendered or has changed its rendering size.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnSizeChanged(object sender, WindowSizeChangedEventArgs e);

        /// <summary>
        /// Occurs when the value of the Visible property changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnVisibilityChanged(object sender, VisibilityChangedEventArgs e);
    }
}