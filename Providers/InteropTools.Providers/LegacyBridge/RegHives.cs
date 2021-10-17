// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

namespace InteropTools.Providers
{
    public enum RegHives
    {
        HKEY_CLASSES_ROOT = int.MinValue,
        HKEY_CURRENT_USER = -2147483647,
        HKEY_LOCAL_MACHINE = -2147483646,
        HKEY_USERS = -2147483645,
        HKEY_PERFORMANCE_DATA = -2147483644,
        HKEY_CURRENT_CONFIG = -2147483643,
        HKEY_DYN_DATA = -2147483642,
        HKEY_CURRENT_USER_LOCAL_SETTINGS = -2147483641
    }
}