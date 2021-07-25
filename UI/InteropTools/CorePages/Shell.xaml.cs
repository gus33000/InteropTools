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
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace InteropTools.CorePages
{
    public sealed partial class Shell : UserControl
    {

        private double titlebarheight = 0;

        public string _SwitchSession = InteropTools.Resources.TextResources.Shell_SwitchSession;

        private bool _initialized = false;

        private async void RunInUIThread(Action function)
        {
            await
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
            {
                function();
            });
        }

        private async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x =>
            {
                function();
            });
        }

        private void UpdateBackButtonVisibility()
        {
            Shell shell = (Shell)App.AppContent;
            AppViewBackButtonVisibility visibility = AppViewBackButtonVisibility.Collapsed;
            BackButtonBg.Visibility = Visibility.Collapsed;

            if (shell.RootFrame.CanGoBack)
            {
                visibility = AppViewBackButtonVisibility.Visible;
                BackButtonBg.Visibility = Visibility.Visible;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = visibility;
        }

        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        public void ReSetupTitlebar()
        {
            SetupTitleBar();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged1;
            titlebarheight = CoreApplication.GetCurrentView().TitleBar.Height;
            Window.Current.SetTitleBar(CustomTitleBar);
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

            CoreApplication.GetCurrentView().TitleBar.IsVisibleChanged += TitleBar_IsVisibleChanged;
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged;

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

            IEnumerable<IGrouping<GroupItem, NavigationItem>> groups = from c in vm.TopItems
                                                                       group c by new GroupItem(c, true);
            topitems.Source = groups;

            sampleTreeView2.AllowDrop = false;
            sampleTreeView2.CanDrag = false;
            sampleTreeView2.CanDragItems = false;
            sampleTreeView2.CanReorderItems = false;

            foreach (IGrouping<GroupItem, NavigationItem> element in groups)
            {
                GroupItem groupitem = element.Key;
                TreeNode2 groupnode = new()
                {
                    Data = new NavigationItemData()
                    {
                        GroupItem = groupitem
                    }
                };

                if (!sampleTreeView2.RootNode.Contains(groupnode))
                { sampleTreeView2.RootNode.Add(groupnode); }

                foreach (NavigationItem item in element)
                {
                    TreeNode2 itemnode = new()
                    {
                        Data = new NavigationItemData()
                        {
                            NavigationItem = item
                        }
                    };
                    treeviewnodes.Add(itemnode);
                    groupnode.Add(itemnode);
                }
            }

            IEnumerable<IGrouping<GroupItem, NavigationItem>> groups2 = from c in vm.TopItems
                                                                        group c by new GroupItem(c, false);
            topitems2.Source = groups2;
            vm.SelectedItem = vm.TopItems.First();
            ViewModel = vm;
            ViewModel.SelectedTopItem.PropertyChanged += SelectedTopItem_PropertyChanged;
            ViewModel.SelectedItem.PropertyChanged += SelectedItem_PropertyChanged;

            _initialized = true;
            SetupTitleBar();
            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
            CoreApplication.GetCurrentView().TitleBar.LayoutMetricsChanged += TitleBar_LayoutMetricsChanged1;
            titlebarheight = CoreApplication.GetCurrentView().TitleBar.Height;
            Window.Current.SetTitleBar(CustomTitleBar);
            Window.Current.SizeChanged += Current_SizeChanged;
            Window.Current.Activated += Current_Activated;
            Redraw(true);
            Windows.Foundation.Rect size = Window.Current.Bounds;
            if (SplitView.IsSwipeablePaneOpen)
            {
                SearchListButton.Visibility = Visibility.Collapsed;
                SearchBox.Visibility = Visibility.Visible;

                sampleTreeView2.Visibility = Visibility.Visible;
                TopList2.Visibility = Visibility.Collapsed;
            }
            else
            {
                SearchListButton.Visibility = Visibility.Visible;
                SearchBox.Visibility = Visibility.Collapsed;

                if (SplitView.DisplayMode != SplitViewDisplayMode.Overlay)
                {
                    sampleTreeView2.Visibility = Visibility.Collapsed;
                    TopList2.Visibility = Visibility.Visible;
                }
            }
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

            groups = from c in vm.TopItems
                     group c by new GroupItem(c, true);
            topitems.Source = groups;

            sampleTreeView2.AllowDrop = false;
            sampleTreeView2.CanDrag = false;
            sampleTreeView2.CanDragItems = false;
            sampleTreeView2.CanReorderItems = false;

            sampleTreeView2.RootNode.Clear();

            foreach (IGrouping<GroupItem, NavigationItem> element in groups)
            {
                GroupItem groupitem = element.Key;
                TreeNode2 groupnode = new()
                {
                    Data = new NavigationItemData()
                    {
                        GroupItem = groupitem
                    }
                };

                if (!sampleTreeView2.RootNode.Contains(groupnode))
                { sampleTreeView2.RootNode.Add(groupnode); }

                foreach (NavigationItem item in element)
                {
                    TreeNode2 itemnode = new()
                    {
                        Data = new NavigationItemData()
                        {
                            NavigationItem = item
                        }
                    };
                    treeviewnodes.Add(itemnode);
                    groupnode.Add(itemnode);
                }
            }

            groups2 = from c in vm.TopItems
                      group c by new GroupItem(c, false);
            topitems2.Source = groups2;
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

        public Shell(object args)
        {
            InitializeComponent();
            Load(args);
        }

        private void SelectedItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ViewModel.SelectedItem == ViewModel.SelectedBottomItem)
            {
                sampleTreeView2.SelectedItem = null;
            }
        }

        public ObservableRangeCollection<TreeNode2> treeviewnodes = new();

        private void SelectedTopItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (ViewModel.SelectedItem == ViewModel.SelectedTopItem)
            {
                sampleTreeView2.SelectedItem = treeviewnodes.First(x => (x.Data as NavigationItemData).NavigationItem == ViewModel.SelectedTopItem);
            }

            else
            {
                sampleTreeView2.SelectedItem = null;
            }
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            new SettingsViewModel();
            UpdateBackButtonVisibility();
        }

        private void SetupTitleBar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                StatusBar.GetForCurrentView().BackgroundOpacity = 0;
            }

            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
            ApplicationViewTitleBar titlebar = ApplicationView.GetForCurrentView().TitleBar;
            SolidColorBrush transparentColorBrush = new() { Opacity = 0 };
            Color transparentColor = transparentColorBrush.Color;
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

            Color hovercolor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush).Color;
            hovercolor.A = 32;
            titlebar.ButtonHoverBackgroundColor = hovercolor;
            titlebar.ButtonHoverForegroundColor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush).Color;
            hovercolor.A = 64;
            titlebar.ButtonPressedBackgroundColor = hovercolor;
            titlebar.ButtonPressedForegroundColor = (Application.Current.Resources["ApplicationForegroundThemeBrush"] as SolidColorBrush).Color;
        }

        private void TitleBar_LayoutMetricsChanged1(CoreApplicationViewTitleBar sender, object args)
        {
            titlebarheight = CoreApplication.GetCurrentView().TitleBar.Height;
            Redraw(true);
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

        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            try
            {
                Shell shell = (Shell)App.AppContent;
                shell.Redraw(false);
            }

            catch
            {
                // ignored
            }
        }

        private void Current_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            try
            {
                Shell shell = (Shell)App.AppContent;
                shell.Redraw(false);
            }

            catch
            {
                // ignored
            }
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

        private void RootFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
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

        public ObservableRangeCollection<NavigationItem> recentitems = new();
        public ObservableRangeCollection<NavigationItem> recentitems5max = new();

        private async void CheckAndUnlockSSH()
        {
            if (App.MainRegistryHelper.AllowsRegistryEditing())
            {
                bool useCMD = await App.IsCMDSupported();

                if (useCMD)
                {
                    App.RegistryHelper = new CCMDProvider();
                }
            }
        }

        private void TitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            Redraw(true);
        }

        private void TitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            Redraw(true);
        }

        private string AppTitle
        {
            get
            {
                string title = ApplicationView.GetForCurrentView().Title;
                return title == "" ? Package.Current.DisplayName : title;
            }
        }

        private string UpperCaseAppTitle
        {
            get
            {
                string title = ApplicationView.GetForCurrentView().Title.ToUpper();
                return title == "" ? Package.Current.DisplayName.ToUpper() : title;
            }
        }

        public ShellViewModel ViewModel
        {
            get;
            private set;
        }

        public Frame RootFrame => Frame;

        public string mode;

        public void Redraw(bool ignoresizecheck)
        {
            Windows.Foundation.Rect size = Window.Current.Bounds;
            CustomTitleBarPanel.Height = titlebarheight;
            string currentmode;
            if (size.Width >= 1024)
            {
                currentmode = "big";
            }

            else
                if (size.Width >= 720)
            {
                currentmode = "medium";
            }

            else
            {
                currentmode = "small";
            }

            if (mode == null || mode != currentmode || ignoresizecheck)
            {
                double tmpvalue = 0;

                mode = currentmode;

                if (size.Width >= 1024)
                {
                    BackDrop.Visibility = Visibility.Collapsed;
                    Acrylic.Visibility = Visibility.Visible;

                    SplitView.DisplayMode = SplitViewDisplayMode.CompactInline;
                    SplitView.OpenPaneLength = 320;
                    NarrowHeader.Visibility = Visibility.Collapsed;
                    NarrowHeaderBg.Visibility = Visibility.Collapsed;
                    PaneHeader.Visibility = Visibility.Visible;
                    Thickness padding = Frame.Padding;
                    padding.Left = 48;
                    padding.Top = 6;
                    padding.Right = 48;
                    padding.Bottom = 0;
                    Frame.Padding = padding;
                    Frame.PageTitleVisibility = Visibility.Visible;
                    TitlebarBackground.Visibility = Visibility.Collapsed;

                    if (CoreApplication.GetCurrentView().TitleBar.IsVisible)
                    {
                        CustomTitleBar.SetValue(Grid.RowProperty, 1);
                        padding.Left = 0;
                        padding.Top = titlebarheight;
                        padding.Right = 0;
                        padding.Bottom = 0;
                        SplitViewPaneGrid.Padding = padding;
                        Thickness margin = Frame.Margin;
                        margin.Left = 0;
                        margin.Top = titlebarheight;
                        margin.Right = 0;
                        margin.Bottom = 0;
                        NarrowHeader.Margin = margin;
                        NarrowHeaderBg.Margin = margin;
                        margin.Top += tmpvalue;
                        Frame.Margin = margin;
                        CustomTitleBar.Visibility = Visibility.Visible;
                    }

                    else
                    {
                        CustomTitleBar.SetValue(Grid.RowProperty, 1);
                        padding.Left = 0;
                        padding.Top = 0;
                        padding.Right = 0;
                        padding.Bottom = 0;
                        SplitViewPaneGrid.Padding = padding;
                        Thickness margin = Frame.Margin;
                        margin.Left = 0;
                        margin.Top = 0;
                        margin.Right = 0;
                        margin.Bottom = 0;
                        NarrowHeader.Margin = margin;
                        NarrowHeaderBg.Margin = margin;
                        margin.Top += tmpvalue;
                        Frame.Margin = margin;
                        CustomTitleBar.Visibility = Visibility.Collapsed;
                    }
                }

                else
                    if (size.Width >= 720)
                {
                    BackDrop.Visibility = Visibility.Visible;
                    Acrylic.Visibility = Visibility.Collapsed;

                    SplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                    SplitView.OpenPaneLength = 256;
                    NarrowHeader.Visibility = Visibility.Collapsed;
                    NarrowHeaderBg.Visibility = Visibility.Collapsed;
                    PaneHeader.Visibility = Visibility.Visible;
                    Thickness padding = Frame.Padding;
                    padding.Left = 48;
                    padding.Top = 6;
                    padding.Right = 48;
                    padding.Bottom = 0;
                    Frame.Padding = padding;
                    Frame.PageTitleVisibility = Visibility.Visible;
                    TitlebarBackground.Visibility = Visibility.Collapsed;

                    if (CoreApplication.GetCurrentView().TitleBar.IsVisible)
                    {
                        CustomTitleBar.SetValue(Grid.RowProperty, 1);
                        padding.Left = 0;
                        padding.Top = titlebarheight;
                        padding.Right = 0;
                        padding.Bottom = 0;
                        SplitViewPaneGrid.Padding = padding;
                        Thickness margin = Frame.Margin;
                        margin.Left = 0;
                        margin.Top = titlebarheight;
                        margin.Right = 0;
                        margin.Bottom = 0;
                        NarrowHeader.Margin = margin;
                        NarrowHeaderBg.Margin = margin;
                        margin.Top += tmpvalue;
                        Frame.Margin = margin;
                        CustomTitleBar.Visibility = Visibility.Visible;
                    }

                    else
                    {
                        CustomTitleBar.SetValue(Grid.RowProperty, 1);
                        padding.Left = 0;
                        padding.Top = 0;
                        padding.Right = 0;
                        padding.Bottom = 0;
                        SplitViewPaneGrid.Padding = padding;
                        Thickness margin = Frame.Margin;
                        margin.Left = 0;
                        margin.Top = 0;
                        margin.Right = 0;
                        margin.Bottom = 0;
                        NarrowHeader.Margin = margin;
                        NarrowHeaderBg.Margin = margin;
                        margin.Top += tmpvalue;
                        Frame.Margin = margin;
                        CustomTitleBar.Visibility = Visibility.Collapsed;
                    }
                }

                else
                {
                    BackDrop.Visibility = Visibility.Visible;
                    Acrylic.Visibility = Visibility.Collapsed;

                    SplitView.DisplayMode = SplitViewDisplayMode.Overlay;
                    NarrowHeader.Visibility = Visibility.Visible;
                    NarrowHeaderBg.Visibility = Visibility.Visible;
                    PaneHeader.Visibility = Visibility.Collapsed;
                    SplitView.OpenPaneLength = 256;
                    Thickness padding = Frame.Padding;
                    padding.Left = 12;
                    padding.Top = 12;
                    padding.Right = 12;
                    padding.Bottom = 0;
                    Frame.Padding = padding;
                    Frame.PageTitleVisibility = Visibility.Collapsed;
                    TitlebarBackground.Visibility = Visibility.Visible;

                    if (CoreApplication.GetCurrentView().TitleBar.IsVisible)
                    {
                        CustomTitleBar.SetValue(Grid.RowProperty, 0);
                        padding.Left = 0;
                        padding.Top = 0;
                        padding.Right = 0;
                        padding.Bottom = 0;
                        SplitViewPaneGrid.Padding = padding;
                        Thickness margin = Frame.Margin;
                        margin.Left = 0;
                        margin.Top = 0;
                        margin.Right = 0;
                        margin.Bottom = 0;
                        Frame.Margin = margin;
                        CustomTitleBar.Visibility = Visibility.Visible;
                        margin.Left = 0;
                        margin.Top = titlebarheight;
                        margin.Right = 0;
                        margin.Bottom = 0;
                        NarrowHeader.Margin = margin;
                        NarrowHeaderBg.Margin = margin;
                    }

                    else
                    {
                        CustomTitleBar.SetValue(Grid.RowProperty, 0);
                        padding.Left = 0;
                        padding.Top = 0;
                        padding.Right = 0;
                        padding.Bottom = 0;
                        SplitViewPaneGrid.Padding = padding;
                        Thickness margin = Frame.Margin;
                        margin.Left = 0;
                        margin.Top = 0;
                        margin.Right = 0;
                        margin.Bottom = 0;
                        Frame.Margin = margin;
                        CustomTitleBar.Visibility = Visibility.Collapsed;
                        margin.Left = 0;
                        margin.Top = 0;
                        margin.Right = 0;
                        margin.Bottom = 0;
                        NarrowHeader.Margin = margin;
                        NarrowHeaderBg.Margin = margin;
                    }
                }

                if (SplitView.IsSwipeablePaneOpen)
                {
                    SearchListButton.Visibility = Visibility.Collapsed;
                    SearchBox.Visibility = Visibility.Visible;

                    sampleTreeView2.Visibility = Visibility.Visible;
                    TopList2.Visibility = Visibility.Collapsed;
                }

                else
                {
                    SearchListButton.Visibility = Visibility.Visible;
                    SearchBox.Visibility = Visibility.Collapsed;

                    if (SplitView.DisplayMode != SplitViewDisplayMode.Overlay)
                    {
                        sampleTreeView2.Visibility = Visibility.Collapsed;
                        TopList2.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private async void ListView_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            await new SelectSessionContentDialog().ShowAsync();
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

            public string Icon
            {
                get;
                set;
            }
            public string DisplayName
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
                    hash = hash * 31 + DisplayName.GetHashCode();
                }

                return hash;
            }
        }

        public void HandleLaunchedEvent(string args)
        {
            if (args == "")
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
                        if (App.RegistryHelper != null && (App.MainRegistryHelper.AllowsRegistryEditing() && App.MainRegistryHelper.IsLocal()))
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

        public async Task HandleFileActivatedEvent(StorageFile file)
        {
            if (App.MainRegistryHelper.AllowsRegistryEditing())
            {
                await new ImportRegContentDialog(file).ShowAsync();
            }
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
                    if ((item.DisplayName.ToLower().Contains(sender.Text.ToLower())) || (item.Description.ToLower().Contains(sender.Text.ToLower())))
                    {
                        resultitems.Add(item);
                    }
                }

                foreach (NavigationItem item in ViewModel.BottomItems)
                {
                    if ((item.DisplayName.ToLower().Contains(sender.Text.ToLower())) || (item.Description.ToLower().Contains(sender.Text.ToLower())))
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
                RootFrame.Navigate((args.SelectedItem as NavigationItem).PageType);
            }

            catch
            {
            }

            SearchBox.Text = "";
        }

        public class NoResultsItem
        {
            public string Icon => "";
            public string DisplayName => "No results";
            public string Description => "No results have been found based on your search criteria";
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                RootFrame.Navigate((args.ChosenSuggestion as NavigationItem).PageType);
                SearchBox.Text = "";
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
                    if ((item.DisplayName.ToLower().Contains(sender.Text.ToLower())) || (item.Description.ToLower().Contains(sender.Text.ToLower())))
                    {
                        resultitems.Add(item);
                    }
                }

                foreach (NavigationItem item in ViewModel.BottomItems)
                {
                    if ((item.DisplayName.ToLower().Contains(sender.Text.ToLower())) || (item.Description.ToLower().Contains(sender.Text.ToLower())))
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

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.IsSplitViewPaneOpen = true;
        }

        private void SplitView_PaneOpenChanged(object sender)
        {
            _ = Window.Current.Bounds;

            if (SplitView.IsSwipeablePaneOpen)
            {
                SearchListButton.Visibility = Visibility.Collapsed;
                SearchBox.Visibility = Visibility.Visible;

                sampleTreeView2.Visibility = Visibility.Visible;
                TopList2.Visibility = Visibility.Collapsed;
            }

            else
            {
                SearchListButton.Visibility = Visibility.Visible;
                SearchBox.Visibility = Visibility.Collapsed;

                if (SplitView.DisplayMode != SplitViewDisplayMode.Overlay)
                {
                    sampleTreeView2.Visibility = Visibility.Collapsed;
                    TopList2.Visibility = Visibility.Visible;
                }
            }
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

        private void SampleTreeView2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0 && !((e.AddedItems.First() as TreeNode2).Data as NavigationItemData).IsGroup)
                {
                    ViewModel.SelectedTopItem = ((e.AddedItems.First() as TreeNode2).Data as NavigationItemData).NavigationItem;
                }

                else
                    if (ViewModel.SelectedTopItem == ViewModel.SelectedItem)
                {
                    (sender as TreeView2).SelectedItem = treeviewnodes.First(x => (x.Data as NavigationItemData).NavigationItem == ViewModel.SelectedTopItem);

                    if ((sender as TreeView2).SelectedItem != treeviewnodes.First(x => (x.Data as NavigationItemData).NavigationItem == ViewModel.SelectedTopItem))
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

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ViewModel.SelectedItem == ViewModel.SelectedBottomItem)
                {
                    sampleTreeView2.SelectedItem = null;
                }

                if (ViewModel.SelectedItem == ViewModel.SelectedTopItem)
                {
                    sampleTreeView2.SelectedItem = treeviewnodes.First(x => (x.Data as NavigationItemData).NavigationItem == ViewModel.SelectedTopItem);
                }
            }

            catch
            {

            }
        }

        private void TopList2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ViewModel != null)
                {
                    if (ViewModel.SelectedItem == ViewModel.SelectedBottomItem)
                    {
                        sampleTreeView2.SelectedItem = null;
                    }

                    if (ViewModel.SelectedItem == ViewModel.SelectedTopItem)
                    {
                        sampleTreeView2.SelectedItem = treeviewnodes.First(x => (x.Data as NavigationItemData).NavigationItem == ViewModel.SelectedTopItem);
                    }
                }
            }

            catch
            {

            }
        }
    }
}