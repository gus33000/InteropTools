using AppPlugin.PluginList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace AppPlugin
{
    /// <summary>
    /// Abstract class that can be implemented to define a simple Plugin that provides one Function.
    /// </summary>
    /// <typeparam name="TIn">The Type that will be passed to the funtion. (Must have a valid <seealso cref="DataContractAttribute"/> )</typeparam>
    /// <typeparam name="TOut">The return type of the function. (Must have a valid <seealso cref="DataContractAttribute"/> )</typeparam>
    /// <typeparam name="TProgress">The type that will be used to report progress. (Must have a valid <seealso cref="DataContractAttribute"/> )</typeparam>
    public abstract class AbstractPlugin<TIn, TOut, TProgress> : AbstractBasePlugin<TOut>
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
        /// <returns>The <see cref="AppPlugin.PluginList<,,,>"/></returns>
        public static async Task<PluginList<TIn, TOut, TProgress>> ListAsync(string pluginName)
        {
            var pluginList = new PluginList<TIn, TOut, TProgress>(pluginName);
            await pluginList.InitAsync();
            return pluginList;
        }

        /// <summary>
        /// Provides the Funktionality of this Plugin.
        /// </summary>
        /// <param name="input">The Input Parameter.</param>
        /// <param name="progress">The Progress that will report data to the Client.</param>
        /// <param name="cancelToken">The cancel token.</param>
        /// <returns>The result of the execution.</returns>
        protected abstract Task<TOut> Execute(AppServiceConnection sender, TIn input, IProgress<TProgress> progress, CancellationToken cancelToken);


        internal override async Task<TOut> PerformStartAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args, Guid? id, CancellationTokenSource cancellationTokenSource)
        {
            var inputString = args.Request.Message[START_KEY] as String;

            var input = Helper.DeSerilize<TIn>(inputString);

            var progress = new Progress<TProgress>(async r =>
            {
                var data = Helper.Serilize(r);
                var dataSet = new ValueSet();
                dataSet.Add(PROGRESS_KEY, data);
                dataSet.Add(ID_KEY, id);
                await sender.SendMessageAsync(dataSet);
            });

            var output = await Execute(sender, input, progress, cancellationTokenSource.Token);
            return output;
        }



    }
}
