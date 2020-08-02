using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using System.Collections.ObjectModel;
using Windows.UI.Core;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;
using InteropTools.Resources;
using Windows.UI.Notifications;
using Windows.System.Profile;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using Windows.Foundation.Metadata;

namespace InteropTools
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static Localization local = new Localization();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            /*if (ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Application", "RequiresPointerMode"))
                RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;*/

            UnhandledException += async (s, args) =>
            {
                args.Handled = true;

                try
                {
                    await new MessageDialog(args.Exception?.ToString() ?? string.Empty, local.App_UnhandledException).ShowAsync();
                }
                catch { }
            };

            //RefreshTile();
        }
        
        public ObservableCollection<ViewLifetimeControl> SecondaryViews = new ObservableCollection<ViewLifetimeControl>();
        private CoreDispatcher mainDispatcher;
        public CoreDispatcher MainDispatcher
        {
            get
            {
                return mainDispatcher;
            }
        }
        
        private void RefreshTile()
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

            var content = new TileContent
            {
                Visual = new TileVisual
                {
                    TileSmall = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = "Assets/Tiles/Small/" + tileimg + ".png"
                            }
                        }
                    },
                    TileMedium = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = "Assets/Tiles/Medium/" + tileimg + ".png"
                            }
                        }
                    },
                    TileWide = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = "Assets/Tiles/Wide/" + tileimg + ".png"
                            }
                        }
                    },
                    TileLarge = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = "Assets/Tiles/Large/" + tileimg + ".png"
                            }
                        }
                    }
                }
            };
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().Update(new TileNotification(content.GetXml()));
        }

        private async Task<ViewLifetimeControl> createMainPageAsync()
        {
            ViewLifetimeControl viewControl = null;
            await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // This object is used to keep track of the views and important
                // details about the contents of those views across threads
                // In your app, you would probably want to track information
                // like the open document or page inside that window
                viewControl = ViewLifetimeControl.CreateForCurrentView();
                // Increment the ref count because we just created the view and we have a reference to it                
                viewControl.StartViewInUse();
                
                var frame = new Frame();
                frame.Navigate(typeof(Shell), viewControl);
                Window.Current.Content = frame;
                // This is a change from 8.1: In order for the view to be displayed later it needs to be activated.
                Window.Current.Activate();
            });

            ((App)App.Current).SecondaryViews.Add(viewControl);

            return viewControl;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // disabled, obscures the hamburger button, enable if you need it
                DebugSettings.EnableFrameRateCounter = false;
            }
#endif

            // CoreApplication.EnablePrelaunch was introduced in Windows 10 version 1607
            bool canEnablePrelaunch = Windows.Foundation.Metadata.ApiInformation.IsMethodPresent("Windows.ApplicationModel.Core.CoreApplication", "EnablePrelaunch");

            if (Window.Current.Content == null)
            {
                bool loadState = (e.PreviousExecutionState == ApplicationExecutionState.Terminated);
                Pages.SplashScreen extendedSplash = new Pages.SplashScreen(e.SplashScreen, loadState, e.Arguments);
                Window.Current.Content = extendedSplash;

                mainDispatcher = Window.Current.Dispatcher;

                if (e.PrelaunchActivated == false)
                {
                    // On Windows 10 version 1607 or later, this code signals that this app wants to participate in prelaunch
                    if (canEnablePrelaunch)
                    {
                        TryEnablePrelaunch();
                    }

                    // Ensure the current window is active
                    Window.Current.Activate();
                }
            }
            else
            {
                // second and later
                var selectedView = await createMainPageAsync();
                if (selectedView != null)
                {
                    selectedView.StartViewInUse();
                    var viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(selectedView.Id, ViewSizePreference.Default, ApplicationView.GetForCurrentView().Id, ViewSizePreference.Default);

                    if (!viewShown)
                    {
                        foreach (var item in SecondaryViews)
                        {
                            if (item != selectedView)
                            {
                                var result = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(selectedView.Id, ViewSizePreference.Default, item.Id, ViewSizePreference.Default);
                                if (result)
                                    break;
                            }
                        }
                    }

                    await selectedView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Window.Current.Activate();
                    });

                    selectedView.StopViewInUse();
                }
            }
        }
        
        /// <summary>
        /// Encapsulates the call to CoreApplication.EnablePrelaunch() so that the JIT
        /// won't encounter that call (and prevent the app from running when it doesn't
        /// find it), unless this method gets called. This method should only
        /// be called when the caller determines that we are running on a system that
        /// supports CoreApplication.EnablePrelaunch().
        /// </summary>
        private void TryEnablePrelaunch()
        {
            CoreApplication.EnablePrelaunch(true);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
