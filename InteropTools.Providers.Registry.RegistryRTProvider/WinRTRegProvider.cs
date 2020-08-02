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

using System;
using System.Collections.Generic;
using RegistryRT;
using System.Text;
using System.Linq;

namespace InteropTools.Providers.Registry.RegistryRTProvider
{
    internal class WinRTRegProvider : IRegProvider
    {
        RegistryRT.Registry helper = new RegistryRT.Registry();

        private static Dictionary<REG_HIVES, RegistryHive> _hives = new Dictionary<REG_HIVES, RegistryHive>
        {
            { REG_HIVES.HKEY_CLASSES_ROOT, RegistryHive.HKEY_CLASSES_ROOT },
            { REG_HIVES.HKEY_CURRENT_USER, RegistryHive.HKEY_CURRENT_USER },
            { REG_HIVES.HKEY_LOCAL_MACHINE, RegistryHive.HKEY_LOCAL_MACHINE },
            { REG_HIVES.HKEY_USERS, RegistryHive.HKEY_USERS },
            //{ REG_HIVES.HKEY_PERFORMANCE_DATA, REG_HIVES.HKEY_PERFORMANCE_DATA },
            { REG_HIVES.HKEY_CURRENT_CONFIG, RegistryHive.HKEY_CURRENT_CONFIG },
            //{ REG_HIVES.HKEY_DYN_DATA, REG_HIVES.HKEY_DYN_DATA },
            { REG_HIVES.HKEY_CURRENT_USER_LOCAL_SETTINGS, RegistryHive.HKEY_CURRENT_USER_LOCAL_SETTINGS }
        };

        private static Dictionary<REG_VALUE_TYPE, RegistryType> _valtypes = new Dictionary<REG_VALUE_TYPE, RegistryType>
        {
            { REG_VALUE_TYPE.REG_NONE, RegistryType.None },
            { REG_VALUE_TYPE.REG_SZ, RegistryType.String },
            { REG_VALUE_TYPE.REG_EXPAND_SZ, RegistryType.VariableString },
            { REG_VALUE_TYPE.REG_BINARY, RegistryType.Binary },
            { REG_VALUE_TYPE.REG_DWORD, RegistryType.Integer },
            { REG_VALUE_TYPE.REG_DWORD_BIG_ENDIAN, RegistryType.IntegerBigEndian },
            { REG_VALUE_TYPE.REG_LINK, RegistryType.SymbolicLink },
            { REG_VALUE_TYPE.REG_MULTI_SZ, RegistryType.MultiString },
            { REG_VALUE_TYPE.REG_RESOURCE_LIST, RegistryType.ResourceList },
            { REG_VALUE_TYPE.REG_FULL_RESOURCE_DESCRIPTOR, RegistryType.HardwareResourceList },
            { REG_VALUE_TYPE.REG_RESOURCE_REQUIREMENTS_LIST, RegistryType.ResourceRequirement },
            { REG_VALUE_TYPE.REG_QWORD, RegistryType.Long }
        };


        public bool IsSupported(REG_OPERATION operation)
        {
            try
            {
                helper.InitNTDLLEntryPoints();
            }
            catch
            {

            }
            return true;
        }

        public REG_STATUS RegAddKey(REG_HIVES hive, String key)
        {
            try
            {
                return helper.CreateKey(_hives[hive], key) ? REG_STATUS.SUCCESS : REG_STATUS.FAILED;
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegDeleteKey(REG_HIVES hive, String key, bool recursive)
        {
            try
            {
                if (recursive)
                {
                    return helper.DeleteKeysRecursive(_hives[hive], key) ? REG_STATUS.SUCCESS : REG_STATUS.FAILED;
                }

                return helper.DeleteKey(_hives[hive], key) ? REG_STATUS.SUCCESS : REG_STATUS.FAILED;
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegDeleteValue(REG_HIVES hive, String key, String name)
        {
            try
            {
                return helper.DeleteValue(_hives[hive], key, name) ? REG_STATUS.SUCCESS : REG_STATUS.FAILED;
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegEnumKey(REG_HIVES? hive, String key, out IReadOnlyList<REG_ITEM> items)
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

            try
            {
                String[] subkeys;
                var didSucceed = helper.GetSubKeyList(_hives[(REG_HIVES)hive], key, out subkeys);
                String[] subvalues;
                var didSucceed2 = helper.GetValueList(_hives[(REG_HIVES)hive], key, out subvalues);

                if (subkeys != null)
                {
                    foreach (var subkey in subkeys)
                    {
                        list.Add(new REG_ITEM { Name = subkey, Hive = (REG_HIVES)hive, Key = key, Type = REG_TYPE.KEY });
                    }
                }

                if (subvalues != null)
                {
                    foreach (var subval in subvalues)
                    {
                        var type = helper.GetValueInfo2(_hives[(REG_HIVES)hive], key, subval, 0);
                        
                        uint valtype = 0;

                        try
                        {
                            byte[] data;
                            RegQueryValue((REG_HIVES)hive, key, subval, type, out valtype, out data);
                            list.Add(new REG_ITEM { Name = subval, Hive = (REG_HIVES)hive, Key = key, Type = REG_TYPE.VALUE, Data = data, ValueType = valtype });
                        }
                        catch
                        {
                            list.Add(new REG_ITEM { Name = subval, Hive = (REG_HIVES)hive, Key = key, Type = REG_TYPE.VALUE, Data = null, ValueType = type });
                        }

                    }
                }

                if (didSucceed || didSucceed2)
                {
                    items = list;
                    return REG_STATUS.SUCCESS;
                }
            }
            catch
            {

            }

            items = new List<REG_ITEM>();
            return REG_STATUS.FAILED;
        }

        public REG_KEY_STATUS RegQueryKeyStatus(REG_HIVES hive, String key)
        {
            try
            {
                var ret = helper.GetKeyStatus(_hives[hive], key);

                if (ret == 0)
                {
                    return REG_KEY_STATUS.FOUND;
                }

                if (ret == 0xC0000022)
                {
                    return REG_KEY_STATUS.ACCESS_DENIED;
                }
            }
            catch
            {

            }
            return REG_KEY_STATUS.NOT_FOUND;
        }

        public REG_STATUS RegRenameKey(REG_HIVES hive, String key, String newname)
        {
            try
            {
                return helper.RenameKey(_hives[hive], key, newname) ? REG_STATUS.SUCCESS : REG_STATUS.FAILED;
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }
        
        public REG_STATUS RegQueryKeyLastModifiedTime(REG_HIVES hive, String key, out long lastmodified)
        {
            try
            {
                var result = helper.GetKeyLastWriteTime(_hives[hive], key, out lastmodified);
                return result ? REG_STATUS.SUCCESS : REG_STATUS.FAILED;
            }
            catch
            {
                lastmodified = long.MinValue;
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, uint valtype, out uint outvaltype, out byte[] data)
        {
            try
            {
                uint valtypetmp;
                if (!helper.QueryValue(_hives[hive], key, regvalue, out valtypetmp, out data))
                {
                    data = new byte[0];
                    outvaltype = uint.MinValue;
                    return REG_STATUS.FAILED;
                }
                outvaltype = valtypetmp;
                return REG_STATUS.SUCCESS;
            }
            catch
            {
                data = new byte[0];
                outvaltype = uint.MinValue;
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, byte[] data)
        {
            try
            {
                var result = helper.WriteValue(_hives[hive], key, regvalue, data, valtype);
                return result ? REG_STATUS.SUCCESS : REG_STATUS.FAILED;
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegLoadHive(string hivepath, string mountedname, bool InUser)
        {
            try
            {
                var result = helper.LoadHive(hivepath, mountedname, InUser);
                return result ? REG_STATUS.SUCCESS : REG_STATUS.FAILED;
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegUnloadHive(string mountedname, bool InUser)
        {
            try
            {
                var result = helper.UnloadHive(mountedname, InUser);
                return result ? REG_STATUS.SUCCESS : REG_STATUS.FAILED;
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }
    }
}
