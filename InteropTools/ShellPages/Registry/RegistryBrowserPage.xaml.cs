using InteropTools.ContentDialogs.Registry;
using InteropTools.CorePages;
using InteropTools.Providers;
using InteropTools.ShellPages.Core;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Shell = InteropTools.CorePages.Shell;

namespace InteropTools.ShellPages.Registry
{
    public sealed partial class RegistryBrowserPage : Page
    {
        private readonly IRegistryProvider _helper;
        private RegistryItemCustom currentEditItem;

        public RegistryBrowserPage()
        {
            InitializeComponent();
            _helper = App.MainRegistryHelper;
            Unloaded += RegistryBrowserPage_Unloaded;

            Breadcrumbbar.OnItemClick += Breadcrumbbar_OnItemClick;
            ObservableCollection<BreadCrumbControl.BreadCrumbItem> BreadCrumbItemsList = new()
            {
                new BreadCrumbControl.BreadCrumbItem() { DisplayName = _helper.GetFriendlyName(), ItemObject = null }
            };
            Breadcrumbbar.ItemsSource = BreadCrumbItemsList;
            SystemNavigationManager.GetForCurrentView().BackRequested += RegistryBrowserPage_BackRequested;

            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
              AppViewBackButtonVisibility.Visible;
        }

        public PageGroup PageGroup => PageGroup.Registry;
        public string PageName => "Registry Browser";

        public RegistryItemCustom GetItemFromId(string id)
        {
            RegHives hiveconv = RegHives.HKEY_LOCAL_MACHINE;

            try
            {
                hiveconv = (RegHives)Enum.Parse(typeof(RegHives), id.Split('%')[0]);
            }
            catch
            {
            }

            RegistryItemType typeconv = RegistryItemType.Hive;

            try
            {
                typeconv = (RegistryItemType)Enum.Parse(typeof(RegistryItemType), id.Split('%')[3]);
            }
            catch
            {
            }

            uint regtypeconv = 0u;

            try
            {
                regtypeconv = uint.Parse(id.Split('%')[5]);
            }
            catch
            {
            }

            return new RegistryItemCustom()
            {
                Hive = hiveconv,
                Key = id.Split('%')[1],
                Name = id.Split('%')[2],
                Type = typeconv,
                Value = id.Split('%')[4],
                ValueType = regtypeconv
            };
        }

        private static int GetHexVal(char hex)
        {
            int val = hex;
            //For uppercase A-F letters:
            //return val - (val < 58 ? 48 : 55);
            //For lowercase a-f letters:
            //return val - (val < 58 ? 48 : 87);
            //Or the two combined, but a bit slower:
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }

        private static bool GetHiveFromName(string hivename, out RegHives reghive)
        {
            switch (hivename.ToUpper())
            {
                case "HKCC":
                case "HKEY_CURRENT_CONFIG":
                    {
                        reghive = RegHives.HKEY_CURRENT_CONFIG;
                        return true;
                    }

                case "HKCR":
                case "HKEY_CLASSES_ROOT":
                    {
                        reghive = RegHives.HKEY_CLASSES_ROOT;
                        return true;
                    }

                case "HKCU":
                case "HKEY_CURRENT_USER":
                    {
                        reghive = RegHives.HKEY_CURRENT_USER;
                        return true;
                    }

                case "HKCULS":
                case "HKEY_CURRENT_USER_LOCAL_SETTINGS":
                    {
                        reghive = RegHives.HKEY_CURRENT_USER_LOCAL_SETTINGS;
                        return true;
                    }

                case "HKDD":
                case "HKEY_DYNAMIC_DATA":
                case "HKEY_DYN_DATA":
                    {
                        reghive = RegHives.HKEY_DYN_DATA;
                        return true;
                    }

                case "HKLM":
                case "HKEY_LOCAL_MACHINE":
                    {
                        reghive = RegHives.HKEY_LOCAL_MACHINE;
                        return true;
                    }

                case "HKPD":
                case "HKEY_PERFORMANCE_DATA":
                    {
                        reghive = RegHives.HKEY_PERFORMANCE_DATA;
                        return true;
                    }

                case "HKU":
                case "HKEY_USERS":
                    {
                        reghive = RegHives.HKEY_USERS;
                        return true;
                    }

                default:
                    {
                        reghive = RegHives.HKEY_LOCAL_MACHINE;
                        return false;
                    }
            }
        }

        private static string GetRegistryHiveName(RegHives hive)
        {
            return Enum.GetName(typeof(RegHives), hive);
        }

        private static async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => function());
        }

        private async void RunInUiThread(Action function)
        {
            await
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => function());
        }

        private static async void ShowKeyMessageBox(string s)
        {
            await new ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(s + "\nThe above path was copied to your clipboard",
                ResourceManager.Current.MainResourceMap.GetValue("Resources/Current_Key", ResourceContext.GetForCurrentView()).ValueAsString);
        }

        private static async void ShowKeyUnableToAddMessageBox()
        {
            await new ContentDialogs.Core.MessageDialogContentDialog().ShowMessageDialog(
              ResourceManager.Current.MainResourceMap.GetValue("Resources /We_couldn_t_add_the_specified_key__no_changes_to_the_phone_registry_were_made_", ResourceContext.GetForCurrentView()).ValueAsString,
              ResourceManager.Current.MainResourceMap.GetValue("Resources/Something_went_wrong", ResourceContext.GetForCurrentView()).ValueAsString);
        }

        private static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 == 1)
            {
                throw new Exception("The binary key cannot have an odd number of digits");
            }

            byte[] arr = new byte[hex.Length >> 1];

            for (int i = 0; i < (hex.Length >> 1); ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + GetHexVal(hex[(i << 1) + 1]));
            }

            return arr;
        }

        private async void AddKey(RegHives hive, string keypath)
        {
            string title = ResourceManager.Current.MainResourceMap.GetValue("Resources/Do_you_really_want_to_add_that_key_", ResourceContext.GetForCurrentView()).ValueAsString;
            string content = "We will add " + keypath + " to the phone registry.";
            bool command = await new ContentDialogs.Core.DualMessageDialogContentDialog().ShowDualMessageDialog(title, content,
                          ResourceManager.Current.MainResourceMap.GetValue("Resources/Add_the_key", ResourceContext.GetForCurrentView()).ValueAsString,
                          ResourceManager.Current.MainResourceMap.GetValue("Resources/Don_t_add_the_key", ResourceContext.GetForCurrentView()).ValueAsString);

            if (command)
            {
                RunInThreadPool(async () =>
            {
                HelperErrorCodes status = await _helper.AddKey(hive, keypath);
                RunInUiThread(() =>
                {
                    if (status == HelperErrorCodes.Failed)
                    {
                        ShowKeyUnableToAddMessageBox();
                    }
                    else
                    {
                        string path = "";

                        if (keypath.Split('\\').Length - 1 >= 0)
                        {
                            path = string.Join(@"\", keypath.Split('\\').Take(keypath.Split('\\').Length - 1));
                        }

                        RegistryItemCustom item = new()
                        {
                            Name = keypath.Split('\\')[0],
                            Hive = hive,
                            Key = path,
                            Type = RegistryItemType.Key,
                            Value = "",
                            ValueType = 0
                        };
                        BrowserCtrl.ChangeCurrentItem(item);
                        PathInput.Text = "";
                        JumpToGrid.Visibility = Visibility.Collapsed;
                        JumpToButton.IsChecked = false;
                    }
                });
            });
            }
        }

        private async void AddKeyButton_Click(object sender, RoutedEventArgs e)
        {
            if (BrowserCtrl._currentRegItem == null)
            {
                return;
            }

            string keypath = BrowserCtrl._currentRegItem.Key;

            if (string.IsNullOrEmpty(keypath))
            {
                keypath = BrowserCtrl._currentRegItem.Name;
            }
            else
            {
                keypath = keypath + @"\" + BrowserCtrl._currentRegItem.Name;
            }

            if (BrowserCtrl._currentRegItem.Type == RegistryItemType.Hive)
            {
                keypath = "";
            }

            RegHives hive = BrowserCtrl._currentRegItem.Hive;
            await new AddKeyContentDialog(hive, keypath, "").ShowAsync();

            if (BrowserCtrl._currentRegItem == null)
            {
                BrowserCtrl.ChangeCurrentItem();
            }
            else
            {
                BrowserCtrl.ChangeCurrentItem(BrowserCtrl._currentRegItem);
            }
        }

        private void AddValueButton_Click(object sender, RoutedEventArgs e)
        {
            if (BrowserCtrl._currentRegItem == null)
            {
                return;
            }

            ShowCreateValueDialog();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            BrowserCtrl.SortByType = !BrowserCtrl.SortByType;
            BrowserCtrl.RefreshListView();
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            ShowFavoriteDialog();
        }

        private void Breadcrumbbar_OnItemClick(object sender, BreadCrumbControl.ItemClickEventArgs e)
        {
            if (e.ClickedItem == null)
            {
                BrowserCtrl.ChangeCurrentItem();
            }
            else
            {
                BrowserCtrl.ChangeCurrentItem((RegistryItemCustom)e.ClickedItem);
            }
        }

        private void BrowserControl_OnCurrentItemChanged(object sender,
            BrowserControl.CurrentItemChangedEventArgs e)
        {
            if (e.newItem == null)
            {
                BreadCrumbBarIcon.Text = "";
                MountHive.Visibility = Visibility.Collapsed;
            }

            if (e.newItem != null)
            {
                if (e.newItem.Type == RegistryItemType.Hive && (e.newItem.Hive == RegHives.HKEY_LOCAL_MACHINE || e.newItem.Hive == RegHives.HKEY_USERS))
                {
                    MountHive.Visibility = Visibility.Visible;
                }
                else
                {
                    MountHive.Visibility = Visibility.Collapsed;
                }

                ObservableCollection<BreadCrumbControl.BreadCrumbItem> BreadCrumbItemsList = new();

                if (e.newItem.Type != RegistryItemType.Value)
                {
                    if (e.newItem.Type == RegistryItemType.Hive)
                    {
                        BreadCrumbBarIcon.Text = "";
                        BreadCrumbItemsList.Add(new BreadCrumbControl.BreadCrumbItem() { DisplayName = _helper.GetFriendlyName(), ItemObject = null });
                        BreadCrumbItemsList.Add(new BreadCrumbControl.BreadCrumbItem() { DisplayName = e.newItem.Hive.ToString(), ItemObject = e.newItem });
                        Breadcrumbbar.ItemsSource = BreadCrumbItemsList;
                    }
                    else
                        if ((e.newItem.Key?.Length == 0) || (e.newItem.Key == null))
                    {
                        BreadCrumbBarIcon.Text = "";
                        BreadCrumbItemsList.Add(new BreadCrumbControl.BreadCrumbItem() { DisplayName = _helper.GetFriendlyName(), ItemObject = null });
                        BreadCrumbItemsList.Add(new BreadCrumbControl.BreadCrumbItem() { DisplayName = e.newItem.Hive.ToString(), ItemObject = new RegistryItemCustom() { Hive = e.newItem.Hive, Key = null, Name = e.newItem.Hive.ToString(), Type = RegistryItemType.Hive, Value = null, ValueType = 0 } });
                        BreadCrumbItemsList.Add(new BreadCrumbControl.BreadCrumbItem() { DisplayName = e.newItem.Name, ItemObject = e.newItem });
                        Breadcrumbbar.ItemsSource = BreadCrumbItemsList;
                    }
                    else
                    {
                        BreadCrumbBarIcon.Text = "";
                        BreadCrumbItemsList.Add(new BreadCrumbControl.BreadCrumbItem() { DisplayName = _helper.GetFriendlyName(), ItemObject = null });
                        BreadCrumbItemsList.Add(new BreadCrumbControl.BreadCrumbItem() { DisplayName = e.newItem.Hive.ToString(), ItemObject = new RegistryItemCustom() { Hive = e.newItem.Hive, Key = null, Name = e.newItem.Hive.ToString(), Type = RegistryItemType.Hive, Value = null, ValueType = 0 } });
                        string current = "";

                        foreach (string item in e.newItem.Key.Split('\\'))
                        {
                            BreadCrumbItemsList.Add(new BreadCrumbControl.BreadCrumbItem() { DisplayName = item, ItemObject = new RegistryItemCustom() { Hive = e.newItem.Hive, Key = current, Name = item, Type = RegistryItemType.Key, Value = null, ValueType = 0 } });

                            if (current?.Length == 0)
                            {
                                current = item;
                            }
                            else
                            {
                                current = current + @"\" + item;
                            }
                        }

                        BreadCrumbItemsList.Add(new BreadCrumbControl.BreadCrumbItem() { DisplayName = e.newItem.Name, ItemObject = e.newItem });
                        Breadcrumbbar.ItemsSource = BreadCrumbItemsList;
                    }
                }
                else
                {
                    _ = e.newItem.Key ?? "";
                    ShowEditValueDialog(e.newItem, false);
                    //await
                    //new EditRegValueContentDialog(e.newItem.Hive, key, e.newItem.Name, e.newItem.ValueType, true)
                    //.Init();
                }
            }
            else
            {
                ObservableCollection<BreadCrumbControl.BreadCrumbItem> BreadCrumbItemsList = new()
                {
                    new BreadCrumbControl.BreadCrumbItem() { DisplayName = _helper.GetFriendlyName(), ItemObject = null }
                };
                Breadcrumbbar.ItemsSource = BreadCrumbItemsList;
            }
        }

        private void FavCancel_Click(object sender, RoutedEventArgs e)
        {
            HideFavoriteDialog();
        }

        private void FavoriteListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            BrowserCtrl.ChangeCurrentItem((e.ClickedItem as BrowserControl.Item)?.regitem);
            HideFavoriteDialog();
        }

        private List<BrowserControl.Item> GetFavoriteItemList()
        {
            List<BrowserControl.Item> itemlist = new();
            //try
            //{
            ApplicationData applicationData = ApplicationData.Current;
            ApplicationDataContainer localSettings = applicationData.LocalSettings;
            object strlist = localSettings.Values["browserfavlist"];

            if ((strlist == null) || (strlist.GetType() != typeof(string)))
            {
                localSettings.Values["browserfavlist"] = "";
            }

            strlist = localSettings.Values["browserfavlist"];

            if ((string)strlist != "")
            {
                List<string> list = ((string)strlist).Split('\n').ToList();

                foreach (string item in list)
                {
                    try
                    {
                        Debug.WriteLine(item);
                        Debug.WriteLine(localSettings.Values[item].GetType());

                        if ((localSettings.Values[item].GetType() == typeof(bool)) && ((bool)localSettings.Values[item]))
                        {
                            itemlist.Add(new BrowserControl.Item(GetItemFromId(string.Join("_", item.Split('_').Skip(1)))));
                        }
                    }
                    catch
                    {
                    }
                }
            }

            return itemlist;
        }

        private uint GetSelectedType()
        {
            if (TypeSelector.SelectedIndex != 12)
            {
                if (ValueTypeInput != null)
                {
                    ValueTypeInput.Visibility = Visibility.Collapsed;
                }

                switch (TypeSelector.SelectedIndex)
                {
                    case 0:
                        {
                            return (uint)RegTypes.REG_BINARY;
                        }

                    case 1:
                        {
                            return (uint)RegTypes.REG_FULL_RESOURCE_DESCRIPTOR;
                        }

                    case 2:
                        {
                            return (uint)RegTypes.REG_DWORD;
                        }

                    case 3:
                        {
                            return (uint)RegTypes.REG_DWORD_BIG_ENDIAN;
                        }

                    case 4:
                        {
                            return (uint)RegTypes.REG_QWORD;
                        }

                    case 5:
                        {
                            return (uint)RegTypes.REG_MULTI_SZ;
                        }

                    case 6:
                        {
                            return (uint)RegTypes.REG_NONE;
                        }

                    case 7:
                        {
                            return (uint)RegTypes.REG_RESOURCE_LIST;
                        }

                    case 8:
                        {
                            return (uint)RegTypes.REG_RESOURCE_REQUIREMENTS_LIST;
                        }

                    case 9:
                        {
                            return (uint)RegTypes.REG_SZ;
                        }

                    case 10:
                        {
                            return (uint)RegTypes.REG_LINK;
                        }

                    case 11:
                        {
                            return (uint)RegTypes.REG_EXPAND_SZ;
                        }
                }

                return 0;
            }

            try
            {
                if (ValueTypeInput != null)
                {
                    ValueTypeInput.Visibility = Visibility.Visible;
                    uint val = uint.Parse(ValueTypeInput.Text);
                    return val;
                }
            }
            catch
            {
            }

            return 0;
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

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (ValEditCtrl.Visibility == Visibility.Visible)
            {
                HideEditValueDialog();
            }
            else if (ValCreateCtrl.Visibility == Visibility.Visible)
            {
                HideCreateValueDialog();
            }
            else if (FavListCtrl.Visibility == Visibility.Visible)
            {
                HideFavoriteDialog();
            }
            else if (!BrowserCtrl.GoBack())
            {
                SystemNavigationManager.GetForCurrentView().BackRequested -= RegistryBrowserPage_BackRequested;

                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
                }

                Shell shell = (Shell)App.AppContent;
                shell.RootFrame.Navigate(typeof(WelcomePage));
                shell.RootFrame.BackStack.Clear();
            }
            e.Handled = true;
        }

        private void HideEditValueDialog()
        {
            BrowserCtrl.Visibility = Visibility.Visible;
            Storyboard sb = Resources["RevertAnimation"] as Storyboard;
            sb.Completed += Sb_Completed;
            sb.Begin();
        }

        private void HideFavoriteDialog()
        {
            BrowserCtrl.Visibility = Visibility.Visible;
            Storyboard sb = Resources["RevertAnimationFavorite"] as Storyboard;
            sb.Completed += Sb_Completed3;
            sb.Begin();
        }

        private async void JumpToButton_Click(object sender, RoutedEventArgs e)
        {
            if (JumpToGrid.Visibility == Visibility.Collapsed)
            {
                JumpToGrid.Visibility = Visibility.Visible;
                JumpToButton.IsChecked = true;
                PathInput.Focus(FocusState.Pointer);
                RegistryItemCustom currentitem = BrowserCtrl._currentRegItem;
                string pathinput = "";

                if (currentitem != null)
                {
                    switch (currentitem.Type)
                    {
                        case RegistryItemType.Hive:
                            pathinput = currentitem.Hive.ToString();
                            break;

                        case RegistryItemType.Key:
                            pathinput = currentitem.Hive.ToString() + @"\" + currentitem.Key + @"\" + currentitem.Name;

                            if (string.IsNullOrEmpty(currentitem.Key))
                            {
                                pathinput = currentitem.Hive.ToString() + @"\" + currentitem.Name;
                            }

                            break;

                        case RegistryItemType.Value:
                            break;
                    }
                }

                PathInput.Text = pathinput;

                if (App.Fancyness)
                {
                    await JumpToGrid.Offset(offsetX: 0.0f,
                                            offsetY: -100.0f,
                                            duration: 0,
                                            delay: 0
                                           ).Fade(
                                             value: 0,
                                             duration: 0,
                                             delay: 0
                                           ).StartAsync();
                    JumpToGrid.Offset(offsetX: 0.0f,
                                      offsetY: 0.0f,
                                      duration: 300,
                                      delay: 0
                                     ).Fade(
                                       value: 1,
                                       duration: 100,
                                       delay: 150
                                     ).Start();
                    BrowserCtrl.Fade(
                      value: 0.5f,
                      duration: 300,
                      delay: 0
                    ).Start();
                }
            }
            else
            {
                if (App.Fancyness)
                {
                    JumpToGrid.Offset(offsetX: 0.0f,
                                      offsetY: -100.0f,
                                      duration: 300,
                                      delay: 0
                                     ).Fade(
                                       value: 0,
                                       duration: 100,
                                       delay: 0
                                     ).Start();
                    await BrowserCtrl.Fade(
                      value: 1,
                      duration: 300,
                      delay: 0
                    ).StartAsync();
                }

                JumpToGrid.Visibility = Visibility.Collapsed;
                JumpToButton.IsChecked = false;
            }
        }

        private void KeyActionButton_Click(object sender, RoutedEventArgs e)
        {
            string hivename;
            string keypath;

            if (PathInput.Text.Contains("\\"))
            {
                hivename = PathInput.Text.Split(char.Parse("\\"))[0];
                keypath = PathInput.Text.Substring(hivename.Length + 1, PathInput.Text.Length - hivename.Length - 1);
            }
            else
            {
                hivename = PathInput.Text;
                keypath = "";
            }

            bool result = GetHiveFromName(hivename, out RegHives hive);

            if (result)
            {
                RunInThreadPool(async () =>
                {
                    KeyStatus status = await _helper.GetKeyStatus(hive, keypath);
                    RunInUiThread(() =>
                    {
                        switch (status)
                        {
                            case KeyStatus.Found:
                                {
                                    if (keypath != "")
                                    {
                                        string path = "";

                                        if (keypath.Split('\\').Length - 1 >= 0)
                                        {
                                            path = string.Join(@"\",
                                                               keypath.Split('\\').Take(keypath.Split('\\').Length - 1));
                                        }

                                        RegistryItemCustom item = new()
                                        {
                                            Name = keypath.Split('\\')[0],
                                            Hive = hive,
                                            Key = path,
                                            Type = RegistryItemType.Key,
                                            Value = "",
                                            ValueType = 0
                                        };
                                        BrowserCtrl.ChangeCurrentItem(item);
                                    }
                                    else
                                    {
                                        RegistryItemCustom item = new()
                                        {
                                            Name = hive.ToString(),
                                            Hive = hive,
                                            Key = "",
                                            Type = RegistryItemType.Hive,
                                            Value = "",
                                            ValueType = 0
                                        };
                                        BrowserCtrl.ChangeCurrentItem(item);
                                    }

                                    PathInput.Text = "";
                                    JumpToGrid.Visibility = Visibility.Collapsed;
                                    JumpToButton.IsChecked = false;
                                    break;
                                }

                            case KeyStatus.NotFound:
                                {
                                    AddKey(hive, keypath);
                                    break;
                                }
                        }
                    });
                });
            }
        }

        private async void MountHive_Click(object sender, RoutedEventArgs e)
        {
            if (BrowserCtrl._currentRegItem.Type == RegistryItemType.Hive && (BrowserCtrl._currentRegItem.Hive == RegHives.HKEY_LOCAL_MACHINE || BrowserCtrl._currentRegItem.Hive == RegHives.HKEY_USERS))
            {
                MountHive.Visibility = Visibility.Visible;

                bool inUser = false;

                if (BrowserCtrl._currentRegItem.Hive == RegHives.HKEY_USERS)
                {
                    inUser = true;
                }

                Windows.Storage.Pickers.FileOpenPicker picker = new()
                {
                    ViewMode = Windows.Storage.Pickers.PickerViewMode.List,
                    SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.ComputerFolder
                };

                picker.FileTypeFilter.Add("*");

                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    await new MountHiveContentDialog().MountHive(file.Path, inUser);
                    if (BrowserCtrl._currentRegItem == null)
                    {
                        BrowserCtrl.ChangeCurrentItem();
                    }
                    else
                    {
                        BrowserCtrl.ChangeCurrentItem(BrowserCtrl._currentRegItem);
                    }
                }
            }
            else
            {
                MountHive.Visibility = Visibility.Collapsed;
            }
        }

        private void PathInput_LostFocus(object sender, RoutedEventArgs e)
        {
            string hivename;
            string keypath;

            if (PathInput.Text.Contains("\\"))
            {
                hivename = PathInput.Text.Split(char.Parse("\\"))[0];
                keypath = PathInput.Text.Substring(hivename.Length + 1, PathInput.Text.Length - hivename.Length - 1);
            }
            else
            {
                hivename = PathInput.Text;
                keypath = "";
            }

            bool result = GetHiveFromName(hivename, out RegHives hive);

            if (result)
            {
                RunInThreadPool(async () =>
                {
                    KeyStatus status = await _helper.GetKeyStatus(hive, keypath);
                    RunInUiThread(() =>
                    {
                        switch (status)
                        {
                            case KeyStatus.Found:
                                {
                                    KeyActionButton.IsEnabled = true;
                                    KeyActionIcon.Symbol = Symbol.Forward;
                                    break;
                                }

                            case KeyStatus.NotFound:
                                {
                                    KeyActionButton.IsEnabled = true;
                                    KeyActionIcon.Symbol = Symbol.Add;
                                    break;
                                }

                            case KeyStatus.AccessDenied:
                            case KeyStatus.Unknown:
                                {
                                    KeyActionButton.IsEnabled = false;
                                    KeyActionIcon.Symbol = Symbol.Cancel;
                                    break;
                                }
                        }
                    });
                });
            }
            else
            {
                KeyActionButton.IsEnabled = false;
                KeyActionIcon.Symbol = Symbol.Cancel;
            }
        }

        private void RegistryBrowserPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (ValEditCtrl.Visibility == Visibility.Visible)
            {
                HideEditValueDialog();
            }
            else if (ValCreateCtrl.Visibility == Visibility.Visible)
            {
                HideCreateValueDialog();
            }
            else if (FavListCtrl.Visibility == Visibility.Visible)
            {
                HideFavoriteDialog();
            }
            else if (!BrowserCtrl.GoBack())
            {
                SystemNavigationManager.GetForCurrentView().BackRequested -= RegistryBrowserPage_BackRequested;

                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
                }

                Shell shell = (Shell)App.AppContent;
                shell.RootFrame.Navigate(typeof(WelcomePage));
                shell.RootFrame.BackStack.Clear();
            }
            e.Handled = true;
        }

        private void RegistryBrowserPage_Unloaded(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().BackRequested -= RegistryBrowserPage_BackRequested;

            if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
            {
                HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
            }
        }

        private void Sb_Completed(object sender, object e)
        {
            ValEditCtrl.Visibility = Visibility.Collapsed;
            MainCommandBar.Visibility = Visibility.Visible;
        }

        private void Sb_Completed3(object sender, object e)
        {
            FavListCtrl.Visibility = Visibility.Collapsed;
            MainCommandBar.Visibility = Visibility.Visible;
        }

        private async void ShowEditValueDialog(RegistryItemCustom edititem, bool refresh)
        {
            currentEditItem = edititem;

            if (!refresh)
            {
                ValEditCtrl.Visibility = Visibility.Visible;
                Storyboard sb = Resources["PlayAnimation"] as Storyboard;
                sb.Begin();
                BrowserCtrl.Visibility = Visibility.Collapsed;
                MainCommandBar.Visibility = Visibility.Collapsed;
            }

            ValueTypeInput.Visibility = Visibility.Collapsed;
            ValueTypeInput.Text = "";
            EditItemTitle.Text = currentEditItem.Name;

            if (currentEditItem.Name?.Length == 0)
            {
                EditItemTitle.Text = "(Default)";
            }

            switch (currentEditItem.Type)
            {
                case RegistryItemType.Hive:
                    {
                        EditItemDesc.Text = ResourceManager.Current.MainResourceMap.GetValue("Resources/Hive", ResourceContext.GetForCurrentView()).ValueAsString;
                        break;
                    }

                case RegistryItemType.Key:
                    {
                        EditItemDesc.Text = ResourceManager.Current.MainResourceMap.GetValue("Resources/Key", ResourceContext.GetForCurrentView()).ValueAsString;
                        break;
                    }

                case RegistryItemType.Value:
                    {
                        if (currentEditItem.ValueType < 12)
                        {
                            EditItemDesc.Text = GetValueTypeName((RegTypes)currentEditItem.ValueType);
                        }
                        else
                        {
                            EditItemDesc.Text = "Custom: " + currentEditItem.ValueType;
                        }

                        break;
                    }

                default:
                    {
                        EditItemDesc.Text = ResourceManager.Current.MainResourceMap.GetValue("Resources/Unknown", ResourceContext.GetForCurrentView()).ValueAsString;
                        break;
                    }
            }

            uint regtype;
            string regvalue;
            GetKeyValueReturn2 ret = await _helper.GetKeyValue(currentEditItem.Hive, currentEditItem.Key ?? "", currentEditItem.Name, currentEditItem.ValueType); regtype = ret.regtype; regvalue = ret.regvalue;

            switch (regtype)
            {
                case (uint)RegTypes.REG_BINARY:
                    {
                        TypeSelector.SelectedIndex = 0;
                        break;
                    }

                case (uint)RegTypes.REG_FULL_RESOURCE_DESCRIPTOR:
                    {
                        TypeSelector.SelectedIndex = 1;
                        break;
                    }

                case (uint)RegTypes.REG_DWORD:
                    {
                        TypeSelector.SelectedIndex = 2;
                        break;
                    }

                case (uint)RegTypes.REG_DWORD_BIG_ENDIAN:
                    {
                        TypeSelector.SelectedIndex = 3;
                        break;
                    }

                case (uint)RegTypes.REG_QWORD:
                    {
                        TypeSelector.SelectedIndex = 4;
                        break;
                    }

                case (uint)RegTypes.REG_MULTI_SZ:
                    {
                        TypeSelector.SelectedIndex = 5;
                        break;
                    }

                case (uint)RegTypes.REG_NONE:
                    {
                        TypeSelector.SelectedIndex = 6;
                        break;
                    }

                case (uint)RegTypes.REG_RESOURCE_LIST:
                    {
                        TypeSelector.SelectedIndex = 7;
                        break;
                    }

                case (uint)RegTypes.REG_RESOURCE_REQUIREMENTS_LIST:
                    {
                        TypeSelector.SelectedIndex = 8;
                        break;
                    }

                case (uint)RegTypes.REG_SZ:
                    {
                        TypeSelector.SelectedIndex = 9;
                        break;
                    }

                case (uint)RegTypes.REG_LINK:
                    {
                        TypeSelector.SelectedIndex = 10;
                        break;
                    }

                case (uint)RegTypes.REG_EXPAND_SZ:
                    {
                        TypeSelector.SelectedIndex = 11;
                        break;
                    }

                default:
                    {
                        TypeSelector.SelectedIndex = 12;
                        ValueTypeInput.Visibility = Visibility.Visible;
                        ValueTypeInput.Text = regtype.ToString();
                        break;
                    }
            }

            if (ValueDataInput != null)
            {
                ValueDataInput.Text = regvalue;
            }
        }

        private void ShowFavoriteDialog()
        {
            FavListCtrl.Visibility = Visibility.Visible;
            Storyboard sb = Resources["PlayAnimationFavorite"] as Storyboard;
            sb.Begin();
            BrowserCtrl.Visibility = Visibility.Collapsed;
            MainCommandBar.Visibility = Visibility.Collapsed;
            FavoriteListView.ItemsSource = GetFavoriteItemList();
        }

        private void TypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ValueDataInput != null)
                {
                    if (ValidateValue(GetSelectedType(), ValueDataInput.Text))
                    {
                        ValEditAccept.IsEnabled = true;
                        ValueDataBorder.BorderThickness = new Thickness(0);
                    }
                    else
                    {
                        ValEditAccept.IsEnabled = false;
                        ValueDataBorder.BorderThickness = new Thickness(2);
                    }
                }
            }
            catch
            {
            }
        }

        private void ValEditAccept_Click(object sender, RoutedEventArgs e)
        {
            if (ValueDataInput != null)
            {
                _helper.SetKeyValue(currentEditItem.Hive, currentEditItem.Key ?? "", currentEditItem.Name, GetSelectedType(), ValueDataInput.Text);

                if (BrowserCtrl._currentRegItem == null)
                {
                    BrowserCtrl.ChangeCurrentItem();
                }
                else
                {
                    BrowserCtrl.ChangeCurrentItem(BrowserCtrl._currentRegItem);
                }

                HideEditValueDialog();
            }
        }

        private void ValEditCancel_Click(object sender, RoutedEventArgs e)
        {
            HideEditValueDialog();
        }

        private void ValEditRefresh_Click(object sender, RoutedEventArgs e)
        {
            ShowEditValueDialog(currentEditItem, true);
        }

        private bool ValidateValue(ulong type, string str)
        {
            switch (type)
            {
                case (ulong)RegTypes.REG_DWORD:
                    {
                        try
                        {
                            uint.Parse(str);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }

                case (ulong)RegTypes.REG_QWORD:
                    {
                        try
                        {
                            ulong.Parse(str);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }

                case (ulong)RegTypes.REG_MULTI_SZ:
                    {
                        return true;
                    }

                case (ulong)RegTypes.REG_SZ:
                    {
                        return !str.Contains("\n");
                    }

                case (ulong)RegTypes.REG_EXPAND_SZ:
                    {
                        return !str.Contains("\n");
                    }

                default:
                    {
                        try
                        {
                            byte[] buffer = StringToByteArrayFastest(str);

                            return true;
                        }
                        catch
                        {
                            return false;
                        }
                    }
            }
        }

        private void ValueDataInput_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if (ValueDataInput != null)
            {
                if (ValidateValue(GetSelectedType(), ValueDataInput.Text))
                {
                    ValEditAccept.IsEnabled = true;
                    ValueDataBorder.BorderThickness = new Thickness(0);
                }
                else
                {
                    ValEditAccept.IsEnabled = false;
                    ValueDataBorder.BorderThickness = new Thickness(2);
                }
            }
        }

        #region CreateValuePanel

        private void CreateTypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (CreateValueDataInput != null)
                {
                    if (ValidateValue(GetSelectedTypeCreate(), CreateValueDataInput.Text))
                    {
                        ValCreateAccept.IsEnabled = true;
                        CreateValueDataBorder.BorderThickness = new Thickness(0);
                    }
                    else
                    {
                        ValCreateAccept.IsEnabled = false;
                        CreateValueDataBorder.BorderThickness = new Thickness(2);
                    }
                }
            }
            catch
            {
            }
        }

        private void CreateValueDataInput_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            try
            {
                if (CreateValueDataInput != null)
                {
                    if (ValidateValue(GetSelectedTypeCreate(), CreateValueDataInput.Text))
                    {
                        ValCreateAccept.IsEnabled = true;
                        CreateValueDataBorder.BorderThickness = new Thickness(0);
                    }
                    else
                    {
                        ValCreateAccept.IsEnabled = false;
                        CreateValueDataBorder.BorderThickness = new Thickness(2);
                    }
                }
            }
            catch
            {
            }
        }

        private uint GetSelectedTypeCreate()
        {
            if (CreateTypeSelector.SelectedIndex != 12)
            {
                if (CreateValueTypeInput != null)
                {
                    CreateValueTypeInput.Visibility = Visibility.Collapsed;
                }

                switch (CreateTypeSelector.SelectedIndex)
                {
                    case 0:
                        {
                            return (uint)RegTypes.REG_BINARY;
                        }

                    case 1:
                        {
                            return (uint)RegTypes.REG_FULL_RESOURCE_DESCRIPTOR;
                        }

                    case 2:
                        {
                            return (uint)RegTypes.REG_DWORD;
                        }

                    case 3:
                        {
                            return (uint)RegTypes.REG_DWORD_BIG_ENDIAN;
                        }

                    case 4:
                        {
                            return (uint)RegTypes.REG_QWORD;
                        }

                    case 5:
                        {
                            return (uint)RegTypes.REG_MULTI_SZ;
                        }

                    case 6:
                        {
                            return (uint)RegTypes.REG_NONE;
                        }

                    case 7:
                        {
                            return (uint)RegTypes.REG_RESOURCE_LIST;
                        }

                    case 8:
                        {
                            return (uint)RegTypes.REG_RESOURCE_REQUIREMENTS_LIST;
                        }

                    case 9:
                        {
                            return (uint)RegTypes.REG_SZ;
                        }

                    case 10:
                        {
                            return (uint)RegTypes.REG_LINK;
                        }

                    case 11:
                        {
                            return (uint)RegTypes.REG_EXPAND_SZ;
                        }
                }

                return 0;
            }

            try
            {
                if (CreateValueTypeInput != null)
                {
                    CreateValueTypeInput.Visibility = Visibility.Visible;
                    uint val = uint.Parse(CreateValueTypeInput.Text);
                    return val;
                }
            }
            catch
            {
            }

            return 0;
        }

        private void HideCreateValueDialog()
        {
            BrowserCtrl.Visibility = Visibility.Visible;
            Storyboard sb = Resources["RevertAnimationCreate"] as Storyboard;
            sb.Completed += Sb_Completed2;
            sb.Begin();
        }

        private void Sb_Completed2(object sender, object e)
        {
            ValEditCtrl.Visibility = Visibility.Collapsed;
            MainCommandBar.Visibility = Visibility.Visible;
        }

        private void ShowCreateValueDialog()
        {
            ValCreateCtrl.Visibility = Visibility.Visible;
            Storyboard sb = Resources["PlayAnimationCreate"] as Storyboard;
            sb.Begin();
            BrowserCtrl.Visibility = Visibility.Collapsed;
            MainCommandBar.Visibility = Visibility.Collapsed;
            if (CreateValueTypeInput != null)
            {
                CreateValueTypeInput.Visibility = Visibility.Collapsed;
                CreateValueTypeInput.Text = "";
            }
            if (CreateValueTypeInput != null)
            {
                CreateValueDataInput.Text = "";
            }
            CreateTypeSelector.SelectedIndex = 9;
        }

        private void ValCreateAccept_Click(object sender, RoutedEventArgs e)
        {
            string keypath = BrowserCtrl._currentRegItem.Key;

            if (string.IsNullOrEmpty(keypath))
            {
                keypath = BrowserCtrl._currentRegItem.Name;
            }
            else
            {
                keypath = keypath + @"\" + BrowserCtrl._currentRegItem.Name;
            }

            if (BrowserCtrl._currentRegItem.Type == RegistryItemType.Hive)
            {
                keypath = "";
            }

            RegHives hive = BrowserCtrl._currentRegItem.Hive;
            _helper.SetKeyValue(hive, keypath, ValueNameInput.Text, GetSelectedTypeCreate(), CreateValueDataInput.Text);

            if (BrowserCtrl._currentRegItem == null)
            {
                BrowserCtrl.ChangeCurrentItem();
            }
            else
            {
                BrowserCtrl.ChangeCurrentItem(BrowserCtrl._currentRegItem);
            }

            HideCreateValueDialog();
        }

        private void ValCreateCancel_Click(object sender, RoutedEventArgs e)
        {
            HideCreateValueDialog();
        }

        private void ValueNameSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ValueNameInput != null)
                {
                    if (ValueNameSelector.SelectedIndex == 1)
                    {
                        ValueNameInput.Text = "";
                        ValueNameInput.IsEnabled = false;
                    }
                    else
                    {
                        ValueNameInput.Text = "";
                        ValueNameInput.IsEnabled = true;
                    }
                }
            }
            catch
            {
            }
        }

        #endregion CreateValuePanel
    }
}