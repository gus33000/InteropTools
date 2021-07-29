using AppPlugin.PluginList;
using System;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace AppPlugin
{
    /// <summary>
    /// Abstract class that can be implemented to define a simple Plugin that provides one Function that takes an additional option argument.
    /// </summary>
    /// <typeparam name="TIn">The Type that will be passed to the funtion. (Must have a valid <seealso cref="DataContractAttribute"/> )</typeparam>
    /// <typeparam name="TOut">The return type of the function. (Must have a valid <seealso cref="DataContractAttribute"/> )</typeparam>
    /// <typeparam name="TOption">The Type that will be used to pass the options to f.</typeparam>
    /// <typeparam name="TProgress">The type that will be used to report progress. (Must have a valid <seealso cref="DataContractAttribute"/> )</typeparam>
    public abstract class AbstractPlugin<TIn, TOut, TOption, TProgress> : AbstractBasePlugin<TOut>
    {
        /// <summary>
        /// Instanziate the Plugin.
        /// </summary>
        /// <remarks>
        /// Normaly an AppService uses its own process without UI. It also does not provide a SyncronisationContext. This results that async/await calls will run in the ThreadPool. This includes the Progress report. If the Plugin spans many Tasks, progress will be reported with higher latency.
        /// </remarks>
        /// <param name="useSyncronisationContext">Discrips if the code should be called using a SyncronisationContext.</param>
        public AbstractPlugin(bool useSyncronisationContext = true) : base(useSyncronisationContext)
        {
        }

        /// <summary>
        /// Returns an Object that Lists the Availaible Plugins.
        /// </summary>
        /// <remarks>
        /// The <paramref name="pluginName"/> length must be less or equal to 39, because of a limitation of the appmanifest.
        /// </remarks>
        /// <exception cref="ArgumentException">
        /// the length of <paramref name="pluginName"/> is 40 or greater.
        /// </exception>
        /// <param name="pluginName">The Plugin name defined in the appmanifest.</param>
        /// <returns>The <see cref="PluginList<,,,>"/></returns
        public static async Task<PluginList<TIn, TOut, TOption, TProgress>> ListAsync(string pluginName)
        {
            PluginList<TIn, TOut, TOption, TProgress> pluginList = new(pluginName);
            await pluginList.InitAsync();
            return pluginList;
        }

        /// <summary>
        /// Provides the Funktionality of this Plugin.
        /// </summary>
        /// <param name="input">The Input Parameter.</param>
        /// <param name="options">The options that contains additional configuration parameter for the function.</param>
        /// <param name="progress">The Progress that will report data to the Client.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns>The result of the execution.</returns>
        protected abstract Task<TOut> ExecuteAsync(TIn input, TOption options, IProgress<TProgress> progress, CancellationToken cancelToken);

        /// <summary>
        /// Generates the Prototype Options the client can manipulate and pass back to the Plugin.
        /// </summary>
        /// <returns>The prototype options.</returns>
        protected abstract Task<TOption> GetDefaultOptionsAsync();

        internal override async Task RequestRecivedAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            if (args.Request.Message.ContainsKey(OPTIONS_REQUEST_KEY))
            {
                TOption options = await GetDefaultOptionsAsync();

                string optionString = Helper.Serilize(options);
                ValueSet valueSet = new()
                {
                    { RESULT_KEY, optionString }
                };
                await args.Request.SendResponseAsync(valueSet);
            }
            else
            {
                await base.RequestRecivedAsync(sender, args);
            }
        }

        internal override async Task<TOut> PerformStartAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args, Guid? id, CancellationTokenSource cancellationTokenSource)
        {
            string inputString = args.Request.Message[START_KEY] as string;
            string optionString = args.Request.Message[OPTION_KEY] as string;

            TIn input = Helper.DeSerilize<TIn>(inputString);
            TOption options = Helper.DeSerilize<TOption>(optionString);

            Progress<TProgress> progress = new(async r =>
            {
                string data = Helper.Serilize(r);
                ValueSet dataSet = new()
                {
                    { PROGRESS_KEY, data },
                    { ID_KEY, id }
                };
                await sender.SendMessageAsync(dataSet);
            });

            TOut output = await ExecuteAsync(input, options, progress, cancellationTokenSource.Token);
            return output;
        }
    }
}
