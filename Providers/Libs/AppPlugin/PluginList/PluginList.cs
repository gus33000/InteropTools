using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppExtensions;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace AppPlugin.PluginList
{
    public class PluginList<TIn, TOut, TProgress> : AbstractPluginList<TOut, PluginList<TIn, TOut, TProgress>.PluginProvider>
    {
        internal PluginList(string pluginName) : base(pluginName)
        {
        }

        internal override PluginProvider CreatePluginProvider(AppExtension ext, string serviceName)
        {
            return new PluginProvider(ext, serviceName);
        }

        public new sealed class PluginProvider : AbstractPluginList<TOut, PluginProvider>.PluginProvider, IPlugin<TIn, TOut, TProgress>
        {
            internal PluginProvider(AppExtension ext, string serviceName) : base(ext, serviceName)
            {
            }

            private Task<PluginConnection> GetPluginConnection(IProgress<TProgress> progress, CancellationToken cancelTokem)
            {
                return PluginConnection.CreateAsync(ServiceName, Extension, progress, cancelTokem);
            }

            public async Task<TOut> ExecuteAsync(TIn input, IProgress<TProgress> progress = null, CancellationToken cancelTokem = default)
            {
                using (PluginConnection plugin = await GetPluginConnection(progress, cancelTokem))
                {
                    return await plugin.ExecuteAsync(input);
                }
            }
        }

        private sealed class PluginConnection : IDisposable
        {
            private readonly AppServiceConnection connection;
            private bool isDisposed;
            private readonly IProgress<TProgress> progress;
            private readonly CancellationToken cancelTokem;
            private readonly Guid id = Guid.NewGuid();

            private PluginConnection(AppServiceConnection connection, IProgress<TProgress> progress, CancellationToken cancelTokem = default)
            {
                this.connection = connection;
                connection.ServiceClosed += Connection_ServiceClosed;
                connection.RequestReceived += Connection_RequestReceived;
                this.progress = progress;
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
                if (!args.Request.Message.ContainsKey(AbstractPlugin<object, object, object>.PROGRESS_KEY))
                {
                    return;
                }

                if (!args.Request.Message.ContainsKey(AbstractPlugin<object, object, object>.ID_KEY))
                {
                    return;
                }

                Guid id = (Guid)args.Request.Message[AbstractPlugin<object, object, object>.ID_KEY];
                if (this.id != id)
                {
                    return;
                }

                string progressString = args.Request.Message[AbstractPlugin<object, object, object>.PROGRESS_KEY] as string;

                TProgress progress = Helper.DeSerilize<TProgress>(progressString);

                this.progress?.Report(progress);
                await args.Request.SendResponseAsync(new ValueSet());
            }

            private void Connection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
            {
                Dispose();
            }

            public static async Task<PluginConnection> CreateAsync(string serviceName, AppExtension appExtension, IProgress<TProgress> progress, CancellationToken cancelTokem = default)
            {
                AppServiceConnection connection = new()
                {
                    AppServiceName = serviceName,
                    PackageFamilyName = appExtension.Package.Id.FamilyName
                };

                PluginConnection pluginConnection = new(connection, progress, cancelTokem);
                AppServiceConnectionStatus status = await connection.OpenAsync();

                //If the new connection opened successfully we're done here
                if (status == AppServiceConnectionStatus.Success)
                {
                    return pluginConnection;
                }
                else
                {
                    //Clean up before we go
                    Exceptions.ConnectionFailureException exception = new(status, connection);
                    connection.Dispose();
                    throw exception;
                }
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

            public async Task<TOut> ExecuteAsync(TIn input)
            {
                if (isDisposed)
                {
                    throw new ObjectDisposedException(ToString());
                }

                string inputString = Helper.Serilize(input);

                ValueSet inputs = new()
                {
                    { AbstractPlugin<object, object, object>.START_KEY, inputString },
                    { AbstractPlugin<object, object, object>.ID_KEY, id }
                };
                AppServiceResponse response = await connection.SendMessageAsync(inputs);

                if (response.Status != AppServiceResponseStatus.Success)
                {
                    throw new Exceptions.ConnectionFailureException(response.Status);
                }

                if (response.Message.ContainsKey(AbstractPlugin<object, object, object>.ERROR_KEY))
                {
                    throw new Exceptions.PluginException(response.Message[AbstractPlugin<object, object, object>.ERROR_KEY] as string);
                }

                if (!response.Message.ContainsKey(AbstractPlugin<object, object, object>.RESULT_KEY))
                {
                    return default;
                }

                string outputString = response.Message[AbstractPlugin<object, object, object>.RESULT_KEY] as string;

                if (string.IsNullOrWhiteSpace(outputString))
                {
                    return default;
                }

                TOut output = Helper.DeSerilize<TOut>(outputString);

                return output;
            }
        }
    }
}
