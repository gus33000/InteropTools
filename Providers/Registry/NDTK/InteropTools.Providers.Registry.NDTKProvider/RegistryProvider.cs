// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Windows.ApplicationModel.Background;

namespace InteropTools.Providers.Registry.NDTKProvider
{
    public sealed class RegistryProvider : IBackgroundTask
    {
        private readonly IBackgroundTask internalTask = new RegistryProviderIntern();

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            internalTask.Run(taskInstance);
        }
    }
}