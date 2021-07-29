﻿using AppPlugin;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;

namespace InteropTools.Providers.Registry.Definition
{
    public abstract class RegistryProvidersWithOptions : AbstractPlugin<string, string, double>//TransfareOptions
    {
        public const string PLUGIN_NAME = "InteropTools.Providers.Registry";

        protected sealed override Task<string> Execute(AppServiceConnection sender, string input, IProgress<double> progress, CancellationToken cancelToken)//, TransfareOptions options
        {
            /*if (options.OptionsIdentifyer != GetOptionsGuid())
                throw new ArgumentException("Option Not generated by this Plugin", nameof(options));*/
            return ExecuteAsync(sender, input, progress, cancelToken); //, options.Options;
        }

        /*protected sealed override async Task<TransfareOptions> GetDefaultOptionsAsync()
        => new TransfareOptions() { Options = await GetOptions() };*/

        protected abstract Task<string> ExecuteAsync(AppServiceConnection sender, string input, IProgress<double> progress, CancellationToken cancelToken); //, Options options

        protected abstract Task<Options> GetOptions();

        protected abstract Guid GetOptionsGuid();
    }
}