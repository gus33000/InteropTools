// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;

namespace Intense.UI
{
    /// <summary>
    /// Extension methods for <see cref="AppearanceManager"/>.
    /// </summary>
    public static class AppearanceManagerExtensions
    {
        /// <summary>
        /// Registers an event sink for <see cref="AppearanceManager"/> to weakly handle its events.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="eventSink"></param>
        public static void RegisterEventSink(this AppearanceManager manager, IAppearanceManagerEventSink eventSink)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (eventSink == null)
            {
                throw new ArgumentNullException(nameof(eventSink));
            }

            AppearanceManager.AccentColorChanged +=
                new WeakEventHandler<IAppearanceManagerEventSink, object, object, EventArgs>(eventSink)
                {
                    Handle = (t, o, e) => t.OnAccentColorChanged(o, e),
                    Detach = (h, m) => AppearanceManager.AccentColorChanged -= h.OnEvent
                }.OnEvent;

            AppearanceManager.SystemAccentColorChanged +=
                new WeakEventHandler<IAppearanceManagerEventSink, object, object, EventArgs>(eventSink)
                {
                    Handle = (t, o, e) => t.OnSystemAccentColorChanged(o, e),
                    Detach = (h, m) => AppearanceManager.SystemAccentColorChanged -= h.OnEvent
                }.OnEvent;

            manager.ThemeChanged +=
                new WeakEventHandler<IAppearanceManagerEventSink, AppearanceManager, object, EventArgs>(eventSink)
                {
                    Handle = (t, o, e) => t.OnThemeChanged(o, e), Detach = (h, m) => m.ThemeChanged -= h.OnEvent
                }.OnEvent;
        }
    }
}
