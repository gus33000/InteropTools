using InteropTools.CorePages;
using InteropTools.Providers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Management.Deployment;
using Windows.System;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace InteropTools.ShellPages.Registry
{
    public sealed partial class DefaultAppsPage : Page
    {
        public string PageName => "Default Apps";
        public PageGroup PageGroup => PageGroup.General;

        private readonly IRegistryProvider _helper;

        private readonly ObservableCollection<AppAssotiationItem> _itemlist =
          new();

        private readonly string _resUnknown = ResourceManager.Current.MainResourceMap.GetValue("Resources/Unknown", ResourceContext.GetForCurrentView()).
                                              ValueAsString;

        private readonly string _resNeutral =
          ResourceManager.Current.MainResourceMap.GetValue(
            "Resources/Neutral", ResourceContext.GetForCurrentView()).ValueAsString;

        public DefaultAppsPage()
        {
            InitializeComponent();
            _helper = App.MainRegistryHelper;
            Load();
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

        private void Load()
        {
            RunInThreadPool(async () =>
            {
                IReadOnlyList<RegistryItemCustom> items = await _helper.GetRegistryItems2(RegHives.HKEY_LOCAL_MACHINE,
                                                     @"SOFTWARE\Microsoft\DefaultApplications");
                await RunInUiThread(() =>
                {
                    LoadingBar.Minimum = 0;
                    LoadingBar.Maximum = items.Count;
                });
                int counter = -1;

                foreach (RegistryItemCustom item in items)
                {
                    counter++;
                    await RunInUiThread(() => { LoadingBar.Value = counter; });
                    AppAssotiationItem appasso = new() { Extension = item.Name };
                    List<AppAssotiation> listasso = new();
                    RegTypes regtype;
                    string regvalue;
                    GetKeyValueReturn ret = await _helper.GetKeyValue(RegHives.HKEY_CLASSES_ROOT, item.Name, "Content Type",
                                        RegTypes.REG_SZ);
                    regtype = ret.regtype;
                    regvalue = ret.regvalue;
                    appasso.Description = regvalue;
                    IReadOnlyList<RegistryItemCustom> items2 = await _helper.GetRegistryItems2(RegHives.HKEY_CURRENT_USER, @"Software\Classes\" + item.Name + @"\OpenWithProgids");

                    if (items2 != null)
                    {
                        foreach (RegistryItemCustom item2 in items2)
                        {
                            AppAssotiation appAssotiation = new();
                            IReadOnlyList<RegistryItemCustom> items3 = await _helper.GetRegistryItems2(RegHives.HKEY_CURRENT_USER,
                                                                  @"Software\Classes\" + item2.Name + @"\Application");

                            bool add
                                  = true;

                            foreach (RegistryItemCustom item3 in items3)
                            {
                                switch (item3.Name)
                                {
                                    case "ApplicationName":
                                        {
                                            appAssotiation.Title = item3.Value;
                                            break;
                                        }

                                    case "ApplicationDescription":
                                        {
                                            appAssotiation.Description = item3.Value;
                                            break;
                                        }

                                    case "AppUserModelID":
                                        {
                                            if (item3.Value.Contains("!"))
                                            {
                                                foreach (AppAssotiation listAssoItem in listasso)
                                                {
                                                    if (listAssoItem.Launchuri.ToLower() ==
                                                        item3.Value.Split('!')[0].ToLower())
                                                    {
                                                        Debug.WriteLine("First problem");

                                                        add
                                                              = false;
                                                    }
                                                }

                                                if (add)
                                                {
                                                    if (item3.Value.Split('!')[0].ToLower() == item.Value.ToLower())
                                                    {
                                                        appasso.Defaultapp = listasso.Count;
                                                    }
                                                }

                                                appAssotiation.Launchuri = item3.Value.Split('!')[0];
                                            }

                                            else
                                            {
                                                foreach (AppAssotiation listAssoItem in listasso)
                                                {
                                                    if (listAssoItem.Launchuri.ToLower() == item3.Value.ToLower())
                                                    {
                                                        Debug.WriteLine("Second problem");

                                                        add
                                                              = false;
                                                    }
                                                }

                                                if (add)
                                                {
                                                    if (item3.Value.ToLower() == item.Value.ToLower())
                                                    {
                                                        appasso.Defaultapp = listasso.Count;
                                                    }

                                                    appAssotiation.Launchuri = item3.Value;
                                                }
                                            }

                                            break;
                                        }
                                }
                            }

                            if (appAssotiation.Title == null)
                            {
                                appAssotiation.Title = appAssotiation.Launchuri;
                            }

                            if (add)
                            {
                                Debug.WriteLine("new object " + appAssotiation.Title);
                                listasso.Add(appAssotiation);
                            }
                        }
                    }

                    appasso.Applist = listasso;

                    if (listasso.Count != 0)
                    {
                        await RunInUiThread(() => { _itemlist.Add(appasso); });
                    }
                }

                try
                {
                    List<string> pfnlist = (from item in _itemlist from item2 in item.Applist select item2.Launchuri.ToLower()).ToList();
                    List<Packageinfos> packages = new();
                    PackageManager pkgman = new();
                    ObservableRangeCollection<Package> applist = new();
                    applist.AddRange(pkgman.FindPackagesForUserWithPackageTypes("", PackageTypes.None));
                    applist.AddRange(pkgman.FindPackagesForUserWithPackageTypes("", PackageTypes.Bundle));
                    applist.AddRange(pkgman.FindPackagesForUserWithPackageTypes("", PackageTypes.Framework));
                    applist.AddRange(pkgman.FindPackagesForUserWithPackageTypes("", PackageTypes.Main));

                    try
                    {
                        applist.AddRange(pkgman.FindPackagesForUserWithPackageTypes("", PackageTypes.Optional));
                    }

                    catch
                    {
                    }

                    applist.AddRange(pkgman.FindPackagesForUserWithPackageTypes("", PackageTypes.Resource));
                    applist.AddRange(pkgman.FindPackagesForUserWithPackageTypes("", PackageTypes.Xap));
                    List<Package> pkgs = new();

                    foreach (Package item in applist)
                    {
                        if (!pkgs.Contains(item))
                        {
                            pkgs.Add(item);
                        }
                    }

                    pkgs = pkgs.OrderBy(x => x.Id.FamilyName).ToList();

                    foreach (Package package in pkgs)
                    {
                        if (pfnlist.Contains(package.Id.FamilyName.ToLower()))
                        {
                            Packageinfos app = new() { Packagefamillyname = package.Id.FamilyName.ToLower() };
                            string arch = _resUnknown;

                            switch (package.Id.Architecture)
                            {
                                case ProcessorArchitecture.Arm:
                                    {
                                        arch = "ARM";
                                        break;
                                    }

                                case ProcessorArchitecture.Neutral:
                                    {
                                        arch = _resNeutral;
                                        break;
                                    }

                                case ProcessorArchitecture.Unknown:
                                    {
                                        arch = _resUnknown;
                                        break;
                                    }

                                case ProcessorArchitecture.X64:
                                    {
                                        arch = "x64";
                                        break;
                                    }

                                case ProcessorArchitecture.X86:
                                    {
                                        arch = "x86";
                                        break;
                                    }
                            }

                            app.Title = package.Id.FamilyName;
                            app.Description = arch + " " + package.Id.Version.Major + "." + package.Id.Version.Minor + "." +
                                              package.Id.Version.Build + "." + package.Id.Version.Revision;

                            try
                            {
                                IReadOnlyList<AppListEntry> appEntries = await package.GetAppListEntriesAsync();

                                foreach (AppListEntry appEntry in appEntries)
                                {
                                    try
                                    {
                                        app.Title = appEntry.DisplayInfo.DisplayName;
                                    }

                                    catch
                                    {
                                        // ignored
                                    }

                                    try
                                    {
                                        app.Description = appEntry.DisplayInfo.Description + "\n" + arch + " " +
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
                                            Height = 48,
                                            Width = 48
                                        };
                                        Windows.Storage.Streams.IRandomAccessStreamWithContentType applogo = await appEntry.DisplayInfo.GetLogo(logosize).OpenReadAsync();
                                        await RunInUiThread(() =>
                                        {
                                            BitmapImage bitmapImage = new();
                                            bitmapImage.SetSource(applogo);

                                            app.logo = bitmapImage;
                                        });
                                    }

                                    catch
                                    {
                                        // ignored
                                    }
                                }
                            }
                            catch
                            {
                                // ignored
                            }

                            packages.Add(app);
                        }
                    }

                    foreach (Packageinfos package in packages)
                    {
                        int maincounter = -1;

                        foreach (AppAssotiationItem item in _itemlist)
                        {
                            maincounter++;
                            int counter2 = -1;

                            foreach (AppAssotiation item2 in item.Applist)
                            {
                                counter2++;

                                if (!string.Equals(item2.Launchuri, package.Packagefamillyname, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    continue;
                                }

                                int maincounter1 = maincounter;
                                int counter3 = counter2;
                                _itemlist[maincounter1].Applist[counter3].Description = package.Description;
                                _itemlist[maincounter1].Applist[counter3].logo = package.logo;
                                _itemlist[maincounter1].Applist[counter3].Title = package.Title;
                            }
                        }
                    }

                    await RunInUiThread(() =>
                    {
                        FileAssociationsListView.ItemsSource = _itemlist;
                        LoadingRing.IsActive = false;
                        ProgressPanel.Visibility = Visibility.Collapsed;
                    });
                }

                catch
                {
                }
            });
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppAssotiationItem item = (AppAssotiationItem)((ComboBox)sender).DataContext;
            AppAssotiation selectedItem = (AppAssotiation)((ComboBox)sender).SelectedItem;

            if (selectedItem != null)
            {
                _helper.SetKeyValue(RegHives.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\DefaultApplications", item.Extension,
                                    RegTypes.REG_SZ, selectedItem.Launchuri);
            }
        }

        private class AppAssotiationItem
        {
            public string Extension { get; internal set; }
            public string Description { get; internal set; }
            public int Defaultapp { get; internal set; }
            public List<AppAssotiation> Applist { get; internal set; }
        }

        private class AppAssotiation
        {
            public string Title { get; internal set; }
            public string Description { get; internal set; }
            public dynamic logo { get; internal set; }
            public string Launchuri { get; internal set; }
        }

        private class Packageinfos
        {
            public string Title { get; internal set; }
            public string Description { get; internal set; }
            public dynamic logo { get; internal set; }
            public string Packagefamillyname { get; internal set; }
        }
    }
}