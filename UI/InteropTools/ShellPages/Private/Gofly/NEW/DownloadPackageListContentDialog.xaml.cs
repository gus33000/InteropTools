using GetAppxPackages.WU;
using Newtonsoft.Json;
using StoreProjectApp.Presentation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StoreProjectApp.ContentDialogs.NEW
{
    public sealed partial class DownloadPackageListContentDialog : ContentDialog
    {
        private List<Item> ItemsList = new List<Item>();

        public class Item
        {
            public string Name { get; set; }
            public string Symbol { get; set; }
            public string Type { get; set; }
            public string Uri { get; set; }
        }

        public DownloadPackageListContentDialog(Product item, string token, string ring, string content = "Active")
        {
            this.InitializeComponent();
            Load(item, token, ring, content);
        }

        public class Rootobject
        {
            [JsonProperty(PropertyName = "blob.version")]
            public long blobversion { get; set; }
            [JsonProperty(PropertyName = "content.bundledPackages")]
            public string[] contentbundledPackages { get; set; }
            [JsonProperty(PropertyName = "content.isMain")]
            public bool contentisMain { get; set; }
            [JsonProperty(PropertyName = "content.packageId")]
            public string contentpackageId { get; set; }
            [JsonProperty(PropertyName = "content.productId")]
            public string contentproductId { get; set; }
            [JsonProperty(PropertyName = "content.targetPlatforms")]
            public ContentTargetplatforms[] contenttargetPlatforms { get; set; }
            [JsonProperty(PropertyName = "content.type")]
            public int contenttype { get; set; }
            public Policy policy { get; set; }
            public Policy2 policy2 { get; set; }
        }

        public class Policy
        {
            public string categoryfirst { get; set; }
            public string categorysecond { get; set; }
            public bool optOutbackupRestore { get; set; }
            public bool optOutremoveableMedia { get; set; }
        }

        public class Policy2
        {
            public int ageRating { get; set; }
            public bool optOutDVR { get; set; }
            public Thirdpartyapprating[] thirdPartyAppRatings { get; set; }
        }

        public class Thirdpartyapprating
        {
            public int level { get; set; }
            public int systemId { get; set; }
        }

        public class ContentTargetplatforms
        {
            [JsonProperty(PropertyName = "platform.maxVersionTested")]
            public long platformmaxVersionTested { get; set; }
            [JsonProperty(PropertyName = "platform.minVersion")]
            public long platformminVersion { get; set; }
            [JsonProperty(PropertyName = "platform.target")]
            public int platformtarget { get; set; }
        }

        public async void Load(Product item, string testtoken, string ring, string content)
        {
            var catid = item.WUCatID;

            Parser utils = new Parser();

            var uplist = await utils.ScanUpdates(testtoken, "<Id>" + catid + "</Id>", "Application", ring, content);
            foreach (var update in uplist)
            {
                if (update.IsAppxFramework)
                    continue;

                if (update.UpdateFiles != null)
                {
                    foreach (var file in update.UpdateFiles)
                    {
                        var pfn = update.PackageMoniker;
                        var ext = file.FileName.Split('.')[1];

                        if (ext.ToLower().EndsWith("cab"))
                            continue;

                        string archstring = "";

                        try
                        {
                            archstring = pfn.Split('_')[pfn.Split('_').Length - 3];
                        }
                        catch
                        {

                        }

                        //if (archstring == "neutral")
                        //{
                            if (update.ApplicabilityBlob != null)
                            {
                                Rootobject applicability = JsonConvert.DeserializeObject<Rootobject>(update.ApplicabilityBlob);
                                
                                if (applicability.contentbundledPackages != null)
                                {
                                    archstring += " (";

                                    string newstr = "";

                                    foreach (var itm in applicability.contentbundledPackages)
                                    {
                                        if (newstr != "")
                                            newstr += ", ";
                                        newstr += itm.Split('_')[pfn.Split('_').Length - 3];
                                    }

                                    archstring += newstr + ")";
                                }
                                
                                if (applicability.contenttargetPlatforms != null)
                                {
                                    archstring += "\nfor ";

                                    string newstr = "";

                                    foreach (var itm in applicability.contenttargetPlatforms)
                                    {
                                        if (newstr != "")
                                            newstr += ", ";

                                        newstr += itm.platformtarget.ToString().Replace("10", "Windows Holographic").Replace("0", "Windows Universal").Replace("3", "Windows Desktop").Replace("4", "Windows Mobile").Replace("5", "Windows Xbox").Replace("6", "Windows Team") + " ";

                                        ulong version = ulong.Parse(itm.platformminVersion.ToString());
                                        ulong major = (version & 0xFFFF000000000000L) >> 48;
                                        ulong minor = (version & 0x0000FFFF00000000L) >> 32;
                                        ulong build = (version & 0x00000000FFFF0000L) >> 16;
                                        ulong revision = (version & 0x000000000000FFFFL);
                                        var osVersion = $"{major}.{minor}.{build}.{revision}";

                                        newstr += "(minimum: " + osVersion;

                                        version = ulong.Parse(itm.platformmaxVersionTested.ToString());
                                        major = (version & 0xFFFF000000000000L) >> 48;
                                        minor = (version & 0x0000FFFF00000000L) >> 32;
                                        build = (version & 0x00000000FFFF0000L) >> 16;
                                        revision = (version & 0x000000000000FFFFL);
                                        osVersion = $"{major}.{minor}.{build}.{revision}";

                                        newstr += " maximum: " + osVersion + ")";
                                    }

                                    archstring += newstr;
                                }
                            }
                        //}

                        string Type = "";
                        string Symbol = "";
                        switch (ext.ToLower())
                        {
                            case "appxbundle":
                                {
                                    Type = archstring + "\nbundle package";// for " + platstring;
                                    Symbol = "";
                                    break;
                                }
                            case "appx":
                                {
                                    Type = archstring + "\npackage";// for " + platstring;
                                    Symbol = "";
                                    break;
                                }
                            case "xap":
                                {
                                    Type = archstring + "\nwp package";// for " + platstring;
                                    Symbol = "";
                                    break;
                                }
                            case "xvc":
                                {
                                    Type = archstring + "\nxbox package";// for " + platstring;
                                    Symbol = "";
                                    break;
                                }
                            default:
                                {
                                    Type = archstring + "\npackage";// for " + platstring;
                                    Symbol = "";
                                    break;
                                }
                        }

                        Type += " last modified on " + file.Modified;

                        ItemsList.Add(new Item { Name = pfn + "." + ext, Type = Type, Symbol = Symbol, Uri = file.Url });
                    }
                }
            }

            var nItemsList = ItemsList.OrderByAlphaNumeric(x => x.Name);

            /*var packages = item.ItemsList;
            ItemsList.Clear();
            foreach (var package in packages)
            {
                string archstring = "";
                foreach (string s in package.Architectures)
                {
                    if (archstring == "")
                    {
                        archstring = s;
                    }
                    else
                    {
                        archstring = archstring + " " + s;
                    }

                }
                string platstring = "";
                foreach (string s in package.PlatformDependencies)
                {
                    if (platstring == "")
                    {
                        platstring = s.Replace(".", " ");
                    }
                    else
                    {
                        platstring = platstring + " " + s.Replace(".", " ");
                    }

                }
                string Type = "";
                string Symbol = "";
                switch (package.PackageFormat.ToLower())
                {
                    case "appxbundle":
                        {
                            Type = archstring + " bundle package for " + platstring;
                            Symbol = "";
                            break;
                        }
                    case "appx": 
                        {
                            Type = archstring + " package for " + platstring;
                            Symbol = "";
                            break;
                        }
                    case "xap":
                        {
                            Type = archstring + " wp package for " + platstring;
                            Symbol = "";
                            break;
                        }
                    case "xvc":
                        {
                            Type = archstring + " xbox package for " + platstring;
                            Symbol = "";
                            break;
                        }
                    default:
                        {
                            Type = archstring + " package for " + platstring;
                            Symbol = "";
                            break;
                        }
                }
                string Name = package.PackageFullName + "." + package.PackageFormat;
                ItemsList.Add(new Item { Name = Name, Type = Type, Symbol = Symbol, Uri = package.PackageUri });
            }*/

            loadingbar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            PackageList.ItemsSource = nItemsList;
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            folderPicker.ViewMode = PickerViewMode.Thumbnail;
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder.Path != null)
            {
                var obj = new DownloadPackageProgressListContentDialog(PackageList.SelectedItems, folder);
                await obj.ShowAsync();
            }
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
