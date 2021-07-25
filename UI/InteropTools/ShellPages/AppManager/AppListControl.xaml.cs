using InteropTools.CorePages;
using InteropTools.Presentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.System;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InteropTools.ShellPages.AppManager
{
    public class VolumeDisplayitem
    {

        public string _MountPoint = InteropTools.Resources.TextResources.ApplicationManager_MountPoint;
        public string _PackageStore = InteropTools.Resources.TextResources.ApplicationManager_PackageStore;
        public string _Name = InteropTools.Resources.TextResources.ApplicationManager_Name;
        public string _SystemVolume = InteropTools.Resources.TextResources.ApplicationManager_SystemVolume;
        public string _Offline = InteropTools.Resources.TextResources.ApplicationManager_Offline;
        public string _SupportsHardLinks = InteropTools.Resources.TextResources.ApplicationManager_SupportsHardLinks;

        public Visibility AllVisibility => Volume == null ? Visibility.Visible : Visibility.Collapsed;

        public Visibility VolumeVisibility => Volume != null ? Visibility.Visible : Visibility.Collapsed;

        public PackageVolume Volume
        {
            get;
            set;
        }
    }

    public class TypeDisplayitem
    {
        public string TypeName => Type == null ? InteropTools.Resources.TextResources.ApplicationManager_AllTypes : Type.ToString();

        public PackageTypes? Type
        {
            get;
            set;
        }
    }

    public sealed partial class AppListControl : UserControl
    {
        public AppListControl()
        {
            InitializeComponent();
            Refresh();
        }

        public string _PackageList = InteropTools.Resources.TextResources.ApplicationManager_PackageList;
        public string _FilterBoxPlaceHolderText = InteropTools.Resources.TextResources.ApplicationManager_FilterBoxPlaceHolderText;
        public string _AllVolumes = InteropTools.Resources.TextResources.ApplicationManager_AllVolumes;
        public string _AllTypes = InteropTools.Resources.TextResources.ApplicationManager_AllTypes;

        private void PackageList_ItemClick(object sender, ItemClickEventArgs e)
        {
            try
            {
                Item selectedItem = (Item)e.ClickedItem;
                (App.AppContent as Shell).RootFrame.Navigate(typeof(AppProductPage), selectedItem.FullName);
            }
            catch
            {

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

        private ObservableRangeCollection<Item> _filteredItemsList = new();

        private ObservableRangeCollection<Item> _itemsList = new();

        public ObservableCollection<VolumeDisplayitem> _volumelist = new();
        public ObservableCollection<TypeDisplayitem> _typelist = new();

        private void _filteredItemsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<AlphaKeyGroup<Item>> itemSource = AlphaKeyGroup<Item>.CreateGroups(_filteredItemsList, CultureInfo.InvariantCulture,
                             s => s.DisplayName, true);
            ((CollectionViewSource)Resources["AppsGroups"]).Source = itemSource;
        }

        private void ItemsList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            List<Item> tmplist = new();

            foreach (Item item in _itemsList)
            {
                if (item.DisplayName.ToLower().Contains(FilterBox.Text.ToLower()))
                {
                    if ((VolListView.SelectedItem as VolumeDisplayitem).Volume == null)
                    {
                        if ((TypeListView.SelectedItem as TypeDisplayitem).Type == null)
                        {
                            tmplist.Add(item);
                        }

                        else
                            if ((TypeListView.SelectedItem as TypeDisplayitem).Type == item.type)
                        {
                            tmplist.Add(item);
                        }
                    }

                    else
                        if (item.volume == (VolListView.SelectedItem as VolumeDisplayitem).Volume)
                    {
                        if ((TypeListView.SelectedItem as TypeDisplayitem).Type == null)
                        {
                            tmplist.Add(item);
                        }

                        else
                            if ((TypeListView.SelectedItem as TypeDisplayitem).Type == item.type)
                        {
                            tmplist.Add(item);
                        }
                    }
                }
            }

            _filteredItemsList.AddRange(tmplist);
        }

        private void Refresh()
        {
            PackageListPanel.IsHitTestVisible = false;
            PackageListPanel.Opacity = 0.5;
            RunInThreadPool(async () =>
            {
                try
                {
                    _volumelist = new ObservableCollection<VolumeDisplayitem>();
                    _typelist = new ObservableCollection<TypeDisplayitem>();
                    _itemsList = new ObservableRangeCollection<Item>();
                    _filteredItemsList = new ObservableRangeCollection<Item>();
                    _itemsList.CollectionChanged += ItemsList_CollectionChanged;
                    _filteredItemsList.CollectionChanged += _filteredItemsList_CollectionChanged;
                    await RunInUiThread(() =>
                    {
                        LoadingText.Text = "Fetching available system volumes...";
                        LoadingStack.Visibility = Visibility.Visible;
                    });
                    List<AlphaKeyGroup<Item>> itemSource = AlphaKeyGroup<Item>.CreateGroups(_filteredItemsList, CultureInfo.InvariantCulture,
                                     s => s.DisplayName, true);
                    await RunInUiThread(() =>
                    {
                        ((CollectionViewSource)Resources["AppsGroups"]).Source = itemSource;
                    });
                    List<Item> tmplist = new();

                    _volumelist.Add(new VolumeDisplayitem());

                    IReadOnlyList<PackageVolume> vols = null;

                    try
                    {
                        vols = await new PackageManager().GetPackageVolumesAsync();

                        foreach (PackageVolume vol in vols)
                        {
                            _volumelist.Add(new VolumeDisplayitem() { Volume = vol });
                        }
                    }
                    catch
                    {

                    }

                    await RunInUiThread(() =>
                    {
                        VolListView.ItemsSource = _volumelist;
                        VolListView.SelectedIndex = 0;
                    });
                    await RunInUiThread(() =>
                    {
                        LoadingText.Text = "Fetching available package types...";
                        LoadingStack.Visibility = Visibility.Visible;
                    });
                    IEnumerable<PackageTypes> pkgtypes = Enum.GetValues(typeof(PackageTypes)).Cast<PackageTypes>();
                    _typelist.Add(new TypeDisplayitem());

                    foreach (PackageTypes type in pkgtypes)
                    {
                        _typelist.Add(new TypeDisplayitem() { Type = type });
                    }

                    await RunInUiThread(() =>
                    {
                        TypeListView.ItemsSource = _typelist;
                        TypeListView.SelectedIndex = 0;
                    });
                    await RunInUiThread(() =>
                    {
                        LoadingText.Text = "Determining the number of packages present in the system...";
                        LoadingStack.Visibility = Visibility.Visible;
                    });
                    int numofpkgs = 0;

                    try
                    {
                        foreach (PackageVolume vol in vols)
                        {
                            foreach (PackageTypes type in pkgtypes)
                            {
                                IList<Package> pkgs = vol.FindPackagesForUserWithPackageTypes("", type);
                                numofpkgs += pkgs.Count();
                            }
                        }
                    }
                    catch
                    {
                        foreach (PackageTypes type in pkgtypes)
                        {
                            IEnumerable<Package> pkgs = new PackageManager().FindPackagesForUserWithPackageTypes("", type);
                            numofpkgs += pkgs.Count();
                        }
                    }

                    double count = 0;

                    try
                    {
                        foreach (PackageVolume vol in vols)
                        {
                            ObservableRangeCollection<Package> applist = new();

                            foreach (PackageTypes type in pkgtypes)
                            {
                                IList<Package> pkgs = vol.FindPackagesForUserWithPackageTypes("", type);

                                foreach (Package package in pkgs)
                                {
                                    count++;
                                    await RunInUiThread(() =>
                                    {
                                        LoadingText.Text = string.Format("Fetching information for packages... ({0}%)", Math.Round(count / numofpkgs * 100, 0));
                                    });
                                    string arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchUnknown;

                                    switch (package.Id.Architecture)
                                    {
                                        case ProcessorArchitecture.Arm:
                                            {
                                                arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchARM;
                                                break;
                                            }

                                        case ProcessorArchitecture.Neutral:
                                            {
                                                arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchNeutral;
                                                break;
                                            }

                                        case ProcessorArchitecture.Unknown:
                                            {
                                                arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchUnknown;
                                                break;
                                            }

                                        case ProcessorArchitecture.X64:
                                            {
                                                arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchx64;
                                                break;
                                            }

                                        case ProcessorArchitecture.X86:
                                            {
                                                arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchx86;
                                                break;
                                            }
                                    }

                                    string displayname = package.Id.FamilyName;
                                    string description = arch + " " + package.Id.Version.Major + "." + package.Id.Version.Minor + "." +
                                                      package.Id.Version.Build + "." + package.Id.Version.Revision;
                                    dynamic logo = "";

                                    try
                                    {
                                        IReadOnlyList<AppListEntry> appEntries = await package.GetAppListEntriesAsync();

                                        foreach (AppListEntry appEntry in appEntries)
                                        {
                                            try
                                            {
                                                displayname = appEntry.DisplayInfo.DisplayName;
                                            }

                                            catch
                                            {
                                                // ignored
                                            }

                                            try
                                            {
                                                description = appEntry.DisplayInfo.Description + "\n" + arch + " " +
                                                              package.Id.Version.Major + "." + package.Id.Version.Minor + "." +
                                                              package.Id.Version.Build + "." + package.Id.Version.Revision;
                                            }

                                            catch
                                            {
                                                // ignored
                                            }

                                            try
                                            {
                                                Size logosize = new()
                                                {
                                                    Height = 160,
                                                    Width = 160
                                                };
                                                Windows.Storage.Streams.IRandomAccessStreamWithContentType applogo = await appEntry.DisplayInfo.GetLogo(logosize).OpenReadAsync();
                                                await RunInUiThread(() =>
                                                {
                                                    BitmapImage bitmapImage = new();
                                                    bitmapImage.SetSource(applogo);
                                                    logo = bitmapImage;
                                                });
                                            }

                                            catch
                                            {
                                                // ignored
                                            }

                                            break;
                                        }
                                    }

                                    catch
                                    {
                                        // ignored
                                    }

                                    if (string.IsNullOrEmpty(displayname.Trim()))
                                    {
                                        displayname = package.Id.FamilyName;
                                    }

                                    tmplist.Add(new Item
                                    {
                                        DisplayName = displayname,
                                        FullName = package.Id.FullName,
                                        Description = description,
                                        logo = logo,
                                        volume = vol,
                                        type = type
                                    });
                                }
                            }
                        }
                    }
                    catch
                    {
                        ObservableRangeCollection<Package> applist = new();

                        foreach (PackageTypes type in pkgtypes)
                        {
                            IEnumerable<Package> pkgs = new PackageManager().FindPackagesForUserWithPackageTypes("", type);

                            foreach (Package package in pkgs)
                            {
                                count++;
                                await RunInUiThread(() =>
                                {
                                    LoadingText.Text = string.Format("Fetching information for packages... ({0}%)", Math.Round(count / numofpkgs * 100, 0));
                                });
                                string arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchUnknown;

                                switch (package.Id.Architecture)
                                {
                                    case ProcessorArchitecture.Arm:
                                        {
                                            arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchARM;
                                            break;
                                        }

                                    case ProcessorArchitecture.Neutral:
                                        {
                                            arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchNeutral;
                                            break;
                                        }

                                    case ProcessorArchitecture.Unknown:
                                        {
                                            arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchUnknown;
                                            break;
                                        }

                                    case ProcessorArchitecture.X64:
                                        {
                                            arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchx64;
                                            break;
                                        }

                                    case ProcessorArchitecture.X86:
                                        {
                                            arch = InteropTools.Resources.TextResources.ApplicationManager_PackageListPackageArchx86;
                                            break;
                                        }
                                }

                                string displayname = package.Id.FamilyName;
                                string description = arch + " " + package.Id.Version.Major + "." + package.Id.Version.Minor + "." +
                                                  package.Id.Version.Build + "." + package.Id.Version.Revision;
                                dynamic logo = "";

                                try
                                {
                                    IReadOnlyList<AppListEntry> appEntries = await package.GetAppListEntriesAsync();

                                    foreach (AppListEntry appEntry in appEntries)
                                    {
                                        try
                                        {
                                            displayname = appEntry.DisplayInfo.DisplayName;
                                        }

                                        catch
                                        {
                                            // ignored
                                        }

                                        try
                                        {
                                            description = appEntry.DisplayInfo.Description + "\n" + arch + " " +
                                                          package.Id.Version.Major + "." + package.Id.Version.Minor + "." +
                                                          package.Id.Version.Build + "." + package.Id.Version.Revision;
                                        }

                                        catch
                                        {
                                            // ignored
                                        }

                                        try
                                        {
                                            Size logosize = new()
                                            {
                                                Height = 160,
                                                Width = 160
                                            };
                                            Windows.Storage.Streams.IRandomAccessStreamWithContentType applogo = await appEntry.DisplayInfo.GetLogo(logosize).OpenReadAsync();
                                            await RunInUiThread(() =>
                                            {
                                                BitmapImage bitmapImage = new();
                                                bitmapImage.SetSource(applogo);
                                                logo = bitmapImage;
                                            });
                                        }

                                        catch
                                        {
                                            // ignored
                                        }

                                        break;
                                    }
                                }

                                catch
                                {
                                    // ignored
                                }

                                if (string.IsNullOrEmpty(displayname.Trim()))
                                {
                                    displayname = package.Id.FamilyName;
                                }

                                tmplist.Add(new Item
                                {
                                    DisplayName = displayname,
                                    FullName = package.Id.FullName,
                                    Description = description,
                                    logo = logo,
                                    volume = null,
                                    type = type
                                });
                            }
                        }
                    }

                    await RunInUiThread(() =>
                    {
                        _itemsList.AddRange(tmplist);
                    });
                }

                catch (Exception caughtEx)
                {
                    await RunInUiThread(async () =>
                    {
                        await
                        new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(string.Format(InteropTools.Resources.TextResources.ApplicationManager_PackageListError,
                            "0x" + string.Format("{0:x}", caughtEx.HResult) + " " +
                            caughtEx.Message + " " + caughtEx.StackTrace));
                    });
                }

                finally
                {
                    await RunInUiThread(() =>
                    {
                        LoadingStack.Visibility = Visibility.Collapsed;
                        PackageListPanel.IsHitTestVisible = true;
                        PackageListPanel.Opacity = 1;
                    });
                }
            });
        }


        private void FilterBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            VolumeDisplayitem selectedvol = VolListView.SelectedItem as VolumeDisplayitem;
            TypeDisplayitem selectedtype = TypeListView.SelectedItem as TypeDisplayitem;
            string filtertext = FilterBox.Text;
            RunInThreadPool(async () =>
            {
                ObservableRangeCollection<Item> _filteredItemsListtmp = new();
                _filteredItemsListtmp.AddRange(_filteredItemsList);

                if (_filteredItemsListtmp.Count != 0)
                {
                    for (int i = _filteredItemsListtmp.Count - 1; i >= 0; i--)
                    {
                        _filteredItemsListtmp.RemoveAt(i);
                    }
                }

                foreach (Item item in _itemsList)
                {
                    if (item.DisplayName.ToLower().Contains(filtertext.ToLower()))
                    {
                        if (selectedvol.Volume == null)
                        {
                            if (selectedtype.Type == null)
                            {
                                _filteredItemsListtmp.Add(item);
                            }

                            else
                                if (selectedtype.Type == item.type)
                            {
                                _filteredItemsListtmp.Add(item);
                            }
                        }

                        else
                            if (item.volume == selectedvol.Volume)
                        {
                            if (selectedtype.Type == null)
                            {
                                _filteredItemsListtmp.Add(item);
                            }

                            else
                                if (selectedtype.Type == item.type)
                            {
                                _filteredItemsListtmp.Add(item);
                            }
                        }
                    }
                }

                await RunInUiThread(() =>
                {
                    _filteredItemsList.ClearList();
                    _filteredItemsList.AddRange(_filteredItemsListtmp);
                });
            });
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            Item item = (Item)((StackPanel)sender).DataContext;
            MenuFlyout flyout = new()
            {
                Placement = FlyoutPlacementMode.Top
            };
            MenuFlyoutItem flyoutitem1 = new()
            {
                Text = InteropTools.Resources.TextResources.ApplicationManager_PackageListUninstall
            };
            flyoutitem1.Click += async (sender_, e_) =>
            {
                try
                {
                    await new PackageManager().RemovePackageAsync(item.FullName, RemovalOptions.None);
                }

                catch
                {
                }

                Refresh();
            };
            flyout.Items?.Add(flyoutitem1);
            flyout.ShowAt((StackPanel)sender, e.GetPosition((StackPanel)sender));
        }
        public class Item
        {
            public string DisplayName { get; set; }
            public string Description { get; set; }

            public string FullName { get; set; }

            public dynamic logo { get; set; }

            public PackageVolume volume { get; set; }

            public PackageTypes type { get; set; }

            public string typeicon => type.ToString() switch
            {
                "Bundle" => "",
                "Framework" => "",
                "Main" => "",
                "Optional" => "",
                "Resource" => "",
                "Xap" => "",
                _ => "",
            };
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void VolListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            VolFlyout.Hide();
        }

        private void VolListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((VolListView.SelectedItem as VolumeDisplayitem) != null)
            {
                VolSelect.Content = (VolListView.SelectedItem as VolumeDisplayitem).Volume == null ? InteropTools.Resources.TextResources.ApplicationManager_AllVolumes :
                                    (VolListView.SelectedItem as VolumeDisplayitem).Volume.MountPoint + " (" + (VolListView.SelectedItem as VolumeDisplayitem).Volume.PackageStorePath.Replace((
                                          VolListView.SelectedItem as VolumeDisplayitem).Volume.MountPoint, (VolListView.SelectedItem as VolumeDisplayitem).Volume.Name) + ")";
                VolumeDisplayitem selectedvol = VolListView.SelectedItem as VolumeDisplayitem;
                TypeDisplayitem selectedtype = TypeListView.SelectedItem as TypeDisplayitem;
                string filtertext = FilterBox.Text;
                RunInThreadPool(async () =>
                {
                    ObservableRangeCollection<Item> _filteredItemsListtmp = new();
                    _filteredItemsListtmp.AddRange(_filteredItemsList);

                    if (_filteredItemsListtmp.Count != 0)
                    {
                        for (int i = _filteredItemsListtmp.Count - 1; i >= 0; i--)
                        {
                            _filteredItemsListtmp.RemoveAt(i);
                        }
                    }

                    foreach (Item item in _itemsList)
                    {
                        if (item.DisplayName.ToLower().Contains(filtertext.ToLower()))
                        {
                            if (selectedvol.Volume == null)
                            {
                                if (selectedtype.Type == null)
                                {
                                    _filteredItemsListtmp.Add(item);
                                }

                                else
                                    if (selectedtype.Type == item.type)
                                {
                                    _filteredItemsListtmp.Add(item);
                                }
                            }

                            else
                                if (item.volume == selectedvol.Volume)
                            {
                                if (selectedtype.Type == null)
                                {
                                    _filteredItemsListtmp.Add(item);
                                }

                                else
                                    if (selectedtype.Type == item.type)
                                {
                                    _filteredItemsListtmp.Add(item);
                                }
                            }
                        }
                    }

                    await RunInUiThread(() =>
                    {
                        _filteredItemsList.ClearList();
                        _filteredItemsList.AddRange(_filteredItemsListtmp);
                    });
                });
            }
        }

        private void TypeSelect_Click(object sender, RoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }

        private void TypeListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            TypeFlyout.Hide();
        }

        private void TypeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((TypeListView.SelectedItem as TypeDisplayitem) != null)
            {
                TypeSelect.Content = (TypeListView.SelectedItem as TypeDisplayitem).TypeName;
                VolumeDisplayitem selectedvol = VolListView.SelectedItem as VolumeDisplayitem;
                TypeDisplayitem selectedtype = TypeListView.SelectedItem as TypeDisplayitem;
                string filtertext = FilterBox.Text;
                RunInThreadPool(async () =>
                {
                    ObservableRangeCollection<Item> _filteredItemsListtmp = new();
                    _filteredItemsListtmp.AddRange(_filteredItemsList);

                    if (_filteredItemsListtmp.Count != 0)
                    {
                        for (int i = _filteredItemsListtmp.Count - 1; i >= 0; i--)
                        {
                            _filteredItemsListtmp.RemoveAt(i);
                        }
                    }

                    foreach (Item item in _itemsList)
                    {
                        if (item.DisplayName.ToLower().Contains(filtertext.ToLower()))
                        {
                            if (selectedvol.Volume == null)
                            {
                                if (selectedtype.Type == null)
                                {
                                    _filteredItemsListtmp.Add(item);
                                }

                                else
                                    if (selectedtype.Type == item.type)
                                {
                                    _filteredItemsListtmp.Add(item);
                                }
                            }

                            else
                                if (item.volume == selectedvol.Volume)
                            {
                                if (selectedtype.Type == null)
                                {
                                    _filteredItemsListtmp.Add(item);
                                }

                                else
                                    if (selectedtype.Type == item.type)
                                {
                                    _filteredItemsListtmp.Add(item);
                                }
                            }
                        }
                    }

                    await RunInUiThread(() =>
                    {
                        _filteredItemsList.ClearList();
                        _filteredItemsList.AddRange(_filteredItemsListtmp);
                    });
                });
            }
        }



        private void SelectButton_Checked(object sender, RoutedEventArgs e)
        {
            PackageList.SelectionMode = ListViewSelectionMode.Multiple;
            PackageList.IsMultiSelectCheckBoxEnabled = true;
            PackageList.IsItemClickEnabled = false;
            UninstallSelectedButton.Visibility = Visibility.Visible;
        }

        private void SelectButton_Unchecked(object sender, RoutedEventArgs e)
        {
            PackageList.SelectionMode = ListViewSelectionMode.None;
            PackageList.IsMultiSelectCheckBoxEnabled = false;
            PackageList.IsItemClickEnabled = true;
            UninstallSelectedButton.Visibility = Visibility.Collapsed;
        }

        private async void UninstallSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            PackageListPanel.IsHitTestVisible = false;
            PackageListPanel.Opacity = 0.5;
            LoadingStack.Visibility = Visibility.Visible;
            foreach (Item item in PackageList.SelectedItems)
            {
                try
                {
                    IAsyncOperationWithProgress<DeploymentResult, DeploymentProgress> task = new PackageManager().RemovePackageAsync(item.FullName, RemovalOptions.None);
                    await task.AsTask(new Progress<DeploymentProgress>(progress =>
                    {
                        LoadingText.Text = string.Format("Uninstalling {0}... ({1}%)", item.FullName, progress.percentage);
                    }));
                }
                catch
                {

                }
            }

            LoadingStack.Visibility = Visibility.Collapsed;
            PackageListPanel.IsHitTestVisible = true;
            PackageListPanel.Opacity = 1;

            PackageList.SelectionMode = ListViewSelectionMode.None;
            PackageList.IsMultiSelectCheckBoxEnabled = false;
            PackageList.IsItemClickEnabled = true;
            UninstallSelectedButton.Visibility = Visibility.Collapsed;

            Refresh();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }
    }
}
