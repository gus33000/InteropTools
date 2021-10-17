using Windows.ApplicationModel.Background;

namespace InteropTools.Providers.Applications.WinRTProvider
{
    public sealed class ApplicationsProvider : IBackgroundTask
    {
        private readonly IBackgroundTask internalTask = new ApplicationsProviderIntern();

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            internalTask.Run(taskInstance);
        }
    }
}