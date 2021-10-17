using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppExtensions;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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

        private readonly ObservableCollection<TPluginProvider> plugins = new ObservableCollection<TPluginProvider>();
        public ReadOnlyObservableCollection<TPluginProvider> Plugins { get; }

        internal AbstractPluginList(string pluginName)
        {
            if (pluginName == null)
                throw new ArgumentNullException(nameof(pluginName));
            this.pluginName = pluginName;
            if (pluginName.Length > 39)
                throw new ArgumentException($"The Plugin name is longer than 39. (was {pluginName.Length})");
            this.workerThread = new Nito.AsyncEx.AsyncContextThread();
            this.Plugins = new ReadOnlyObservableCollection<TPluginProvider>(this.plugins);
        }

        public async Task InitAsync()
        {
            this.catalog = AppExtensionCatalog.Open(this.pluginName);

            // set up extension management events
            this.catalog.PackageInstalled += this.Catalog_PackageInstalled;
            this.catalog.PackageUpdated += this.Catalog_PackageUpdated;
            this.catalog.PackageUninstalling += this.Catalog_PackageUninstalling;
            this.catalog.PackageUpdating += this.Catalog_PackageUpdating;
            this.catalog.PackageStatusChanged += this.Catalog_PackageStatusChanged;



            // Scan all extensions

            await FindAllExtensionsAsync();
        }



        private async Task FindAllExtensionsAsync()
        {
            // load all the extensions currently installed
            var extensions = await this.catalog.FindAllAsync();

            await this.workerThread.Factory.StartNew(async () =>
            {
                foreach (var ext in extensions)
                    await LoadExtensionAsync(ext);
            }).Unwrap();
        }


        private async void Catalog_PackageInstalled(AppExtensionCatalog sender, AppExtensionPackageInstalledEventArgs args)
        {
            await this.workerThread.Factory.StartNew(async () =>
            {
                foreach (var ext in args.Extensions)
                    await LoadExtensionAsync(ext);
            }).Unwrap();
        }


        // package has been updated, so reload the extensions

        private async void Catalog_PackageUpdated(AppExtensionCatalog sender, AppExtensionPackageUpdatedEventArgs args)
        {
            await this.workerThread.Factory.StartNew(async () =>
            {
                foreach (var ext in args.Extensions)
                    await LoadExtensionAsync(ext);
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
            if (!(args.Package.Status.VerifyIsOK()))
            {
                // if it's offline unload only
                if (args.Package.Status.PackageOffline)
                    await UnloadExtensionsAsync(args.Package);
                // package is being serviced or deployed
                else if (args.Package.Status.Servicing || args.Package.Status.DeploymentInProgress)
                {                    // ignore these package status events                
                }
                // package is tampered or invalid or some other issue
                // glyphing the extensions would be a good user experience
                else
                    await RemoveExtensionsAsync(args.Package);

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
            if (!ext.Package.Status.VerifyIsOK()
                || ext.Package.SignatureKind != PackageSignatureKind.None)
            {
                // if this package doesn't meet our requirements
                // ignore it and return
                return;
            }

            // if its already existing then this is an update
            var existingExt = this.plugins.Where(e => e.UniqueId == identifier).FirstOrDefault();

            // new extension
            if (existingExt == null)
            {
                // get extension properties
                var properties = await ext.GetExtensionPropertiesAsync() as PropertySet;

                var servicesProperty = properties[SERVICE_KEY] as PropertySet;
                var serviceName = servicesProperty["#text"].ToString();


                try
                {
                    // create new extension
                    var nExt = CreatePluginProvider(ext, serviceName);

                    // Add it to extension list
                    this.plugins.Add(nExt);
                    nExt.IsEnabled = true;
                }
                catch (Exception e)
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
            await this.workerThread.Factory.StartNew(() =>
            {
                foreach (var item in this.plugins.Where(ext => ext.Extension.Package.Id.FamilyName == package.Id.FamilyName))
                    item.IsEnabled = true;
            });
        }

        // unloads all extensions associated with a package - used for updating and when package status goes away
        private async Task UnloadExtensionsAsync(Package package)
        {
            await this.workerThread.Factory.StartNew(() =>
            {
                foreach (var item in this.plugins.Where(ext => ext.Extension.Package.Id.FamilyName == package.Id.FamilyName))
                    item.IsEnabled = false;
            });
        }

        // removes all extensions associated with a package - used when removing a package or it becomes invalid
        private async Task RemoveExtensionsAsync(Package package)
        {
            await this.workerThread.Factory.StartNew(() =>
            {
                foreach (var item in this.plugins.Where(ext => ext.Extension.Package.Id.FamilyName == package.Id.FamilyName).ToArray())
                {
                    item.IsEnabled = false;
                    this.plugins.Remove(item);
                }
            });
        }


        public abstract class PluginProvider
        {
            public AppExtension Extension { get; private set; }
            public Task<BitmapImage> Logo
            {
                get
                {
                    // get logo 
                    return this.Extension.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1)).OpenReadAsync()
                        .AsTask()
                        .ContinueWith(filestreamTask =>
                        {
                            var logo = new BitmapImage();
                            logo.SetSource(filestreamTask.Result);
                            return logo;
                        });
                }
            }
            protected string ServiceName { get; private set; }

            internal PluginProvider(AppExtension ext, string serviceName)
            {
                this.Extension = ext;
                this.ServiceName = serviceName;
            }


            public string UniqueId => this.Extension.AppInfo.AppUserModelId + "!" + this.Extension.Id;

            public bool IsEnabled { get; internal set; }



            internal async Task UpdateAsync(AppExtension ext)
            {
                // ensure this is the same uid
                string identifier = ext.AppInfo.AppUserModelId + "!" + ext.Id;
                if (identifier != this.UniqueId)
                {
                    return;
                }

                // get extension properties
                var properties = await ext.GetExtensionPropertiesAsync() as PropertySet;

                // get logo 
                var filestream = await (ext.AppInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(1, 1))).OpenReadAsync();
                var logo = new BitmapImage();
                logo.SetSource(filestream);

                // update the extension
                this.Extension = ext;


                #region Update Properties
                // update app service information
                this.ServiceName = null;
                if (properties != null)
                {
                    if (properties.ContainsKey("Service"))
                    {
                        var serviceProperty = properties["Service"] as PropertySet;
                        this.ServiceName = serviceProperty["#text"].ToString();
                    }
                }

                if (this.ServiceName == null)
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

            private PluginConnection(AppServiceConnection connection, CancellationToken cancelTokem = default(CancellationToken))
            {
                this.connection = connection;
                connection.ServiceClosed += this.Connection_ServiceClosed;
                connection.RequestReceived += this.Connection_RequestReceived;

                this.cancelTokem = cancelTokem;
                cancelTokem.Register(this.Canceld);
            }

            private async void Canceld()
            {
                var valueSet = new ValueSet();

                valueSet.Add(AbstractPlugin<object, object, object>.ID_KEY, this.id);
                valueSet.Add(AbstractPlugin<object, object, object>.CANCEL_KEY, true);

                await this.connection.SendMessageAsync(valueSet);
            }

            private async void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
            {
                if (!args.Request.Message.ContainsKey(AbstractPlugin<object, object, object>.ID_KEY))
                    return;

                var id = (Guid)args.Request.Message[AbstractPlugin<object, object, object>.ID_KEY];
                if (this.id != id)
                    return;
                var valueSet = await RequestRecived(sender, args);
                await args.Request.SendResponseAsync(new ValueSet());
            }

            protected abstract Task<ValueSet> RequestRecived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args);

            private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
            {
                Dispose();
            }

            public void Dispose()
            {
                if (this.isDisposed)
                    return;
                this.connection.Dispose();
                this.isDisposed = true;
            }

        }

        #region IDisposable Support
        private bool disposedValue = false; // Dient zur Erkennung redundanter Aufrufe.

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.workerThread.Dispose();
                }


                this.disposedValue = true;
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
