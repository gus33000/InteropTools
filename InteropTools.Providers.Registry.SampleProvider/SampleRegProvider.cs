/*++

Copyright (c) 2016  Interop Tools Development Team
Copyright (c) 2017  Gustave M.

Module Name:

    WinRTRegProvider.cs

Abstract:

    This module implements the WinRT Registry Provider Interface.

Author:

    Gustave M.     (gus33000)       20-Mar-2017

Revision History:

    Gustave M. (gus33000) 20-Mar-2017

        Initial Implementation.

--*/

using System.Collections.Generic;

namespace InteropTools.Providers.Registry.SampleProvider
{
    internal class SampleRegProvider : IRegProvider
    {
        public bool IsSupported(REG_OPERATION operation)
        {
            return true;
        }

        public REG_STATUS RegAddKey(REG_HIVES hive, string key)
        {
            return REG_STATUS.SUCCESS;
        }

        public REG_STATUS RegDeleteKey(REG_HIVES hive, string key, bool recursive)
        {
            return REG_STATUS.SUCCESS;
        }

        public REG_STATUS RegDeleteValue(REG_HIVES hive, string key, string name)
        {
            return REG_STATUS.SUCCESS;
        }

        public REG_STATUS RegEnumKey(REG_HIVES? hive, string key, out IReadOnlyList<REG_ITEM> items)
        {
            List<REG_ITEM> list = new List<REG_ITEM>();

            if (hive == null)
            {
                list.Add(new REG_ITEM { Name = "HKEY_CLASSES_ROOT (HKCR)", Hive = REG_HIVES.HKEY_CLASSES_ROOT, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM { Name = "HKEY_CURRENT_CONFIG (HKCC)", Hive = REG_HIVES.HKEY_CURRENT_CONFIG, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM { Name = "HKEY_CURRENT_USER (HKCU)", Hive = REG_HIVES.HKEY_CURRENT_USER, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM { Name = "HKEY_CURRENT_USER_LOCAL_SETTINGS (HKCULS)", Hive = REG_HIVES.HKEY_CURRENT_USER_LOCAL_SETTINGS, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM { Name = "HKEY_DYN_DATA (HKDD)", Hive = REG_HIVES.HKEY_DYN_DATA, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM { Name = "HKEY_LOCAL_MACHINE (HKLM)", Hive = REG_HIVES.HKEY_LOCAL_MACHINE, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM { Name = "HKEY_PERFORMANCE_DATA (HKPD)", Hive = REG_HIVES.HKEY_PERFORMANCE_DATA, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM { Name = "HKEY_USERS (HKU)", Hive = REG_HIVES.HKEY_USERS, Type = REG_TYPE.HIVE });

                items = list;
                return REG_STATUS.SUCCESS;
            }

            items = new List<REG_ITEM>
            {
                new REG_ITEM
                {
                    Name = "Test 1",
                    Hive = hive,
                    Key = key,
                    Type = REG_TYPE.KEY,
                    Data = null,
                    ValueType = (uint)REG_VALUE_TYPE.REG_NONE
                },
                new REG_ITEM
                {
                    Name = "est 2",
                    Hive = hive,
                    Key = key,
                    Type = REG_TYPE.KEY,
                    Data = null,
                    ValueType = (uint)REG_VALUE_TYPE.REG_NONE
                },
                new REG_ITEM
                {
                    Name = "st 3",
                    Hive = hive,
                    Key = key,
                    Type = REG_TYPE.KEY,
                    Data = null,
                    ValueType = (uint)REG_VALUE_TYPE.REG_NONE
                },
                new REG_ITEM
                {
                    Name = "t 4",
                    Hive = hive,
                    Key = key,
                    Type = REG_TYPE.KEY,
                    Data = null,
                    ValueType = (uint)REG_VALUE_TYPE.REG_NONE
                },
                new REG_ITEM
                {
                    Name = "Test 5",
                    Hive = hive,
                    Key = key,
                    Type = REG_TYPE.KEY,
                    Data = null,
                    ValueType = (uint)REG_VALUE_TYPE.REG_NONE
                },
                new REG_ITEM
                {
                    Name = "est 6",
                    Hive = hive,
                    Key = key,
                    Type = REG_TYPE.KEY,
                    Data = null,
                    ValueType = (uint)REG_VALUE_TYPE.REG_NONE
                },
                new REG_ITEM
                {
                    Name = "st 7",
                    Hive = hive,
                    Key = key,
                    Type = REG_TYPE.VALUE,
                    Data = System.Text.Encoding.Unicode.GetBytes("Test value"),
                    ValueType = (uint)REG_VALUE_TYPE.REG_SZ
                },
                new REG_ITEM
                {
                    Name = "t 8",
                    Hive = hive,
                    Key = key,
                    Type = REG_TYPE.VALUE,
                    Data = System.Text.Encoding.Unicode.GetBytes("Test value"),
                    ValueType = (uint)REG_VALUE_TYPE.REG_SZ
                },
                new REG_ITEM
                {
                    Name = "Test 9",
                    Hive = hive,
                    Key = key,
                    Type = REG_TYPE.VALUE,
                    Data = System.Text.Encoding.Unicode.GetBytes("Test value"),
                    ValueType = (uint)REG_VALUE_TYPE.REG_SZ
                },
                new REG_ITEM
                {
                    Name = "est 10",
                    Hive = hive,
                    Key = key,
                    Type = REG_TYPE.VALUE,
                    Data = System.Text.Encoding.Unicode.GetBytes("Test value"),
                    ValueType = (uint)REG_VALUE_TYPE.REG_SZ
                },
                new REG_ITEM
                {
                    Name = "st 11",
                    Hive = hive,
                    Key = key,
                    Type = REG_TYPE.VALUE,
                    Data = System.Text.Encoding.Unicode.GetBytes("Test value"),
                    ValueType = (uint)REG_VALUE_TYPE.REG_SZ
                },
                new REG_ITEM
                {
                    Name = "t 12",
                    Hive = hive,
                    Key = key,
                    Type = REG_TYPE.VALUE,
                    Data = System.Text.Encoding.Unicode.GetBytes("Test value"),
                    ValueType = (uint)REG_VALUE_TYPE.REG_SZ
                }
            };
            return REG_STATUS.SUCCESS;
        }

        public REG_KEY_STATUS RegQueryKeyStatus(REG_HIVES hive, string key)
        {
            return REG_KEY_STATUS.FOUND;
        }

        public REG_STATUS RegRenameKey(REG_HIVES hive, string key, string newname)
        {
            return REG_STATUS.SUCCESS;
        }

        public REG_STATUS RegQueryKeyLastModifiedTime(REG_HIVES hive, string key, out long lastmodified)
        {
            lastmodified = 0;
            return REG_STATUS.SUCCESS;
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, uint valtype, out uint outvaltype, out byte[] data)
        {
            outvaltype = (uint)REG_VALUE_TYPE.REG_SZ;
            data = System.Text.Encoding.Unicode.GetBytes("Test value");
            return REG_STATUS.SUCCESS;
        }

        public REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, byte[] data)
        {
            return REG_STATUS.SUCCESS;
        }

        public REG_STATUS RegLoadHive(string hivepath, string mountedname, bool InUser)
        {
            return REG_STATUS.SUCCESS;
        }

        public REG_STATUS RegUnloadHive(string mountedname, bool InUser)
        {
            return REG_STATUS.SUCCESS;
        }
    }
}
