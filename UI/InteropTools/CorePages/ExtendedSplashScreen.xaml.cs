using InteropTools.Classes;
using InteropTools.Presentation;
using Microsoft.Toolkit.Uwp.Helpers;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
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

namespace InteropTools.CorePages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplashScreen : Page
    {
        private Rect splashImageRect;       // Rect to store splash screen image coordinates.
        private SplashScreen splashScreen; // Variable to hold the splash screen object.

        private object arguments;

        public delegate void Dismissed(ExtendedSplashScreen splash);

        public static event Dismissed OnDismissed = null;

        public SettingsViewModel ViewModel { get; } = new SettingsViewModel();

        public ExtendedSplashScreen()
        {
            InitializeComponent();

            // Listen for window resize events to reposition the extended splash screen image accordingly.
            // This ensures that the extended splash screen formats properly in response to window resizing.
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(ExtendedSplash_OnResize);

            SetupTitleBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is LaunchActivatedEventArgs e2)
            {
                splashScreen = e2.SplashScreen;
                arguments = e2.Arguments;
            }
            else if (e.Parameter is FileActivatedEventArgs e1)
            {
                splashScreen = e1.SplashScreen;
                arguments = e1.Files[0] as StorageFile;
            }
            else if (e.Parameter is IActivatedEventArgs e3)
            {
                splashScreen = e3.SplashScreen;
                arguments = e3;
            }

            if (splashScreen != null)
            {
                // Register an event handler to be executed when the splash screen has been dismissed.
                splashScreen.Dismissed += new TypedEventHandler<SplashScreen, object>(DismissedEventHandler);

                // Retrieve the window coordinates of the splash screen image.
                splashImageRect = splashScreen.ImageLocation;
                PositionImage();
                PositionRing();
            }
            else
            {
                _ = ThreadPool.RunAsync((o) =>
                {
                    OnDismissed?.Invoke(this);

                    if (OnDismissed != null)
                    {
                        foreach (Delegate d in OnDismissed.GetInvocationList())
                        {
                            OnDismissed -= (Dismissed)d;
                        }
                    }
                });
            }

            base.OnNavigatedTo(e);
        }

        private void SetupTitleBar()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar.GetForCurrentView().BackgroundOpacity = 0;
            }

            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            ApplicationViewTitleBar titlebar = ApplicationView.GetForCurrentView().TitleBar;
            SolidColorBrush transparentColorBrush = new() { Opacity = 0 };
            Windows.UI.Color transparentColor = transparentColorBrush.Color;
            titlebar.BackgroundColor = transparentColor;
            titlebar.ButtonBackgroundColor = transparentColor;
            titlebar.ButtonInactiveBackgroundColor = transparentColor;

            if (Application.Current.Resources["ApplicationForegroundThemeBrush"] is SolidColorBrush solidColorBrush)
            {
                titlebar.ButtonForegroundColor = solidColorBrush.Color;
                titlebar.ButtonInactiveForegroundColor = solidColorBrush.Color;
            }

            if (Application.Current.Resources["ApplicationForegroundThemeBrush"] is SolidColorBrush colorBrush)
            {
                titlebar.ForegroundColor = colorBrush.Color;
            }

            Windows.UI.Color hovercolor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush)?.Color ?? default;
            hovercolor.A = 32;
            titlebar.ButtonHoverBackgroundColor = hovercolor;
            titlebar.ButtonHoverForegroundColor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush)?.Color;
            hovercolor.A = 64;
            titlebar.ButtonPressedBackgroundColor = hovercolor;
            titlebar.ButtonPressedForegroundColor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush)?.Color;

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        }

        private void FlipViewItem_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.Delta.Translation.X != 0)
            {
                e.Handled = true;
            }
        }

        private async Task ShowLoadingUIAsync()
        {
            string buildString = VersionHelper.GetBuildString();
            VersionText.Text = buildString;

            await FadeInBg.BeginAsync();

            extendedSplashImage2.Source = new BitmapImage(new Uri(DeviceFamilyAssetHelper.GetTileAssetPath("Tiles")));

            await FadeInLogoSwitch.BeginAsync();

            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            if ((localSettings.Values["EULAAccepted"] as bool?) != true)
            {
                LoadingPanel.Visibility = Visibility.Collapsed;
                EULAFlipView.Visibility = Visibility.Visible;
            }
            else if ((localSettings.Values["LastVersion"] as string) != buildString)
            {
                localSettings.Values["LastVersion"] = buildString;
                LoadingPanel.Visibility = Visibility.Collapsed;
                OOBEFlipView.Visibility = Visibility.Visible;
                OOBEFlipView.SelectedIndex = 0;
            }
            else
            {
                SessionManager.AddNewSession(arguments);
            }
        }

        private void ExtendedSplash_OnResize(object sender, WindowSizeChangedEventArgs e)
        {
            // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
            if (splashScreen != null)
            {
                // Update the coordinates of the splash screen image.
                splashImageRect = splashScreen.ImageLocation;
                PositionImage();
                PositionRing();
            }
        }

        // Position the extended splash screen image in the same location as the system splash screen image.
        private void PositionImage()
        {
            PositionImage(extendedSplashImage);
            PositionImage(extendedSplashImage2);
        }

        private void PositionImage(Image SplashImage)
        {
            if (SystemInformation.DeviceFamily != "Windows.Xbox")
            {
                SplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
                SplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
                SplashImage.Height = splashImageRect.Height;
                SplashImage.Width = splashImageRect.Width;
            }
        }

        private void PositionRing()
        {
            splashProgressRing.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (splashProgressRing.Width * 0.5));
            splashProgressRing.SetValue(Canvas.TopProperty, splashImageRect.Y + splashImageRect.Height + (splashImageRect.Height * 0.1));
        }

        private async void DismissedEventHandler(SplashScreen sender, object e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await ShowLoadingUIAsync());

            OnDismissed?.Invoke(this);

            if (OnDismissed != null)
            {
                foreach (Delegate d in OnDismissed.GetInvocationList())
                {
                    OnDismissed -= (Dismissed)d;
                }
            }
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
            SessionManager.AddNewSession(arguments);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;

            localSettings.Values["EULAAccepted"] = true;

            EULAFlipView.Visibility = Visibility.Collapsed;
            LoadingPanel.Visibility = Visibility.Visible;

            PackageVersion appver = Package.Current.Id.Version;

            if ((localSettings.Values["LastVersion"] as string) != string.Format("{0}.{1}.{2}.{3}", appver.Major, appver.Minor, appver.Build, appver.Revision))
            {
                localSettings.Values["LastVersion"] = string.Format("{0}.{1}.{2}.{3}", appver.Major, appver.Minor, appver.Build, appver.Revision);
                LoadingPanel.Visibility = Visibility.Collapsed;
                OOBEFlipView.Visibility = Visibility.Visible;
                OOBEFlipView.SelectedIndex = 0;
            }
            else
            {
                SessionManager.AddNewSession(arguments);
            }
        }
    }
}
