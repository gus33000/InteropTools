using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Intense.UI
{
    /// <summary>
    /// The <see cref="Frame"/> event sink for tracking navigation events.
    /// </summary>
    public interface IFrameNavigationEventSink
    {
        /// <summary>
        /// Occurs when the content that is being navigated to has been found and is available from the Content property, although it may not have completed loading.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnNavigated(object sender, NavigationEventArgs e);
        /// <summary>
        /// Occurs when a new navigation is requested.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnNavigating(object sender, NavigatingCancelEventArgs e);
        /// <summary>
        /// Occurs when an error is raised while navigating to the requested content.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e);
        /// <summary>
        /// Occurs when a new navigation is requested while a current navigation is in progress.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnNavigationStopped(object sender, NavigationEventArgs e);
    }
}
