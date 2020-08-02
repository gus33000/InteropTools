using RegistryRT;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace RegistryHelper
{
    public sealed class RegistryRTProvider : IRegistryProvider
    {
        Registry helper = new Registry();

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

        public RegistryRTProvider()
        {
            try
            {
                helper.InitNTDLLEntryPoints();
            }
            catch
            {

            }
        }

        public Boolean IsSupported()
        {
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
                        var type = helper.GetValueInfo(_hives[(REG_HIVES)hive], key, subval, 0);

                        var propertype = _valtypes.FirstOrDefault(x => x.Value == type).Key;

                        REG_VALUE_TYPE valtype;
                        String data = "";
                        try
                        {
                            CRegistryHelper helper = new CRegistryHelper();
                            helper.RegQueryValue((REG_HIVES)hive, key, subval, propertype, out valtype, out data);
                        }
                        catch
                        {

                        }

                        list.Add(new REG_ITEM { Name = subval, Hive = (REG_HIVES)hive, Key = key, Type = REG_TYPE.VALUE, DataAsString = data, ValueType = propertype });
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

        public REG_STATUS RegQueryMultiString(REG_HIVES hive, String key, String regvalue, out String[] data)
        {
            try
            {
                byte[] buffer;
                REG_VALUE_TYPE regtype;
                var result = RegQueryValue(hive, key, regvalue, out regtype, out buffer);
                if (result != REG_STATUS.SUCCESS)
                {
                    data = new string[0];
                    return result;
                }

                string strNullTerminated = Encoding.Unicode.GetString(buffer);
                if (strNullTerminated.Substring(strNullTerminated.Length - 2) == "\0\0")
                {
                    // The REG_MULTI_SZ is properly terminated.
                    // Remove the array terminator, and the final string terminator.
                    strNullTerminated = strNullTerminated.Substring(0, strNullTerminated.Length - 2);
                }
                else if (strNullTerminated.Substring(strNullTerminated.Length - 1) == "\0")
                {
                    // The REG_MULTI_SZ is improperly terminated (only one terminator).
                    // Remove it.
                    strNullTerminated = strNullTerminated.Substring(0, strNullTerminated.Length - 1);
                }
                // Split by null terminator.
                data = strNullTerminated.Split('\0');
                return result;
            }
            catch
            {
                data = new String[0];
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegQueryVariableString(REG_HIVES hive, String key, String regvalue, out String data)
        {
            try
            {
                byte[] buffer;
                REG_VALUE_TYPE regtype;
                var result = RegQueryValue(hive, key, regvalue, out regtype, out buffer);
                if (result != REG_STATUS.SUCCESS)
                {
                    data = "";
                    return result;
                }
                data = Encoding.Unicode.GetString(buffer).TrimEnd('\0');
                return result;
            }
            catch
            {
                data = "";
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegQueryDword(REG_HIVES hive, String key, String regvalue, out UInt32 data)
        {
            try
            {
                byte[] buffer;
                REG_VALUE_TYPE regtype;
                var result = RegQueryValue(hive, key, regvalue, out regtype, out buffer);
                if (result != REG_STATUS.SUCCESS)
                {
                    data = uint.MinValue;
                    return result;
                }
                data = BitConverter.ToUInt32(buffer, 0);
                return result;
            }
            catch
            {
                data = uint.MinValue;
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegQueryQword(REG_HIVES hive, String key, String regvalue, out UInt64 data)
        {
            try
            {
                byte[] buffer;
                REG_VALUE_TYPE regtype;
                var result = RegQueryValue(hive, key, regvalue, out regtype, out buffer);
                if (result != REG_STATUS.SUCCESS)
                {
                    data = uint.MinValue;
                    return result;
                }
                data = BitConverter.ToUInt64(buffer, 0);
                return result;
            }
            catch
            {
                data = ulong.MinValue;
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegQueryString(REG_HIVES hive, String key, String regvalue, out String data)
        {
            try
            {
                byte[] buffer;
                REG_VALUE_TYPE regtype;
                var result = RegQueryValue(hive, key, regvalue, out regtype, out buffer);
                if (result != REG_STATUS.SUCCESS)
                {
                    data = "";
                    return result;
                }
                data = Encoding.Unicode.GetString(buffer).TrimEnd('\0');
                return result;
            }
            catch
            {
                data = "";
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, String key, String regvalue, REG_VALUE_TYPE valtype, out REG_VALUE_TYPE outvaltype, out Byte[] data)
        {
            try
            {
                return RegQueryValue(hive, key, regvalue, out outvaltype, out data);
            }
            catch
            {
                data = new Byte[0];
                outvaltype = REG_VALUE_TYPE.REG_NONE;
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, String key, String regvalue, out REG_VALUE_TYPE outvaltype, out Byte[] data)
        {
            try
            {
                RegistryType valtype;
                if (!helper.QueryValue(_hives[hive], key, regvalue, out valtype, out data))
                {
                    data = new byte[0];
                    outvaltype = REG_VALUE_TYPE.REG_NONE;
                    return REG_STATUS.FAILED;
                }
                outvaltype = _valtypes.FirstOrDefault(x => x.Value == valtype).Key;
                return REG_STATUS.SUCCESS;
            }
            catch
            {
                data = new byte[0];
                outvaltype = REG_VALUE_TYPE.REG_NONE;
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegSetDword(REG_HIVES hive, String key, String regvalue, UInt32 data)
        {
            try
            {
                return RegSetValue(hive, key, regvalue, REG_VALUE_TYPE.REG_DWORD, BitConverter.GetBytes(data));
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegSetMultiString(REG_HIVES hive, String key, String regvalue, [ReadOnlyArray] String[] data)
        {
            try
            {
                return RegSetValue(hive, key, regvalue, REG_VALUE_TYPE.REG_MULTI_SZ, Encoding.Unicode.GetBytes(string.Join("\0", data) + "\0\0"));
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegSetQword(REG_HIVES hive, String key, String regvalue, UInt64 data)
        {
            try
            {
                return RegSetValue(hive, key, regvalue, REG_VALUE_TYPE.REG_DWORD, BitConverter.GetBytes(data));
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegSetString(REG_HIVES hive, String key, String regvalue, String data)
        {
            try
            {
                // string must be null terminated, so do that ourselves.
                return RegSetValue(hive, key, regvalue, REG_VALUE_TYPE.REG_SZ, Encoding.Unicode.GetBytes(data + '\0'));
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegSetValue(REG_HIVES hive, String key, String regvalue, REG_VALUE_TYPE valtype, [ReadOnlyArray] Byte[] data)
        {
            try
            {
                var result = helper.WriteValue(_hives[hive], key, regvalue, data, _valtypes[valtype]);
                return result ? REG_STATUS.SUCCESS : REG_STATUS.FAILED;
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegSetVariableString(REG_HIVES hive, String key, String regvalue, String data)
        {
            try
            {
                // string must be null terminated, so do that ourselves.
                return RegSetValue(hive, key, regvalue, REG_VALUE_TYPE.REG_SZ, Encoding.Unicode.GetBytes(data + '\0'));
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

        public REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, [ReadOnlyArray] byte[] data)
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
                        list.Add(new REG_ITEM_CUSTOM { Name = subkey, Hive = (REG_HIVES)hive, Key = key, Type = REG_TYPE.KEY });
                    }
                }

                if (subvalues != null)
                {
                    foreach (var subval in subvalues)
                    {
                        var type = helper.GetValueInfo2(_hives[(REG_HIVES)hive], key, subval, 0);
                        
                        uint valtype;
                        String data = "";
                        try
                        {
                            CRegistryHelper helper = new CRegistryHelper();
                            helper.RegQueryValue((REG_HIVES)hive, key, subval, type, out valtype, out data);
                        }
                        catch
                        {

                        }

                        list.Add(new REG_ITEM_CUSTOM { Name = subval, Hive = (REG_HIVES)hive, Key = key, Type = REG_TYPE.VALUE, DataAsString = data, ValueType = type });
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

            items = new List<REG_ITEM_CUSTOM>();
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegLoadHive(string FilePath, string mountpoint, bool inUser)
        {
            try
            {
                var result = helper.LoadHive(FilePath, mountpoint, inUser);
                return result ? REG_STATUS.SUCCESS : REG_STATUS.FAILED;
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegUnloadHive(string mountpoint, bool inUser)
        {
            try
            {
                var result = helper.UnloadHive(mountpoint, inUser);
                return result ? REG_STATUS.SUCCESS : REG_STATUS.FAILED;
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
        }
    }
}
