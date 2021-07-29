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

namespace InteropTools.Providers.OSReboot.NDTKProvider
{
    internal class NDTKRebootProvider : IRebootProvider
    {
        private NRPC _nrpc;

        public bool IsSupported(REBOOT_OPERATION operation)
        {
            if (_nrpc == null)
            {
                try
                {
                    _nrpc = new NRPC();
                    uint ret = _nrpc.Initialize();
                }
                catch
                {
                    _nrpc = null;
                    return false;
                }
            }

            return true;
        }

        public REBOOT_STATUS SystemReboot()
        {
            if (!IsSupported(REBOOT_OPERATION.SystemReboot))
            {
                throw new NotImplementedException();
            }

            return _nrpc.SystemReboot() == 0 ? REBOOT_STATUS.SUCCESS : REBOOT_STATUS.FAILED;
        }
    }
}