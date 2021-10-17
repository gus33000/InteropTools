// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

namespace InteropTools.Providers.Registry.RegistryRTProvider
{
    internal enum REG_OPERATION
    {
        RegQueryKeyLastModifiedTime,
        RegAddKey,
        RegDeleteKey,
        RegDeleteValue,
        RegRenameKey,
        RegQueryKeyStatus,
        RegQueryValue,
        RegSetValue,
        RegEnumKey,
        RegLoadHive,
        RegUnloadHive
    }
}