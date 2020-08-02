using Intense.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
            this.SetDarkThemeCommand = new RelayCommand(o => ThemeManager.Theme = ApplicationTheme.Dark, o => ThemeManager.Theme == ApplicationTheme.Light);
            this.SetLightThemeCommand = new RelayCommand(o => ThemeManager.Theme = ApplicationTheme.Light, o => ThemeManager.Theme == ApplicationTheme.Dark);
            this.ToggleThemeCommand = new RelayCommand(o => ThemeManager.Theme = ThemeManager.Theme == ApplicationTheme.Dark ? ApplicationTheme.Light : ApplicationTheme.Dark);

            ThemeManager.ThemeChanged += new WeakEventHandler<ThemeCommands, object, object, EventArgs>(this) {
                Handle = (t, o, e) => t.OnThemeChanged(o, e),
                Detach = (h, m) => ThemeManager.ThemeChanged -= h.OnEvent
            }.OnEvent;
        }

        private void OnThemeChanged(object o, EventArgs e)
        {
            this.SetDarkThemeCommand.OnCanExecuteChanged();
            this.SetLightThemeCommand.OnCanExecuteChanged();
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
    }
}
