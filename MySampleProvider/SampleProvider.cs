using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.System.Diagnostics.DevicePortal;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace MySampleProvider
{
    public sealed class SampleProvider : IBackgroundTask
    {
        private BackgroundTaskDeferral taskDeferral;
        private DevicePortalConnection devicePortalConnection;

        // Implement background task handler with a DevicePortalConnection
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Take a deferral to allow the background task to continue executing 
            this.taskDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;

            // Create a DevicePortal client from an AppServiceConnection 
            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            var appServiceConnection = details.AppServiceConnection;
            this.devicePortalConnection = DevicePortalConnection.GetForAppServiceConnection(appServiceConnection);

            // Add Closed, RequestReceived handlers 
            devicePortalConnection.Closed += DevicePortalConnection_Closed;
            devicePortalConnection.RequestReceived += DevicePortalConnection_RequestReceived;
        }

        // Sample RequestReceived echo handler: respond with an HTML page including the query and some additional process information. 
        private void DevicePortalConnection_RequestReceived(DevicePortalConnection sender, DevicePortalConnectionRequestReceivedEventArgs args)
        {
            var req = args.RequestMessage;
            var res = args.ResponseMessage;

            if (req.RequestUri.AbsolutePath.EndsWith("/echo"))
            {
                // construct an html response message
                string con = "<h1>" + req.RequestUri.AbsoluteUri + "</h1><br/>";
                var proc = Windows.System.Diagnostics.ProcessDiagnosticInfo.GetForCurrentProcess();
                con += String.Format("This process is consuming {0} bytes (Working Set)<br/>", proc.MemoryUsage.GetReport().WorkingSetSizeInBytes);
                con += String.Format("The process PID is {0}<br/>", proc.ProcessId);
                con += String.Format("The executable filename is {0}", proc.ExecutableFileName);
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
        private void DevicePortalConnection_Closed(DevicePortalConnection sender, DevicePortalConnectionClosedEventArgs args)
        {
            
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            
        }
    }
}
