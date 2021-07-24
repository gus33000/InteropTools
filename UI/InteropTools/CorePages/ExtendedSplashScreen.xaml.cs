using InteropTools.Presentation;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.System.Profile;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.CorePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplashScreen
    {
        internal Rect splashImageRect; // Rect to store splash screen image coordinates.
        private readonly SplashScreen splash; // Variable to hold the splash screen object.
        internal bool dismissed = false; // Variable to track splash screen dismissal status.
        internal Frame rootFrame;
        private readonly double ScaleFactor;
        private readonly object args;


        private void SetupTitleBar()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar.GetForCurrentView().BackgroundOpacity = 0;
            }

            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);
            ApplicationViewTitleBar titlebar = ApplicationView.GetForCurrentView().TitleBar;
            SolidColorBrush transparentColorBrush = new SolidColorBrush { Opacity = 0 };
            Windows.UI.Color transparentColor = transparentColorBrush.Color;
            titlebar.BackgroundColor = transparentColor;
            titlebar.ButtonBackgroundColor = transparentColor;
            titlebar.ButtonInactiveBackgroundColor = transparentColor;
            SolidColorBrush solidColorBrush = Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush;

            if (solidColorBrush != null)
            {
                titlebar.ButtonForegroundColor = solidColorBrush.Color;
                titlebar.ButtonInactiveForegroundColor = solidColorBrush.Color;
            }

            SolidColorBrush colorBrush = Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush;

            if (colorBrush != null)
            {
                titlebar.ForegroundColor = colorBrush.Color;
            }

            Windows.UI.Color hovercolor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush).Color;
            hovercolor.A = 32;
            titlebar.ButtonHoverBackgroundColor = hovercolor;
            titlebar.ButtonHoverForegroundColor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush).Color;
            hovercolor.A = 64;
            titlebar.ButtonPressedBackgroundColor = hovercolor;
            titlebar.ButtonPressedForegroundColor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush).Color;
        }

        private void FlipViewItem_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.Delta.Translation.X != 0)
            {
                e.Handled = true;
            }
        }

        public ExtendedSplashScreen(SplashScreen splashscreen, bool loadState, object args)
        {
            ViewModel = new SettingsViewModel();
            this.args = args;
            RequestedTheme = ElementTheme.Dark;
            ScaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            InitializeComponent();
            Loaded += ExtendedSplashScreen_Loaded;
            SetupTitleBar();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            // Listen for window resize events to reposition the extended splash screen image accordingly.
            // This is important to ensure that the extended splash screen is formatted properly in response to snapping, unsnapping, rotation, etc...
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(ExtendedSplash_OnResize);
            splash = splashscreen;

            if (splash != null)
            {
                // Register an event handler to be executed when the splash screen has been dismissed.
                splash.Dismissed += new TypedEventHandler<SplashScreen, object>(DismissedEventHandler);
                // Retrieve the window coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();
                // Optional: Add a progress ring to your splash screen to show users that content is loading
                PositionRing();
            }

            // Create a Frame to act as the navigation context
            rootFrame = new Frame();
            // Restore the saved session state if necessary
            RestoreStateAsync(loadState);
        }

        public SettingsViewModel ViewModel { get; }

        private async void ExtendedSplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            new SettingsViewModel();
            await FadeInBg.BeginAsync();
            string devicefamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            string tileimg = "generic";

            switch (devicefamily.ToLower())
            {
                case "windows.desktop":
                    {
                        tileimg = "desktop";
                        break;
                    }

                case "windows.xbox":
                    {
                        tileimg = "xbox";
                        break;
                    }

                case "windows.holographic":
                    {
                        tileimg = "holographic";
                        break;
                    }

                case "windows.team":
                    {
                        tileimg = "team";
                        break;
                    }

                case "windows.iot":
                    {
                        tileimg = "iot";
                        break;
                    }

                case "windows.mobile":
                    {
                        tileimg = "phone";
                        break;
                    }

                default:
                    {
                        tileimg = "generic";
                        break;
                    }
            }

            extendedSplashImage2.Source = new BitmapImage(new Uri("ms-appx:///Assets/Tiles/" + tileimg + ".png"));
            string buildString = "";
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream resource = assembly.GetManifestResourceStream("InteropTools.Resources.BuildDate.txt");
            string builddate = new StreamReader(resource).ReadLine().Replace("\r", "");
            PackageVersion appver = Package.Current.Id.Version;
            string appverstr = string.Format("{0}.{1}.{2}.{3}", appver.Major, appver.Minor, appver.Build, appver.Revision);
            buildString = appverstr + " (fbl_prerelease(gustavem)";
            Type myType = Type.GetType("InteropTools.ShellPages.Private.YourWindowsBuildPage");

            if (myType != null)
            {
                buildString = buildString + "/private";
            }

            buildString = buildString + "." + builddate + ")";
            VersionText.Text = buildString;
            await FadeInLogoSwitch.BeginAsync();

            //await new UpdateAvailableContentDialog().DownloadAndInstallAllUpdatesAsync();

            /*RunInThreadPool(async () => {
                await RunInUiThread(() => {
                    App.AddNewSession(args);
                });
            });*/

            /*RunInThreadPool(async () => {
			    await Task.Delay(2000);
			    await RunInUiThread(() => {
			        App.AddNewSession(args);
			    });
			});*/

            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            if (localSettings.Values["EULAAccepted"] as bool? != true)
            {
                LoadingPanel.Visibility = Visibility.Collapsed;
                EULAFlipView.Visibility = Visibility.Visible;
            }
            else if (localSettings.Values["LastVersion"] as string != string.Format("{0}.{1}.{2}.{3}", appver.Major, appver.Minor, appver.Build, appver.Revision))
            {
                localSettings.Values["LastVersion"] = string.Format("{0}.{1}.{2}.{3}", appver.Major, appver.Minor, appver.Build, appver.Revision);
                LoadingPanel.Visibility = Visibility.Collapsed;
                OOBEFlipView.Visibility = Visibility.Visible;
                OOBEFlipView.SelectedIndex = 0;
            }
            else
            {
                App.AddNewSession(args);
            }
        }

        private static async Task RunInUiThread(Action function)
        {
            await
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => { function(); });
        }

        private static async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => { function(); });
        }

        private async void RestoreStateAsync(bool loadState)
        {
            if (loadState)
            {
                // TODO: write code to load state
            }
        }

        // Position the extended splash screen image in the same location as the system splash screen image.
        private void PositionImage()
        {
            extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.Left);
            extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Top);
            /*if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                extendedSplashImage.Height = splashImageRect.Height / ScaleFactor;
                extendedSplashImage.Width = splashImageRect.Width / ScaleFactor;
            }
            else
            {*/
            extendedSplashImage.Height = splashImageRect.Height;
            extendedSplashImage.Width = splashImageRect.Width;
            //}

            extendedSplashImage2.SetValue(Canvas.LeftProperty, splashImageRect.Left);
            extendedSplashImage2.SetValue(Canvas.TopProperty, splashImageRect.Top);
            /*if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                extendedSplashImage2.Height = splashImageRect.Height / ScaleFactor;
                extendedSplashImage2.Width = splashImageRect.Width / ScaleFactor;
            }
            else
            {*/
            extendedSplashImage2.Height = splashImageRect.Height;
            extendedSplashImage2.Width = splashImageRect.Width;
            //}
        }

        private void PositionRing()
        {
            splashProgressRing.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (splashProgressRing.Width * 0.5));
            splashProgressRing.SetValue(Canvas.TopProperty, (splashImageRect.Y + splashImageRect.Height + splashImageRect.Height * 0.1));
        }

        private void ExtendedSplash_OnResize(object sender, WindowSizeChangedEventArgs e)
        {
            // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
            if (splash != null)
            {
                // Update the coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();
                PositionRing();
            }
        }

        // Include code to be executed when the system has transitioned from the splash screen to the extended splash screen (application's first view).
        private async void DismissedEventHandler(SplashScreen sender, object e)
        {
            dismissed = true;
            // Complete app setup operations here...
            // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
            await RunInUiThread(() =>
            {
                if (sender != null)
                {
                    // Update the coordinates of the splash screen image.
                    splashImageRect = sender.ImageLocation;
                    PositionImage();
                    PositionRing();
                }
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OOBEFlipView.Visibility = Visibility.Visible;
            OOBEFlipView.SelectedIndex = 1;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OOBEFlipView.Visibility = Visibility.Visible;
            OOBEFlipView.SelectedIndex = 0;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            OOBEFlipView.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;
            RunInThreadPool(async () =>
            {
                await RunInUiThread(() =>
                {
                    App.AddNewSession(args);
                });
            });
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            App.Current.Exit();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            localSettings.Values["EULAAccepted"] = true;

            EULAFlipView.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;

            PackageVersion appver = Package.Current.Id.Version;

            if (localSettings.Values["LastVersion"] as string != string.Format("{0}.{1}.{2}.{3}", appver.Major, appver.Minor, appver.Build, appver.Revision))
            {
                localSettings.Values["LastVersion"] = string.Format("{0}.{1}.{2}.{3}", appver.Major, appver.Minor, appver.Build, appver.Revision);
                LoadingPanel.Visibility = Visibility.Collapsed;
                OOBEFlipView.Visibility = Visibility.Visible;
                OOBEFlipView.SelectedIndex = 0;
            }
            else
            {
                App.AddNewSession(args);
            }
        }
    }
}
