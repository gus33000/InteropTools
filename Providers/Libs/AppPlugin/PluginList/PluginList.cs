using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

    public class PluginList<TIn, TOut, TProgress> : AbstractPluginList<TOut, PluginList<TIn, TOut, TProgress>.PluginProvider>
    {

        internal PluginList(string pluginName) : base(pluginName)
        {

        }

        internal override PluginProvider CreatePluginProvider(AppExtension ext, string serviceName)
            => new PluginProvider(ext, serviceName);

        public new sealed class PluginProvider : AbstractPluginList<TOut, PluginProvider>.PluginProvider, IPlugin<TIn, TOut, TProgress>
        {

            internal PluginProvider(AppExtension ext, string serviceName) : base(ext, serviceName)
            {
            }

            private Task<PluginConnection> GetPluginConnection(IProgress<TProgress> progress, CancellationToken cancelTokem) => PluginConnection.CreateAsync(this.ServiceName, this.Extension, progress, cancelTokem);

            public async Task<TOut> ExecuteAsync(TIn input, IProgress<TProgress> progress = null, CancellationToken cancelTokem = default(CancellationToken))
            {
                using (var plugin = await GetPluginConnection(progress, cancelTokem))
                    return await plugin.ExecuteAsync(input);
            }

        }

        private sealed class PluginConnection : IDisposable
        {
            private readonly AppServiceConnection connection;
            private bool isDisposed;
            private readonly IProgress<TProgress> progress;
            private readonly CancellationToken cancelTokem;
            private readonly Guid id = Guid.NewGuid();

            private PluginConnection(AppServiceConnection connection, IProgress<TProgress> progress, CancellationToken cancelTokem = default(CancellationToken))
            {
                this.connection = connection;
                connection.ServiceClosed += this.Connection_ServiceClosed;
                connection.RequestReceived += this.Connection_RequestReceived;
                this.progress = progress;
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
                if (!args.Request.Message.ContainsKey(AbstractPlugin<object, object, object>.PROGRESS_KEY))
                    return;
                if (!args.Request.Message.ContainsKey(AbstractPlugin<object, object, object>.ID_KEY))
                    return;

                var id = (Guid)args.Request.Message[AbstractPlugin<object, object, object>.ID_KEY];
                if (this.id != id)
                    return;

                var progressString = args.Request.Message[AbstractPlugin<object, object, object>.PROGRESS_KEY] as string;

                var progress = Helper.DeSerilize<TProgress>(progressString);


                this.progress?.Report(progress);
                await args.Request.SendResponseAsync(new ValueSet());
            }

            private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
            {
                Dispose();
            }

            public static async Task<PluginConnection> CreateAsync(string serviceName, AppExtension appExtension, IProgress<TProgress> progress, CancellationToken cancelTokem = default(CancellationToken))
            {
                var connection = new AppServiceConnection();
                connection.AppServiceName = serviceName;
                connection.PackageFamilyName = appExtension.Package.Id.FamilyName;

                var pluginConnection = new PluginConnection(connection, progress, cancelTokem);
                var status = await connection.OpenAsync();



                //If the new connection opened successfully we're done here
                if (status == AppServiceConnectionStatus.Success)
                {
                    return pluginConnection;
                }
                else
                {
                    //Clean up before we go
                    var exception = new Exceptions.ConnectionFailureException(status, connection);
                    connection.Dispose();
                    connection = null;
                    throw exception;
                }
            }

            public void Dispose()
            {
                if (this.isDisposed)
                    return;
                this.connection.Dispose();
                this.isDisposed = true;
            }

            public async Task<TOut> ExecuteAsync(TIn input)
            {
                if (this.isDisposed)
                    throw new ObjectDisposedException(this.ToString());

                string inputString = Helper.Serilize(input);

                var inputs = new ValueSet();
                inputs.Add(AbstractPlugin<object, object, object>.START_KEY, inputString);
                inputs.Add(AbstractPlugin<object, object, object>.ID_KEY, this.id);
                var response = await this.connection.SendMessageAsync(inputs);

                if (response.Status != AppServiceResponseStatus.Success)
                    throw new Exceptions.ConnectionFailureException(response.Status);
                if (response.Message.ContainsKey(AbstractPlugin<object, object, object>.ERROR_KEY))
                    throw new Exceptions.PluginException(response.Message[AbstractPlugin<object, object, object>.ERROR_KEY] as string);
                if (!response.Message.ContainsKey(AbstractPlugin<object, object, object>.RESULT_KEY))
                    return default(TOut);
                var outputString = response.Message[AbstractPlugin<object, object, object>.RESULT_KEY] as string;

                if (String.IsNullOrWhiteSpace(outputString))
                    return default(TOut);

                var output = Helper.DeSerilize<TOut>(outputString);

                return output;

            }
        }
    }
}
