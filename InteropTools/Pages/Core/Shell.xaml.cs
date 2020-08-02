using Intense.Presentation;
using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using InteropTools.Presentation;
using Windows.UI.Core;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.Foundation;
using System.Reflection;
using Windows.Graphics.Display;
//using TreeViewControl;
using InteropTools.Pages;
using InteropTools.Handlers;
using System.Collections.Generic;
using InteropTools.Pages.Core;
using InteropTools.Pages.Applications;
using InteropTools.Pages.Certificates;
using InteropTools.Pages.Extras;
using InteropTools.Pages.IO;
using InteropTools.Pages.Registry;
using InteropTools.Pages.SSH;
using InteropTools.Pages.Unlock;
using Windows.System.Profile;

namespace InteropTools
{
    public sealed partial class Shell : Page
    {
        private string Shell_Tools = App.local.Shell_Tools;
        private string Shell_Search = App.local.Shell_Search;

        private ObservableRangeCollection<TreeViewNode> treeviewnodes = new ObservableRangeCollection<TreeViewNode>();

        ViewLifetimeControl thisViewControl;
        CoreDispatcher mainDispatcher;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(ViewLifetimeControl))
            {
                thisViewControl = (ViewLifetimeControl)e.Parameter;
                mainDispatcher = ((App)App.Current).MainDispatcher;

                // When this view is finally release, clean up state
                thisViewControl.Released += ViewLifetimeControl_Released;
                thisViewControl.PropertyChanged += ThisViewControl_PropertyChanged;

                Titlebar.UpdateTitle();
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.IsSplitViewPaneOpen = true;
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Only get results when it was a user typing,
            // otherwise assume the value got filled in by TextMemberPath
            // or the handler for SuggestionChosen.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                //Set the ItemsSource to be your filtered dataset
                List<NoResultsItem> noresultitems = new List<NoResultsItem>();
                noresultitems.Add(new NoResultsItem());
                List<NavigationItem> resultitems = new List<NavigationItem>();

                foreach (var item in ViewModel.TopItems)
                {
                    if ((item.DisplayName.ToLower().Contains(sender.Text.ToLower())) || (item.Description != null && item.Description.ToLower().Contains(sender.Text.ToLower())))
                    {
                        resultitems.Add(item);
                    }
                }

                foreach (var item in ViewModel.BottomItems)
                {
                    if ((item.DisplayName.ToLower().Contains(sender.Text.ToLower())) || (item.Description != null && item.Description.ToLower().Contains(sender.Text.ToLower())))
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
            public string Icon
            {
                get
                {
                    return "";
                }
            }
            public string DisplayName
            {
                get
                {
                    return App.local.Shell_NoResults;
                }
            }
            public string Description
            {
                get
                {
                    return App.local.Shell_NoResultsTip;
                }
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                var item = args.ChosenSuggestion as NavigationItem;
                if (item == null)
                    return;

                // User selected an item from the suggestion list, take an action on it here.
                RootFrame.Navigate((args.ChosenSuggestion as NavigationItem).PageType);
                SearchBox.Text = "";
            }

            else
            {
                // Use args.QueryText to determine what to do.
                //Set the ItemsSource to be your filtered dataset
                List<NoResultsItem> noresultitems = new List<NoResultsItem>();
                noresultitems.Add(new NoResultsItem());
                List<NavigationItem> resultitems = new List<NavigationItem>();

                foreach (var item in ViewModel.TopItems)
                {
                    if ((item.DisplayName.ToLower().Contains(sender.Text.ToLower())) || (item.Description != null && item.Description.ToLower().Contains(sender.Text.ToLower())))
                    {
                        resultitems.Add(item);
                    }
                }

                foreach (var item in ViewModel.BottomItems)
                {
                    if ((item.DisplayName.ToLower().Contains(sender.Text.ToLower())) || (item.Description != null && item.Description.ToLower().Contains(sender.Text.ToLower())))
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

        private void ThisViewControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Title")
                Titlebar.UpdateTitle();
        }

        private async void ViewLifetimeControl_Released(Object sender, EventArgs e)
        {
            ((ViewLifetimeControl)sender).Released -= ViewLifetimeControl_Released;
            // The ViewLifetimeControl object is bound to UI elements on the main thread
            // So, the object must be removed from that thread
            await mainDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                ((App)App.Current).SecondaryViews.Remove(thisViewControl);
            });

            // The released event is fired on the thread of the window
            // it pertains to.
            //
            // It's important to make sure no work is scheduled on this thread
            // after it starts to close (no data binding changes, no changes to
            // XAML, creating new objects in destructors, etc.) since
            // that will throw exceptions
            Window.Current.Close();
        }

        public Shell()
        {
            InitializeComponent();
            Loaded += Shell_Loaded;
            Init();

            var vm = new ShellViewModel();

            bool ismobile = AnalyticsInfo.VersionInfo.DeviceFamily.ToLower() == "windows.mobile";

            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Welcome", PageType = typeof(WelcomePage), GroupIcon = "", GroupName = "Core" });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Providers", PageType = typeof(ProviderPage), GroupIcon = "", GroupName = "Core" });
            if (ismobile)
                vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Store", PageType = typeof(StorePage), GroupIcon = "", GroupName = "Core" });

            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Application Manager", PageType = typeof(ApplicationManagerPage), GroupIcon = "", GroupName = "Applications" });
            if (ismobile)
                vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Application Defaults", PageType = typeof(DefaultApplicationsPage), GroupIcon = "", GroupName = "Applications" });

            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Certificate Manager", PageType = typeof(CertificateManagerPage), GroupIcon = "", GroupName = "Certificates" });

            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Device Information", PageType = typeof(DeviceInformationPage), GroupIcon = "", GroupName = "Extras" });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Device Manager", PageType = typeof(DeviceManagerPage), GroupIcon = "", GroupName = "Extras" });
            if (ismobile)
                vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "IU Manager", PageType = typeof(IUManagerPage), GroupIcon = "", GroupName = "Extras" });
            if (ismobile)
                vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Notification LED", PageType = typeof(NotificationLEDPage), GroupIcon = "", GroupName = "Extras" });
            if (ismobile)
                vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Tweaks", PageType = typeof(TweaksPage), GroupIcon = "", GroupName = "Extras" });
            if (ismobile)
                vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "File Explorer", PageType = typeof(FileExplorerPage), GroupIcon = "", GroupName = "IO" });
            if (ismobile)
                vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "File Search", PageType = typeof(FileSearchPage), GroupIcon = "", GroupName = "IO" });

            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Registry Editor", PageType = typeof(RegistryEditorPage), GroupIcon = "", GroupName = "Registry" });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Registry Explorer", PageType = typeof(RegistryExplorerPage), GroupIcon = "", GroupName = "Registry" });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Registry Import", PageType = typeof(RegistryImportPage), GroupIcon = "", GroupName = "Registry" });
            vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Registry Search", PageType = typeof(RegistrySearchPage), GroupIcon = "", GroupName = "Registry" });

            if (ismobile)
                vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "SSH Console", PageType = typeof(SSHConsolePage), GroupIcon = "", GroupName = "SSH" });
            if (ismobile)
                vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "SSH User Account Manager", PageType = typeof(SSHUserAccountManagerPage), GroupIcon = "", GroupName = "SSH" });

            if (ismobile)
                vm.TopItems.Add(new NavigationItem { Icon = "", DisplayName = "Interop Unlock", PageType = typeof(InteropUnlockPage), GroupIcon = "", GroupName = "Unlock" });

            vm.BottomItems.Add(new NavigationItem { Icon = "", DisplayName = App.local.Settings, Description = App.local.Settings_Desc, PageType = typeof(SettingsPage) });

            // select the first top item
            vm.SelectedItem = vm.TopItems.First();

            ViewModel = vm;

            LoadLists();
        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            new SettingsHandler().Initialize();
        }

        private void LoadLists()
        {
            //var groups = from c in ViewModel.TopItems group c by new GroupItem(c, true);
            //topitems.Source = groups;

            /*TreeView.AllowDrop = false;
            TreeView.CanDrag = false;
            TreeView.CanDragItems = false;
            TreeView.CanReorderItems = false;

            foreach (var element in groups)
            {
                var groupitem = element.Key;
                var groupnode = new TreeViewNode()
                {
                    Content = new NavigationItemData()
                    {
                        GroupItem = groupitem
                    }
                };

                if (!TreeView.RootNodes.Contains(groupnode))
                { TreeView.RootNodes.Add(groupnode); }

                foreach (var item in element)
                {
                    var itemnode = new TreeViewNode()
                    {
                        Content = new NavigationItemData()
                        {
                            NavigationItem = item
                        }
                    };
                    treeviewnodes.Add(itemnode);
                    groupnode.Children.Add(itemnode);
                }
            }*/

            var groups2 = from c in ViewModel.TopItems group c by new GroupItem(c, false);
            topitems2.Source = groups2;

            ViewModel.SelectedTopItem.PropertyChanged += SelectedTopItem_PropertyChanged;
            ViewModel.SelectedItem.PropertyChanged += SelectedItem_PropertyChanged;
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
                var eqobj = obj as GroupItem;

                if (eqobj == null)
                {
                    return false;
                }

                return eqobj.DisplayName == DisplayName;
            }

            public override int GetHashCode()
            {
                var hash = 23;

                if (DisplayName != null)
                {
                    hash = hash * 31 + DisplayName.GetHashCode();
                }

                return hash;
            }
        }

        private void Init()
        {
            Redraw();

            Window.Current.SizeChanged += (object sender, WindowSizeChangedEventArgs e) => Redraw();
            Window.Current.Activated += (object sender, WindowActivatedEventArgs e) => Redraw();

            // hook-up shell root frame navigation events
            RootFrame.NavigationFailed += OnNavigationFailed;
            RootFrame.Navigated += (object sender, NavigationEventArgs e) => UpdateBackButtonVisibility();

            // listen for back button clicks (both soft- and hardware)
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += OnBackPressed;
            }

            UpdateBackButtonVisibility();
        }

        public ShellViewModel ViewModel { get; private set; }

        public Frame RootFrame
        {
            get
            {
                return ShellFrame;
            }
        }

        // handle hardware back button press
        void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            if (RootFrame.CanGoBack)
            {
                e.Handled = true;
                RootFrame.GoBack();
            }
        }

        // handle software back button press
        void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            if (RootFrame.CanGoBack)
            {
                e.Handled = true;
                RootFrame.GoBack();
            }
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

        private void UpdateBackButtonVisibility()
        {
            var visibility = AppViewBackButtonVisibility.Collapsed;
            if (RootFrame.CanGoBack)
            {
                visibility = AppViewBackButtonVisibility.Visible;
            }

            Titlebar.BackButtonVisibility = visibility;
        }
        
        private int? previouswidthcap = null;

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Redraw();
        }

        private void Titlebar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var padding = SplitViewPaneGrid.Padding;
            padding.Top = e.NewSize.Height;

            NarrowHeader.Margin = padding;

            if (Window.Current.Bounds.Width < 720)
                padding.Top = 0;
            SplitViewPaneGrid.Padding = padding;
            ShellGrid.Padding = padding;
        }

        private void Redraw()
        {
            this.SetExtended(NormalBackDropPane, true, true, false, true);
            this.SetExtended(MainContent, true, true, true, true);
            this.SetExtended(ContentGrid, false, true, true, true);
            this.SetShrinked(ShellFrame, false, true, true, true);
            this.SetShrinked(SplitView, true, true, true, true);
            this.SetShrinked(NarrowHeaderGrid, true, true, true, false);

            if (NarrowHeaderGrid.Visibility == Visibility.Visible)
            {
                this.SetShrinked(SplitView, true, false, true, true);
            }

            if (Window.Current.Bounds.Width >= 1024)
            {
                SplitView.DisplayMode = SplitViewDisplayMode.CompactInline;
                if (previouswidthcap != 1024) SplitView.IsSwipeablePaneOpen = true;
                SplitView.OpenPaneLength = 320;
                NarrowHeaderGrid.Visibility = Visibility.Collapsed;
                PaneHeader.Visibility = Visibility.Visible;

                var pad = ShellFrame.Padding;
                pad.Left = 48;
                pad.Top = 6;
                pad.Right = 48;
                pad.Bottom = 6;
                ShellFrame.Padding = pad;

                TitlebarNarrowBackground.Visibility = Visibility.Collapsed;

                ShellFrame.PageTitleVisibility = Visibility.Visible;
                previouswidthcap = 1024;
            }
            else if (Window.Current.Bounds.Width >= 720)
            {
                SplitView.DisplayMode = SplitViewDisplayMode.CompactOverlay;
                if (previouswidthcap != 720) SplitView.IsSwipeablePaneOpen = false;
                SplitView.OpenPaneLength = 256;
                NarrowHeaderGrid.Visibility = Visibility.Collapsed;
                PaneHeader.Visibility = Visibility.Visible;

                var pad = ShellFrame.Padding;
                pad.Left = 48;
                pad.Top = 6;
                pad.Right = 48;
                pad.Bottom = 6;
                ShellFrame.Padding = pad;

                TitlebarNarrowBackground.Visibility = Visibility.Collapsed;

                ShellFrame.PageTitleVisibility = Visibility.Visible;
                previouswidthcap = 720;
            }
            else
            {
                SplitView.DisplayMode = SplitViewDisplayMode.Overlay;
                if (previouswidthcap != 0) SplitView.IsSwipeablePaneOpen = false;
                SplitView.OpenPaneLength = 256;
                NarrowHeaderGrid.Visibility = Visibility.Visible;
                PaneHeader.Visibility = Visibility.Collapsed;

                var pad = ShellFrame.Padding;
                pad.Left = 12;
                pad.Top = 12;
                pad.Right = 12;
                pad.Bottom = 12;
                ShellFrame.Padding = pad;

                TitlebarNarrowBackground.Visibility = Visibility.Visible;

                ShellFrame.PageTitleVisibility = Visibility.Collapsed;
                previouswidthcap = 0;
            }

            HandleSplitViewItems();
        }

        private void TreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandleSplitViewItems();

            try
            {
                if (e.AddedItems.Count > 0 && !((e.AddedItems.First() as TreeViewNode).Content as NavigationItemData).IsGroup)
                {
                    ViewModel.SelectedTopItem = ((e.AddedItems.First() as TreeViewNode).Content as NavigationItemData).NavigationItem;
                }
                else if (ViewModel.SelectedTopItem == ViewModel.SelectedItem)
                {
                    (sender as TreeView).SelectedNodes[0] = treeviewnodes.First(x => (x.Content as NavigationItemData).NavigationItem == ViewModel.SelectedTopItem);

                    if ((sender as TreeView).SelectedNodes[0] != treeviewnodes.First(x => (x.Content as NavigationItemData).NavigationItem == ViewModel.SelectedTopItem))
                    {
                        (sender as TreeView).SelectedNodes.RemoveAt(0);
                    }
                }
                else
                {
                    (sender as TreeView).SelectedNodes.RemoveAt(0);
                }
            }

            catch
            {
                (sender as TreeView).SelectedNodes.RemoveAt(0);
            }
        }

        private void TopList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandleSplitViewItems();

            /*try
            {
                if (ViewModel.SelectedItem == ViewModel.SelectedBottomItem)
                {
                    (sender as TreeView).SelectedNodes.RemoveAt(0);
                }

                if (ViewModel.SelectedItem == ViewModel.SelectedTopItem)
                {
                    TreeView.SelectedNodes.Add(treeviewnodes.First(x => (x.Content as NavigationItemData).NavigationItem == ViewModel.SelectedTopItem));
                }
            }

            catch
            {

            }*/
        }

        private void SelectedItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            HandleSplitViewItems();

            /*if (ViewModel.SelectedItem == ViewModel.SelectedBottomItem)
            {
                TreeView.SelectedNodes.RemoveAt(0);
            }*/
        }

        private void SelectedTopItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            HandleSplitViewItems();

            /*if (ViewModel.SelectedItem == ViewModel.SelectedTopItem)
            {
                TreeView.SelectedNodes.Add(treeviewnodes.First(x => (x.Content as NavigationItemData).NavigationItem == ViewModel.SelectedTopItem));
            }

            else
            {
                TreeView.SelectedNodes.RemoveAt(0);
            }*/
        }

        private void TreeView_ItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
        {
            HandleSplitViewItems();

            /*try
            {
                if (!((args.InvokedItem as TreeViewNode).Content as NavigationItemData).IsGroup)
                {
                    ViewModel.SelectedTopItem = ((args.InvokedItem as TreeViewNode).Content as NavigationItemData).NavigationItem;
                }
                else if (ViewModel.SelectedTopItem == ViewModel.SelectedItem)
                {
                    (sender as TreeView).SelectedNodes[0] = treeviewnodes.First(x => (x.Content as NavigationItemData).NavigationItem == ViewModel.SelectedTopItem);

                    if ((sender as TreeView).SelectedNodes[0] != treeviewnodes.First(x => (x.Content as NavigationItemData).NavigationItem == ViewModel.SelectedTopItem))
                    {
                        (sender as TreeView).SelectedNodes.RemoveAt(0);
                    }
                }
                else
                {
                    (sender as TreeView).SelectedNodes.RemoveAt(0);
                }
            }

            catch
            {
                (sender as TreeView).SelectedNodes.RemoveAt(0);
            }*/
        }

        private void SplitView_PaneOpenChanged(object sender)
        {
            HandleSplitViewItems();
        }

        private void SplitView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            HandleSplitViewItems();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HandleSplitViewItems();
        }

        private void SplitView_FocusDisengaged(Control sender, FocusDisengagedEventArgs args)
        {
            HandleSplitViewItems();
        }

        private void SplitView_FocusEngaged(Control sender, FocusEngagedEventArgs args)
        {
            HandleSplitViewItems();
        }

        private void HandleSplitViewItems()
        {
            if (Window.Current.Bounds.Width >= 720 && !(SplitView.IsSwipeablePaneOpen || SplitView.IsPaneOpen))
            {
                TopList.Visibility = Visibility.Visible;
                //TreeView.Visibility = Visibility.Collapsed;
            }
            else
            {
                //TreeView.Visibility = Visibility.Visible;
                //TopList.Visibility = Visibility.Collapsed;
                TopList.Visibility = Visibility.Visible;
            }

            if (SplitView.IsSwipeablePaneOpen || SplitView.IsPaneOpen)
            {
                SearchListButton.Visibility = Visibility.Collapsed;
                SearchBox.Visibility = Visibility.Visible;
            }
            else
            {
                SearchListButton.Visibility = Visibility.Visible;
                SearchBox.Visibility = Visibility.Collapsed;
            }

            if (Window.Current.Bounds.Width >= 1024)
            {
                // Window is in full mode

                NormalBackDropPane.Visibility = Visibility.Collapsed;
                NormalBackdropSearch.Visibility = Visibility.Collapsed;

                if (!(SplitView.IsSwipeablePaneOpen || SplitView.IsPaneOpen))
                {
                    // Window has Splitview pane closed and is in full mode
                    HostBackdropPane.Visibility = Visibility.Collapsed;
                    HostBackdropSearch.Visibility = Visibility.Collapsed;
                    NormalBackground.Visibility = Visibility.Visible;
                }
                else
                {
                    // Window has Splitview pane opened and is in full mode
                    HostBackdropPane.Visibility = Visibility.Visible;
                    HostBackdropSearch.Visibility = Visibility.Visible;
                    NormalBackground.Visibility = Visibility.Collapsed;
                }
            }
            else if (Window.Current.Bounds.Width >= 720)
            {
                // Window is in compact mode

                HostBackdropPane.Visibility = Visibility.Collapsed;
                HostBackdropSearch.Visibility = Visibility.Collapsed;
                NormalBackground.Visibility = Visibility.Visible;

                if (!(SplitView.IsSwipeablePaneOpen || SplitView.IsPaneOpen))
                {
                    // Window has Splitview pane closed and is in compact mode
                    NormalBackDropPane.Visibility = Visibility.Collapsed;
                    NormalBackdropSearch.Visibility = Visibility.Collapsed;
                }
                else
                {
                    // Window has Splitview pane opened and is in compact mode
                    NormalBackDropPane.Visibility = Visibility.Visible;
                    NormalBackdropSearch.Visibility = Visibility.Visible;
                }
            }
            else
            {
                // Window is in minimal mode
                NormalBackDropPane.Visibility = Visibility.Visible;
                NormalBackdropSearch.Visibility = Visibility.Visible;

                HostBackdropPane.Visibility = Visibility.Collapsed;
                HostBackdropSearch.Visibility = Visibility.Collapsed;
                NormalBackground.Visibility = Visibility.Visible;
            }
        }
    }
}