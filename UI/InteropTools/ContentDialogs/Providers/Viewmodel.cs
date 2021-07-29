using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using RebootPlugin = AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider; //, InteropTools.Providers.OSReboot.Definition.TransfareOptions
using RegPlugin = AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider; //, InteropTools.Providers.Registry.Definition.TransfareOptions

namespace InteropTools.ContentDialogs.Providers
{
    public class Viewmodel : DependencyObject
    {
        public class DisplayablePlugin
        {
            public BitmapImage Logo { get; set; }
            public RegPlugin Plugin { get; set; }

            public DisplayablePlugin(RegPlugin Plugin)
            {
                this.Plugin = Plugin;
            }
        }

        public ObservableCollection<DisplayablePlugin> RegPlugins { get; }
                  = new ObservableCollection<DisplayablePlugin>();

        public ObservableCollection<RebootPlugin> RebootPlugins { get; }
                  = new ObservableCollection<RebootPlugin>();

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
            AppPlugin.PluginList.PluginList<string, string, double> reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (RebootPlugin item in reglist.Plugins)
            {
                DisplayablePlugin itm = new(item)
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
                            DisplayablePlugin itm = new(item)
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
                            foreach (DisplayablePlugin plugin in RegPlugins)
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

            AppPlugin.PluginList.PluginList<string, string, double> rebootlist = await InteropTools.Providers.OSReboot.Definition.OSRebootProvidersWithOptions.ListAsync(InteropTools.Providers.OSReboot.Definition.OSRebootProvidersWithOptions.PLUGIN_NAME);

            foreach (RebootPlugin item in rebootlist.Plugins)
            {
                RebootPlugins.Add(item);
            } (rebootlist.Plugins as INotifyCollectionChanged).CollectionChanged += async (sender, e) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (e.NewItems != null)
                    {
                        foreach (RebootPlugin item in e.NewItems.OfType<RebootPlugin>())
                        {
                            RebootPlugins.Add(item);
                        }
                    }

                    if (e.OldItems != null)
                    {
                        foreach (RebootPlugin item in e.OldItems.OfType<RebootPlugin>())
                        {
                            RebootPlugins.Remove(item);
                        }
                    }
                });
            };
        }
    }
}
