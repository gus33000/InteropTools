// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using Intense.UI;
using Windows.UI.Xaml;

namespace Intense.Presentation
{
    /// <summary>
    /// Provides commands for modifying the app's theme at runtime.
    /// </summary>
    [Obsolete("Use Intense.Presentation.AppearanceCommands")]
    public class ThemeCommands
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeCommands"/> class.
        /// </summary>
        public ThemeCommands()
        {
            SetDarkThemeCommand = new RelayCommand(o => ThemeManager.Theme = ApplicationTheme.Dark, o => ThemeManager.Theme == ApplicationTheme.Light);
            SetLightThemeCommand = new RelayCommand(o => ThemeManager.Theme = ApplicationTheme.Light, o => ThemeManager.Theme == ApplicationTheme.Dark);
            ToggleThemeCommand = new RelayCommand(o => ThemeManager.Theme = ThemeManager.Theme == ApplicationTheme.Dark ? ApplicationTheme.Light : ApplicationTheme.Dark);

            ThemeManager.ThemeChanged += new WeakEventHandler<ThemeCommands, object, object, EventArgs>(this)
            {
                Handle = (t, o, e) => t.OnThemeChanged(o, e),
                Detach = (h, m) => ThemeManager.ThemeChanged -= h.OnEvent
            }.OnEvent;
        }

        /// <summary>
        /// The command for setting the dark theme.
        /// </summary>
        public Command SetDarkThemeCommand { get; }

        /// <summary>
        /// The command for setting the light theme.
        /// </summary>
        public Command SetLightThemeCommand { get; }

        /// <summary>
        /// The command for toggling between the light and dark theme.
        /// </summary>
        public Command ToggleThemeCommand { get; }

        private void OnThemeChanged(object o, EventArgs e)
        {
            SetDarkThemeCommand.OnCanExecuteChanged();
            SetLightThemeCommand.OnCanExecuteChanged();
        }
    }
}