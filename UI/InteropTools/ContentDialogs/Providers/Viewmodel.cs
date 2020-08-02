using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using RegPlugin = AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider; //, InteropTools.Providers.Registry.Definition.TransfareOptions
using RebootPlugin = AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider; //, InteropTools.Providers.OSReboot.Definition.TransfareOptions
using Windows.System.Threading;
using Windows.UI.Xaml.Media.Imaging;

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
            await ThreadPool.RunAsync(x => { function(); });
        }

        private async Task InitAsync()
        {
            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var item in reglist.Plugins)
            {
                var itm = new DisplayablePlugin(item);
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
                            var itm = new DisplayablePlugin(item);
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
                this.RebootPlugins.Add(item);

            (rebootlist.Plugins as INotifyCollectionChanged).CollectionChanged += async (sender, e) =>
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (e.NewItems != null)
                        foreach (var item in e.NewItems.OfType<RebootPlugin>())
                            this.RebootPlugins.Add(item);
                    if (e.OldItems != null)
                        foreach (var item in e.OldItems.OfType<RebootPlugin>())
                            this.RebootPlugins.Remove(item);
                });
            };
        }
    }
}
