// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

/*++

Copyright (c) 2016  Interop Tools Development Team
Copyright (c) 2017  Gustave M.

Module Name:

    NDTKRegProvider.cs

Abstract:

    This module implements the NDTK Registry Provider Interface.

Author:

    Gustave M.     (gus33000)       20-Mar-2017

Revision History:

    Gustave M. (gus33000) 20-Mar-2017

        Initial Implementation.

--*/

using System;
using System.Runtime.InteropServices;

namespace InteropTools.Providers.OSReboot.FlightingProvider
{
    internal class FlightingRebootProvider : IRebootProvider
    {
        [DllImport("FlightingClientDll.dll")]
        public static extern int Reboot();

        public bool IsSupported(REBOOT_OPERATION operation)
        {
            return true;
        }

        public REBOOT_STATUS SystemReboot()
        {
            if (!IsSupported(REBOOT_OPERATION.SystemReboot))
            {
                throw new NotImplementedException();
            }

            return Reboot() == 0 ? REBOOT_STATUS.SUCCESS : REBOOT_STATUS.FAILED;
        }
    }
}