using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.System.Diagnostics.DevicePortal;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace MySampleProvider
{
    public sealed class SampleProvider : IBackgroundTask
    {
        private DevicePortalConnection devicePortalConnection;
        private BackgroundTaskDeferral taskDeferral;

        // Implement background task handler with a DevicePortalConnection
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Take a deferral to allow the background task to continue executing
            taskDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;

            // Create a DevicePortal client from an AppServiceConnection
            AppServiceTriggerDetails details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            AppServiceConnection appServiceConnection = details.AppServiceConnection;
            devicePortalConnection = DevicePortalConnection.GetForAppServiceConnection(appServiceConnection);

            // Add Closed, RequestReceived handlers
            devicePortalConnection.Closed += DevicePortalConnection_Closed;
            devicePortalConnection.RequestReceived += DevicePortalConnection_RequestReceived;
        }

        private void DevicePortalConnection_Closed(DevicePortalConnection sender, DevicePortalConnectionClosedEventArgs args)
        {
        }

        // Sample RequestReceived echo handler: respond with an HTML page including the query and some additional process information.
        private void DevicePortalConnection_RequestReceived(DevicePortalConnection sender, DevicePortalConnectionRequestReceivedEventArgs args)
        {
            HttpRequestMessage req = args.RequestMessage;
            HttpResponseMessage res = args.ResponseMessage;

            if (req.RequestUri.AbsolutePath.EndsWith("/echo"))
            {
                // construct an html response message
                string con = "<h1>" + req.RequestUri.AbsoluteUri + "</h1><br/>";
                Windows.System.Diagnostics.ProcessDiagnosticInfo proc = Windows.System.Diagnostics.ProcessDiagnosticInfo.GetForCurrentProcess();
                con += string.Format("This process is consuming {0} bytes (Working Set)<br/>", proc.MemoryUsage.GetReport().WorkingSetSizeInBytes);
                con += string.Format("The process PID is {0}<br/>", proc.ProcessId);
                con += string.Format("The executable filename is {0}", proc.ExecutableFileName);
                res.Content = new HttpStringContent(con);
                res.Content.Headers.ContentType = new HttpMediaTypeHeaderValue("text/html");
                res.StatusCode = HttpStatusCode.Ok;
            }

            /*if (req.RequestUri.LocalPath.ToLower().Contains("/www/"))
            {
                var filePath = req.RequestUri.AbsolutePath.Replace('/', '\\').ToLower();
                filePath = filePath.Replace("\\backgroundprovider", "");
                try
                {
                    var fileStream = Windows.ApplicationModel.Package.Current.InstalledLocation.OpenStreamForReadAsync(filePath).GetAwaiter().GetResult();
                    res.StatusCode = HttpStatusCode.Ok;
                    res.Content = new HttpStreamContent(fileStream.AsInputStream());
                    res.Content.Headers.ContentType = new HttpMediaTypeHeaderValue("text/html");
                }
                catch (FileNotFoundException e)
                {
                    string con = String.Format("<h1>{0} - not found</h1>\r\n", filePath);
                    con += "Exception: " + e.ToString();
                    res.Content = new HttpStringContent(con);
                    res.StatusCode = HttpStatusCode.NotFound;
                    res.Content.Headers.ContentType = new HttpMediaTypeHeaderValue("text/html");
                }
            }*/
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
        }
    }
}