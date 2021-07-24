using InteropTools.CorePages;
using InteropTools.Providers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TreeViewControl;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;
using Windows.System.Threading;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InteropTools.ShellPages.Registry
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BrowservNextPage : Page
    {
        public string PageName => "Registry Browser vNext";
        public PageGroup PageGroup => PageGroup.Registry;

        private readonly Compositor _compositor;

        public BrowservNextPage()
        {
            InitializeComponent();
            // Get the current compositor
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            sampleTreeView2.AllowDrop = false;
            sampleTreeView2.CanDrag = false;
            sampleTreeView2.CanDragItems = false;
            sampleTreeView2.CanReorderItems = false;
            TreeNode2 rootNode = CreateFolderNode("This device", null);
            sampleTreeView2.RootNode.Add(rootNode);
            sampleTreeView2.ItemClick += SampleTreeView2_ItemClick;
            ListBrowser.ItemsSource = _itemlist;
        }

        private readonly ObservableRangeCollection<Item> _itemlist = new ObservableRangeCollection<Item>();

        private async Task RunInUIThread(Action function)
        {
            await
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => { function(); });
        }

        private async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => { function(); });
        }


        private void SampleTreeView2_ItemClick(object sender, ItemClickEventArgs e)
        {
            _itemlist.ClearList();
            TreeNode2 node = e.ClickedItem as TreeNode2;
            RunInThreadPool(async () =>
            {
                if (node != null)
                {
                    FileSystemData data = node.Data as FileSystemData;

                    if (data != null)
                    {
                        if (node.IsExpanded && data.IsFolder)
                        {
                            string key = data.RegItem.Key;

                            if (data.RegItem.Type == RegistryItemType.KEY)
                            {
                                if ((key == "") || (key == null))
                                {
                                    key = data.RegItem.Name;
                                }

                                else
                                {
                                    key += @"\" + data.RegItem.Name;
                                }
                            }

                            if (key == null)
                            {
                                key = "";
                            }

                            IReadOnlyList<RegistryItemCustom> items = await App.MainRegistryHelper.GetRegistryItems2(data.RegItem.Hive, key);

                            foreach (RegistryItemCustom item in items)
                            {
                                if (item.Type == RegistryItemType.VALUE)
                                { await RunInUIThread(() => _itemlist.Add(new Item(item))); }
                            }

                            await RunInUIThread(() => ListBrowser.ItemsSource = _itemlist);
                        }

                        if (!node.HasItems)
                        {
                            if (data.RegItem == null)
                            {
                                IReadOnlyList<RegistryItemCustom> hives = await App.MainRegistryHelper.GetRegistryHives2();

                                foreach (RegistryItemCustom hive in hives)
                                {
                                    await RunInUIThread(() => node.Add(CreateFolderNode(hive.Name, hive)));
                                }
                            }

                            else
                            {
                                string key = data.RegItem.Key;

                                if (data.RegItem.Type == RegistryItemType.KEY)
                                {
                                    if ((key == "") || (key == null))
                                    {
                                        key = data.RegItem.Name;
                                    }

                                    else
                                    {
                                        key += @"\" + data.RegItem.Name;
                                    }
                                }

                                if (key == null)
                                {
                                    key = "";
                                }

                                IReadOnlyList<RegistryItemCustom> items = await App.MainRegistryHelper.GetRegistryItems2(data.RegItem.Hive, key);

                                foreach (RegistryItemCustom item in items)
                                {
                                    if (item.Type != RegistryItemType.VALUE)
                                    { await RunInUIThread(() => node.Add(CreateFolderNode(item.Name, item))); }
                                }
                            }
                        }
                    }
                }
            });
        }

        private async void SampleTreeView2_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            TreeNode2 node = args.Item as TreeNode2;

            if (node != null)
            {
                FileSystemData data = node.Data as FileSystemData;

                if (data != null)
                {
                    if (node.IsExpanded)
                    {
                        node.Clear();

                        if (data.RegItem == null)
                        {
                            IReadOnlyList<RegistryItemCustom> hives = await App.MainRegistryHelper.GetRegistryHives2();

                            foreach (RegistryItemCustom hive in hives)
                            {
                                TreeNode2 newnode = CreateFolderNode(hive.Name, hive);
                                node.Add(newnode);
                            }
                        }

                        else
                        {
                            string key = data.RegItem.Key;

                            if (data.RegItem.Type == RegistryItemType.KEY)
                            {
                                if ((key == "") || (key == null))
                                {
                                    key = data.RegItem.Name;
                                }

                                else
                                {
                                    key += @"\" + data.RegItem.Name;
                                }
                            }

                            if (key == null)
                            {
                                key = "";
                            }

                            IReadOnlyList<RegistryItemCustom> items = await App.MainRegistryHelper.GetRegistryItems2(data.RegItem.Hive, key);

                            foreach (RegistryItemCustom item in items)
                            {
                                if (item.Type != RegistryItemType.VALUE)
                                { node.Add(CreateFolderNode(item.Name, item)); }
                            }
                        }
                    }
                }
            }
        }

        private static TreeNode2 CreateFolderNode(string name, RegistryItemCustom item)
        {
            return new TreeNode2() { Data = new FileSystemData(name) { RegItem = item } };
        }


        private List<BrowserControl.Item> GetFavoriteItemList()
        {
            List<BrowserControl.Item> itemlist = new List<BrowserControl.Item>();
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

            //} catch (Exception e)
            //{
            //    new MessageDialog(e.StackTrace, e.Message + e.HResult).ShowAsync();
            //}
            return itemlist;
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

        private async void ListBrowser_ItemClick(object sender, ItemClickEventArgs e)
        {
            RegistryItemCustom currentEditItem = (e.ClickedItem as Item).regitem;

            ValEditCtrl.Visibility = Visibility.Visible;

            ValueTypeInput.Visibility = Visibility.Collapsed;
            ValueTypeInput.Text = "";
            EditItemTitle.Text = currentEditItem.Name;

            if (currentEditItem.Name == "")
            {
                EditItemTitle.Text = "(Default)";
            }

            switch (currentEditItem.Type)
            {
                case RegistryItemType.HIVE:
                    {
                        EditItemDesc.Text = ResourceManager.Current.MainResourceMap.GetValue("Resources/Hive", ResourceContext.GetForCurrentView()).ValueAsString;
                        break;
                    }

                case RegistryItemType.KEY:
                    {
                        EditItemDesc.Text = ResourceManager.Current.MainResourceMap.GetValue("Resources/Key", ResourceContext.GetForCurrentView()).ValueAsString;
                        break;
                    }

                case RegistryItemType.VALUE:
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
            GetKeyValueReturn2 ret = await App.MainRegistryHelper.GetKeyValue(currentEditItem.Hive, currentEditItem.Key ?? "", currentEditItem.Name, currentEditItem.ValueType); regtype = ret.regtype; regvalue = ret.regvalue;

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

            ValueDataInput.Text = regvalue;
        }

        public class Item : INotifyPropertyChanged
        {
            public Item(RegistryItemCustom regitem)
            {
                this.regitem = regitem;
            }

            public Visibility IsFavorite
            {
                get
                {
                    try
                    {
                        string id = regitem.Hive.ToString() + "%" + (regitem.Key == null ? "" : regitem.Key) + "%" + (regitem.Name == null ? "" : regitem.Name) + "%" + regitem.Type.ToString() + "%" +
                                 (regitem.Value == null ? "" : regitem.Value) + "%" + regitem.ValueType.ToString();
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
                        string id = regitem.Hive.ToString() + "%" + (regitem.Key == null ? "" : regitem.Key) + "%" + (regitem.Name == null ? "" : regitem.Name) + "%" + regitem.Type.ToString() + "%" +
                                 (regitem.Value == null ? "" : regitem.Value) + "%" + regitem.ValueType.ToString();
                        ApplicationData applicationData = ApplicationData.Current;
                        ApplicationDataContainer localSettings = applicationData.LocalSettings;
                        localSettings.Values["browserfav_" + id] = (value == Visibility.Visible);
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

            public string DisplayName
            {
                get
                {
                    if (regitem.Name == "")
                    {
                        return "(Default)";
                    }

                    return regitem.Name;
                }
            }

            public string LastModified
            {
                get
                {
                    switch (regitem.Type)
                    {
                        case RegistryItemType.HIVE:
                            {
                                //DateTime time;
                                //var res = App.MainRegistryHelper.GetKeyLastModifiedTime(this.regitem.Hive, null, out time);

                                //if (res != HelperErrorCodes.SUCCESS)
                                //{
                                return "";
                                //}

                                //return time.ToString();
                            }

                        case RegistryItemType.KEY:
                            {
                                //DateTime time;
                                //var res = App.MainRegistryHelper.GetKeyLastModifiedTime(this.regitem.Hive, this.regitem.Key + @"\" + this.regitem.Name, out time);

                                //if (res != HelperErrorCodes.SUCCESS)
                                //{
                                return "";
                                //}

                                //return time.ToString();
                            }

                        case RegistryItemType.VALUE:
                            {
                                return "";
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

            public RegistryItemCustom regitem { get; }

            public event PropertyChangedEventHandler PropertyChanged;

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
        }

        private void TypeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void ValueDataInput_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
        }

        private void ValEditRefresh_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ValEditCancel_Click(object sender, RoutedEventArgs e)
        {
            ValEditCtrl.Visibility = Visibility.Collapsed;
        }

        private void ValEditAccept_Click(object sender, RoutedEventArgs e)
        {
        }

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

            RegistryItemType typeconv = RegistryItemType.HIVE;

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

        private void FavoriteListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //BrowserCtrl.ChangeCurrentItem((e.ClickedItem as BrowserControl.Item).regitem);
            //HideFavoriteDialog();
        }
        private void RefreshFavoriteDialog()
        {
            FavoriteListView.ItemsSource = GetFavoriteItemList();
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshFavoriteDialog();
        }

        /*private void HideFavoriteDialog()
		{
		    BrowserCtrl.Visibility = Visibility.Visible;

		    Storyboard sb = this.Resources["RevertAnimationFavorite"] as Storyboard;
		    sb.Completed += Sb_Completed3;
		    sb.Begin();
		}*/

        /*private void Sb_Completed3(object sender, object e)
		{
		    FavListCtrl.Visibility = Visibility.Collapsed;
		    MainCommandBar.Visibility = Visibility.Visible;
		}*/

    }
}
