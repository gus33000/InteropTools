using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppExtensions;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace AppPlugin.PluginList
{

    public class PluginList<TIn, TOut, TOption, TProgress> : AbstractPluginList<TOut, PluginList<TIn, TOut, TOption, TProgress>.PluginProvider>
    {

        internal PluginList(string pluginName) : base(pluginName)
        {

        }

        public new sealed class PluginProvider : AbstractPluginList<TOut, PluginProvider>.PluginProvider, IPlugin<TIn, TOut, TOption, TProgress>
        {
            public Task<TOption> PrototypeOptions { get; }

            internal PluginProvider(AppExtension ext, string serviceName) : base(ext, serviceName)
            {
                PrototypeOptions = GetPlugin(null, default).ContinueWith(x => x.Result.RequestOptionsAsync()).Unwrap();
            }


            private Task<PluginConnection> GetPlugin(IProgress<TProgress> progress, CancellationToken cancelTokem)
            {
                return PluginConnection.CreateAsync(ServiceName, Extension, progress, cancelTokem);
            }

            public async Task<TOut> ExecuteAsync(TIn input, TOption options, IProgress<TProgress> progress = null, CancellationToken cancelTokem = default)
            {
                using (PluginConnection plugin = await GetPlugin(progress, cancelTokem))
                {
                    return await plugin.ExecuteAsync(input, options);
                }
            }
        }

        internal override PluginProvider CreatePluginProvider(AppExtension ext, string serviceName)
        {
            return new PluginProvider(ext, serviceName);
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
                ValueSet valueSet = new ValueSet
                {
                    { AbstractPlugin<object, object, object>.ID_KEY, id },
                    { AbstractPlugin<object, object, object>.CANCEL_KEY, true }
                };

                await connection.SendMessageAsync(valueSet);
            }

            public async Task<TOption> RequestOptionsAsync()
            {
                if (isDisposed)
                {
                    throw new ObjectDisposedException(ToString());
                }

                ValueSet inputs = new ValueSet
                {
                    { AbstractPlugin<object, object, object>.OPTIONS_REQUEST_KEY, true }
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

                string resultString = response.Message[AbstractPlugin<object, object, object>.RESULT_KEY] as string;

                if (string.IsNullOrWhiteSpace(resultString))
                {
                    return default;
                }

                TOption output = Helper.DeSerilize<TOption>(resultString);

                return output;
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
                AppServiceConnection connection = new AppServiceConnection();

                PluginConnection pluginConnection = new PluginConnection(connection, progress, cancelTokem);
                connection.AppServiceName = serviceName;

                connection.PackageFamilyName = appExtension.Package.Id.FamilyName;

                AppServiceConnectionStatus status = await connection.OpenAsync();

                //If the new connection opened successfully we're done here
                if (status == AppServiceConnectionStatus.Success)
                {
                    return pluginConnection;
                }
                else
                {
                    //Clean up before we go
                    Exceptions.ConnectionFailureException exception = new Exceptions.ConnectionFailureException(status, connection);
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

            public async Task<TOut> ExecuteAsync(TIn input, TOption option)
            {
                if (isDisposed)
                {
                    throw new ObjectDisposedException(ToString());
                }

                string inputString = Helper.Serilize(input);
                string optionString = Helper.Serilize(option);

                ValueSet inputs = new ValueSet
                {
                    { AbstractPlugin<object, object, object>.START_KEY, inputString },
                    { AbstractPlugin<object, object, object>.OPTION_KEY, optionString },
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
