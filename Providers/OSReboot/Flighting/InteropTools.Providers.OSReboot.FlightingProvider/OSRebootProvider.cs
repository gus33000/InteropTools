using Windows.ApplicationModel.Background;

namespace InteropTools.Providers.OSReboot.FlightingProvider
{
    public sealed class OSRebootProvider : IBackgroundTask
    {
        private readonly IBackgroundTask internalTask = new OSRebootProviderIntern();

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            internalTask.Run(taskInstance);
        }
    }
}