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
using System.Collections.Generic;

namespace InteropTools.AppExtensibilityBackgroundTask
{
    internal class AppExtensibilityRegProvider : IAppExtensibilityProvider
    {
        public REG_STATUS RegAddKey(REG_HIVES hive, string key)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegDeleteKey(REG_HIVES hive, string key, bool recursive)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegDeleteValue(REG_HIVES hive, string key, string name)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegEnumKey(REG_HIVES? hive, string key, out IReadOnlyList<REG_ITEM> items)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegLoadHive(string hivepath, string mountedname, bool InUser)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegQueryKeyLastModifiedTime(REG_HIVES hive, string key, out long lastmodified)
        {
            throw new NotImplementedException();
        }

        public REG_KEY_STATUS RegQueryKeyStatus(REG_HIVES hive, string key)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, uint valtype, out uint outvaltype, out byte[] data)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegRenameKey(REG_HIVES hive, string key, string newname)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, byte[] data)
        {
            throw new NotImplementedException();
        }

        public REG_STATUS RegUnloadHive(string mountedname, bool InUser)
        {
            throw new NotImplementedException();
        }
    }
}