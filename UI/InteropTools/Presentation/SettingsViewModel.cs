using Intense.Presentation;
using Intense.UI;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
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
		private DisplayableTheme selectedTheme;
        private SolidColorBrush selectedBrush;
        private bool useSystemAccentColor;
		private bool requireAuthAtStartUp;
        private bool useMDL2;
        private bool useTimeStamps;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/>.
        /// </summary>
        public SettingsViewModel()
		{
			var applicationData = ApplicationData.Current;
			var localSettings = applicationData.LocalSettings;

			if ((localSettings.Values["useSystemAccentColor"] == null) || (localSettings.Values["useSystemAccentColor"].GetType() != typeof(bool)))
			{
				localSettings.Values["useSystemAccentColor"] = true;
			}

			this.useSystemAccentColor = (bool)localSettings.Values["useSystemAccentColor"];

			if ((localSettings.Values["requireAuthAtStartUp"] == null) || (localSettings.Values["requireAuthAtStartUp"].GetType() != typeof(bool)))
			{
				localSettings.Values["requireAuthAtStartUp"] = true;
			}

			this.requireAuthAtStartUp = (bool)localSettings.Values["requireAuthAtStartUp"];

            if ((localSettings.Values["useMDL2"] == null) || (localSettings.Values["useMDL2"].GetType() != typeof(bool)))
            {
                localSettings.Values["useMDL2"] = false;
            }

            this.useMDL2 = (bool)localSettings.Values["useMDL2"];

            if ((localSettings.Values["useTimeStamps"] == null) || (localSettings.Values["useTimeStamps"].GetType() != typeof(bool)))
            {
                localSettings.Values["useTimeStamps"] = false;
            }

            this.useTimeStamps = (bool)localSettings.Values["useTimeStamps"];

            if ((localSettings.Values["selectedBrush"] == null) || localSettings.Values["selectedBrush"].GetType() != typeof(string))
			{
				localSettings.Values["selectedBrush"] = null;
                this.selectedBrush = new SolidColorBrush(AppearanceManager.SystemAccentColor);
            }

			else
			{
				int argb = Int32.Parse(((string)localSettings.Values["selectedBrush"]).Replace("#", ""), NumberStyles.HexNumber);
				Color color = Color.FromArgb((byte)((argb & -16777216) >> 0x18),
				                             (byte)((argb & 0xff0000) >> 0x10),
				                             (byte)((argb & 0xff00) >> 8),
				                             (byte)(argb & 0xff));
				this.SelectedBrush = new SolidColorBrush(color);
			}

			this.Brushes.AddRange(AccentColors.Windows10.Select(c => new SolidColorBrush(c)));
			this.Themes = ImmutableList.Create(
			                new DisplayableTheme("Default", null),
			                new DisplayableTheme("Dark", ApplicationTheme.Dark),
			                new DisplayableTheme("Light", ApplicationTheme.Light));
			// ensure viewmodel state reflects actual appearance
			var manager = AppearanceManager.GetForCurrentView();

			if ((localSettings.Values["selectedTheme"] == null) || (localSettings.Values["selectedTheme"].GetType() != typeof(String)))
			{
				localSettings.Values["selectedTheme"] = null;
				this.selectedTheme = this.Themes.First();
			}

			else
			{
				this.selectedTheme = this.Themes.FirstOrDefault(t => t.Theme.ToString() == (string)localSettings.Values["selectedTheme"]);
			}

			if (this.selectedTheme != null && manager != null)
			{
				try
				{
					if (this.selectedTheme.Theme.HasValue)
					{
						manager.Theme = this.selectedTheme.Theme.Value;
					}

					else
					{
						manager.Theme = Application.Current.RequestedTheme;
					}
				}

				catch
				{
				}
			}

			if (AppearanceManager.AccentColor == null)
			{
				this.useSystemAccentColor = true;
			}

			else
			{
				this.selectedBrush = this.Brushes.FirstOrDefault(b => b.Color == AppearanceManager.AccentColor);
			}

			localSettings.Values["useSystemAccentColor"] = this.useSystemAccentColor;
			localSettings.Values["requireAuthAtStartUp"] = this.requireAuthAtStartUp;
			localSettings.Values["selectedBrush"] = this.selectedBrush == null ? null : this.selectedBrush.Color.ToString();
            localSettings.Values["useMDL2"] = this.useMDL2;
            localSettings.Values["useTimeStamps"] = this.useTimeStamps;
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
			get {
				return this.selectedBrush;
			}

			set
			{
				if (Set(ref this.selectedBrush, value))
				{
					if (!this.useSystemAccentColor && value != null)
					{
						AppearanceManager.AccentColor = value.Color;
						var applicationData = ApplicationData.Current;
						var localSettings = applicationData.LocalSettings;
						localSettings.Values["selectedBrush"] = value.Color.ToString();
					}
				}
			}
		}
        
        /// <summary>
        /// Gets the collection of themes.
        /// </summary>
        public IReadOnlyList<DisplayableTheme> Themes { get; }
		/// <summary>
		/// Gets or sets the selected theme.
		/// </summary>
		public DisplayableTheme SelectedTheme
		{
			get {
				return this.selectedTheme;
			}

			set
			{
				var applicationData = ApplicationData.Current;
				var localSettings = applicationData.LocalSettings;
				localSettings.Values["selectedTheme"] = value == null ? null : value.Theme.ToString();

				if (Set(ref this.selectedTheme, value) && value != null && value.Theme.HasValue)
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
        /// Gets or sets a value indicating whether to use the system accent color.
        /// </summary>
        public bool UseSystemAccentColor
		{
			get
			{
				return this.useSystemAccentColor;
			}

			set
			{
				if (Set(ref this.useSystemAccentColor, value))
				{
					var applicationData = ApplicationData.Current;
					var localSettings = applicationData.LocalSettings;
					localSettings.Values["useSystemAccentColor"] = value;

					if (value)
					{
						AppearanceManager.AccentColor = null;
					}

					else
						if (this.SelectedBrush != null)
						{
							AppearanceManager.AccentColor = this.SelectedBrush.Color;
						}
				}
			}
		}

		public bool UseAuthAtStartUp
		{
			get
			{
				return this.requireAuthAtStartUp;
			}

			set
			{
				RunInThreadPool(async () =>
				{
					await RunInUiThread(async () =>
					{
						if (await AskCreds())
						{
							if (Set(ref this.requireAuthAtStartUp, value))
							{
								var applicationData = ApplicationData.Current;
								var localSettings = applicationData.LocalSettings;
								localSettings.Values["requireAuthAtStartUp"] = value;
							}
						}
					});
				});
			}
		}
        
        public bool UseMDL2
        {
            get
            {
                return this.useMDL2;
            }

            set
            {
                if (Set(ref this.useMDL2, value))
                {
                    var applicationData = ApplicationData.Current;
                    var localSettings = applicationData.LocalSettings;
                    localSettings.Values["useMDL2"] = value;
                }
            }
        }

        public bool UseTimeStamps
        {
            get
            {
                return this.useTimeStamps;
            }

            set
            {
                if (Set(ref this.useTimeStamps, value))
                {
                    var applicationData = ApplicationData.Current;
                    var localSettings = applicationData.LocalSettings;
                    localSettings.Values["useTimeStamps"] = value;
                }
            }
        }

        private static async void RunInThreadPool(Action function)
		{
			await ThreadPool.RunAsync(x => { function(); });
		}

		private static async Task RunInUiThread(Action function)
		{
			await
			CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
			() => { function(); });
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