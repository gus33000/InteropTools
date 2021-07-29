using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppExtensions;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Media.Imaging;

namespace AppPlugin.PluginList
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class AbstractPluginList<TOut, TPluginProvider> : IDisposable
        where TPluginProvider : AbstractPluginList<TOut, TPluginProvider>.PluginProvider
    {
        private readonly Nito.AsyncEx.AsyncContextThread workerThread;
        private AppExtensionCatalog catalog;
        private const string SERVICE_KEY = "Service";
        private readonly string pluginName;

        private readonly ObservableCollection<TPluginProvider> plugins = new();
        public ReadOnlyObservableCollection<TPluginProvider> Plugins { get; }

        internal AbstractPluginList(string pluginName)
        {
            this.pluginName = pluginName ?? throw new ArgumentNullException(nameof(pluginName));
            if (pluginName.Length > 39)
            {
                throw new ArgumentException($"The Plugin name is longer than 39. (was {pluginName.Length})");
            }

            workerThread = new Nito.AsyncEx.AsyncContextThread();
            Plugins = new ReadOnlyObservableCollection<TPluginProvider>(plugins);
        }

        public async Task InitAsync()
        {
            catalog = AppExtensionCatalog.Open(pluginName);

            // set up extension management events
            catalog.PackageInstalled += Catalog_PackageInstalled;
            catalog.PackageUpdated += Catalog_PackageUpdated;
            catalog.PackageUninstalling += Catalog_PackageUninstalling;
            catalog.PackageUpdating += Catalog_PackageUpdating;
            catalog.PackageStatusChanged += Catalog_PackageStatusChanged;

            // Scan all extensions

            await FindAllExtensionsAsync();
        }

        private async Task FindAllExtensionsAsync()
        {
            // load all the extensions currently installed
            IReadOnlyList<AppExtension> extensions = await catalog.FindAllAsync();

            await workerThread.Factory.StartNew(async () =>
            {
                foreach (AppExtension ext in extensions)
                {
                    await LoadExtensionAsync(ext);
                }
            }).Unwrap();
        }

        private async void Catalog_PackageInstalled(AppExtensionCatalog sender, AppExtensionPackageInstalledEventArgs args)
        {
            await workerThread.Factory.StartNew(async () =>
            {
                foreach (AppExtension ext in args.Extensions)
                {
                    await LoadExtensionAsync(ext);
                }
            }).Unwrap();
        }

        // package has been updated, so reload the extensions

        private async void Catalog_PackageUpdated(AppExtensionCatalog sender, AppExtensionPackageUpdatedEventArgs args)
        {
            await workerThread.Factory.StartNew(async () =>
            {
                foreach (AppExtension ext in args.Extensions)
                {
                    await LoadExtensionAsync(ext);
                }
            }).Unwrap();
        }

        // package is updating, so just unload the extensions

        private async void Catalog_PackageUpdating(AppExtensionCatalog sender, AppExtensionPackageUpdatingEventArgs args)
        {
            await UnloadExtensionsAsync(args.Package);
        }

        // package is removed, so unload all the extensions in the package and remove it

        private async void Catalog_PackageUninstalling(AppExtensionCatalog sender, AppExtensionPackageUninstallingEventArgs args)
        {
            await RemoveExtensionsAsync(args.Package);
        }

        // package status has changed, could be invalid, licensing issue, app was on USB and removed, etc
        private async void Catalog_PackageStatusChanged(AppExtensionCatalog sender, AppExtensionPackageStatusChangedEventArgs args)
        {
            // get package status
            if (!args.Package.Status.VerifyIsOK())
            {
                // if it's offline unload only
                if (args.Package.Status.PackageOffline)
                {
                    await UnloadExtensionsAsync(args.Package);
                }
                // package is being serviced or deployed
                else if (args.Package.Status.Servicing || args.Package.Status.DeploymentInProgress)
                {                    // ignore these package status events                
                }
                // package is tampered or invalid or some other issue
                // glyphing the extensions would be a good user experience
                else
                {
                    await RemoveExtensionsAsync(args.Package);
                }
            }
            // if package is now OK, attempt to load the extensions
            else
            {
                // try to load any extensions associated with this package
                await LoadExtensionsAsync(args.Package);
            }
        }

        // loads an extension
        private async Task LoadExtensionAsync(AppExtension ext)
        {
            // get unique identifier for this extension
            string identifier = ext.AppInfo.AppUserModelId + "!" + ext.Id;

            // load the extension if the package is OK
            /*if (!ext.Package.Status.VerifyIsOK()
                || ext.Package.SignatureKind != PackageSignatureKind.None)
            {
                // if this package doesn't meet our requirements
                // ignore it and return
                return;
            }*/

            // if its already existing then this is an update
            TPluginProvider existingExt = plugins.FirstOrDefault(e => e.UniqueId == identifier);

            // new extension
            if (existingExt == null)
            {
                // get extension properties
                PropertySet properties = await ext.GetExtensionPropertiesAsync() as PropertySet;

                PropertySet servicesProperty = properties[SERVICE_KEY] as PropertySet;
                string serviceName = servicesProperty["#text"].ToString();

                try
                {
                    // create new extension
                    TPluginProvider nExt = CreatePluginProvider(ext, serviceName);

                    // Add it to extension list
                    plugins.Add(nExt);
                    nExt.IsEnabled = true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            // update
            else
            {
                // update the extension
                await existingExt.UpdateAsync(ext);
            }
        }

        internal abstract TPluginProvider CreatePluginProvider(AppExtension ext, string serviceName);

        // loads all extensions associated with a package - used for when package status comes back
        private async Task LoadExtensionsAsync(Package package)
        {
            await workerThread.Factory.StartNew(() =>
            {
                foreach (TPluginProvider item in plugins.Where(ext => ext.Extension.Package.Id.FamilyName == package.Id.FamilyName))
                {
                    item.IsEnabled = true;
                }
            });
        }

        // unloads all extensions associated with a package - used for updating and when package status goes away
        private async Task UnloadExtensionsAsync(Package package)
        {
            await workerThread.Factory.StartNew(() =>
            {
                foreach (TPluginProvider item in plugins.Where(ext => ext.Extension.Package.Id.FamilyName == package.Id.FamilyName))
                {
                    item.IsEnabled = false;
                }
            });
        }

        // removes all extensions associated with a package - used when removing a package or it becomes invalid
        private async Task RemoveExtensionsAsync(Package package)
        {
            await workerThread.Factory.StartNew(() =>
            {
                foreach (TPluginProvider item in plugins.Where(ext => ext.Extension.Package.Id.FamilyName == package.Id.FamilyName).ToArray())
                {
                    item.IsEnabled = false;
                    plugins.Remove(item);
                }
            });
        }

        public abstract class PluginProvider
        {
            public AppExtension Extension { get; private set; }
            public Task<BitmapImage> Logo =>
                    // get logo 
                    Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync()
                        .AsTask()
                        .ContinueWith(filestreamTask =>
                        {
                            BitmapImage logo = new();
                            logo.SetSource(filestreamTask.Result);
                            return logo;
                        });
            protected string ServiceName { get; private set; }

            internal PluginProvider(AppExtension ext, string serviceName)
            {
                Extension = ext;
                ServiceName = serviceName;
            }

            public string UniqueId => Extension.AppInfo.AppUserModelId + "!" + Extension.Id;

            public bool IsEnabled { get; internal set; }

            internal async Task UpdateAsync(AppExtension ext)
            {
                // ensure this is the same uid
                string identifier = ext.AppInfo.AppUserModelId + "!" + ext.Id;
                if (identifier != UniqueId)
                {
                    return;
                }

                // get extension properties

                // get logo 
                Windows.Storage.Streams.IRandomAccessStreamWithContentType filestream = await ext.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync();
                BitmapImage logo = new();
                logo.SetSource(filestream);

                // update the extension
                Extension = ext;

                #region Update Properties
                // update app service information
                ServiceName = null;
                if (await ext.GetExtensionPropertiesAsync() is PropertySet properties)
                {
                    if (properties.ContainsKey("Service"))
                    {
                        PropertySet serviceProperty = properties["Service"] as PropertySet;
                        ServiceName = serviceProperty["#text"].ToString();
                    }
                }

                if (ServiceName == null)
                {
                    throw new Exception();
                }
                #endregion
            }
        }

        private abstract class PluginConnection : IDisposable
        {
            private readonly AppServiceConnection connection;
            private bool isDisposed;
            private readonly CancellationToken cancelTokem;
            private readonly Guid id = Guid.NewGuid();

            private PluginConnection(AppServiceConnection connection, CancellationToken cancelTokem = default)
            {
                this.connection = connection;
                connection.ServiceClosed += Connection_ServiceClosed;
                connection.RequestReceived += Connection_RequestReceived;

                this.cancelTokem = cancelTokem;
                cancelTokem.Register(Canceld);
            }

            private async void Canceld()
            {
                ValueSet valueSet = new()
                {
                    { AbstractPlugin<object, object, object>.ID_KEY, id },
                    { AbstractPlugin<object, object, object>.CANCEL_KEY, true }
                };

                await connection.SendMessageAsync(valueSet);
            }

            private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
            {
                if (!args.Request.Message.ContainsKey(AbstractPlugin<object, object, object>.ID_KEY))
                {
                    return;
                }

                Guid id = (Guid)args.Request.Message[AbstractPlugin<object, object, object>.ID_KEY];
                if (this.id != id)
                {
                    return;
                }

                _ = await RequestRecived(sender, args);
                await args.Request.SendResponseAsync(new ValueSet());
            }

            protected abstract Task<ValueSet> RequestRecived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args);

            private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
            {
                Dispose();
            }

            public void Dispose()
            {
                if (isDisposed)
                {
                    return;
                }

                connection.Dispose();
                isDisposed = true;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    workerThread.Dispose();
                }

                disposedValue = true;
            }
        }

        // Dieser Code wird hinzugefügt, um das Dispose-Muster richtig zu implementieren.
        public void Dispose()
        {
            // Ändern Sie diesen Code nicht. Fügen Sie Bereinigungscode in Dispose(bool disposing) weiter oben ein.
            Dispose(true);
        }
        #endregion
    }
}
