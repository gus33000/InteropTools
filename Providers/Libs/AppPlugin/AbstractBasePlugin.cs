using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

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
        private readonly Dictionary<Guid, CancellationTokenSource> idDirectory = new();
        private readonly bool useSyncronisationContext;
        private AsyncContextThread worker;

        void IBackgroundTask.Run(IBackgroundTaskInstance taskInstance)
        {
            if (useSyncronisationContext)
            {
                worker = new AsyncContextThread();
            }

            dereffal = taskInstance.GetDeferral();

            AppServiceTriggerDetails details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            details.AppServiceConnection.RequestReceived += AppServiceConnection_RequestReceivedAsync;
            details.AppServiceConnection.ServiceClosed += AppServiceConnection_ServiceClosed; ;
            taskInstance.Canceled += TaskInstance_Canceled;
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            worker?.Dispose();
            dereffal?.Complete();
            dereffal = null;
        }

        private void AppServiceConnection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            worker?.Dispose();
            dereffal?.Complete();
            dereffal = null;
        }

        private async void AppServiceConnection_RequestReceivedAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            AppServiceDeferral messageDereffal = args.GetDeferral();
            try
            {
                // if we have a worker we use that.
                if (worker != null)
                {
                    await worker.Factory.Run(() => RequestRecivedAsync(sender, args));
                }
                else
                {
                    await RequestRecivedAsync(sender, args);
                }
            }
            finally
            {
                messageDereffal.Complete();
            }
        }

        internal virtual async Task RequestRecivedAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            if (args.Request.Message.ContainsKey(START_KEY))
            {
                await StartMessageAsync(sender, args);
            }
            else if (args.Request.Message.ContainsKey(CANCEL_KEY))
            {
                CancelMessage(sender, args);
            }
        }

        private void CancelMessage(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            if (!args.Request.Message.ContainsKey(CANCEL_KEY))
            {
                return;
            }

            if (!args.Request.Message.ContainsKey(ID_KEY))
            {
                return;
            }

            Guid id = (Guid)args.Request.Message[ID_KEY];
            bool shouldCancel = (bool)args.Request.Message[CANCEL_KEY];
            if (!shouldCancel)
            {
                return;
            }

            if (!idDirectory.ContainsKey(id))
            {
                return;
            }

            idDirectory[id].Cancel();
        }

        private async Task StartMessageAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Guid? id = null;
            try
            {
                id = (Guid)args.Request.Message[ID_KEY];
                if (idDirectory.ContainsKey(id.Value))
                {
                    throw new Exceptions.PluginException("Start was already send.");
                }

                CancellationTokenSource cancellationTokenSource = new();
                idDirectory.Add(id.Value, cancellationTokenSource);

                object output = await PerformStartAsync(sender, args, id, cancellationTokenSource);

                string outputString = Helper.Serilize(output);
                ValueSet valueSet = new()
                {
                    { ID_KEY, id.Value },
                    { RESULT_KEY, outputString }
                };
                await args.Request.SendResponseAsync(valueSet);
            }
            catch (Exception e)
            {
                ValueSet valueSet = new()
                {
                    { ERROR_KEY, e.Message },
                    { ID_KEY, id.Value }
                };
                await args.Request.SendResponseAsync(valueSet);
            }
            finally
            {
                if (id.HasValue)
                {
                    idDirectory.Remove(id.Value);
                }
            }
        }

        internal abstract Task<TOut> PerformStartAsync(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args, Guid? id, CancellationTokenSource cancellationTokenSource);
    }
}
