using InteropTools.Handlers;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System.Profile;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplashScreen : Page
    {
        private string Splash_EULA = App.local.Splash_EULA;
        private string Splash_Accept = App.local.Splash_Accept;
        private string Splash_Deny = App.local.Splash_Deny;

        internal Rect splashImageRect; // Rect to store splash screen image coordinates.
        private Windows.ApplicationModel.Activation.SplashScreen splash; // Variable to hold the splash screen object.
        internal bool dismissed = false; // Variable to track splash screen dismissal status.
        internal Frame rootFrame;
        private double ScaleFactor;

        object args;
        
        private void SetupTitleBar()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar.GetForCurrentView().BackgroundOpacity = 0;
            }
            
            var titlebar = ApplicationView.GetForCurrentView().TitleBar;
            var transparentColorBrush = new SolidColorBrush { Opacity = 0 };
            var transparentColor = transparentColorBrush.Color;
            titlebar.BackgroundColor = transparentColor;
            titlebar.ButtonBackgroundColor = transparentColor;
            titlebar.ButtonInactiveBackgroundColor = transparentColor;
            var solidColorBrush = Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush;

            if (solidColorBrush != null)
            {
                titlebar.ButtonForegroundColor = solidColorBrush.Color;
                titlebar.ButtonInactiveForegroundColor = solidColorBrush.Color;
            }

            var colorBrush = Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush;

            if (colorBrush != null)
            {
                titlebar.ForegroundColor = colorBrush.Color;
            }

            var hovercolor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush).Color;
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

        public SplashScreen(Windows.ApplicationModel.Activation.SplashScreen splashscreen, bool loadState, object args)
        {
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
                splash.Dismissed += new TypedEventHandler<Windows.ApplicationModel.Activation.SplashScreen, Object>(DismissedEventHandler);
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
        
        private Uri GetMatchingLogo()
        {
            var devicefamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            var tileimg = "generic";

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

            return new Uri("ms-appx:///Assets/Tiles/" + tileimg + ".png");
        }

        private async void ExtendedSplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            new SettingsHandler().Initialize();
            await FadeInBg.BeginAsync();
            extendedSplashImage2.Source = new BitmapImage(GetMatchingLogo());

            VersionText.Text = new VersionHandler().BuildString;
            await FadeInLogoSwitch.BeginAsync();
            
            if (new SettingsHandler().EULAAccepted != true)
            {
                LoadingPanel.Visibility = Visibility.Collapsed;
                EULAFlipView.Visibility = Visibility.Visible;
            }
            else
            {
                SetupApp();
            }
        }

        private void SetupApp()
        {
            /*//var frame = new CoreFrame();
            Window.Current.Content = new Shell();//frame;
            //frame.MainContent = new Shell();*/

            var frame = new Frame();
            frame.Navigate(typeof(Shell), args);
            Window.Current.Content = frame;
        }
        
        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
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

        void RestoreStateAsync(bool loadState)
        {
            if (loadState)
            {
                // TODO: write code to load state
            }
        }

        // Position the extended splash screen image in the same location as the system splash screen image.
        void PositionImage()
        {
            extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.Left);
            extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Top);
            extendedSplashImage.Height = splashImageRect.Height;
            extendedSplashImage.Width = splashImageRect.Width;
            extendedSplashImage2.SetValue(Canvas.LeftProperty, splashImageRect.Left);
            extendedSplashImage2.SetValue(Canvas.TopProperty, splashImageRect.Top);
            extendedSplashImage2.Height = splashImageRect.Height;
            extendedSplashImage2.Width = splashImageRect.Width;
        }

        void PositionRing()
        {
            splashProgressRing.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (splashProgressRing.Width * 0.5));
            splashProgressRing.SetValue(Canvas.TopProperty, (splashImageRect.Y + splashImageRect.Height + splashImageRect.Height * 0.1));
        }

        void ExtendedSplash_OnResize(Object sender, WindowSizeChangedEventArgs e)
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
        async void DismissedEventHandler(Windows.ApplicationModel.Activation.SplashScreen sender, object e)
        {
            dismissed = true;
            // Complete app setup operations here...
            // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
            await RunInUiThread(() => {
                if (sender != null)
                {
                    // Update the coordinates of the splash screen image.
                    splashImageRect = sender.ImageLocation;
                    PositionImage();
                    PositionRing();
                }
            });
        }
        
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            new SettingsHandler().EULAAccepted = true;

            EULAFlipView.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;

            var appver = Package.Current.Id.Version;

            SetupApp();
        }
    }
}
