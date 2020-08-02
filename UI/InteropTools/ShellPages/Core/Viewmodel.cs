using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using RegPlugin = AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider; //, InteropTools.Providers.Registry.Definition.TransfareOptions
using RebootPlugin = AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider; //, InteropTools.Providers.OSReboot.Definition.TransfareOptions
using ApplicationPlugin = AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider; //, InteropTools.Providers.Applications.Definition.TransfareOptions
using Windows.UI.Xaml.Media.Imaging;
using Windows.System.Threading;

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
            await ThreadPool.RunAsync(x => { function(); });
        }

        private async Task InitAsync()
        {
            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var item in reglist.Plugins)
            {
                var itm = new DisplayableRegPlugin(item);
                itm.Logo = new BitmapImage();
                await itm.Logo.SetSourceAsync(await item.Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync());
                this.RegPlugins.Add(itm);
            }

            (reglist.Plugins as INotifyCollectionChanged).CollectionChanged += async (sender, e) =>
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems.OfType<RegPlugin>())
                        {
                            var itm = new DisplayableRegPlugin(item);
                            itm.Logo = new BitmapImage();
                            await itm.Logo.SetSourceAsync(await item.Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync());
                            this.RegPlugins.Add(itm);
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems.OfType<RegPlugin>())
                        {
                            foreach (var plugin in this.RegPlugins)
                                if (plugin.Plugin == item)
                                {
                                    this.RegPlugins.Remove(plugin);
                                    break;
                                }
                        }
                    }
                });
            };

            var rebootlist = await InteropTools.Providers.OSReboot.Definition.OSRebootProvidersWithOptions.ListAsync(InteropTools.Providers.OSReboot.Definition.OSRebootProvidersWithOptions.PLUGIN_NAME);

            foreach (var item in rebootlist.Plugins)
            {
                var itm = new DisplayablePowerPlugin(item);
                itm.Logo = new BitmapImage();
                await itm.Logo.SetSourceAsync(await item.Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync());
                this.RebootPlugins.Add(itm);
            }

            (rebootlist.Plugins as INotifyCollectionChanged).CollectionChanged += async (sender, e) =>
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems.OfType<RebootPlugin>())
                        {
                            var itm = new DisplayablePowerPlugin(item);
                            itm.Logo = new BitmapImage();
                            await itm.Logo.SetSourceAsync(await item.Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync());
                            this.RebootPlugins.Add(itm);
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems.OfType<RebootPlugin>())
                        {
                            foreach (var plugin in this.RebootPlugins)
                                if (plugin.Plugin == item)
                                {
                                    this.RebootPlugins.Remove(plugin);
                                    break;
                                }
                        }
                    }
                });
            };

            var applicationlist = await InteropTools.Providers.Applications.Definition.ApplicationProvidersWithOptions.ListAsync(InteropTools.Providers.Applications.Definition.ApplicationProvidersWithOptions.PLUGIN_NAME);

            foreach (var item in applicationlist.Plugins)
            {
                var itm = new DisplayableApplicationPlugin(item);
                itm.Logo = new BitmapImage();
                await itm.Logo.SetSourceAsync(await item.Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync());
                this.ApplicationPlugins.Add(itm);
            }

            (applicationlist.Plugins as INotifyCollectionChanged).CollectionChanged += async (sender, e) =>
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (var item in e.NewItems.OfType<ApplicationPlugin>())
                        {
                            var itm = new DisplayableApplicationPlugin(item);
                            itm.Logo = new BitmapImage();
                            await itm.Logo.SetSourceAsync(await item.Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync());
                            this.ApplicationPlugins.Add(itm);
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (var item in e.OldItems.OfType<ApplicationPlugin>())
                        {
                            foreach (var plugin in this.ApplicationPlugins)
                                if (plugin.Plugin == item)
                                {
                                    this.ApplicationPlugins.Remove(plugin);
                                    break;
                                }
                        }
                    }
                });
            };
        }
    }
}
