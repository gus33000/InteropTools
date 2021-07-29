using InteropTools.Classes;
using InteropTools.CorePages;
using InteropTools.Providers;
using InteropTools.Resources;
using Microsoft.HockeyApp;
using Microsoft.Services.Store.Engagement;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Shell = InteropTools.CorePages.Shell;

namespace InteropTools
{
    public sealed partial class App : Application
    {
        public static IRegistryProvider RegistryHelper
        {
            get => SessionManager.CurrentSession != null ? SessionManager.Sessions[(int)SessionManager.CurrentSession].Helper : null;

            set
            {
                if (SessionManager.CurrentSession != null)
                {
                    SessionManager.Sessions[(int)SessionManager.CurrentSession].Helper = value;
                }
            }
        }

        public static IRegistryProvider MainRegistryHelper = new MainRegistryProvider();

        public static readonly TextResources textResources = new();

        public static bool Fancyness = false;

        private static readonly Rect bounds = ApplicationView.GetForCurrentView().VisibleBounds;
        private static readonly double scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
        public static readonly Size size = new(bounds.Width * scaleFactor, bounds.Height * scaleFactor);

        public App()
        {
            UnhandledException += async (s, args) =>
            {
                args.Handled = true;

                try
                {
                    await new MessageDialog(args.Exception?.ToString() ?? string.Empty, "Unhandled exception").ShowAsync();
                }
                catch { }
            };

            HockeyClient.Current.Configure("8f0c0303cc9b40648d6f3962bbac2b40", new TelemetryConfiguration()
            {
                Collectors = WindowsCollectors.Metadata | WindowsCollectors.Session | WindowsCollectors.UnhandledException,
                EnableDiagnostics = true
            });

            InitializeComponent();
        }

        public static UIElement AppContent
        {
            get
            {
                if (Window.Current.Content == null)
                {
                    Window.Current.Content = new CoreFrame();
                }

                CoreFrame frame = Window.Current.Content as CoreFrame;
                return frame.FrameContent;
            }

            set
            {
                if (Window.Current.Content == null)
                {
                    Window.Current.Content = new CoreFrame();
                }

                CoreFrame frame = Window.Current.Content as CoreFrame;
                frame.FrameContent = value;
            }
        }

        protected override async void OnFileActivated(FileActivatedEventArgs args)
        {
            StorageFile file = args.Files[0] as StorageFile;
            RefreshTile();
            RefreshJumpList();

            if (SessionManager.CurrentSession == null)
            {
                //  Display an extended splash screen if app was not previously running.
                if (args.PreviousExecutionState != ApplicationExecutionState.Running)
                {
                    Frame frame = new();
                    frame.Navigate(typeof(ExtendedSplashScreen), args);
                    AppContent = frame;
                    Window.Current.Activate();
                }
                else
                {
                    SessionManager.AddNewSession(file);
                }
            }
            else if ((AppContent as Shell) == null)
            {
                //  Display an extended splash screen if app was not previously running.
                if (args.PreviousExecutionState != ApplicationExecutionState.Running)
                {
                    Frame frame = new();
                    frame.Navigate(typeof(ExtendedSplashScreen), args);
                    AppContent = frame;
                    Window.Current.Activate();
                }
                else
                {
                    SessionManager.AddNewSession(file);
                }
            }
            else
            {
                Shell currentContent = AppContent as Shell;
                await currentContent.HandleFileActivatedEvent(file);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                base.OnActivated(args);

                try
                {
                    if (args is ToastNotificationActivatedEventArgs)
                    {
                        ToastNotificationActivatedEventArgs toastActivationArgs = args as ToastNotificationActivatedEventArgs;

                        StoreServicesEngagementManager engagementManager = StoreServicesEngagementManager.GetDefault();
                        string originalArgs = engagementManager.ParseArgumentsAndTrackAppLaunch(
                            toastActivationArgs.Argument);

                        // Use the originalArgs variable to access the original arguments
                        // that were passed to the app.
                    }
                }
                catch
                {
                }

                ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;

                SetupThemes();

                try
                {
                    StoreServicesEngagementManager engagementManager = StoreServicesEngagementManager.GetDefault();
                    await engagementManager.RegisterNotificationChannelAsync();
                }
                catch
                {
                }

#if DEBUG
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    // disabled, obscures the hamburger button, enable if you need it
                    DebugSettings.EnableFrameRateCounter = true;
                }
#endif

                RefreshTile();
                RefreshJumpList();

                if (SessionManager.CurrentSession == null)
                {
                    //  Display an extended splash screen if app was not previously running.
                    if (eventArgs.PreviousExecutionState != ApplicationExecutionState.Running)
                    {
                        Frame frame = new();
                        frame.Navigate(typeof(ExtendedSplashScreen), eventArgs);
                        AppContent = frame;
                        Window.Current.Activate();
                    }
                    else
                    {
                        SessionManager.AddNewSession("");
                    }
                }
            }
            else if (args.Kind == ActivationKind.ProtocolForResults && Window.Current.Content == null && ((ProtocolForResultsActivatedEventArgs)args).Uri.AbsoluteUri.Contains("interoptools-appextensionregistrar"))
            {
                // Setup temp frame, ask for provider, show dialog saying hey do you want to allow app, and close / suspend.
                // Also save the app pfn that asked.

                ProtocolForResultsActivatedEventArgs protocolForResultsArgs = (ProtocolForResultsActivatedEventArgs)args;

                // Window management
                if (Window.Current.Content is not Frame rootFrame)
                {
                    rootFrame = new Frame();
                    Window.Current.Content = rootFrame;
                }

                // Open the page that we created to handle activation for results.
                rootFrame.Navigate(typeof(LaunchedForResultsPage), protocolForResultsArgs);

                // Ensure the current window is active.
                Window.Current.Activate();
            }
            else
            {
                base.OnActivated(args);

                try
                {
                    if (args is ToastNotificationActivatedEventArgs)
                    {
                        ToastNotificationActivatedEventArgs toastActivationArgs = args as ToastNotificationActivatedEventArgs;

                        StoreServicesEngagementManager engagementManager = StoreServicesEngagementManager.GetDefault();
                        string originalArgs = engagementManager.ParseArgumentsAndTrackAppLaunch(
                            toastActivationArgs.Argument);

                        // Use the originalArgs variable to access the original arguments
                        // that were passed to the app.
                    }
                }
                catch
                {
                }
            }
        }

        private void SetupThemes()
        {
            if (ApiInformation.IsMethodPresent("Windows.UI.Composition.Compositor", "CreateHostBackdropBrush"))
            {
                ApplicationData applicationData = ApplicationData.Current;
                ApplicationDataContainer localSettings = applicationData.LocalSettings;

                if ((localSettings.Values["useMDL2"] == null) || (localSettings.Values["useMDL2"].GetType() != typeof(bool)))
                {
                    localSettings.Values["useMDL2"] = false;
                }

                bool useMDL2 = (bool)localSettings.Values["useMDL2"];

                if (!useMDL2)
                {
                    Resources.MergedDictionaries.Add
                    (
                        new ResourceDictionary { Source = new Uri("ms-appx:///Themes/rs2_neon.xaml") }
                    );
                }
                else
                {
                    Resources.MergedDictionaries.Add
                    (
                        new ResourceDictionary { Source = new Uri("ms-appx:///Themes/rs1_neon.xaml") }
                    );
                }
            }
            else
            {
                Resources.MergedDictionaries.Add
                (
                    new ResourceDictionary { Source = new Uri("ms-appx:///Themes/rs1_neon.xaml") }
                );
            }
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            SetupThemes();

            try
            {
                StoreServicesEngagementManager engagementManager = StoreServicesEngagementManager.GetDefault();
                await engagementManager.RegisterNotificationChannelAsync();
            }
            catch
            {
            }

            RefreshTile();
            RefreshJumpList();

            if (SessionManager.CurrentSession == null)
            {
                //  Display an extended splash screen if app was not previously running.
                if (e.PreviousExecutionState != ApplicationExecutionState.Running)
                {
                    Frame frame = new();
                    frame.Navigate(typeof(ExtendedSplashScreen), e);
                    AppContent = frame;
                    Window.Current.Activate();
                }
                else
                {
                    SessionManager.AddNewSession(e.Arguments);
                }
            }

            string args = e.Arguments;

            if (AppContent is not Shell currentContent)
            {
                return;
            }

            Shell shell = currentContent;
            shell.HandleLaunchedEvent(args);
        }

        private void RefreshTile()
        {
            TileContent content = new()
            {
                Visual = new TileVisual
                {
                    TileSmall = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = DeviceFamilyAssetHelper.GetTileAssetPath("Small")
                            }
                        }
                    },
                    TileMedium = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = DeviceFamilyAssetHelper.GetTileAssetPath("Medium")
                            }
                        }
                    },
                    TileWide = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = DeviceFamilyAssetHelper.GetTileAssetPath("Wide")
                            }
                        }
                    },
                    TileLarge = new TileBinding
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = new TileBackgroundImage()
                            {
                                Source = DeviceFamilyAssetHelper.GetTileAssetPath("Large")
                            }
                        }
                    }
                }
            };

            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
            TileUpdateManager.CreateTileUpdaterForApplication().Update(new TileNotification(content.GetXml()));
        }

        private async void RefreshJumpList()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.StartScreen.JumpList"))
            {
                if (JumpList.IsSupported())
                {
                    JumpList jumpList = await JumpList.LoadCurrentAsync();
                    jumpList.Items.Clear();
                    JumpListItem item1 = JumpListItem.CreateWithArguments("RegistryEditorPage", TextResources.Shell_RegistryEditorTitle);
                    item1.Description = TextResources.Shell_RegistryEditorDesc;
                    item1.Logo = new Uri("ms-appx:///Assets/JumpList/registryeditor.png");
                    JumpListItem item2 = JumpListItem.CreateWithArguments("RegistryBrowserPage", TextResources.Shell_RegistryBrowserTitle);
                    item2.Description = TextResources.Shell_RegistryBrowserDesc;
                    item2.Logo = new Uri("ms-appx:///Assets/JumpList/registrybrowser.png");
                    JumpListItem item3 = JumpListItem.CreateWithArguments("RegistrySearchPage", TextResources.Shell_RegistrySearchTitle);
                    item3.Description = TextResources.Shell_RegistrySearchDesc;
                    item3.Logo = new Uri("ms-appx:///Assets/JumpList/registrysearch.png");
                    JumpListItem item4 = JumpListItem.CreateWithArguments("TweaksPage", TextResources.Shell_TweaksTitle);
                    item4.Description = TextResources.Shell_TweaksDesc;
                    item4.Logo = new Uri("ms-appx:///Assets/JumpList/tweaks.png");
                    JumpListItem item6 = JumpListItem.CreateWithArguments("AppManagerPage", TextResources.Shell_ApplicationsTitle);
                    item6.Description = TextResources.Shell_ApplicationsDescription;
                    item6.Logo = new Uri("ms-appx:///Assets/JumpList/apps.png");
                    JumpListItem item7 = JumpListItem.CreateWithArguments("CertificatesPage", TextResources.Shell_CertificatesTitle);
                    item7.Description = TextResources.Shell_CertificatesDesc;
                    item7.Logo = new Uri("ms-appx:///Assets/JumpList/certs.png");
                    JumpListItem item8 = JumpListItem.CreateWithArguments("InteropUnlockPage", TextResources.Shell_InteropUnlockTitle);
                    item8.Description = TextResources.Shell_InteropUnlockDesc;
                    item8.Logo = new Uri("ms-appx:///Assets/JumpList/interopunlock.png");
                    JumpListItem item9 = JumpListItem.CreateWithArguments("YourDevicePage", TextResources.Shell_DeviceInfoTitle);
                    item9.Description = TextResources.Shell_DeviceInfoDesc;
                    item9.Logo = new Uri("ms-appx:///Assets/JumpList/yourdevice.png");
                    jumpList.SystemGroupKind = JumpListSystemGroupKind.None;
                    jumpList.Items.Add(item1);
                    jumpList.Items.Add(item2);
                    jumpList.Items.Add(item3);
                    jumpList.Items.Add(item4);
                    jumpList.Items.Add(item6);
                    jumpList.Items.Add(item7);
                    jumpList.Items.Add(item8);
                    jumpList.Items.Add(item9);
                    await jumpList.SaveAsync();
                }
            }
        }
    }
}