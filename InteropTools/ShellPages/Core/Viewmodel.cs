using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using ApplicationPlugin = AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider; //, InteropTools.Providers.Applications.Definition.TransfareOptions

using RebootPlugin = AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider; //, InteropTools.Providers.OSReboot.Definition.TransfareOptions
using RegPlugin = AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider; //, InteropTools.Providers.Registry.Definition.TransfareOptions

namespace InteropTools.ShellPages.Core
{
    public class Viewmodel : DependencyObject
    {
        public class DisplayableRegPlugin
        {
            public BitmapImage Logo { get; set; }
            public RegPlugin Plugin { get; set; }

            public DisplayableRegPlugin(RegPlugin Plugin)
            {
                this.Plugin = Plugin;
            }
        }

        public class DisplayablePowerPlugin
        {
            public BitmapImage Logo { get; set; }
            public RebootPlugin Plugin { get; set; }

            public DisplayablePowerPlugin(RebootPlugin Plugin)
            {
                this.Plugin = Plugin;
            }
        }

        public class DisplayableApplicationPlugin
        {
            public BitmapImage Logo { get; set; }
            public ApplicationPlugin Plugin { get; set; }

            public DisplayableApplicationPlugin(ApplicationPlugin Plugin)
            {
                this.Plugin = Plugin;
            }
        }

        public ObservableCollection<DisplayableRegPlugin> RegPlugins { get; }
                  = new ObservableCollection<DisplayableRegPlugin>();

        public ObservableCollection<DisplayablePowerPlugin> RebootPlugins { get; }
                  = new ObservableCollection<DisplayablePowerPlugin>();

        public ObservableCollection<DisplayableApplicationPlugin> ApplicationPlugins { get; }
                  = new ObservableCollection<DisplayableApplicationPlugin>();

        public Viewmodel()
        {
#pragma warning disable CS4014
            InitAsync();
#pragma warning restore CS4014
        }

        private async void RunInThreadPool(Action function)
        {
            await ThreadPool.RunAsync(x => function());
        }

        private async Task InitAsync()
        {
            AppPlugin.PluginList.PluginList<string, string, double> reglist = await Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (RebootPlugin item in reglist.Plugins)
            {
                DisplayableRegPlugin itm = new(item)
                {
                    Logo = new BitmapImage()
                };
                await itm.Logo.SetSourceAsync(await item.Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync());
                RegPlugins.Add(itm);
            }

            (reglist.Plugins as INotifyCollectionChanged).CollectionChanged += async (sender, e) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (RebootPlugin item in e.NewItems.OfType<RegPlugin>())
                        {
                            DisplayableRegPlugin itm = new(item)
                            {
                                Logo = new BitmapImage()
                            };
                            await itm.Logo.SetSourceAsync(await item.Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync());
                            RegPlugins.Add(itm);
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (RebootPlugin item in e.OldItems.OfType<RegPlugin>())
                        {
                            foreach (DisplayableRegPlugin plugin in RegPlugins)
                            {
                                if (plugin.Plugin == item)
                                {
                                    RegPlugins.Remove(plugin);
                                    break;
                                }
                            }
                        }
                    }
                });
            };

            AppPlugin.PluginList.PluginList<string, string, double> rebootlist = await Providers.OSReboot.Definition.OSRebootProvidersWithOptions.ListAsync(Providers.OSReboot.Definition.OSRebootProvidersWithOptions.PLUGIN_NAME);

            foreach (RebootPlugin item in rebootlist.Plugins)
            {
                DisplayablePowerPlugin itm = new(item)
                {
                    Logo = new BitmapImage()
                };
                await itm.Logo.SetSourceAsync(await item.Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync());
                RebootPlugins.Add(itm);
            }

            (rebootlist.Plugins as INotifyCollectionChanged).CollectionChanged += async (sender, e) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (RebootPlugin item in e.NewItems.OfType<RebootPlugin>())
                        {
                            DisplayablePowerPlugin itm = new(item)
                            {
                                Logo = new BitmapImage()
                            };
                            await itm.Logo.SetSourceAsync(await item.Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync());
                            RebootPlugins.Add(itm);
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (RebootPlugin item in e.OldItems.OfType<RebootPlugin>())
                        {
                            foreach (DisplayablePowerPlugin plugin in RebootPlugins)
                            {
                                if (plugin.Plugin == item)
                                {
                                    RebootPlugins.Remove(plugin);
                                    break;
                                }
                            }
                        }
                    }
                });
            };

            AppPlugin.PluginList.PluginList<string, string, double> applicationlist = await Providers.Applications.Definition.ApplicationProvidersWithOptions.ListAsync(Providers.Applications.Definition.ApplicationProvidersWithOptions.PLUGIN_NAME);

            foreach (RebootPlugin item in applicationlist.Plugins)
            {
                DisplayableApplicationPlugin itm = new(item)
                {
                    Logo = new BitmapImage()
                };
                await itm.Logo.SetSourceAsync(await item.Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync());
                ApplicationPlugins.Add(itm);
            }

            (applicationlist.Plugins as INotifyCollectionChanged).CollectionChanged += async (sender, e) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (RebootPlugin item in e.NewItems.OfType<ApplicationPlugin>())
                        {
                            DisplayableApplicationPlugin itm = new(item)
                            {
                                Logo = new BitmapImage()
                            };
                            await itm.Logo.SetSourceAsync(await item.Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync());
                            ApplicationPlugins.Add(itm);
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (RebootPlugin item in e.OldItems.OfType<ApplicationPlugin>())
                        {
                            foreach (DisplayableApplicationPlugin plugin in ApplicationPlugins)
                            {
                                if (plugin.Plugin == item)
                                {
                                    ApplicationPlugins.Remove(plugin);
                                    break;
                                }
                            }
                        }
                    }
                });
            };
        }
    }
}