using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Intense.UI
{
    /// <summary>
    /// Extension methods for <see cref="Window"/>.
    /// </summary>
    public static class WindowExtensions
    {
        /// <summary>
        /// Registers an event sink for <see cref="Window"/> to weakly handle its events.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="eventSink"></param>
        public static void RegisterEventSink(this Window window, IWindowEventSink eventSink)
        {
            if (window == null) {
                throw new ArgumentNullException(nameof(window));
            }
            if (eventSink == null) {
                throw new ArgumentNullException(nameof(eventSink));
            }

            window.Activated += new WeakEventHandler<IWindowEventSink, Window, object, WindowActivatedEventArgs>(eventSink) {
                Handle = (t, o, e) => t.OnActivated(o, e),
                Detach = (h, w) => w.Activated -= h.OnEvent
            }.OnEvent;
            window.Closed += new WeakEventHandler<IWindowEventSink, Window, object, CoreWindowEventArgs>(eventSink) {
                Handle = (t, o, e) => t.OnClosed(o, e),
                Detach = (h, w) => w.Closed -= h.OnEvent
            }.OnEvent;
            window.SizeChanged += new WeakEventHandler<IWindowEventSink, Window, object, WindowSizeChangedEventArgs>(eventSink) {
                Handle = (t, o, e) => t.OnSizeChanged(o, e),
                Detach = (h, w) => w.SizeChanged -= h.OnEvent
            }.OnEvent;
            window.VisibilityChanged += new WeakEventHandler<IWindowEventSink, Window, object, VisibilityChangedEventArgs>(eventSink) {
                Handle = (t, o, e) => t.OnVisibilityChanged(o, e),
                Detach = (h, w) => w.VisibilityChanged -= h.OnEvent
            }.OnEvent;
        }
    }
}
