using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using InteropTools.ContentDialogs.Registry;
using InteropTools.Presentation;
using InteropTools.Providers;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.ApplicationModel.Resources.Core;
using InteropTools.Classes;
using InteropTools.CorePages;
using Windows.Storage;
using InteropTools.ContentDialogs.Core;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InteropTools.ShellPages.IO
{
    public sealed partial class BrowserControl
    {
        public delegate void CurrentItemChangedEvent(object sender, CurrentItemChangedEventArgs e);

        public FileItem _currentRegItem;


        public class FileItem
        {
            public FileItemType type { get; set; }
            public string name { get; set; }
            public string path { get; set; }
        }

        public enum FileItemType
        {
            FOLDER,
            FILE
        }
        
        private readonly ObservableRangeCollection<Item> _itemlist = new ObservableRangeCollection<Item>();
        private bool _moving;

        private bool _preventclick;

        public bool SortByType = false;

        public BrowserControl()
        {
            InitializeComponent();
            this.Loaded += BrowserControl_Loaded;
        }

        private void BrowserControl_Loaded(Object sender, RoutedEventArgs e)
        {
            _itemlist.CollectionChanged += Itemlist_CollectionChanged;
            ChangeCurrentItem(new FileItem() { name = @"C:\", path = "" });
        }

        public event CurrentItemChangedEvent OnCurrentItemChanged;

        private void UpdateCurrentItemChanged(FileItem previousItem, FileItem newItem)
        {
            // Make sure someone is listening to event
            if (OnCurrentItemChanged == null)
            {
                return;
            }

            var args = new CurrentItemChangedEventArgs(previousItem, newItem);
            OnCurrentItemChanged(this, args);
        }

        private void Itemlist_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshListView();
        }

        public void RefreshListView()
        {
            if (!SortByType)
            {
                var itemSource = AlphaKeyGroup<Item>.CreateGroups(_itemlist, CultureInfo.CurrentUICulture,
                    s => s.DisplayName, true);
                ZoomGrid.Style = Resources["NormalAlphaViewStyle"] as Style;
                ((CollectionViewSource)Resources["FileGroups"]).Source = itemSource;
            }
            else
            {
                var itemSource = _itemlist.OrderBy(x => x.DisplayName).GroupBy(x => x.Description);
                ZoomGrid.Style = Resources["AltTextViewStyle"] as Style;
                ((CollectionViewSource)Resources["FileGroups"]).Source = itemSource;
            }
        }

        private void SlidableListItem_RightCommandRequested(object sender, EventArgs e)
        {
            _moving = true;

            _preventclick = true;

            var item = (Item)((SlidableListItem)sender).DataContext;
            DeleteItem(item.fileitem);
        }

        private static FileItem GetPreviousItem(FileItem item)
        {
            if (item == null)
            {
                return null;
            }

            if (item.path == @"C:\")
            {
                return null;
            }

            var key = item.path;

            var path = "";
            if (!(key.Split('\\').Length - 1 < 0))
            {
                path = string.Join(@"\", key.Split('\\').Take(key.Split('\\').Length - 1));
            }

            return new FileItem
            {
                name = key.Split('\\').Last(),
                path = path,
                type = FileItemType.FOLDER
            };
        }

        private async void DeleteItem(FileItem item)
        {
            /*var key = item.path;
            if (item.type == RegistryItemType.KEY)
            {
                if ((key == "") || (key == null))
                {
                    key = item.name;
                }
                else
                {
                    key += @"\" + item.name;
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
                var title = ResourceManager.Current.MainResourceMap.GetValue("Resources/Do_you_really_want_to_delete_that_value", ResourceContext.GetForCurrentView()).ValueAsString;
                var content = item.Name + " will be deleted for ever and you won't be able to recover.";

                var command = await new InteropTools.ContentDialogs.Core.DualMessageDialogContentDialog().ShowDualMessageDialog(title, content, ResourceManager.Current.MainResourceMap.GetValue("Resources/Delete_the_value", ResourceContext.GetForCurrentView()).ValueAsString, ResourceManager.Current.MainResourceMap.GetValue("Resources/Keep_the_value", ResourceContext.GetForCurrentView()).ValueAsString);

                if (command)
                {
                    RunInThreadPool(() =>
                    {
                        var status = _helper.DeleteValue(item.Hive, key, item.Name);
                        RunInUIThread(() =>
                        {
                            if (status == HelperErrorCodes.FAILED)
                                ShowValueUnableToDeleteMessageBox();

                            ChangeCurrentItem(GetPreviousItem(item));
                        });
                    });
                }
            }*/
        }

        private static async void ShowValueUnableToDeleteMessageBox()
        {
            await new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(
                    ResourceManager.Current.MainResourceMap.GetValue("Resources/We_couldn_t_delete_the_specified_value__no_changes_to_the_phone_registry_were_made", ResourceContext.GetForCurrentView()).ValueAsString,
                    ResourceManager.Current.MainResourceMap.GetValue("Resources/Something_went_wrong", ResourceContext.GetForCurrentView()).ValueAsString);
        }

        private static async void ShowKeyUnableToDeleteMessageBox()
        {
            await new InteropTools.ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(ResourceManager.Current.MainResourceMap.GetValue("Resources/We_couldn_t_delete_the_specified_key__no_changes_to_the_phone_registry_were_made_", ResourceContext.GetForCurrentView()).ValueAsString,
                    ResourceManager.Current.MainResourceMap.GetValue("Resources/Something_went_wrong", ResourceContext.GetForCurrentView()).ValueAsString);
        }

        private void SlidableListItem_LeftCommandRequested(object sender, EventArgs e)
        {
            _moving = true;

            _preventclick = true;

            var item = (Item)((SlidableListItem)sender).DataContext;
            item.IsFavorite = (item.IsFavorite == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible);
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

        private void ListBrowser_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (_preventclick)
            {
                return;
            }

            var item = (Item)e.ClickedItem;

            if (item != null)
            {

                if (item.fileitem.type == FileItemType.FOLDER)
                {
                    UpdateCurrentItemChanged(_currentRegItem, item.fileitem);
                    return;
                }
                ChangeCurrentItem(item.fileitem);
            }
        }

        public bool GoBack()
        {
            if (_currentRegItem == null)
            {
                return false;
            }

            /*if (_currentRegItem.Type == RegistryItemType.HIVE)
            {
                ChangeCurrentItem();
                return true;
            }*/
            ChangeCurrentItem(GetPreviousItem(_currentRegItem));

            return true;
        }

        /*public void ChangeCurrentItem()
        {
            ClearItemList();
            RunInThreadPool(() =>
            {
                var items = _helper.GetRegistryHives2();

                RunInUIThread(() =>
                {
                    _itemlist.AddRange(items.Select(x => new Item(x)));
                    UpdateCurrentItemChanged(_currentRegItem, null);
                    _currentRegItem = null;
                });
            });
        }*/

        public void ChangeCurrentItem(FileItem regitem)
        {
            if (regitem.type == FileItemType.FILE)
            {
                return;
            }

            var key = regitem.path;
            if (regitem.type == FileItemType.FOLDER)
            {
                if ((key == "") || (key == null))
                {
                    key = regitem.name;
                }
                else
                {
                    key += @"\" + regitem.name;
                }
            }

            if (key == null)
            {
                key = "";
            }

            ClearItemList();

            RunInThreadPool(() =>
            {
                var items = DirPath(key);

                RunInUIThread(() =>
                {
                    _itemlist.AddRange(items.Select(x => new Item(x)));
                    
                    UpdateCurrentItemChanged(_currentRegItem, regitem);

                    var oldItem = _currentRegItem;
                    _currentRegItem = regitem;

                    if (oldItem == null)
                    {
                        return;
                    }

                    foreach (var itm in _itemlist)
                    {
                        if ((itm.fileitem.name == oldItem.name) && (itm.fileitem.path == oldItem.path))
                        {
                            ListBrowser.ScrollIntoView(itm, ScrollIntoViewAlignment.Leading);
                            break;
                        }
                    }
                });
            });
        }

        private void ClearItemList()
        {
            _itemlist.ClearList();
        }

        private List<FileItem> DirPath(string path)
        {
            var items = RegistryHelper.CIOHelper.FindFolders(path);
            var items2 = RegistryHelper.CIOHelper.FindItemsUnderPath(path);

            List<FileItem> itemlist = new List<FileItem>();

            foreach (var item in items)
            {
                itemlist.Add(new FileItem() { name = item, path = path, type = FileItemType.FOLDER });
            }
            
            foreach (var item in items2)
            {
                itemlist.Add(new FileItem() { name = item, path = path, type = FileItemType.FILE });
            }

            return itemlist;
        }

        private void StackPanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            /*var sndr = (StackPanel)sender;
            var item = (Item)sndr.DataContext;

            var flyout = new MenuFlyout { Placement = FlyoutPlacementMode.Top };

            var flyoutsubitems = new MenuFlyoutSubItem
            {
                Text =
                    ResourceManager.Current.MainResourceMap.GetValue("Resources/Copy",
                        ResourceContext.GetForCurrentView()).ValueAsString
            };

            var flyoutitem = new MenuFlyoutItem
            {
                Text =
                    ResourceManager.Current.MainResourceMap.GetValue("Resources/Copy_name",
                        ResourceContext.GetForCurrentView()).ValueAsString
            };

            flyoutitem.Click += (sender_, e_) =>
            {
                var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
                dataPackage.SetText(item.regitem.Name);
                Clipboard.SetContent(dataPackage);
            };

            var flyoutitem2 = new MenuFlyoutItem
            {
                Text =
                    ResourceManager.Current.MainResourceMap.GetValue("Resources/Copy_key_location",
                        ResourceContext.GetForCurrentView()).ValueAsString
            };

            if ((item.regitem.Key == null) || (item.regitem.Key == ""))
            {
                flyoutitem2.IsEnabled = false;
            }

            flyoutitem2.Click += (sender_, e_) =>
            {
                if (string.IsNullOrEmpty(item.regitem.Key))
                {
                    return;
                }

                var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
                dataPackage.SetText(item.regitem.Key);
                Clipboard.SetContent(dataPackage);
            };

            var flyoutitem3 = new MenuFlyoutItem
            {
                Text =
                    ResourceManager.Current.MainResourceMap.GetValue("Resources/Copy_hive_name",
                        ResourceContext.GetForCurrentView()).ValueAsString
            };

            flyoutitem3.Click += (sender_, e_) =>
            {
                var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
                dataPackage.SetText(item.regitem.Hive.ToString());
                Clipboard.SetContent(dataPackage);
            };

            var flyoutitem4 = new MenuFlyoutItem
            {
                Text =
                    ResourceManager.Current.MainResourceMap.GetValue("Resources/Copy_full_details",
                        ResourceContext.GetForCurrentView()).ValueAsString
            };

            flyoutitem4.Click += (sender_, e_) =>
            {
                var str = "";
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
                                $"[{item.regitem.Key}]\r\nName: {item.regitem.Name}\r\nType: {item.regitem.Type}\r\nHive: {item.regitem.Hive.ToString()}";
                            break;
                        }
                    case RegistryItemType.VALUE:
                        {
                            str =
                                $"[{item.regitem.Key}]\r\nName: {item.regitem.Name}\r\nType: {item.regitem.Type.ToString()}\r\nHive: {item.regitem.Hive.ToString()}\r\nValue Type: {item.regitem.ValueType.ToString()}\r\nValue: {item.regitem.Value}";
                            break;
                        }
                }
                if (str == "")
                {
                    return;
                }

                var dataPackage = new DataPackage { RequestedOperation = DataPackageOperation.Copy };
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

                var flyoutitem5 = new MenuFlyoutSeparator();
                flyout.Items.Add(flyoutitem5);

                var flyoutitem6 = new MenuFlyoutItem
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
                                var key = "";
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
                                var Key = item.regitem.Key;
                                if (item.regitem.Type == RegistryItemType.KEY)
                                {
                                    if ((Key == "") || (Key == null))
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

                var flyoutitem7 = new MenuFlyoutItem
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
                                DeleteItem(item.fileitem);
                                break;
                            }
                        case RegistryItemType.VALUE:
                            {
                                DeleteItem(item.fileitem);
                                break;
                            }
                    }
                };
            }

            flyout.ShowAt((StackPanel)sender, e.GetPosition((StackPanel)sender));*/
        }

        private void ListBrowser_RefreshRequested(object sender, EventArgs e)
        {
            /*if (_currentRegItem == null)
            {
                ChangeCurrentItem();
            }
            else
            {*/
                ChangeCurrentItem(_currentRegItem);
            //}
        }

        private void ListBrowser_PullProgressChanged(object sender, RefreshProgressEventArgs e)
        {
            refreshindicator.Opacity = e.PullProgress;
            RefreshRotation.Angle = e.PullProgress * 360;
        }

        private async void RunInUIThread(Action function)
        {
            await
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () => { function(); });
        }

        private async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => { function(); });
        }

        public class CurrentItemChangedEventArgs
        {
            public CurrentItemChangedEventArgs(FileItem previousItem, FileItem newItem)
            {
                this.previousItem = previousItem;
                this.newItem = newItem;
            }

            public FileItem previousItem { get; internal set; }
            public FileItem newItem { get; internal set; }
        }

        public class Item : INotifyPropertyChanged
        {
            public Item(FileItem fileitem)
            {
                this.fileitem = fileitem;
            }

            public Visibility IsFavorite
            {
                get
                {
                    try
                    {
                        var id = fileitem.type.ToString() + "%" + (fileitem.path == null ? "" : fileitem.path) + "%" + (fileitem.name == null ? "" : fileitem.name);

                        var applicationData = ApplicationData.Current;
                        var localSettings = applicationData.LocalSettings;

                        var value = localSettings.Values["browserfilefav_" + id];

                        if (value == null)
                        {
                            return Visibility.Collapsed;
                        }

                        if (value.GetType() == typeof(bool))
                        {
                            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
                        }
                    } catch (Exception e)
                    {
                        //new MessageDialogContentDialog().ShowMessageDialog(e.StackTrace, "Get" + e.Message + e.HResult);
                    }
                    return Visibility.Collapsed;
                }
                set
                {
                    try
                    {
                        var id = fileitem.type.ToString() + "%" + (fileitem.path == null ? "" : fileitem.path) + "%" + (fileitem.name == null ? "" : fileitem.name);
                        var applicationData = ApplicationData.Current;
                        var localSettings = applicationData.LocalSettings;

                        localSettings.Values["browserfilefav_" + id] = (value == Visibility.Visible);

                        Debug.WriteLine("browserfilefav_" + id);
                        
                        var strlist = localSettings.Values["browserfilefavlist"];

                        if ((strlist == null) || (strlist.GetType() != typeof(string)))
                        {
                            localSettings.Values["browserfilefavlist"] = "";
                            strlist = localSettings.Values["browserfilefavlist"];
                        }

                        var list = ((string)strlist).Split('\n').ToList();

                        if (value == Visibility.Collapsed)
                        {
                            localSettings.Values.Remove("browserfilefav_" + id);
                            list.Remove("browserfilefav_" + id);
                        }
                        else
                        {
                            list.Add("browserfilefav_" + id);
                        }

                        localSettings.Values["browserfilefavlist"] = String.Join("\n", list);
                        
                        OnPropertyChanged("IsFavorite");
                    }
                    catch (Exception e)
                    {
                        //new MessageDialogContentDialog().ShowMessageDialog(e.StackTrace, "Set" + e.Message + e.HResult);
                    }
                }
            }

            // Create the OnPropertyChanged method to raise the event
            protected void OnPropertyChanged(string name)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, new PropertyChangedEventArgs(name));
                }
            }

            public string Symbol
            {
                get
                {
                    switch (fileitem.type)
                    {
                        case FileItemType.FOLDER:
                            {
                                return "";
                            }
                        case FileItemType.FILE:
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

            public string DisplayName
            {
                get
                {
                    return fileitem.name;
                }
            }

            public string LastModified
            {
                get
                {
                    switch (fileitem.type)
                    {
                        case FileItemType.FILE:
                            {
                                ulong length;
                                RegistryHelper.CIOHelper.GetFileSize(fileitem.path + @"\" + fileitem.name, out length);

                                string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
                                int order = 0;
                                while (length >= 1024 && ++order < sizes.Length)
                                {
                                    length = length / 1024;
                                }

                                // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
                                // show a single decimal place, and no space.
                                string result = String.Format("{0:0.##} {1}", length, sizes[order]);
                                return result;
                            }
                        case FileItemType.FOLDER:
                            {
                                ulong length;
                                RegistryHelper.CIOHelper.GetFolderSize(fileitem.path + @"\" + fileitem.name, out length);

                                string[] sizes = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };
                                int order = 0;
                                while (length >= 1024 && ++order < sizes.Length)
                                {
                                    length = length / 1024;
                                }

                                // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
                                // show a single decimal place, and no space.
                                string result = String.Format("{0:0.##} {1}", length, sizes[order]);
                                return result;
                            }
                        default:
                            {
                                return "";
                            }
                    }
                }
            }

            public string Description
            {
                get
                {
                    switch (fileitem.type)
                    {
                        case FileItemType.FILE:
                            {
                                return "File";
                            }
                        case FileItemType.FOLDER:
                            {
                                return "Folder";
                            }
                        default:
                            {
                                return "Unknown";
                            }
                    }
                }
            }

            public FileItem fileitem { get; }

            public event PropertyChangedEventHandler PropertyChanged;
        }
    }
}