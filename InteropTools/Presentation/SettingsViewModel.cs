using Intense.Presentation;
using Intense.UI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Security.Credentials;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace InteropTools.Presentation
{
    /// <summary>
    /// A sample settings view model.
    /// </summary>
    public class SettingsViewModel
        : NotifyPropertyChanged
    {
        /// <summary>
        /// Gets the brushes.
        /// </summary>
        public ObservableRangeCollection<SolidColorBrush> Brushes = new();

        private bool requireAuthAtStartUp;
        private SolidColorBrush selectedBrush;
        private DisplayableTheme selectedTheme;
        private bool useMDL2;
        private bool useSystemAccentColor;
        private bool useTimeStamps;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/>.
        /// </summary>
        public SettingsViewModel()
        {
            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            if ((localSettings.Values["useSystemAccentColor"] == null) || (localSettings.Values["useSystemAccentColor"].GetType() != typeof(bool)))
            {
                localSettings.Values["useSystemAccentColor"] = true;
            }

            useSystemAccentColor = (bool)localSettings.Values["useSystemAccentColor"];

            if ((localSettings.Values["requireAuthAtStartUp"] == null) || (localSettings.Values["requireAuthAtStartUp"].GetType() != typeof(bool)))
            {
                localSettings.Values["requireAuthAtStartUp"] = true;
            }

            requireAuthAtStartUp = (bool)localSettings.Values["requireAuthAtStartUp"];

            if ((localSettings.Values["useMDL2"] == null) || (localSettings.Values["useMDL2"].GetType() != typeof(bool)))
            {
                localSettings.Values["useMDL2"] = false;
            }

            useMDL2 = (bool)localSettings.Values["useMDL2"];

            if ((localSettings.Values["useTimeStamps"] == null) || (localSettings.Values["useTimeStamps"].GetType() != typeof(bool)))
            {
                localSettings.Values["useTimeStamps"] = false;
            }

            useTimeStamps = (bool)localSettings.Values["useTimeStamps"];

            if ((localSettings.Values["selectedBrush"] == null) || localSettings.Values["selectedBrush"].GetType() != typeof(string))
            {
                localSettings.Values["selectedBrush"] = null;
                selectedBrush = new SolidColorBrush(AppearanceManager.SystemAccentColor);
            }
            else
            {
                int argb = int.Parse(((string)localSettings.Values["selectedBrush"]).Replace("#", ""), NumberStyles.HexNumber);
                Color color = Color.FromArgb((byte)((argb & -16777216) >> 0x18),
                                             (byte)((argb & 0xff0000) >> 0x10),
                                             (byte)((argb & 0xff00) >> 8),
                                             (byte)(argb & 0xff));
                SelectedBrush = new SolidColorBrush(color);
            }

            Brushes.AddRange(AccentColors.Windows10.Select(c => new SolidColorBrush(c)));
            Themes = ImmutableList.Create(
                            new DisplayableTheme("Default", null),
                            new DisplayableTheme("Dark", ApplicationTheme.Dark),
                            new DisplayableTheme("Light", ApplicationTheme.Light));
            // ensure viewmodel state reflects actual appearance
            AppearanceManager manager = AppearanceManager.GetForCurrentView();

            if ((localSettings.Values["selectedTheme"] == null) || (localSettings.Values["selectedTheme"].GetType() != typeof(string)))
            {
                localSettings.Values["selectedTheme"] = null;
                selectedTheme = Themes[0];
            }
            else
            {
                selectedTheme = Themes.FirstOrDefault(t => t.Theme.ToString() == (string)localSettings.Values["selectedTheme"]);
            }

            if (selectedTheme != null && manager != null)
            {
                try
                {
                    manager.Theme = selectedTheme.Theme ?? Application.Current.RequestedTheme;
                }
                catch
                {
                }
            }

            if (AppearanceManager.AccentColor == null)
            {
                useSystemAccentColor = true;
            }
            else
            {
                selectedBrush = Brushes.FirstOrDefault(b => b.Color == AppearanceManager.AccentColor);
            }

            localSettings.Values["useSystemAccentColor"] = useSystemAccentColor;
            localSettings.Values["requireAuthAtStartUp"] = requireAuthAtStartUp;
            localSettings.Values["selectedBrush"] = selectedBrush?.Color.ToString();
            localSettings.Values["useMDL2"] = useMDL2;
            localSettings.Values["useTimeStamps"] = useTimeStamps;
        }

        /// <summary>
        /// Gets or sets the selected brush.
        /// </summary>
        public SolidColorBrush SelectedBrush
        {
            get => selectedBrush;

            set
            {
                if (Set(ref selectedBrush, value))
                {
                    if (!useSystemAccentColor && value != null)
                    {
                        AppearanceManager.AccentColor = value.Color;
                        ApplicationData applicationData = ApplicationData.Current;
                        ApplicationDataContainer localSettings = applicationData.LocalSettings;
                        localSettings.Values["selectedBrush"] = value.Color.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected theme.
        /// </summary>
        public DisplayableTheme SelectedTheme
        {
            get => selectedTheme;

            set
            {
                ApplicationData applicationData = ApplicationData.Current;
                ApplicationDataContainer localSettings = applicationData.LocalSettings;
                localSettings.Values["selectedTheme"] = value?.Theme.ToString();

                if (Set(ref selectedTheme, value) && value?.Theme.HasValue == true)
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
        public IReadOnlyList<DisplayableTheme> Themes { get; }

        public bool UseAuthAtStartUp
        {
            get => requireAuthAtStartUp;

            set => RunInThreadPool(async () =>
                   {
                       await RunInUiThread(async () =>
                       {
                           if (await AskCreds())
                           {
                               if (Set(ref requireAuthAtStartUp, value))
                               {
                                   ApplicationData applicationData = ApplicationData.Current;
                                   ApplicationDataContainer localSettings = applicationData.LocalSettings;
                                   localSettings.Values["requireAuthAtStartUp"] = value;
                               }
                           }
                       });
                   });
        }

        public bool UseMDL2
        {
            get => useMDL2;

            set
            {
                if (Set(ref useMDL2, value))
                {
                    ApplicationData applicationData = ApplicationData.Current;
                    ApplicationDataContainer localSettings = applicationData.LocalSettings;
                    localSettings.Values["useMDL2"] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use the system accent color.
        /// </summary>
        public bool UseSystemAccentColor
        {
            get => useSystemAccentColor;

            set
            {
                if (Set(ref useSystemAccentColor, value))
                {
                    ApplicationData applicationData = ApplicationData.Current;
                    ApplicationDataContainer localSettings = applicationData.LocalSettings;
                    localSettings.Values["useSystemAccentColor"] = value;

                    if (value)
                    {
                        AppearanceManager.AccentColor = null;
                    }
                    else
                        if (SelectedBrush != null)
                    {
                        AppearanceManager.AccentColor = SelectedBrush.Color;
                    }
                }
            }
        }

        public bool UseTimeStamps
        {
            get => useTimeStamps;

            set
            {
                if (Set(ref useTimeStamps, value))
                {
                    ApplicationData applicationData = ApplicationData.Current;
                    ApplicationDataContainer localSettings = applicationData.LocalSettings;
                    localSettings.Values["useTimeStamps"] = value;
                }
            }
        }

        private static async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => function());
        }

        private async static Task RunInUiThread(Action function)
        {
            await
            CoreApplication.MainView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => function());
        }

        private async Task<bool> AskCreds()
        {
            bool authorized = false;

            // Do we have capability to provide credentials from the device
            if (await KeyCredentialManager.IsSupportedAsync())
            {
                // Get credentials for current user and app
                KeyCredentialRetrievalResult result = await KeyCredentialManager.OpenAsync("MyAppCredentials");

                if (result.Credential != null)
                {
                    KeyCredentialOperationResult signResult =
                      await
                      result.Credential.RequestSignAsync(CryptographicBuffer.ConvertStringToBinary("LoginAuth",
                                                         BinaryStringEncoding.Utf8));

                    if (signResult.Status == KeyCredentialStatus.Success)
                    {
                        authorized = true;
                    }
                }

                // No previous saved credentials found
                else
                {
                    KeyCredentialRetrievalResult creationResult =
                      await
                      KeyCredentialManager.RequestCreateAsync("MyAppCredentials",
                          KeyCredentialCreationOption.ReplaceExisting);

                    if (creationResult.Status == KeyCredentialStatus.Success)
                    {
                        authorized = true;
                    }
                }
            }
            else
            {
                authorized = true;
            }

            return authorized;
        }
    }
}