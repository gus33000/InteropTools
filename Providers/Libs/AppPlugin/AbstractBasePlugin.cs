using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Nito.AsyncEx;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using System.Diagnostics;

namespace AppPlugin
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class AbstractBasePlugin<TOut> : IBackgroundTask
    {

        internal AbstractBasePlugin(bool useSyncronisationContext)
        {
            this.useSyncronisationContext = useSyncronisationContext;
        }

        internal const string START_KEY = "Start";
        internal const string PROGRESS_KEY = "Progress";
        internal const string CANCEL_KEY = "Cancel";
        internal const string OPTIONS_REQUEST_KEY = "OptionRequested";
        internal const string ID_KEY = "Id";
        internal const string RESULT_KEY = "Result";
        internal const string ERROR_KEY = "Error";
        internal const string OPTION_KEY = "Option";

        private BackgroundTaskDeferral dereffal;
        private Dictionary<Guid, CancellationTokenSource> idDirectory = new Dictionary<Guid, CancellationTokenSource>();
        private readonly bool useSyncronisationContext;
        private AsyncContextThread worker;

        void IBackgroundTask.Run(IBackgroundTaskInstance taskInstance)
        {
            if (this.useSyncronisationContext)
                this.worker = new Nito.AsyncEx.AsyncContextThread();


            this.dereffal = taskInstance.GetDeferral();

            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            details.AppServiceConnection.RequestReceived += this.AppServiceConnection_RequestReceivedAsync;
            details.AppServiceConnection.ServiceClosed += this.AppServiceConnection_ServiceClosed; ;
            taskInstance.Canceled += this.TaskInstance_Canceled;

        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            this.worker?.Dispose();
            this.dereffal?.Complete();
            this.dereffal = null;
        }

        private void AppServiceConnection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            this.worker?.Dispose();
            this.dereffal?.Complete();
            this.dereffal = null;
        }

        private async void AppServiceConnection_RequestReceivedAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var messageDereffal = args.GetDeferral();
            try
            {
                // if we have a worker we use that.
                if (this.worker != null)
                    await this.worker.Factory.Run(() => RequestRecivedAsync(sender, args));
                else
                    await RequestRecivedAsync(sender, args);

            }
            finally
            {
                messageDereffal.Complete();
            }
        }

        internal virtual async Task RequestRecivedAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            if (args.Request.Message.ContainsKey(START_KEY))
                await StartMessageAsync(sender, args);
            else if (args.Request.Message.ContainsKey(CANCEL_KEY))
                CancelMessage(sender, args);
        }

        private void CancelMessage(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {

            if (!args.Request.Message.ContainsKey(CANCEL_KEY))
                return;

            if (!args.Request.Message.ContainsKey(ID_KEY))
                return;


            var id = (Guid)args.Request.Message[ID_KEY];
            var shouldCancel = (bool)args.Request.Message[CANCEL_KEY];
            if (!shouldCancel)
                return;

            if (!this.idDirectory.ContainsKey(id))
                return;

            this.idDirectory[id].Cancel();

        }


        private async Task StartMessageAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Guid? id = null;
            try
            {
                id = (Guid)args.Request.Message[ID_KEY];
                if (this.idDirectory.ContainsKey(id.Value))
                    throw new Exceptions.PluginException("Start was already send.");
                var cancellationTokenSource = new CancellationTokenSource();
                this.idDirectory.Add(id.Value, cancellationTokenSource);

                object output = await PerformStartAsync(sender, args, id, cancellationTokenSource);

                var outputString = Helper.Serilize(output);
                var valueSet = new Windows.Foundation.Collections.ValueSet();
                valueSet.Add(ID_KEY, id.Value);
                valueSet.Add(RESULT_KEY, outputString);
                await args.Request.SendResponseAsync(valueSet);

            }
            catch (Exception e)
            {
                var valueSet = new ValueSet();
                valueSet.Add(ERROR_KEY, e.Message);
                valueSet.Add(ID_KEY, id.Value);
                await args.Request.SendResponseAsync(valueSet);
            }
            finally
            {
                if (id.HasValue)
                    this.idDirectory.Remove(id.Value);
            }
        }

        internal abstract Task<TOut> PerformStartAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args, Guid? id, CancellationTokenSource cancellationTokenSource);

    }
}
