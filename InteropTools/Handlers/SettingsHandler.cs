using Intense.UI;
using InteropTools.Presentation;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace InteropTools.Handlers
{
    public class SettingsHandler
    {
        private ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;

        public bool useMDL2
        {
            get
            {
                return GetValue<bool>("useMDL2");
            }
            set
            {
                SetValue("useMDL2", value);
            }
        }

        public bool EULAAccepted
        {
            get
            {
                return GetValue<bool>("EULAAccepted");
            }
            set
            {
                SetValue("EULAAccepted", value);
            }
        }

        public bool useSystemAccentColor
        {
            get
            {
                return GetValue<bool>("useSystemAccentColor");
            }
            set
            {
                SetValue("useSystemAccentColor", value);

                if (value)
                {
                    AppearanceManager.AccentColor = null;
                }
                else if (selectedBrush != null)
                {
                    AppearanceManager.AccentColor = selectedBrush.Color;
                }
            }
        }

        public bool requireAuthAtStartUp
        {
            get
            {
                return GetValue<bool>("requireAuthAtStartUp");
            }
            set
            {
                SetValue("requireAuthAtStartUp", value);
            }
        }

        public bool useTimeStamps
        {
            get
            {
                return GetValue<bool>("useTimeStamps");
            }
            set
            {
                SetValue("useTimeStamps", value);
            }
        }

        public SolidColorBrush selectedBrush
        {
            get
            {
                var defaultbrush = new SolidColorBrush(AppearanceManager.SystemAccentColor).Color.ToString();
                var result = GetValue<string>("selectedBrush");

                int argb = Int32.Parse(result.Replace("#", ""), NumberStyles.HexNumber);
                Color color = Color.FromArgb((byte)((argb & -16777216) >> 0x18),
                                             (byte)((argb & 0xff0000) >> 0x10),
                                             (byte)((argb & 0xff00) >> 8),
                                             (byte)(argb & 0xff));

                return new SolidColorBrush(color);
            }
            set
            {
                if (!useSystemAccentColor)
                {
                    AppearanceManager.AccentColor = value.Color;
                }
                var brush = value.Color.ToString();
                SetValue("selectedBrush", brush);
            }
        }

        public DisplayableTheme selectedTheme
        {
            get
            {
                var result = GetValue<string>("selectedTheme");
                if (result != "")
                {
                    if (result == "Dark")
                    {
                        return Themes.ElementAt(1);
                    }
                    else if (result == "Light")
                    {
                        return Themes.ElementAt(2);
                    }
                }
                return Themes.First();
            }
            set
            {
                SetValue("selectedTheme", value.Theme.HasValue ? value.Theme.Value.ToString() : "");
                if (value.Theme.HasValue)
                {
                    AppearanceManager.GetForCurrentView().Theme = value.Theme.Value;
                }
                else
                {
                    AppearanceManager.GetForCurrentView().Theme = Application.Current.RequestedTheme;
                }
            }
        }

        /// <summary>
        /// Gets the collection of themes.
        /// </summary>
        public IReadOnlyList<DisplayableTheme> Themes = ImmutableList.Create(
                            new DisplayableTheme("Default", null),
                            new DisplayableTheme("Dark", ApplicationTheme.Dark),
                            new DisplayableTheme("Light", ApplicationTheme.Light));

        public SettingsHandler()
        {
            InitializeSetting("useMDL2", false);
            InitializeSetting("EULAAccepted", false);
            InitializeSetting("useSystemAccentColor", true);
            InitializeSetting("requireAuthAtStartUp", false);
            InitializeSetting("useTimeStamps", false);
            var defaultbrush = new SolidColorBrush(AppearanceManager.SystemAccentColor).Color.ToString();
            InitializeSetting("selectedBrush", defaultbrush);
        }

        public void Initialize()
        {
            if (useSystemAccentColor)
            {
                AppearanceManager.AccentColor = null;
            }
            else if (selectedBrush != null)
            {
                AppearanceManager.AccentColor = selectedBrush.Color;
            }

            if (selectedTheme.Theme.HasValue)
            {
                AppearanceManager.GetForCurrentView().Theme = selectedTheme.Theme.Value;
            }
            else
            {
                AppearanceManager.GetForCurrentView().Theme = Application.Current.RequestedTheme;
            }
        }

        private void InitializeSetting<T>(string settingname, T defaultvalue)
        {
            if ((localSettings.Values[settingname] == null) || (localSettings.Values[settingname].GetType() != typeof(T)))
            {
                localSettings.Values[settingname] = defaultvalue;
            }
        }

        private T GetValue<T>(string settingname)
        {
            return (T)localSettings.Values[settingname];
        }

        private void SetValue<T>(string settingname, T value)
        {
            localSettings.Values[settingname] = value;
        }
    }
}
