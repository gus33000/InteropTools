// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

namespace InteropTools.Providers.OSReboot.NDTKProvider
{
    internal interface IRebootProvider
    {
        bool IsSupported(REBOOT_OPERATION operation);

        REBOOT_STATUS SystemReboot();
    }
}