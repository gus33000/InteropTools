/*++

Copyright (c) 2016  Interop Tools Development Team
Copyright (c) 2017  Gustave M.

Module Name:

    Plugin.cs

Abstract:

    This module implements the WinRT Application Plugin.

Author:

    Gustave M.     (gus33000)       13-May-2017

Revision History:

    Gustave M. (gus33000) 13-May-2017

        Initial Implementation.

--*/

using InteropTools.Providers.Applications.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Management.Deployment;

namespace InteropTools.Providers.Applications.WinRTProvider
{
    public sealed class ApplicationsProvider : IBackgroundTask
    {
        private readonly IBackgroundTask internalTask = new ApplicationsProviderIntern();
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            internalTask.Run(taskInstance);
        }
    }

    internal class ApplicationsProviderIntern : ApplicationProvidersWithOptions
    {
        public class Item
        {
            public string DisplayName { get; set; }
            public string Description { get; set; }

            public string FullName { get; set; }

            public dynamic logo { get; set; }

            public PackageVolume volume { get; set; }

            public PackageTypes type { get; set; }
        }

        protected override async Task<string> ExecuteAsync(AppServiceConnection sender, string input, IProgress<double> progress, CancellationToken cancelToken)
        {
            string[] arr = input.Split(new string[] { "Q+q:8rKwjyVG\"~@<],TNH!@kcn/qUv:=3=Zs)+gU$Efc:[&Ku^qn,U}&yrRY{}byf<4DV&W!mF>R@Z8uz=>kgj~F[KeB{,]'[Veb" }, StringSplitOptions.None);

            string operation = arr[0];
            Enum.TryParse(operation, true, out APPLICATIONS_OPERATION operationenum);

            List<List<string>> returnvalue = new();
            List<string> returnvalue2 = new();

            switch (operationenum)
            {
                case APPLICATIONS_OPERATION.RemovePackage:
                    {
                        Enum.TryParse<RemovalOptions>(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), out RemovalOptions remop);
                        _ = await new PackageManager().RemovePackageAsync(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2))), remop);

                        returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(nameof(APPLICATIONS_STATUS.SUCCESS))));
                        returnvalue.Add(returnvalue2);
                        break;
                    }
                case APPLICATIONS_OPERATION.RegisterPackage:
                    {
                        Enum.TryParse<DeploymentOptions>(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), out DeploymentOptions remop);
                        _ = await new PackageManager().RegisterPackageAsync(new Uri(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2)))), null, remop);

                        returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(nameof(APPLICATIONS_STATUS.SUCCESS))));
                        returnvalue.Add(returnvalue2);
                        break;
                    }
                case APPLICATIONS_OPERATION.AddPackage:
                    {
                        Enum.TryParse<DeploymentOptions>(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), out DeploymentOptions remop);
                        _ = await new PackageManager().AddPackageAsync(new Uri(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2)))), null, remop);

                        returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(nameof(APPLICATIONS_STATUS.SUCCESS))));
                        returnvalue.Add(returnvalue2);
                        break;
                    }
                case APPLICATIONS_OPERATION.UpdatePackage:
                    {
                        Enum.TryParse<DeploymentOptions>(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(1))), out DeploymentOptions remop);
                        _ = await new PackageManager().UpdatePackageAsync(new Uri(Encoding.UTF8.GetString(Convert.FromBase64String(arr.ElementAt(2)))), null, remop);

                        returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(nameof(APPLICATIONS_STATUS.SUCCESS))));
                        returnvalue.Add(returnvalue2);
                        break;
                    }
                case APPLICATIONS_OPERATION.QueryApplicationVolumes:
                    {
                        IReadOnlyList<PackageVolume> vols = await new PackageManager().GetPackageVolumesAsync();

                        returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(nameof(APPLICATIONS_STATUS.SUCCESS))));
                        returnvalue.Add(returnvalue2);

                        foreach (PackageVolume item in vols)
                        {
                            List<string> itemlist = new()
                            {
                                Convert.ToBase64String(Encoding.UTF8.GetBytes(item.MountPoint)),
                                Convert.ToBase64String(Encoding.UTF8.GetBytes(item.PackageStorePath)),
                                Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Name)),
                                Convert.ToBase64String(Encoding.UTF8.GetBytes(item.IsSystemVolume.ToString())),
                                Convert.ToBase64String(Encoding.UTF8.GetBytes(item.IsOffline.ToString())),
                                Convert.ToBase64String(Encoding.UTF8.GetBytes(item.SupportsHardLinks.ToString()))
                            };
                            returnvalue.Add(itemlist);
                        }
                        break;
                    }
                case APPLICATIONS_OPERATION.QueryApplications:
                    {
                        try
                        {
                            //_volumelist = new ObservableCollection<VolumeDisplayitem>();
                            //_typelist = new ObservableCollection<TypeDisplayitem>();
                            //_itemsList = new ObservableRangeCollection<Item>();
                            //_filteredItemsList = new ObservableRangeCollection<Item>();
                            //_itemsList.CollectionChanged += ItemsList_CollectionChanged;
                            //_filteredItemsList.CollectionChanged += _filteredItemsList_CollectionChanged;
                            /*await RunInUiThread(() => {
                                LoadingText.Text = "Fetching available system volumes...";
                                LoadingStack.Visibility = Visibility.Visible;
                            });
                            var itemSource = AlphaKeyGroup<Item>.CreateGroups(_filteredItemsList, CultureInfo.InvariantCulture,
                                             s => s.DisplayName, true);
                            await RunInUiThread(() => {
                                ((CollectionViewSource)Resources["AppsGroups"]).Source = itemSource;
                            });*/
                            List<Item> tmplist = new();
                            IReadOnlyList<PackageVolume> vols = await new PackageManager().GetPackageVolumesAsync();
                            /*_volumelist.Add(new VolumeDisplayitem());

                            foreach (var vol in vols)
                            {
                                _volumelist.Add(new VolumeDisplayitem() { Volume = vol });
                            }

                            await RunInUiThread(() => {
                                VolListView.ItemsSource = _volumelist;
                                VolListView.SelectedIndex = 0;
                            });
                            await RunInUiThread(() => {
                                LoadingText.Text = "Fetching available package types...";
                                LoadingStack.Visibility = Visibility.Visible;
                            });*/
                            IEnumerable<PackageTypes> pkgtypes = Enum.GetValues(typeof(PackageTypes)).Cast<PackageTypes>();
                            /*_typelist.Add(new TypeDisplayitem());

                            foreach (var type in pkgtypes)
                            {
                                _typelist.Add(new TypeDisplayitem() { Type = type });
                            }*/

                            /*await RunInUiThread(() => {
                                TypeListView.ItemsSource = _typelist;
                                TypeListView.SelectedIndex = 0;
                            });
                            await RunInUiThread(() => {
                                LoadingText.Text = "Determining the number of packages present in the system...";
                                LoadingStack.Visibility = Visibility.Visible;
                            });*/
                            int numofpkgs = 0;

                            foreach (PackageVolume vol in vols)
                            {
                                foreach (PackageTypes type in pkgtypes)
                                {
                                    IList<Package> pkgs = vol.FindPackagesForUserWithPackageTypes("", type);
                                    numofpkgs += pkgs.Count;
                                }
                            }

                            double count = 0;

                            foreach (PackageVolume vol in vols)
                            {
                                List<Package> applist = new();

                                foreach (PackageTypes type in pkgtypes)
                                {
                                    IList<Package> pkgs = vol.FindPackagesForUserWithPackageTypes("", type);

                                    foreach (Package package in pkgs)
                                    {
                                        count++;
                                        /*await RunInUiThread(() => {
                                            LoadingText.Text = String.Format("Fetching information for packages... ({0}%)", Math.Round(count / numofpkgs * 100, 0));
                                        });*/

                                        string arch = package.Id.Architecture.ToString();

                                        string displayname = package.Id.FamilyName;
                                        string description = arch + " " + package.Id.Version.Major + "." + package.Id.Version.Minor + "." +
                                                          package.Id.Version.Build + "." + package.Id.Version.Revision;
                                        dynamic logo = "";

                                        try
                                        {
                                            IReadOnlyList<Windows.ApplicationModel.Core.AppListEntry> appEntries = await package.GetAppListEntriesAsync();

                                            foreach (Windows.ApplicationModel.Core.AppListEntry appEntry in appEntries)
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
                                                    /*await RunInUiThread(() => {
                                                        var bitmapImage = new BitmapImage();
                                                        bitmapImage.SetSource(applogo);
                                                        logo = bitmapImage;
                                                    });*/
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

                            foreach (Item item in tmplist)
                            {
                                List<string> itemlist = new()
                                {
                                    Convert.ToBase64String(Encoding.UTF8.GetBytes(item.DisplayName)),
                                    Convert.ToBase64String(Encoding.UTF8.GetBytes(item.FullName)),
                                    Convert.ToBase64String(Encoding.UTF8.GetBytes(item.Description)),
                                    Convert.ToBase64String(Encoding.UTF8.GetBytes(item.logo)),
                                    Convert.ToBase64String(Encoding.UTF8.GetBytes(item.volume.MountPoint)),
                                    Convert.ToBase64String(Encoding.UTF8.GetBytes(item.type.ToString()))
                                };
                                returnvalue.Add(itemlist);
                            }
                        }
                        catch (Exception)
                        {
                        }

                        returnvalue2.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes(nameof(APPLICATIONS_STATUS.SUCCESS))));
                        returnvalue.Add(returnvalue2);

                        break;
                    }
            }

            string returnstr = "";

            foreach (List<string> str in returnvalue)
            {
                string str2 = string.Join("*[Pp)8/P'=Tu(pm\"fYNh#*7w27V~>bubdt#\"AF~'\\}{jwAE2uY5,~bEVfBZ2%xx+UK?c&Xr@)C6/}j?5rjuB=8+egU\\D@\"; T3M<%", str);
                if (string.IsNullOrEmpty(returnstr))
                {
                    returnstr = str2;
                }
                else
                {
                    returnstr += "Q+q:8rKwjyVG\"~@<],TNH!@kcn/qUv:=3=Zs)+gU$Efc:[&Ku^qn,U}&yrRY{}byf<4DV&W!mF>R@Z8uz=>kgj~F[KeB{,]'[Veb" + str2;
                }
            }

            return returnstr;
        }

        protected override Task<Options> GetOptions()
        {
            return Task.FromResult<Options>(new OSRebootProviderOptions());
        }

        protected override Guid GetOptionsGuid()
        {
            return OSRebootProviderOptions.ID;
        }
    }
}
