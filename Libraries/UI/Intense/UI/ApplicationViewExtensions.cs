// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using Windows.UI.ViewManagement;

namespace Intense.UI
{
    /// <summary>
    /// Extension methods for <see cref="ApplicationView"/>.
    /// </summary>
    public static class ApplicationViewExtensions
    {
        /// <summary>
        /// Registers an event sink for <see cref="ApplicationView"/> to weakly handle its events.
        /// </summary>
        /// <param name="appView"></param>
        /// <param name="eventSink"></param>
        public static void RegisterEventSink(this ApplicationView appView, IApplicationViewEventSink eventSink)
        {
            if (appView == null)
            {
                throw new ArgumentNullException(nameof(appView));
            }

            if (eventSink == null)
            {
                throw new ArgumentNullException(nameof(eventSink));
            }

            appView.Consolidated +=
                new WeakEventHandler<IApplicationViewEventSink, ApplicationView, ApplicationView,
                    ApplicationViewConsolidatedEventArgs>(eventSink)
                {
                    Handle = (t, o, e) => t.OnConsolidated(o, e), Detach = (h, v) => v.Consolidated -= h.OnEvent
                }.OnEvent;

            appView.VisibleBoundsChanged +=
                new WeakEventHandler<IApplicationViewEventSink, ApplicationView, ApplicationView, object>(eventSink)
                {
                    Handle = (t, o, e) => t.OnVisibleBoundsChanged(o, e),
                    Detach = (h, v) => v.VisibleBoundsChanged -= h.OnEvent
                }.OnEvent;
        }
    }
}
