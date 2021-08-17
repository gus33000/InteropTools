using InteropTools.ContentDialogs.Core;
using InteropTools.ContentDialogs.Registry;
using InteropTools.Presentation;
using InteropTools.Providers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InteropTools.ShellPages.Registry
{
    public sealed partial class BrowserControl : UserControl
    {
        public RegistryItemCustom _currentRegItem;

        public bool SortByType;

        private readonly IRegistryProvider _helper;

        private readonly ObservableRangeCollection<Item> _itemlist = new();

        private bool _moving;

        private bool _preventclick;

        public BrowserControl()
        {
            InitializeComponent();
            _helper = App.MainRegistryHelper;
            _itemlist.CollectionChanged += Itemlist_CollectionChanged;
            ChangeCurrentItem();
        }

        public delegate void CurrentItemChangedEvent(object sender, CurrentItemChangedEventArgs e);

        public event EventHandler<CurrentItemChangedEventArgs> OnCurrentItemChanged;

        public void ChangeCurrentItem()
        {
            ClearItemList();
            RunInThreadPool(async () =>
            {
                IReadOnlyList<RegistryItemCustom> items = await _helper.GetRegistryHives2();
                RunInUIThread(() =>
                {
                    _itemlist.AddRange(items.Select(x => new Item(x)));
                    UpdateCurrentItemChanged(_currentRegItem, null);
                    _currentRegItem = null;
                });
            });
        }

        public void ChangeCurrentItem(RegistryItemCustom regitem)
        {
            if (regitem.Type == RegistryItemType.VALUE)
            {
                return;
            }

            string key = regitem.Key;

            if (regitem.Type == RegistryItemType.KEY)
            {
                if ((key?.Length == 0) || (key == null))
                {
                    key = regitem.Name;
                }
                else
                {
                    key += @"\" + regitem.Name;
                }
            }

            if (key == null)
            {
                key = "";
            }

            ClearItemList();
            RunInThreadPool(async () =>
            {
                IReadOnlyList<RegistryItemCustom> items = await _helper.GetRegistryItems2(regitem.Hive, key);
                RunInUIThread(() =>
                {
                    _itemlist.AddRange(items.Select(x => new Item(x)));
                    UpdateCurrentItemChanged(_currentRegItem, regitem);
                    RegistryItemCustom oldItem = _currentRegItem;
                    _currentRegItem = regitem;

                    if (oldItem == null)
                    {
                        return;
                    }

                    foreach (Item itm in _itemlist)
                    {
                        if ((itm.regitem.Name == oldItem.Name) && (itm.regitem.Key == oldItem.Key))
                        {
                            ListBrowser.ScrollIntoView(itm, ScrollIntoViewAlignment.Leading);
                            break;
                        }
                    }
                });
            });
        }

        public bool GoBack()
        {
            if (_currentRegItem == null)
            {
                return false;
            }

            if (_currentRegItem.Type == RegistryItemType.HIVE)
            {
                ChangeCurrentItem();
                return true;
            }

            ChangeCurrentItem(GetPreviousItem(_currentRegItem));
            return true;
        }

        public void RefreshListView()
        {
            if (!SortByType)
            {
                List<AlphaKeyGroup<Item>> itemSource = AlphaKeyGroup<Item>.CreateGroups(_itemlist, CultureInfo.InvariantCulture,
                                 s => s.DisplayName, true);
                ZoomGrid.Style = Resources["NormalAlphaViewStyle"] as Style;
                ((CollectionViewSource)Resources["RegistryGroups"]).Source = itemSource;
            }
            else
            {
                IEnumerable<IGrouping<string, Item>> itemSource = _itemlist.OrderBy(x => x.DisplayName).GroupBy(x => x.Description);
                ZoomGrid.Style = Resources["AltTextViewStyle"] as Style;
                ((CollectionViewSource)Resources["RegistryGroups"]).Source = itemSource;
            }
        }

        private static RegistryItemCustom GetPreviousItem(RegistryItemCustom item)
        {
            if (item == null)
            {
                return null;
            }

            if (item.Type == RegistryItemType.HIVE)
            {
                return null;
            }

            string key = item.Key;
            string path = "";

            if (string.IsNullOrEmpty(key))
            {
                return new RegistryItemCustom
                {
                    Name = item.Hive.ToString(),
                    Hive = item.Hive,
                    Key = path,
                    Type = RegistryItemType.HIVE,
                    Value = "",
                    ValueType = 0
                };
            }

            if (key.Split('\\').Length - 1 >= 0)
            {
                path = string.Join(@"\", key.Split('\\').Take(key.Split('\\').Length - 1));
            }

            return new RegistryItemCustom
            {
                Name = key.Split('\\').Last(),
                Hive = item.Hive,
                Key = path,
                Type = RegistryItemType.KEY,
                Value = "",
                ValueType = 0
            };
        }

        private static async void ShowKeyUnableToDeleteMessageBox()
        {
            await new MessageDialogContentDialog().ShowMessageDialog(
              ResourceManager.Current.MainResourceMap.GetValue("Resources/We_couldn_t_delete_the_specified_key__no_changes_to_the_phone_registry_were_made_", ResourceContext.GetForCurrentView()).ValueAsString,
              ResourceManager.Current.MainResourceMap.GetValue("Resources/Something_went_wrong", ResourceContext.GetForCurrentView()).ValueAsString);
        }

        private static async void ShowValueUnableToDeleteMessageBox()
        {
            await new MessageDialogContentDialog().ShowMessageDialog(
              ResourceManager.Current.MainResourceMap.GetValue("Resources/We_couldn_t_delete_the_specified_value__no_changes_to_the_phone_registry_were_made", ResourceContext.GetForCurrentView()).ValueAsString,
              ResourceManager.Current.MainResourceMap.GetValue("Resources/Something_went_wrong", ResourceContext.GetForCurrentView()).ValueAsString);
        }

        private void ClearItemList()
        {
            _itemlist.ClearList();
        }

        private async void DeleteItem(RegistryItemCustom item)
        {
            string key = item.Key;

            if (item.Type == RegistryItemType.KEY)
            {
                if ((key?.Length == 0) || (key == null))
                {
                    key = item.Name;
                }
                else
                {
                    key += @"\" + item.Name;
                }
            }

            if (item.Type == RegistryItemType.KEY)
            {
                await new DeleteRegKeyContentDialog(item.Hive, key).ShowAsync();
                ChangeCurrentItem(_currentRegItem);
            }

            if (item.Type != RegistryItemType.VALUE)
            {
                return;
            }

            {
                string title = ResourceManager.Current.MainResourceMap.GetValue("Resources/Do_you_really_want_to_delete_that_value", ResourceContext.GetForCurrentView()).ValueAsString;
                string content = item.Name + " will be deleted for ever and you won't be able to recover.";
                bool command = await new DualMessageDialogContentDialog().ShowDualMessageDialog(title, content,
                              ResourceManager.Current.MainResourceMap.GetValue("Resources/Delete_the_value", ResourceContext.GetForCurrentView()).ValueAsString,
                              ResourceManager.Current.MainResourceMap.GetValue("Resources/Keep_the_value", ResourceContext.GetForCurrentView()).ValueAsString);

                if (command)
                {
                    RunInThreadPool(async () =>
                    {
                        HelperErrorCodes status = await _helper.DeleteValue(item.Hive, key, item.Name);
                        RunInUIThread(() =>
                        {
                            if (status == HelperErrorCodes.FAILED)
                            { ShowValueUnableToDeleteMessageBox(); }

                            ChangeCurrentItem(GetPreviousItem(item));
                        });
                    });
                }
            }
        }

        private void Itemlist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshListView();
        }

        private void ListBrowser_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_preventclick)
            {
                return;
            }

            Item item = (Item)e.ClickedItem;

            if (item != null)
            {
                if (item.regitem.Type == RegistryItemType.VALUE)
                {
                    UpdateCurrentItemChanged(_currentRegItem, item.regitem);
                    return;
                }

                ChangeCurrentItem(item.regitem);
            }
        }

        private void ListBrowser_PullProgressChanged(object sender, RefreshProgressEventArgs e)
        {
            refreshindicator.Opacity = e.PullProgress;
            RefreshRotation.Angle = e.PullProgress * 360;
        }

        private void ListBrowser_RefreshRequested(object sender, EventArgs e)
        {
            if (_currentRegItem == null)
            {
                ChangeCurrentItem();
            }
            else
            {
                ChangeCurrentItem(_currentRegItem);
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

        private void SlidableListItem_LeftCommandRequested(object sender, EventArgs e)
        {
            _moving = true;
            _preventclick = true;
            Item item = (Item)((SlidableListItem)sender).DataContext;
            item.IsFavorite = item.IsFavorite == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SlidableListItem_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _preventclick = true;
        }

        private void SlidableListItem_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (!_moving)
            {
                _preventclick = false;
            }

            _moving = false;
        }

        private void SlidableListItem_RightCommandRequested(object sender, EventArgs e)
        {
            _moving = true;
            _preventclick = true;
            Item item = (Item)((SlidableListItem)sender).DataContext;
            DeleteItem(item.regitem);
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            try
            {
                StackPanel sndr = (StackPanel)sender;
                Item item = (Item)sndr.DataContext;
                MenuFlyout flyout = new() { Placement = FlyoutPlacementMode.Top };
                MenuFlyoutSubItem flyoutsubitems = new()
                {
                    Text =
                    ResourceManager.Current.MainResourceMap.GetValue("Resources/Copy",
                    ResourceContext.GetForCurrentView()).ValueAsString
                };
                MenuFlyoutItem flyoutitem = new()
                {
                    Text =
                    ResourceManager.Current.MainResourceMap.GetValue("Resources/Copy_name",
                    ResourceContext.GetForCurrentView()).ValueAsString
                };
                flyoutitem.Click += (sender_, e_) =>
                {
                    DataPackage dataPackage = new() { RequestedOperation = DataPackageOperation.Copy };
                    dataPackage.SetText(item.regitem.Name);
                    Clipboard.SetContent(dataPackage);
                };
                MenuFlyoutItem flyoutitem2 = new()
                {
                    Text =
                    ResourceManager.Current.MainResourceMap.GetValue("Resources/Copy_key_location",
                    ResourceContext.GetForCurrentView()).ValueAsString
                };

                if ((item.regitem.Key == null) || (item.regitem.Key?.Length == 0))
                {
                    flyoutitem2.IsEnabled = false;
                }

                flyoutitem2.Click += (sender_, e_) =>
                {
                    if (string.IsNullOrEmpty(item.regitem.Key))
                    {
                        return;
                    }

                    DataPackage dataPackage = new() { RequestedOperation = DataPackageOperation.Copy };
                    dataPackage.SetText(item.regitem.Key);
                    Clipboard.SetContent(dataPackage);
                };
                MenuFlyoutItem flyoutitem3 = new()
                {
                    Text =
                    ResourceManager.Current.MainResourceMap.GetValue("Resources/Copy_hive_name",
                    ResourceContext.GetForCurrentView()).ValueAsString
                };
                flyoutitem3.Click += (sender_, e_) =>
                {
                    DataPackage dataPackage = new() { RequestedOperation = DataPackageOperation.Copy };
                    dataPackage.SetText(item.regitem.Hive.ToString());
                    Clipboard.SetContent(dataPackage);
                };
                MenuFlyoutItem flyoutitem4 = new()
                {
                    Text =
                    ResourceManager.Current.MainResourceMap.GetValue("Resources/Copy_full_details",
                    ResourceContext.GetForCurrentView()).ValueAsString
                };
                flyoutitem4.Click += (sender_, e_) =>
                {
                    string str = "";

                    switch (item.regitem.Type)
                    {
                        case RegistryItemType.HIVE:
                            {
                                str = string.Format("Name: {0}\r\nType: {1}", item.regitem.Name, item.regitem.Type.ToString());
                                break;
                            }

                        case RegistryItemType.KEY:
                            {
                                str =
                                  $"[{item.regitem.Key}]\r\nName: {item.regitem.Name}\r\nType: {item.regitem.Type}\r\nHive: {item.regitem.Hive}";
                                break;
                            }

                        case RegistryItemType.VALUE:
                            {
                                str =
                                  $"[{item.regitem.Key}]\r\nName: {item.regitem.Name}\r\nType: {item.regitem.Type}\r\nHive: {item.regitem.Hive}\r\nValue Type: {item.regitem.ValueType}\r\nValue: {item.regitem.Value}";
                                break;
                            }
                    }

                    if (str?.Length == 0)
                    {
                        return;
                    }

                    DataPackage dataPackage = new() { RequestedOperation = DataPackageOperation.Copy };
                    dataPackage.SetText(str);
                    Clipboard.SetContent(dataPackage);
                };

                if (flyoutsubitems.Items != null)
                {
                    flyoutsubitems.Items.Add(flyoutitem);
                    flyoutsubitems.Items.Add(flyoutitem2);
                    flyoutsubitems.Items.Add(flyoutitem3);
                    flyoutsubitems.Items.Add(flyoutitem4);
                }

                if (flyout.Items != null)
                {
                    flyout.Items.Add(flyoutsubitems);
                    MenuFlyoutSeparator flyoutitem5 = new();
                    flyout.Items.Add(flyoutitem5);
                    MenuFlyoutItem flyoutitem6 = new()
                    {
                        Text =
                        ResourceManager.Current.MainResourceMap.GetValue("Resources/Rename",
                        ResourceContext.GetForCurrentView()).ValueAsString
                    };
                    flyout.Items.Add(flyoutitem6);

                    if (item.regitem.Type != RegistryItemType.KEY)
                    {
                        flyoutitem6.IsEnabled = false;
                    }

                    flyoutitem6.Click += (sender_, e_) =>
                    {
                        switch (item.regitem.Type)
                        {
                            case RegistryItemType.KEY:
                                {
                                    string key = "";

                                    if (item.regitem.Key != "")
                                    {
                                        key = item.regitem.Key + "\\" + item.regitem.Name;
                                    }
                                    else
                                    {
                                        key = item.regitem.Name;
                                    }

                                    RunInUIThread(async () =>
                                    {
                                        string Key = item.regitem.Key;

                                        if (item.regitem.Type == RegistryItemType.KEY)
                                        {
                                            if ((Key?.Length == 0) || (Key == null))
                                            {
                                                Key = item.regitem.Name;
                                            }
                                            else
                                            {
                                                Key += @"\" + item.regitem.Name;
                                            }
                                        }

                                        await new RenameKeyContentDialog(item.regitem.Hive, key).ShowAsync();
                                        ChangeCurrentItem(GetPreviousItem(item.regitem));
                                    });
                                    break;
                                }

                            case RegistryItemType.VALUE:
                                {
                                    //TODO
                                    break;
                                }
                        }
                    };
                    MenuFlyoutItem flyoutitem7 = new()
                    {
                        Text =
                        ResourceManager.Current.MainResourceMap.GetValue("Resources/Delete",
                        ResourceContext.GetForCurrentView()).ValueAsString
                    };
                    flyout.Items.Add(flyoutitem7);

                    if (item.regitem.Type == RegistryItemType.HIVE)
                    {
                        flyoutitem7.IsEnabled = false;
                    }

                    flyoutitem7.Click += (sender_, e_) =>
                    {
                        switch (item.regitem.Type)
                        {
                            case RegistryItemType.KEY:
                                {
                                    DeleteItem(item.regitem);
                                    break;
                                }

                            case RegistryItemType.VALUE:
                                {
                                    DeleteItem(item.regitem);
                                    break;
                                }
                        }
                    };

                    if (_currentRegItem.Type == RegistryItemType.HIVE && (_currentRegItem.Hive == RegHives.HKEY_LOCAL_MACHINE || _currentRegItem.Hive == RegHives.HKEY_USERS) && item.regitem.Type == RegistryItemType.KEY)
                    {
                        MenuFlyoutItem flyoutitemm = new()
                        {
                            Text = "Unmount this hive"
                        };

                        bool inUser = false;

                        if (_currentRegItem.Hive == RegHives.HKEY_USERS)
                        {
                            inUser = true;
                        }

                        flyout.Items.Add(flyoutitemm);

                        flyoutitemm.Click += async (sender_, e_) =>
                        {
                            bool ret = await new DualMessageDialogContentDialog().ShowDualMessageDialog("Do you really want to unmount this registry hive?", "Unmounting registry hives can potentially make your operating system not boot anymore depending on the registry hives you want to unload.");

                            if (!ret)
                            {
                                return;
                            }

                            await App.MainRegistryHelper.UnloadHive(item.regitem.Name, inUser);

                            if (_currentRegItem == null)
                            {
                                ChangeCurrentItem();
                            }
                            else
                            {
                                ChangeCurrentItem(_currentRegItem);
                            }
                        };
                    }
                }

                flyout.ShowAt((StackPanel)sender, e.GetPosition((StackPanel)sender));
            }
            catch
            {
            }
        }

        private void UpdateCurrentItemChanged(RegistryItemCustom previousItem, RegistryItemCustom newItem)
        {
            // Make sure someone is listening to event
            if (OnCurrentItemChanged == null)
            {
                return;
            }

            CurrentItemChangedEventArgs args = new(previousItem, newItem);
            OnCurrentItemChanged(this, args);
        }

        public class CurrentItemChangedEventArgs
        {
            public CurrentItemChangedEventArgs(RegistryItemCustom previousItem, RegistryItemCustom newItem)
            {
                this.previousItem = previousItem;
                this.newItem = newItem;
            }

            public RegistryItemCustom newItem { get; internal set; }
            public RegistryItemCustom previousItem { get; internal set; }
        }

        public class Item : INotifyPropertyChanged
        {
            public Item(RegistryItemCustom regitem)
            {
                this.regitem = regitem;

                ApplicationData applicationData = ApplicationData.Current;
                ApplicationDataContainer localSettings = applicationData.LocalSettings;

                if ((localSettings.Values["useTimeStamps"] == null) || (localSettings.Values["useTimeStamps"].GetType() != typeof(bool)))
                {
                    localSettings.Values["useTimeStamps"] = false;
                }

                bool useTimeStamps = (bool)localSettings.Values["useTimeStamps"];

                if (useTimeStamps)
                {
                    RunInThreadPool(async () =>
                    {
                        try
                        {
                            switch (regitem.Type)
                            {
                                case RegistryItemType.HIVE:
                                    {
                                        GetKeyLastModifiedTime res = await App.MainRegistryHelper.GetKeyLastModifiedTime(this.regitem.Hive, null);//.ConfigureAwait(false).GetAwaiter().GetResult();

                                        if (res.returncode != HelperErrorCodes.SUCCESS)
                                        {
                                            LastModified = "";
                                            return;
                                        }

                                        DateTime time = res.LastModified;

                                        LastModified = time.ToString();
                                        return;
                                    }

                                case RegistryItemType.KEY:
                                    {
                                        GetKeyLastModifiedTime res = await App.MainRegistryHelper.GetKeyLastModifiedTime(this.regitem.Hive, this.regitem.Key + @"\" + this.regitem.Name);//.ConfigureAwait(false).GetAwaiter().GetResult();

                                        if (res.returncode != HelperErrorCodes.SUCCESS)
                                        {
                                            LastModified = "";
                                            return;
                                        }

                                        DateTime time = res.LastModified;

                                        LastModified = time.ToString();
                                        return;
                                    }

                                case RegistryItemType.VALUE:
                                    {
                                        LastModified = "";
                                        return;
                                    }

                                default:
                                    {
                                        LastModified = "";
                                        return;
                                    }
                            }
                        }
                        catch
                        {
                            LastModified = "";
                            return;
                        }
                    });
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            public string Description
            {
                get
                {
                    switch (regitem.Type)
                    {
                        case RegistryItemType.HIVE:
                            {
                                return ResourceManager.Current.MainResourceMap.GetValue("Resources/Hive", ResourceContext.GetForCurrentView()).ValueAsString;
                            }

                        case RegistryItemType.KEY:
                            {
                                return ResourceManager.Current.MainResourceMap.GetValue("Resources/Key", ResourceContext.GetForCurrentView()).ValueAsString;
                            }

                        case RegistryItemType.VALUE:
                            {
                                if (regitem.ValueType < 12)
                                {
                                    return GetValueTypeName((RegTypes)regitem.ValueType);
                                }

                                return "Custom: " + regitem.ValueType;
                            }

                        default:
                            {
                                return ResourceManager.Current.MainResourceMap.GetValue("Resources/Unknown", ResourceContext.GetForCurrentView()).ValueAsString;
                            }
                    }
                }
            }

            public string DisplayName
            {
                get
                {
                    if (regitem.Name?.Length == 0)
                    {
                        return "(Default)";
                    }

                    return regitem.Name;
                }
            }

            public Visibility IsFavorite
            {
                get
                {
                    try
                    {
                        string id = regitem.Hive.ToString() + "%" + (regitem.Key ?? "") + "%" + (regitem.Name ?? "") + "%" + regitem.Type.ToString() + "%" +
                                 (regitem.Value ?? "") + "%" + regitem.ValueType.ToString();
                        ApplicationData applicationData = ApplicationData.Current;
                        ApplicationDataContainer localSettings = applicationData.LocalSettings;
                        object value = localSettings.Values["browserfav_" + id];

                        if (value == null)
                        {
                            return Visibility.Collapsed;
                        }

                        if (value.GetType() == typeof(bool))
                        {
                            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
                        }
                    }
                    catch (Exception)
                    {
                        //new MessageDialogContentDialog().ShowMessageDialog(e.StackTrace, "Get" + e.Message + e.HResult);
                    }

                    return Visibility.Collapsed;
                }

                set
                {
                    try
                    {
                        string id = regitem.Hive.ToString() + "%" + (regitem.Key ?? "") + "%" + (regitem.Name ?? "") + "%" + regitem.Type.ToString() + "%" +
                                 (regitem.Value ?? "") + "%" + regitem.ValueType.ToString();
                        ApplicationData applicationData = ApplicationData.Current;
                        ApplicationDataContainer localSettings = applicationData.LocalSettings;
                        localSettings.Values["browserfav_" + id] = value == Visibility.Visible;
                        Debug.WriteLine("browserfav_" + id);
                        object strlist = localSettings.Values["browserfavlist"];

                        if ((strlist == null) || (strlist.GetType() != typeof(string)))
                        {
                            localSettings.Values["browserfavlist"] = "";
                            strlist = localSettings.Values["browserfavlist"];
                        }

                        List<string> list = ((string)strlist).Split('\n').ToList();

                        if (value == Visibility.Collapsed)
                        {
                            localSettings.Values.Remove("browserfav_" + id);
                            list.Remove("browserfav_" + id);
                        }
                        else
                        {
                            list.Add("browserfav_" + id);
                        }

                        localSettings.Values["browserfavlist"] = string.Join("\n", list);
                        OnPropertyChanged("IsFavorite");
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            public string LastModified
            {
                get; internal set;
            }

            public RegistryItemCustom regitem { get; }

            public string Symbol
            {
                get
                {
                    switch (regitem.Type)
                    {
                        case RegistryItemType.HIVE:
                            {
                                return "";
                            }

                        case RegistryItemType.KEY:
                            {
                                return "";
                            }

                        case RegistryItemType.VALUE:
                            {
                                return "";
                            }

                        default:
                            {
                                return "";
                            }
                    }
                }
            }

            // Create the OnPropertyChanged method to raise the event
            protected void OnPropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            private string GetValueTypeName(RegTypes type)
            {
                switch (type)
                {
                    case RegTypes.REG_BINARY:
                        {
                            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Binary", ResourceContext.GetForCurrentView()).ValueAsString;
                        }

                    case RegTypes.REG_FULL_RESOURCE_DESCRIPTOR:
                        {
                            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Hardware_Resource_List", ResourceContext.GetForCurrentView()).ValueAsString;
                        }

                    case RegTypes.REG_DWORD:
                        {
                            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Integer", ResourceContext.GetForCurrentView()).ValueAsString;
                        }

                    case RegTypes.REG_DWORD_BIG_ENDIAN:
                        {
                            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Integer_Big_Endian", ResourceContext.GetForCurrentView()).ValueAsString;
                        }

                    case RegTypes.REG_QWORD:
                        {
                            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Long", ResourceContext.GetForCurrentView()).ValueAsString;
                        }

                    case RegTypes.REG_MULTI_SZ:
                        {
                            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Multi_String", ResourceContext.GetForCurrentView()).ValueAsString;
                        }

                    case RegTypes.REG_NONE:
                        {
                            return "None";
                        }

                    case RegTypes.REG_RESOURCE_LIST:
                        {
                            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Resource_List", ResourceContext.GetForCurrentView()).ValueAsString;
                        }

                    case RegTypes.REG_RESOURCE_REQUIREMENTS_LIST:
                        {
                            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Resource_Requirement", ResourceContext.GetForCurrentView()).ValueAsString;
                        }

                    case RegTypes.REG_SZ:
                        {
                            return ResourceManager.Current.MainResourceMap.GetValue("Resources/String", ResourceContext.GetForCurrentView()).ValueAsString;
                        }

                    case RegTypes.REG_LINK:
                        {
                            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Symbolic_Link", ResourceContext.GetForCurrentView()).ValueAsString;
                        }

                    case RegTypes.REG_EXPAND_SZ:
                        {
                            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Variable_String", ResourceContext.GetForCurrentView()).ValueAsString;
                        }
                }

                return ResourceManager.Current.MainResourceMap.GetValue("Resources/Unknown", ResourceContext.GetForCurrentView()).ValueAsString;
            }

            private async void RunInThreadPool(Action function)
            {
                await ThreadPool.RunAsync(x => function());
            }
        }
    }
}