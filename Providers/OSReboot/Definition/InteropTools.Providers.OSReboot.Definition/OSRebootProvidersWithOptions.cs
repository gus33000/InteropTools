using AppPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.ApplicationModel.AppService;

namespace InteropTools.Providers.OSReboot.Definition
{
    public abstract class OSRebootProvidersWithOptions : AbstractPlugin<string, string, double>
    {

        public const String PLUGIN_NAME = "InteropTools.Providers.OSReboot";

        protected sealed override Task<string> Execute(AppServiceConnection sender, string input, IProgress<double> progress, CancellationToken cancelToken)
        {
            return ExecuteAsync(sender, input, progress, cancelToken);
        }
        
        protected abstract Task<string> ExecuteAsync(AppServiceConnection sender, string input, IProgress<double> progress, CancellationToken cancelToken);

        protected abstract Task<Options> GetOptions();
        protected abstract Guid GetOptionsGuid();
    }
}
