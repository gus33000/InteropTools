// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Threading;
using System.Threading.Tasks;
using AppPlugin;
using Windows.ApplicationModel.AppService;

namespace InteropTools.Providers.Applications.Definition
{
    public abstract class ApplicationProvidersWithOptions : AbstractPlugin<string, string, double>
    {
        public const string PLUGIN_NAME = "InteropTools.Providers.Applications";

        protected sealed override Task<string> Execute(AppServiceConnection sender, string input, IProgress<double> progress, CancellationToken cancelToken)
        {
            return ExecuteAsync(sender, input, progress, cancelToken);
        }

        protected abstract Task<string> ExecuteAsync(AppServiceConnection sender, string input, IProgress<double> progress, CancellationToken cancelToken);

        protected abstract Task<Options> GetOptions();

        protected abstract Guid GetOptionsGuid();
    }
}