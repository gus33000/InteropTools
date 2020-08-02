using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
#if ARM
using WinPRTUtils;
#endif

namespace RegistryHelper
{
    public sealed class WinPRTUtilsProvider : IRegistryProvider
    {
        private static Dictionary<REG_VALUE_TYPE, int> _winprtvaltypes = new Dictionary<REG_VALUE_TYPE, int>
        {
            { REG_VALUE_TYPE.REG_NONE , 0 },
            { REG_VALUE_TYPE.REG_SZ , 1 },
            { REG_VALUE_TYPE.REG_EXPAND_SZ , 2 },
            { REG_VALUE_TYPE.REG_BINARY , 3 },
            { REG_VALUE_TYPE.REG_DWORD , 4 },
            { REG_VALUE_TYPE.REG_DWORD_BIG_ENDIAN , 5 },
            { REG_VALUE_TYPE.REG_LINK , 6 },
            { REG_VALUE_TYPE.REG_MULTI_SZ , 7 },
            { REG_VALUE_TYPE.REG_RESOURCE_LIST , 8 },
            { REG_VALUE_TYPE.REG_FULL_RESOURCE_DESCRIPTOR , 9 },
            { REG_VALUE_TYPE.REG_RESOURCE_REQUIREMENTS_LIST , 10 },
            { REG_VALUE_TYPE.REG_QWORD , 11 }
        };

        public Boolean IsSupported()
        {
            return true;
        }

        public REG_STATUS RegDeleteKey(REG_HIVES hive, String key, bool recursive)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegDeleteValue(REG_HIVES hive, String key, String name)
        {
            #if ARM
            if (hive == REG_HIVES.HKEY_LOCAL_MACHINE)
            {
                try
                {
                    RegistryKey regkeyenum = RegistryUtils.EnumRegistryKey(key);
                    if (regkeyenum != null)
                    {
                        if (regkeyenum.Values.Count != 0)
                        {
                            foreach (var val in regkeyenum.Values)
                            {
                                if (val.Name.ToLower() == name.ToLower())
                                {
                                    regkeyenum.Values[regkeyenum.Values.IndexOf(val)].writeDeletesValue = true;
                                    var result = RegistryUtils.WriteRegistryKeys(regkeyenum);
                                    if (result == 0)
                                    {
                                        return REG_STATUS.SUCCESS;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                catch
                {

                }
                return REG_STATUS.FAILED;
            }
#endif
            return REG_STATUS.NOT_IMPLEMENTED;
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

#if ARM
            if ((hive == REG_HIVES.HKEY_LOCAL_MACHINE) && (key != ""))
            {
                try
                {
                    RegistryKey keyenum = RegistryUtils.EnumRegistryKey(key);
                    if (keyenum.SubKeys != null)
                    {
                        if (keyenum.SubKeys.Count != 0)
                        {
                            foreach (var subkey in keyenum.SubKeys)
                            {
                                list.Add(new REG_ITEM { Name = subkey.Name, Hive = (REG_HIVES)hive, Key = key, Type = REG_TYPE.KEY });
                            }
                        }
                    }
                    if (keyenum.Values != null)
                    {
                        if (keyenum.Values.Count != 0)
                        {
                            foreach (var value in keyenum.Values)
                            {
                                REG_VALUE_TYPE valtype;
                                String data = "";
                                try
                                {
                                    CRegistryHelper helper = new CRegistryHelper();
                                    helper.RegQueryValue((REG_HIVES)hive, key, value.Name, _winprtvaltypes.FirstOrDefault(x => x.Value == value.ValueType).Key, out valtype, out data);
                                }
                                catch
                                {

                                }
                                list.Add(new REG_ITEM { Name = value.Name, Hive = (REG_HIVES)hive, Key = key, Type = REG_TYPE.VALUE, ValueType = _winprtvaltypes.FirstOrDefault(x => x.Value == value.ValueType).Key, DataAsString = data });
                            }
                        }
                    }
                    items = list;
                    return REG_STATUS.SUCCESS;
                }
                catch
                {
                    
                }
            }
#endif

            items = new List<REG_ITEM>();
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryMultiString(REG_HIVES hive, String key, String regvalue, out String[] data)
        {
            data = new string[0];
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryVariableString(REG_HIVES hive, String key, String regvalue, out String data)
        {
            data = "";
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryDword(REG_HIVES hive, String key, String regvalue, out UInt32 data)
        {
#if ARM
            if (hive == REG_HIVES.HKEY_LOCAL_MACHINE)
            {
                try
                {
                    var vals = RegistryUtils.EnumRegistryKey(key).Values;
                    foreach (var var in vals)
                    {
                        if (var.Name.ToLower() == regvalue.ToLower())
                        {
                            if (var.ValueType == _winprtvaltypes[REG_VALUE_TYPE.REG_DWORD])
                            {
                                data = uint.Parse(var.Value.Remove(0, 2), System.Globalization.NumberStyles.HexNumber);
                                return REG_STATUS.SUCCESS;
                            }
                            break;
                        }
                    }
                }
                catch
                {

                }
            }
#endif

            data = uint.MinValue;
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_KEY_STATUS RegQueryKeyStatus(REG_HIVES hive, String key)
        {
#if ARM
            if (hive == REG_HIVES.HKEY_LOCAL_MACHINE)
            {
                try
                {
                    var vals = RegistryUtils.EnumRegistryKey(key).Name;
                    return REG_KEY_STATUS.FOUND;
                }
                catch
                {
                    return REG_KEY_STATUS.NOT_FOUND;
                }
            }
            
#endif
            return REG_KEY_STATUS.UNKNOWN;
        }

        public REG_STATUS RegQueryQword(REG_HIVES hive, String key, String regvalue, out UInt64 data)
        {
            data = UInt64.MinValue;
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryString(REG_HIVES hive, String key, String regvalue, out String data)
        {
#if ARM
            if (hive == REG_HIVES.HKEY_LOCAL_MACHINE)
            {
                try
                {
                    var vals = RegistryUtils.EnumRegistryKey(key).Values;
                    foreach (var var in vals)
                    {
                        if (var.Name.ToLower() == regvalue.ToLower())
                        {
                            if (var.ValueType == _winprtvaltypes[REG_VALUE_TYPE.REG_SZ])
                            {
                                data = var.Value;
                                return REG_STATUS.SUCCESS;
                            }
                            break;
                        }
                    }
                }
                catch
                {

                }
            }
#endif

            data = "";
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, String key, String regvalue, REG_VALUE_TYPE valtype, out REG_VALUE_TYPE outvaltype, out Byte[] data)
        {
            outvaltype = REG_VALUE_TYPE.REG_NONE;
            data = new byte[0];
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetDword(REG_HIVES hive, String key, String regvalue, UInt32 data)
        {
#if ARM
            if (hive == REG_HIVES.HKEY_LOCAL_MACHINE)
            {
                try
                {
                    bool valfound = false;
                    var Regkey = RegistryUtils.EnumRegistryKey(key);
                    foreach (var var in Regkey.Values)
                    {
                        if (var.Name.ToLower() == regvalue.ToLower())
                        {
                            valfound = true;
                            if (var.ValueType == _winprtvaltypes[REG_VALUE_TYPE.REG_DWORD])
                            {
                                Regkey.Values[Regkey.Values.IndexOf(var)].Value = "0x" + ((UInt32)data).ToString("X8").ToLower();
                                int result = RegistryUtils.WriteRegistryKeys(Regkey);
                                if (result == 0)
                                {
                                    return REG_STATUS.SUCCESS;
                                }
                            }
                            break;
                        }
                    }
                    if (!valfound)
                    {
                        Regkey.Values.Add(new RegistryKeyValue(_winprtvaltypes[REG_VALUE_TYPE.REG_DWORD], regvalue, "0x" + ((UInt32)data).ToString("X8").ToLower()));
                        int result = RegistryUtils.WriteRegistryKeys(Regkey);
                        if (result == 0)
                        {
                            return REG_STATUS.SUCCESS;
                        }
                    }
                }
                catch
                {

                }
                return REG_STATUS.FAILED;
            }
#endif
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetMultiString(REG_HIVES hive, String key, String regvalue, [ReadOnlyArray] String[] data)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetQword(REG_HIVES hive, String key, String regvalue, UInt64 data)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetString(REG_HIVES hive, String key, String regvalue, String data)
        {
#if ARM
            if (hive == REG_HIVES.HKEY_LOCAL_MACHINE)
            {
                try
                {
                    bool valfound = false;
                    var Regkey = RegistryUtils.EnumRegistryKey(key);
                    foreach (var var in Regkey.Values)
                    {
                        if (var.Name.ToLower() == regvalue.ToLower())
                        {
                            if (var.ValueType == _winprtvaltypes[REG_VALUE_TYPE.REG_SZ])
                            {
                                valfound = true;
                                Regkey.Values[Regkey.Values.IndexOf(var)].Value = data;
                                int result = RegistryUtils.WriteRegistryKeys(Regkey);
                                if (result == 0)
                                {
                                    return REG_STATUS.SUCCESS;
                                }
                            }
                            break;
                        }
                    }
                    if (!valfound)
                    {
                        Regkey.Values.Add(new RegistryKeyValue(1, regvalue, data));
                        int result = RegistryUtils.WriteRegistryKeys(Regkey);
                        if (result == 0)
                        {
                            return REG_STATUS.SUCCESS;
                        }
                    }
                }
                catch
                {

                }
                return REG_STATUS.FAILED;
            }
#endif
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetValue(REG_HIVES hive, String key, String regvalue, REG_VALUE_TYPE valtype, [ReadOnlyArray] Byte[] data)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetVariableString(REG_HIVES hive, String key, String regvalue, String data)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegRenameKey(REG_HIVES hive, String key, String newname)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegAddKey(REG_HIVES hive, String key)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryKeyLastModifiedTime(REG_HIVES hive, String key, out long lastmodified)
        {
            lastmodified = long.MinValue;
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, uint valtype, out uint outvaltype, out byte[] data)
        {
            outvaltype = 0;
            data = new byte[0];
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, [ReadOnlyArray] byte[] data)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegEnumKey(REG_HIVES? hive, string key, out IReadOnlyList<REG_ITEM_CUSTOM> items)
        {
            List<REG_ITEM_CUSTOM> list = new List<REG_ITEM_CUSTOM>();

            if (hive == null)
            {
                list.Add(new REG_ITEM_CUSTOM { Name = "HKEY_CLASSES_ROOT (HKCR)", Hive = REG_HIVES.HKEY_CLASSES_ROOT, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM_CUSTOM { Name = "HKEY_CURRENT_CONFIG (HKCC)", Hive = REG_HIVES.HKEY_CURRENT_CONFIG, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM_CUSTOM { Name = "HKEY_CURRENT_USER (HKCU)", Hive = REG_HIVES.HKEY_CURRENT_USER, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM_CUSTOM { Name = "HKEY_CURRENT_USER_LOCAL_SETTINGS (HKCULS)", Hive = REG_HIVES.HKEY_CURRENT_USER_LOCAL_SETTINGS, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM_CUSTOM { Name = "HKEY_DYN_DATA (HKDD)", Hive = REG_HIVES.HKEY_DYN_DATA, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM_CUSTOM { Name = "HKEY_LOCAL_MACHINE (HKLM)", Hive = REG_HIVES.HKEY_LOCAL_MACHINE, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM_CUSTOM { Name = "HKEY_PERFORMANCE_DATA (HKPD)", Hive = REG_HIVES.HKEY_PERFORMANCE_DATA, Type = REG_TYPE.HIVE });
                list.Add(new REG_ITEM_CUSTOM { Name = "HKEY_USERS (HKU)", Hive = REG_HIVES.HKEY_USERS, Type = REG_TYPE.HIVE });

                items = list;
                return REG_STATUS.SUCCESS;
            }

#if ARM
            if ((hive == REG_HIVES.HKEY_LOCAL_MACHINE) && (key != ""))
            {
                try
                {
                    RegistryKey keyenum = RegistryUtils.EnumRegistryKey(key);
                    if (keyenum.SubKeys != null)
                    {
                        if (keyenum.SubKeys.Count != 0)
                        {
                            foreach (var subkey in keyenum.SubKeys)
                            {
                                list.Add(new REG_ITEM_CUSTOM { Name = subkey.Name, Hive = (REG_HIVES)hive, Key = key, Type = REG_TYPE.KEY });
                            }
                        }
                    }
                    if (keyenum.Values != null)
                    {
                        if (keyenum.Values.Count != 0)
                        {
                            foreach (var value in keyenum.Values)
                            {
                                REG_VALUE_TYPE valtype;
                                String data = "";
                                try
                                {
                                    CRegistryHelper helper = new CRegistryHelper();
                                    helper.RegQueryValue((REG_HIVES)hive, key, value.Name, _winprtvaltypes.FirstOrDefault(x => x.Value == value.ValueType).Key, out valtype, out data);
                                }
                                catch
                                {

                                }
                                list.Add(new REG_ITEM_CUSTOM { Name = value.Name, Hive = (REG_HIVES)hive, Key = key, Type = REG_TYPE.VALUE, ValueType = BitConverter.ToUInt32(BitConverter.GetBytes(value.ValueType), 0), DataAsString = data });
                            }
                        }
                    }
                    items = list;
                    return REG_STATUS.SUCCESS;
                }
                catch
                {

                }
            }
#endif

            items = new List<REG_ITEM_CUSTOM>();
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegLoadHive(string FilePath, string mountpoint, bool inUser)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegUnloadHive(string mountpoint, bool inUser)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }
    }
}
