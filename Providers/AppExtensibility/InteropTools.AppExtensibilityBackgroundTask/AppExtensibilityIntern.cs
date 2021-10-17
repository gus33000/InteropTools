// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

/*++

Copyright (c) 2016  Interop Tools Development Team
Copyright (c) 2017  Gustave M.

Module Name:

    Plugin.cs

Abstract:

    This module implements the NDTK Registry Provider Plugin.

Author:

    Gustave M.     (gus33000)       20-Mar-2017

Revision History:

    Gustave M. (gus33000) 20-Mar-2017

        Initial Implementation.

--*/

using Windows.ApplicationModel.Background;

namespace InteropTools.AppExtensibilityBackgroundTask
{
    public sealed class AppExtensibilityProvider : IBackgroundTask
    {
        private readonly IBackgroundTask internalTask = new AppExtensibilityProviderIntern();

        public void Run(IBackgroundTaskInstance taskInstance) => internalTask.Run(taskInstance);
    }
}
