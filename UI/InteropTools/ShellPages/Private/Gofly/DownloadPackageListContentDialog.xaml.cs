using StoreProjectApp.Presentation;
using System;
using System.Collections.ObjectModel;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StoreProjectApp.ContentDialogs
{
    public sealed partial class DownloadPackageListContentDialog : ContentDialog
    {
        private ObservableCollection<Item> ItemsList = new ObservableCollection<Item>();

        public class Item
        {
            public string Name { get; set; }
            public string Symbol { get; set; }
            public string Type { get; set; }
            public string Uri { get; set; }
        }

        public DownloadPackageListContentDialog(Product item)
        {
            this.InitializeComponent();
            var packages = item.ItemsList;
            ItemsList.Clear();
            foreach (var package in packages)
            {
                //if (package.PackageFullName.ToLower().EndsWith(".cab"))
                    //continue;
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
            }
            PackageList.ItemsSource = ItemsList;
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
