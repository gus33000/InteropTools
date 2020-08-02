using Intense.UI;
using InteropTools.Handlers;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;

namespace InteropTools.Presentation
{
    /// <summary>
    /// A sample settings view model.
    /// </summary>
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private SettingsHandler settingshandler = new SettingsHandler();

        private DisplayableTheme selectedTheme;
        private SolidColorBrush selectedBrush;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void Notify([CallerMemberName]string propertyName = null)
        {
            OnPropertyChanged(propertyName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/>.
        /// </summary>
        public SettingsViewModel()
        {
            Brushes.AddRange(AccentColors.Windows10.Select(c => new SolidColorBrush(c)));
            selectedTheme = settingshandler.selectedTheme;
            selectedBrush = settingshandler.selectedBrush;
            settingshandler.Initialize();
        }

        /// <summary>
        /// Gets the brushes.
        /// </summary>
        public ObservableRangeCollection<SolidColorBrush> Brushes = new ObservableRangeCollection<SolidColorBrush>();

        /// <summary>
        /// Gets or sets the selected brush.
        /// </summary>
        public SolidColorBrush SelectedBrush
        {
            get
            {
                return selectedBrush;
            }

            set
            {
                if (selectedBrush != value)
                {
                    settingshandler.selectedBrush = value;
                    selectedBrush = value;
                    Notify();
                }
            }
        }

        /// <summary>
        /// Gets the collection of themes.
        /// </summary>
        public IReadOnlyList<DisplayableTheme> Themes => settingshandler.Themes;

        /// <summary>
        /// Gets or sets the selected theme.
        /// </summary>
        public DisplayableTheme SelectedTheme
        {
            get
            {
                return selectedTheme;
            }

            set
            {
                if (selectedTheme != value)
                {
                    settingshandler.selectedTheme = value;
                    selectedTheme = value;
                    Notify();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use the system accent color.
        /// </summary>
        public bool UseSystemAccentColor
        {
            get
            {
                return settingshandler.useSystemAccentColor;
            }

            set
            {
                settingshandler.useSystemAccentColor = value;
                Notify();
            }
        }

        public bool UseAuthAtStartUp
        {
            get
            {
                return settingshandler.requireAuthAtStartUp;
            }

            set
            {
                settingshandler.requireAuthAtStartUp = value;
                Notify();
            }
        }

        public bool UseMDL2
        {
            get
            {
                return settingshandler.useMDL2;
            }

            set
            {
                settingshandler.useMDL2 = value;
                Notify();
            }
        }

        public bool UseTimeStamps
        {
            get
            {
                return settingshandler.useTimeStamps;
            }

            set
            {
                settingshandler.useTimeStamps = value;
                Notify();
            }
        }
    }
}