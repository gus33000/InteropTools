using Intense.Presentation;
using InteropTools.ContentDialogs.Core;
using InteropTools.ContentDialogs.Registry;
using InteropTools.Presentation;
using InteropTools.Providers;
using InteropTools.ShellPages.AppManager;
using InteropTools.ShellPages.Certificates;
using InteropTools.ShellPages.Core;
using InteropTools.ShellPages.Registry;
using InteropTools.ShellPages.SSH;
using InteropTools.ShellPages.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TreeViewControl;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation.Metadata;
using Windows.Management.Deployment;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.System.Profile;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using muxc = Microsoft.UI.Xaml.Controls;

namespace InteropTools.CorePages
{
    public sealed partial class Shell : UserControl
    {
        public Shell(object args)
        {
            this.InitializeComponent();

            if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox")
            {
                XboxContentSafeRect.Visibility = Visibility.Visible;
            }

            // Workaround for VisualState issue that should be fixed
            // by https://github.com/microsoft/microsoft-ui-xaml/pull/2271
            // NavigationViewControl.PaneDisplayMode = muxc.NavigationViewPaneDisplayMode.Left;

            Window.Current.SetTitleBar(AppTitleBar);

            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += (s, e) => UpdateAppTitle(s);

            NavigationViewControl.RegisterPropertyChangedCallback(muxc.NavigationView.PaneDisplayModeProperty, new DependencyPropertyChangedCallback(OnPaneDisplayModeChanged));
            Load(args);
        }

        private void OnPaneDisplayModeChanged(DependencyObject sender, DependencyProperty dp)
        {
            var navigationView = sender as muxc.NavigationView;
            AppTitleBar.Visibility = navigationView.PaneDisplayMode == muxc.NavigationViewPaneDisplayMode.Top ? Visibility.Collapsed : Visibility.Visible;
        }

        void UpdateAppTitle(CoreApplicationViewTitleBar coreTitleBar)
        {
            //ensure the custom title bar does not overlap window caption controls
            Thickness currMargin = AppTitleBar.Margin;
            AppTitleBar.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        public string GetAppTitleFromSystem()
        {
            return Windows.ApplicationModel.Package.Current.DisplayName;
        }

        private void NavigationViewControl_PaneClosing(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewPaneClosingEventArgs args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavigationViewControl_PaneOpened(Microsoft.UI.Xaml.Controls.NavigationView sender, object args)
        {
            UpdateAppTitleMargin(sender);
        }

        private void NavigationViewControl_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            Thickness currMargin = AppTitleBar.Margin;
            if (sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
            {
                AppTitleBar.Margin = new Thickness((sender.CompactPaneLength * 2), currMargin.Top, currMargin.Right, currMargin.Bottom);

            }
            else
            {
                AppTitleBar.Margin = new Thickness(sender.CompactPaneLength, currMargin.Top, currMargin.Right, currMargin.Bottom);
            }

            UpdateAppTitleMargin(sender);
        }

        private void UpdateAppTitleMargin(Microsoft.UI.Xaml.Controls.NavigationView sender)
        {
            const int smallLeftIndent = 4, largeLeftIndent = 24;

            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 7))
            {
                AppTitle.TranslationTransition = new Vector3Transition();

                if ((sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
                {
                    AppTitle.Translation = new System.Numerics.Vector3(smallLeftIndent, 0, 0);
                }
                else
                {
                    AppTitle.Translation = new System.Numerics.Vector3(largeLeftIndent, 0, 0);
                }
            }
            else
            {
                Thickness currMargin = AppTitle.Margin;

                if ((sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Expanded && sender.IsPaneOpen) ||
                         sender.DisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal)
                {
                    AppTitle.Margin = new Thickness(smallLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
                else
                {
                    AppTitle.Margin = new Thickness(largeLeftIndent, currMargin.Top, currMargin.Right, currMargin.Bottom);
                }
            }
        }

        private void CtrlF_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            controlsSearchBox.Focus(FocusState.Programmatic);
        }

        public string _SwitchSession = InteropTools.Resources.TextResources.Shell_SwitchSession;
        public string mode;
        public ObservableRangeCollection<NavigationItem> recentitems = new();
        public ObservableRangeCollection<NavigationItem> recentitems5max = new();
        public ObservableRangeCollection<TreeNode2> treeviewnodes = new();
        private bool _initialized;
        private double titlebarheight;

        public Frame RootFrame => rootFrame;

        public ShellViewModel ViewModel
        {
            get;
            private set;
        }

        private string UpperCaseAppTitle
        {
            get
            {
                string title = ApplicationView.GetForCurrentView().Title.ToUpper();
                return title?.Length == 0 ? Package.Current.DisplayName.ToUpper() : title;
            }
        }

        public async Task HandleFileActivatedEvent(StorageFile file)
        {
            if (App.MainRegistryHelper.AllowsRegistryEditing())
            {
                await new ImportRegContentDialog(file).ShowAsync();
            }
        }

        public void HandleLaunchedEvent(string args)
        {
            if (args?.Length == 0)
            {
                return;
            }

            switch (args)
            {
                case "RegistryEditorPage":
                    {
                        if (App.RegistryHelper != null && App.MainRegistryHelper.AllowsRegistryEditing())
                        {
                            RootFrame.Navigate(typeof(RegistryEditorPage));
                        }

                        break;
                    }

                case "RegistryBrowserPage":
                    {
                        if (App.RegistryHelper != null && App.MainRegistryHelper.AllowsRegistryEditing())
                        {
                            RootFrame.Navigate(typeof(RegistryBrowserPage));
                        }

                        break;
                    }

                case "RegistrySearchPage":
                    {
                        if (App.RegistryHelper != null && App.MainRegistryHelper.AllowsRegistryEditing())
                        {
                            RootFrame.Navigate(typeof(RegistrySearchPage));
                        }

                        break;
                    }

                case "InteropUnlockPage":
                    {
                        if (App.RegistryHelper != null && App.MainRegistryHelper.AllowsRegistryEditing())
                        {
                            RootFrame.Navigate(typeof(InteropUnlockPage));
                        }

                        break;
                    }

                case "TweaksPage":
                    {
                        if (App.RegistryHelper != null && App.MainRegistryHelper.AllowsRegistryEditing())
                        {
                            RootFrame.Navigate(typeof(TweaksPage));
                        }

                        break;
                    }

                case "KeyboardCarretPage":
                    {
                        if (App.RegistryHelper != null && App.MainRegistryHelper.AllowsRegistryEditing())
                        {
                            RootFrame.Navigate(typeof(KeyboardCarretPage));
                        }

                        break;
                    }

                case "DefaultAppsPage":
                    {
                        if (App.RegistryHelper != null && App.MainRegistryHelper.AllowsRegistryEditing() && App.MainRegistryHelper.IsLocal())
                        {
                            RootFrame.Navigate(typeof(DefaultAppsPage));
                        }

                        break;
                    }

                case "SSHAccountManagerPage":
                    {
                        if (App.RegistryHelper != null && App.MainRegistryHelper.AllowsRegistryEditing())
                        {
                            RootFrame.Navigate(typeof(SSHAccountManagerPage));
                        }

                        break;
                    }

                case "ConsolePage":
                    {
                        if (App.RegistryHelper != null && App.MainRegistryHelper.AllowsRegistryEditing())
                        {
                            RootFrame.Navigate(typeof(ConsolePage));
                        }

                        break;
                    }

                case "AppManagerPage":
                    {
                        if (App.RegistryHelper != null && App.MainRegistryHelper.IsLocal())
                        {
                            RootFrame.Navigate(typeof(AppManagerPage));
                        }

                        break;
                    }

                case "CertificatesPage":
                    {
                        if (App.RegistryHelper != null && App.MainRegistryHelper.IsLocal())
                        {
                            RootFrame.Navigate(typeof(CertificatesPage));
                        }

                        break;
                    }

                case "YourDevicePage":
                    {
                        if (App.RegistryHelper != null && App.MainRegistryHelper.IsLocal())
                        {
                            RootFrame.Navigate(typeof(YourDevicePage));
                        }

                        break;
                    }
                default:
                    {
                        RootFrame.Navigate(typeof(WelcomePage));
                        break;
                    }
            }
        }

        public async void Load(object args)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += OnBackPressed;
            }

            Loaded += Shell_Loaded;
            ShellViewModel vm = new();

            RootFrame.Navigated += RootFrame_Navigated;
            RootFrame.NavigationFailed += OnNavigationFailed;

            vm.TopItems.Add(new NavigationItem
            {
                Icon = "",
                DisplayName = InteropTools.Resources.TextResources.Shell_WelcomeTitle,
                Description = InteropTools.Resources.TextResources.Shell_WelcomeDesc,
                PageType = typeof(WelcomePage),
                GroupName = "Core",
                GroupIcon = ""
            });

            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Extensions", Description = "View and manage extensions", PageType = typeof(ExtensionsPage), GroupName = "Core", GroupIcon = "" });

            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Store", Description = "Store", PageType = typeof(StorePage), GroupName = "Core", GroupIcon = "" });

            vm.BottomItems.Add(new NavigationItem
            {
                Icon = "",
                DisplayName = InteropTools.Resources.TextResources.Shell_SettingsTitle,
                Description = InteropTools.Resources.TextResources.Shell_SettingsDesc,
                PageType = typeof(SettingsPage)
            });

            vm.SelectedItem = vm.TopItems.First();
            ViewModel = vm;

            _initialized = true;
            Windows.Foundation.Rect size = Window.Current.Bounds;
            await new PickProviderContentDialog().ShowAsync();

            if (App.MainRegistryHelper.AllowsRegistryEditing())
            {
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = InteropTools.Resources.TextResources.Shell_RegistryEditorTitle,
                    Description = InteropTools.Resources.TextResources.Shell_RegistryEditorDesc,
                    PageType = typeof(RegistryEditorPage),
                    GroupName = InteropTools.Resources.TextResources.Shell_RegistryGroupName,
                    GroupIcon = ""
                });
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = InteropTools.Resources.TextResources.Shell_RegistryBrowserTitle,
                    Description = InteropTools.Resources.TextResources.Shell_RegistryBrowserDesc,
                    PageType = typeof(RegistryBrowserPage),
                    GroupName = InteropTools.Resources.TextResources.Shell_RegistryGroupName,
                    GroupIcon = ""
                });
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = InteropTools.Resources.TextResources.Shell_RegistrySearchTitle,
                    Description = InteropTools.Resources.TextResources.Shell_RegistrySearchDesc,
                    PageType = typeof(RegistrySearchPage),
                    GroupName = InteropTools.Resources.TextResources.Shell_RegistryGroupName,
                    GroupIcon = ""
                });
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = InteropTools.Resources.TextResources.Shell_ImportRegFileTitle,
                    Description = InteropTools.Resources.TextResources.Shell_ImportRegFileDesc,
                    PageType = typeof(ImportRegFilePage),
                    GroupName = InteropTools.Resources.TextResources.Shell_RegistryGroupName,
                    GroupIcon = ""
                });
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = InteropTools.Resources.TextResources.Shell_InteropUnlockTitle,
                    Description = InteropTools.Resources.TextResources.Shell_InteropUnlockDesc,
                    PageType = typeof(InteropUnlockPage),
                    GroupName = InteropTools.Resources.TextResources.Shell_UnlockGroupName,
                    GroupIcon = ""
                });
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = "Lumia x50 System Access Unlocker",
                    Description = "Lumia x50 System Access Unlocker",
                    PageType = typeof(x50PlusDevicesNDTKUnlock),
                    GroupName = InteropTools.Resources.TextResources.Shell_UnlockGroupName,
                    GroupIcon = ""
                });
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = InteropTools.Resources.TextResources.Shell_TweaksTitle,
                    Description = InteropTools.Resources.TextResources.Shell_TweaksDesc,
                    PageType = typeof(TweaksPage),
                    GroupName = InteropTools.Resources.TextResources.Shell_TweakGroupName,
                    GroupIcon = ""
                });
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = InteropTools.Resources.TextResources.Shell_KeyboardOptionsTitle,
                    Description = InteropTools.Resources.TextResources.Shell_KeyboardOptionsDesc,
                    PageType = typeof(KeyboardCarretPage),
                    GroupName = InteropTools.Resources.TextResources.Shell_TweakGroupName,
                    GroupIcon = ""
                });

                if (App.MainRegistryHelper.IsLocal())
                {
                    try
                    {
                        IEnumerable<PackageVolume> pkgs = new PackageManager().FindPackageVolumes();
                        vm.TopItems.Add(new NavigationItem
                        {
                            Icon = "",
                            DisplayName = ResourceManager.Current.MainResourceMap.GetValue("Resources/Default_apps", ResourceContext.GetForCurrentView()).ValueAsString,
                            Description = ResourceManager.Current.MainResourceMap.GetValue("Resources/View_and_manage_default_applications", ResourceContext.GetForCurrentView()).ValueAsString,
                            PageType = typeof(DefaultAppsPage),
                            GroupName = InteropTools.Resources.TextResources.Shell_GeneralGroupName,
                            GroupIcon = ""
                        });
                    }
                    catch
                    {
                    }
                }

                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = InteropTools.Resources.TextResources.Shell_SSHAccountManagerTitle,
                    Description = InteropTools.Resources.TextResources.Shell_SSHAccountManagerDesc,
                    PageType = typeof(SSHAccountManagerPage),
                    GroupName = InteropTools.Resources.TextResources.Shell_SSHGroupName,
                    GroupIcon = ""
                });
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = InteropTools.Resources.TextResources.Shell_ConsoleTitle,
                    Description = InteropTools.Resources.TextResources.Shell_ConsoleDesc,
                    PageType = typeof(ConsolePage),
                    GroupName = InteropTools.Resources.TextResources.Shell_SSHGroupName,
                    GroupIcon = ""
                });
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = "Your Windows Build",
                    Description = "View information about your windows build",
                    PageType = typeof(YourWindowsBuildPage),
                    GroupName = InteropTools.Resources.TextResources.Shell_RegistryGroupName,
                    GroupIcon = ""
                });
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = "Notification LED",
                    Description = "Notification LED",
                    PageType = typeof(NotificationLEDPage),
                    GroupName = InteropTools.Resources.TextResources.Shell_TweakGroupName,
                    GroupIcon = ""
                });
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = "Registry Browser vNext",
                    Description = "WIP",
                    PageType = typeof(BrowservNextPage),
                    GroupName = InteropTools.Resources.TextResources.Shell_RegistryGroupName,
                    GroupIcon = ""
                });
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = "Registry History",
                    Description = "WIP",
                    PageType = typeof(RegistryHistory),
                    GroupName = InteropTools.Resources.TextResources.Shell_RegistryGroupName,
                    GroupIcon = ""
                });
            }

            if (App.MainRegistryHelper.IsLocal())
            {
                try
                {
                    IEnumerable<PackageVolume> pkgs = new PackageManager().FindPackageVolumes();
                    vm.TopItems.Add(new NavigationItem
                    {
                        Icon = "",
                        DisplayName = InteropTools.Resources.TextResources.Shell_ApplicationsTitle,
                        Description = InteropTools.Resources.TextResources.Shell_ApplicationsDescription,
                        PageType = typeof(AppManagerPage),
                        GroupName = InteropTools.Resources.TextResources.Shell_GeneralGroupName,
                        GroupIcon = ""
                    });
                }
                catch
                {
                }
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = InteropTools.Resources.TextResources.Shell_CertificatesTitle,
                    Description = InteropTools.Resources.TextResources.Shell_CertificatesDesc,
                    PageType = typeof(CertificatesPage),
                    GroupName = InteropTools.Resources.TextResources.Shell_GeneralGroupName,
                    GroupIcon = ""
                });
                vm.TopItems.Add(new NavigationItem
                {
                    Icon = "",
                    DisplayName = InteropTools.Resources.TextResources.Shell_DeviceInfoTitle,
                    Description = InteropTools.Resources.TextResources.Shell_DeviceInfoDesc,
                    PageType = typeof(YourDevicePage),
                    GroupName = InteropTools.Resources.TextResources.Shell_GeneralGroupName,
                    GroupIcon = ""
                });
            }

            vm.SelectedItem = vm.TopItems.First();

            if (args.GetType() == typeof(string))
            {
                HandleLaunchedEvent(args as string);
            }
            else if (args.GetType() == typeof(StorageFile))
            {
                StorageFile file = (StorageFile)args;
                await HandleFileActivatedEvent(file);
            }

            try
            {
                ApplicationData applicationData = ApplicationData.Current;
                ApplicationDataContainer localSettings = applicationData.LocalSettings;
                object value = localSettings.Values["recentitems_count"];

                if (value != null)
                {
                    for (int i = 0; i <= (int)value; i++)
                    {
                        if (localSettings.Values["recentitems_" + i + "_pagetype"] != null)
                        {
                            try
                            {
                                IEnumerable<NavigationItem> items = ViewModel.TopItems.Where(x => x.PageType.Name == (string)localSettings.Values["recentitems_" + i + "_pagetype"]);
                                recentitems.AddRange(items);
                            }
                            catch
                            {
                            }

                            try
                            {
                                IEnumerable<NavigationItem> items2 = ViewModel.BottomItems.Where(x => x.PageType.Name == (string)localSettings.Values["recentitems_" + i + "_pagetype"]);
                                recentitems.AddRange(items2);
                            }
                            catch
                            {
                            }
                        }
                    }
                }

                object value2 = localSettings.Values["recentitems5_count"];

                if (value2 != null)
                {
                    for (int i = 0; i <= (int)value2; i++)
                    {
                        if (localSettings.Values["recentitems5_" + i + "_pagetype"] != null)
                        {
                            try
                            {
                                IEnumerable<NavigationItem> items = ViewModel.TopItems.Where(x => x.PageType.Name == (string)localSettings.Values["recentitems5_" + i + "_pagetype"]);
                                recentitems5max.AddRange(items);
                            }
                            catch
                            {
                            }

                            try
                            {
                                IEnumerable<NavigationItem> items2 = ViewModel.BottomItems.Where(x => x.PageType.Name == (string)localSettings.Values["recentitems5_" + i + "_pagetype"]);
                                recentitems5max.AddRange(items2);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            recentitems.CollectionChanged += Recentitems_CollectionChanged;
            recentitems5max.CollectionChanged += Recentitems5max_CollectionChanged;

            CheckAndUnlockSSH();
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                RootFrame.Navigate((args.ChosenSuggestion as NavigationItem)?.PageType);
                controlsSearchBox.Text = "";
            }
            else
            {
                // Use args.QueryText to determine what to do.
                //Set the ItemsSource to be your filtered dataset
                List<NoResultsItem> noresultitems = new()
                {
                    new NoResultsItem()
                };
                List<NavigationItem> resultitems = new();

                foreach (NavigationItem item in ViewModel.TopItems)
                {
                    if (item.DisplayName.IndexOf(sender.Text, StringComparison.OrdinalIgnoreCase) >= 0 || item.Description.IndexOf(sender.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        resultitems.Add(item);
                    }
                }

                foreach (NavigationItem item in ViewModel.BottomItems)
                {
                    if (item.DisplayName.IndexOf(sender.Text, StringComparison.OrdinalIgnoreCase) >= 0 || item.Description.IndexOf(sender.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        resultitems.Add(item);
                    }
                }

                sender.ItemsSource = resultitems;

                if (resultitems.Count == 0)
                {
                    sender.ItemsSource = noresultitems;
                }
            }
        }

        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            try
            {
                RootFrame.Navigate((args.SelectedItem as NavigationItem)?.PageType);
            }
            catch
            {
            }

            controlsSearchBox.Text = "";
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing,
            // otherwise assume the value got filled in by TextMemberPath
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                List<NoResultsItem> noresultitems = new()
                {
                    new NoResultsItem()
                };
                List<NavigationItem> resultitems = new();

                foreach (NavigationItem item in ViewModel.TopItems)
                {
                    if (item.DisplayName.IndexOf(sender.Text, StringComparison.OrdinalIgnoreCase) >= 0 || item.Description.IndexOf(sender.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        resultitems.Add(item);
                    }
                }

                foreach (NavigationItem item in ViewModel.BottomItems)
                {
                    if (item.DisplayName.IndexOf(sender.Text, StringComparison.OrdinalIgnoreCase) >= 0 || item.Description.IndexOf(sender.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        resultitems.Add(item);
                    }
                }

                sender.ItemsSource = resultitems;

                if (resultitems.Count == 0)
                {
                    sender.ItemsSource = noresultitems;
                }
            }
        }

        private async void CheckAndUnlockSSH()
        {
            if (App.MainRegistryHelper.AllowsRegistryEditing())
            {
                bool useCMD = await SessionManager.IsCMDSupported();

                if (useCMD)
                {
                    App.RegistryHelper = new CCMDProvider();
                }
            }
        }

        private NavigationItem FindPageItem(Type pagetype)
        {
            foreach (NavigationItem item in ViewModel.TopItems)
            {
                if (item.PageType == pagetype)
                {
                    return item;
                }
            }

            foreach (NavigationItem item in ViewModel.BottomItems)
            {
                if (item.PageType == pagetype)
                {
                    return item;
                }
            }

            return null;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.IsSplitViewPaneOpen = true;
        }

        private async void ListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            await new SelectSessionContentDialog().ShowAsync();
        }

        private void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            if (App.AppContent is not Shell shell)
            {
                return;
            }

            if (shell.RootFrame.SourcePageType == typeof(RegistryBrowserPage))
            {
                return;
            }

            if (shell.RootFrame.CanGoBack)
            {
                shell.RootFrame.GoBack();
            }

            e.Handled = true;
        }

        private void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (App.AppContent is not Shell shell)
            {
                return;
            }

            if (shell.RootFrame.SourcePageType == typeof(RegistryBrowserPage))
            {
                return;
            }

            if (shell.RootFrame.CanGoBack)
            {
                shell.RootFrame.GoBack();
            }

            e.Handled = true;
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void Recentitems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_initialized)
            {
                ApplicationData applicationData = ApplicationData.Current;
                ApplicationDataContainer localSettings = applicationData.LocalSettings;
                object val = localSettings.Values["recentitems_count"];

                if (val != null)
                {
                    for (int i = 0; i <= (int)val; i++)
                    {
                        localSettings.Values["recentitems_" + i + "_pagetype"] = null;
                    }
                }

                localSettings.Values["recentitems_count"] = null;

                foreach (NavigationItem item in recentitems)
                {
                    object value = localSettings.Values["recentitems_count"];

                    if (value == null)
                    {
                        localSettings.Values["recentitems_count"] = 0;
                    }
                    else
                    {
                        localSettings.Values["recentitems_count"] = (int)localSettings.Values["recentitems_count"] + 1;
                    }

                    localSettings.Values["recentitems_" + (int)localSettings.Values["recentitems_count"] + "_pagetype"] = item.PageType.Name;
                }
            }
        }

        private void Recentitems5max_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (_initialized)
            {
                ApplicationData applicationData = ApplicationData.Current;
                ApplicationDataContainer localSettings = applicationData.LocalSettings;
                object val = localSettings.Values["recentitems5_count"];

                if (val != null)
                {
                    for (int i = 0; i <= (int)val; i++)
                    {
                        localSettings.Values["recentitems5_" + i + "_pagetype"] = null;
                    }
                }

                localSettings.Values["recentitems5_count"] = null;

                foreach (NavigationItem item in recentitems5max)
                {
                    object value = localSettings.Values["recentitems5_count"];

                    if (value == null)
                    {
                        localSettings.Values["recentitems5_count"] = 0;
                    }
                    else
                    {
                        localSettings.Values["recentitems5_count"] = (int)localSettings.Values["recentitems5_count"] + 1;
                    }

                    localSettings.Values["recentitems5_" + (int)localSettings.Values["recentitems5_count"] + "_pagetype"] = item.PageType.Name;
                }
            }
        }

        private void RootFrame_Navigated(object sender, NavigationEventArgs e)
        {
            GC.Collect();
            UpdateBackButtonVisibility();
            NavigationItem item = FindPageItem(e.SourcePageType);

            if (item == null)
            {
                return;
            }

            if (item.PageType != typeof(WelcomePage))
            {
                if (recentitems.Contains(item))
                {
                    recentitems.Remove(item);
                }

                recentitems.Insert(0, item);
            }

            if (item.PageType != typeof(WelcomePage))
            {
                if (recentitems5max.Contains(item))
                {
                    recentitems5max.Remove(item);
                }

                recentitems5max.Insert(0, item);

                if (recentitems5max.Count > 5)
                {
                    recentitems5max.RemoveRange(recentitems5max.Reverse().Take(recentitems5max.Count - 5));
                }
            }
        }

        private async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => function());
        }

        private async void RunInUIThread(Action function)
        {
            await
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => function());
        }

        private void SampleTreeView2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0 && ((e.AddedItems[0] as TreeNode2)?.Data as NavigationItemData)?.IsGroup == false)
                {
                    ViewModel.SelectedTopItem = ((e.AddedItems[0] as TreeNode2)?.Data as NavigationItemData)?.NavigationItem;
                }
                else
                    if (ViewModel.SelectedTopItem == ViewModel.SelectedItem)
                {
                    (sender as TreeView2).SelectedItem = treeviewnodes.First(x => (x.Data as NavigationItemData)?.NavigationItem == ViewModel.SelectedTopItem);

                    if ((sender as TreeView2)?.SelectedItem != treeviewnodes.First(x => (x.Data as NavigationItemData)?.NavigationItem == ViewModel.SelectedTopItem))
                    {
                        (sender as TreeView2).SelectedItem = null;
                    }
                }
                else
                {
                    (sender as TreeView2).SelectedItem = null;
                }
            }
            catch
            {
                (sender as TreeView2).SelectedItem = null;
            }
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            new SettingsViewModel();
        }

        private void UpdateBackButtonVisibility()
        {
            Shell shell = (Shell)App.AppContent;
            AppViewBackButtonVisibility visibility = AppViewBackButtonVisibility.Collapsed;

            if (shell.RootFrame.CanGoBack)
            {
                visibility = AppViewBackButtonVisibility.Visible;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = visibility;
        }

        public class GroupItem
        {
            public GroupItem(NavigationItem item, bool shouldShowName)
            {
                if (item.GroupName == null)
                {
                    Visibility = Visibility.Collapsed;
                }

                if (!shouldShowName)
                {
                    Visibility = Visibility.Collapsed;
                }

                Icon = item.GroupIcon;
                DisplayName = item.GroupName;
            }

            public string DisplayName
            {
                get;
                set;
            }

            public string Icon
            {
                get;
                set;
            }

            public Visibility Visibility
            {
                get;
                set;
            }

            public override bool Equals(object obj)
            {
                if (obj is not GroupItem eqobj)
                {
                    return false;
                }

                return eqobj.DisplayName == DisplayName;
            }

            public override int GetHashCode()
            {
                int hash = 23;

                if (DisplayName != null)
                {
                    hash = (hash * 31) + DisplayName.GetHashCode();
                }

                return hash;
            }
        }

        public class NoResultsItem
        {
            public string Description => "No results have been found based on your search criteria";
            public string DisplayName => "No results";
            public string Icon => "";
        }
    }
}