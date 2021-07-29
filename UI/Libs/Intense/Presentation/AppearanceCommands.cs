using Intense.UI;
using Intense.UI.Converters;
using System;
using Windows.UI.Xaml;

namespace Intense.Presentation
{
    /// <summary>
    /// Provides commands for modifying the app's appearance at runtime.
    /// </summary>
    public class AppearanceCommands
        : IAppearanceManagerEventSink
    {
        private readonly ColorToObjectConverter converter = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AppearanceCommands"/> class.
        /// </summary>
        public AppearanceCommands()
        {
            AppearanceManager manager = AppearanceManager.GetForCurrentView();

            SetAccentColorCommand = new RelayCommand(o => AppearanceManager.AccentColor = converter.ConvertBack(o));
            SetDarkThemeCommand = new RelayCommand(o => manager.Theme = ApplicationTheme.Dark, o => manager.Theme == ApplicationTheme.Light);
            SetLightThemeCommand = new RelayCommand(o => manager.Theme = ApplicationTheme.Light, o => manager.Theme == ApplicationTheme.Dark);
            ToggleThemeCommand = new RelayCommand(o => manager.Theme = manager.Theme == ApplicationTheme.Dark ? ApplicationTheme.Light : ApplicationTheme.Dark);

            manager.RegisterEventSink(this);
        }

        void IAppearanceManagerEventSink.OnAccentColorChanged(object source, EventArgs e)
        {
        }

        void IAppearanceManagerEventSink.OnSystemAccentColorChanged(object source, EventArgs e)
        {
        }

        void IAppearanceManagerEventSink.OnThemeChanged(object source, EventArgs e)
        {
            SetDarkThemeCommand.OnCanExecuteChanged();
            SetLightThemeCommand.OnCanExecuteChanged();
        }

        /// <summary>
        /// The command for setting an accent color.
        /// </summary>
        /// <remarks>The command parameter is used to specify the actual color value.</remarks>
        public Command SetAccentColorCommand { get; }
        /// <summary>
        /// The command for setting the dark theme in the current view.
        /// </summary>
        public Command SetDarkThemeCommand { get; }
        /// <summary>
        /// The command for setting the light theme in the current view.
        /// </summary>
        public Command SetLightThemeCommand { get; }
        /// <summary>
        /// The command for toggling between the light and dark theme in the current view.
        /// </summary>
        public Command ToggleThemeCommand { get; }
    }
}
