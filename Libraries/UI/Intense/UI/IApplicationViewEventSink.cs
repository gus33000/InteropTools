// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Windows.UI.ViewManagement;

namespace Intense.UI
{
    /// <summary>
    /// The <see cref="ApplicationView"/> event sink.
    /// </summary>
    public interface IApplicationViewEventSink
    {
        /// <summary>
        /// Occurs when the window is removed from the list of recently used apps, or if the user executes a close gesture on it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnConsolidated(ApplicationView sender, ApplicationViewConsolidatedEventArgs args);

        /// <summary>
        /// This event is raised when the value of VisibleBounds changes, typically as a result of the status bar, app bar, or other chrome being shown or hidden.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void OnVisibleBoundsChanged(ApplicationView sender, object args);
    }
}
