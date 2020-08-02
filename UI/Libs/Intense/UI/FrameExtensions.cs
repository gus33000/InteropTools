using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Intense.UI
{
    /// <summary>
    /// Extension methods for <see cref="Frame"/>.
    /// </summary>
    public static class FrameExtensions
    {
        /// <summary>
        /// Registers an event sink for <see cref="Frame"/> to weakly handle its navigation events.
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="eventSink"></param>
        public static void RegisterEventSink(this Frame frame, IFrameNavigationEventSink eventSink)
        {
            if (frame == null) {
                throw new ArgumentNullException(nameof(frame));
            }
            if (eventSink == null) {
                throw new ArgumentNullException(nameof(eventSink));
            }

            frame.Navigated += new WeakEventHandler<IFrameNavigationEventSink, Frame, object, NavigationEventArgs>(eventSink) {
                Handle = (t, o, e) => t.OnNavigated(o, e),
                Detach = (h, f) => f.Navigated -= h.OnEvent
            }.OnEvent;
            frame.Navigating += new WeakEventHandler<IFrameNavigationEventSink, Frame, object, NavigatingCancelEventArgs>(eventSink) {
                Handle = (t, o, e) => t.OnNavigating(o, e),
                Detach = (h, f) => f.Navigating -= h.OnEvent
            }.OnEvent;
            frame.NavigationFailed += new WeakEventHandler<IFrameNavigationEventSink, Frame, object, NavigationFailedEventArgs>(eventSink) {
                Handle = (t, o, e) => t.OnNavigationFailed(o, e),
                Detach = (h, f) => f.NavigationFailed -= h.OnEvent
            }.OnEvent;
            frame.NavigationStopped += new WeakEventHandler<IFrameNavigationEventSink, Frame, object, NavigationEventArgs>(eventSink) {
                Handle = (t, o, e) => t.OnNavigationStopped(o, e),
                Detach = (h, f) => f.NavigationStopped -= h.OnEvent
            }.OnEvent;
        }
    }
}
