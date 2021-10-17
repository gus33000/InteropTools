// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

#if ARM
using DevProgram;
using System.Linq;
using System.Text;
#endif

namespace RegistryHelper
{
    public sealed class DevProgramProvider : IRegistryProvider
    {
#if ARM
        private static readonly Dictionary<REG_HIVES, RegistryHive> _devproghives =
 new Dictionary<REG_HIVES, RegistryHive>
        {
            { REG_HIVES.HKEY_CLASSES_ROOT, RegistryHive.HKCR },
            { REG_HIVES.HKEY_CURRENT_USER, RegistryHive.HKCU },
            { REG_HIVES.HKEY_LOCAL_MACHINE, RegistryHive.HKLM },
            { REG_HIVES.HKEY_USERS, RegistryHive.HKU },
            { REG_HIVES.HKEY_PERFORMANCE_DATA, RegistryHive.HKPD },
            { REG_HIVES.HKEY_CURRENT_CONFIG, RegistryHive.HKCC },
            { REG_HIVES.HKEY_DYN_DATA, RegistryHive.HKDD },
            { REG_HIVES.HKEY_CURRENT_USER_LOCAL_SETTINGS, RegistryHive.HKCULS }
        };

        private static readonly Dictionary<REG_VALUE_TYPE, RegistryType> _devprogvaltypes =
 new Dictionary<REG_VALUE_TYPE, RegistryType>
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
            { REG_VALUE_TYPE.REG_FULL_RESOURCE_DESCRIPTOR, RegistryType.HardwareResourceLIst },
            { REG_VALUE_TYPE.REG_RESOURCE_REQUIREMENTS_LIST, RegistryType.ResourceRequirement },
            { REG_VALUE_TYPE.REG_QWORD, RegistryType.Long }
        };
#endif

        public bool IsSupported() => true;

        public REG_STATUS RegDeleteKey(REG_HIVES hive, string key, bool recursive)
        {
#if ARM
            bool didSucceed = DevProgramReg.DeleteKey(_devproghives[hive], key, recursive);
            if (didSucceed)
            {
                return REG_STATUS.SUCCESS;
            }
#endif
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegDeleteValue(REG_HIVES hive, string key, string name)
        {
#if ARM
            if (name == null)
            {
                name = "";
            }

            bool didSucceed = DevProgramReg.DeleteValue(_devproghives[hive], key, name);
            if (didSucceed)
            {
                return REG_STATUS.SUCCESS;
            }
#endif
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype,
            out REG_VALUE_TYPE outvaltype, out string data)
        {
            bool hadaccessdenied = false;
            bool hadfailed = false;

            REG_STATUS result = RegQueryValue(hive, key, regvalue, valtype, out REG_VALUE_TYPE valtypetmp,
                out byte[] datatmp);
            if (result == REG_STATUS.SUCCESS)
            {
                outvaltype = valtypetmp;
                data = Convert.RegBufferToString((uint)valtypetmp, datatmp);
                return result;
            }

            REG_STATUS? singleresult = null;

            switch (valtype)
            {
                case REG_VALUE_TYPE.REG_DWORD:
                    {
                        data = "";
                        outvaltype = valtype;
                        REG_STATUS result2 = RegQueryDword(hive, key, regvalue, out uint datatmp2);

                        singleresult = result2;
                        if (result2 == REG_STATUS.SUCCESS)
                        {
                            data = datatmp2.ToString();
                            return result2;
                        }

                        break;
                    }
                case REG_VALUE_TYPE.REG_QWORD:
                    {
                        data = "";
                        outvaltype = valtype;
                        REG_STATUS result2 = RegQueryQword(hive, key, regvalue, out ulong datatmp2);

                        singleresult = result2;
                        if (result2 == REG_STATUS.SUCCESS)
                        {
                            data = datatmp2.ToString();
                            return result2;
                        }

                        break;
                    }
                case REG_VALUE_TYPE.REG_MULTI_SZ:
                    {
                        data = "";
                        outvaltype = valtype;
                        REG_STATUS result2 = RegQueryMultiString(hive, key, regvalue, out string[] datatmp2);

                        singleresult = result2;
                        if (result2 == REG_STATUS.SUCCESS)
                        {
                            data = string.Join("\n", datatmp2);
                            return result2;
                        }

                        break;
                    }
                case REG_VALUE_TYPE.REG_SZ:
                    {
                        data = "";
                        outvaltype = valtype;
                        REG_STATUS result2 = RegQueryString(hive, key, regvalue, out string datatmp2);

                        singleresult = result2;
                        if (result2 == REG_STATUS.SUCCESS)
                        {
                            data = datatmp2;
                            return result2;
                        }

                        break;
                    }
                case REG_VALUE_TYPE.REG_EXPAND_SZ:
                    {
                        data = "";
                        outvaltype = valtype;
                        REG_STATUS result2 = RegQueryVariableString(hive, key, regvalue, out string datatmp2);

                        singleresult = result2;
                        if (result2 == REG_STATUS.SUCCESS)
                        {
                            data = datatmp2;
                            return result2;
                        }

                        break;
                    }
            }

            if (singleresult == REG_STATUS.NOT_IMPLEMENTED)
            {
            }

            if (singleresult == REG_STATUS.ACCESS_DENIED)
            {
                hadaccessdenied = true;
            }

            if (singleresult == REG_STATUS.FAILED)
            {
                hadfailed = true;
            }

            if (result == REG_STATUS.NOT_IMPLEMENTED)
            {
            }

            if (result == REG_STATUS.ACCESS_DENIED)
            {
                hadaccessdenied = true;
            }

            if (result == REG_STATUS.FAILED)
            {
                hadfailed = true;
            }

            if (hadaccessdenied)
            {
                outvaltype = REG_VALUE_TYPE.REG_NONE;
                data = "";
                return REG_STATUS.ACCESS_DENIED;
            }

            if (hadfailed)
            {
                outvaltype = REG_VALUE_TYPE.REG_NONE;
                data = "";
                return REG_STATUS.FAILED;
            }

            outvaltype = REG_VALUE_TYPE.REG_NONE;
            data = "";
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegEnumKey(REG_HIVES? hive, string key, out IReadOnlyList<REG_ITEM> items)
        {
            List<REG_ITEM> list = new();

            if (hive == null)
            {
                list.Add(new REG_ITEM
                {
                    Name = "HKEY_CLASSES_ROOT (HKCR)", Hive = REG_HIVES.HKEY_CLASSES_ROOT, Type = REG_TYPE.HIVE
                });
                list.Add(new REG_ITEM
                {
                    Name = "HKEY_CURRENT_CONFIG (HKCC)", Hive = REG_HIVES.HKEY_CURRENT_CONFIG, Type = REG_TYPE.HIVE
                });
                list.Add(new REG_ITEM
                {
                    Name = "HKEY_CURRENT_USER (HKCU)", Hive = REG_HIVES.HKEY_CURRENT_USER, Type = REG_TYPE.HIVE
                });
                list.Add(new REG_ITEM
                {
                    Name = "HKEY_CURRENT_USER_LOCAL_SETTINGS (HKCULS)",
                    Hive = REG_HIVES.HKEY_CURRENT_USER_LOCAL_SETTINGS,
                    Type = REG_TYPE.HIVE
                });
                list.Add(new REG_ITEM
                {
                    Name = "HKEY_DYN_DATA (HKDD)", Hive = REG_HIVES.HKEY_DYN_DATA, Type = REG_TYPE.HIVE
                });
                list.Add(new REG_ITEM
                {
                    Name = "HKEY_LOCAL_MACHINE (HKLM)", Hive = REG_HIVES.HKEY_LOCAL_MACHINE, Type = REG_TYPE.HIVE
                });
                list.Add(new REG_ITEM
                {
                    Name = "HKEY_PERFORMANCE_DATA (HKPD)",
                    Hive = REG_HIVES.HKEY_PERFORMANCE_DATA,
                    Type = REG_TYPE.HIVE
                });
                list.Add(new REG_ITEM {Name = "HKEY_USERS (HKU)", Hive = REG_HIVES.HKEY_USERS, Type = REG_TYPE.HIVE});

                items = list;
                return REG_STATUS.SUCCESS;
            }
#if ARM
            bool didSucceed = DevProgramReg.GetSubKeyNames(_devproghives[(REG_HIVES)hive], key, out string[] subkeys);

            if ((didSucceed) && (subkeys != null))
            {
                foreach (string subkey in subkeys)
                {
                    list.Add(new REG_ITEM { Name = subkey, Hive = (REG_HIVES)hive, Key = key, Type = REG_TYPE.KEY });
                }
            }

            bool didSucceed2 = DevProgramReg.GetValues(_devproghives[(REG_HIVES)hive], key, out ValueInfo[] values);

            if ((didSucceed2) && (values != null))
            {
                foreach (ValueInfo value in values)
                {
                    string data = "";
                    try
                    {
                        RegQueryValue((REG_HIVES)hive, key, value.Name, _devprogvaltypes.FirstOrDefault(x => x.Value == value.Type).Key, out REG_VALUE_TYPE valtype, out data);
                    }
                    catch
                    {
                    }
                    list.Add(new REG_ITEM { Name = value.Name, Hive = (REG_HIVES)hive, Key = key, Type =
 REG_TYPE.VALUE, DataAsString = data, ValueType = _devprogvaltypes.FirstOrDefault(x => x.Value == value.Type).Key });
                }
            }

            if (didSucceed || didSucceed2)
            {
                items = list;
                return REG_STATUS.SUCCESS;
            }
#endif

            items = new List<REG_ITEM>();
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegQueryMultiString(REG_HIVES hive, string key, string regvalue, out string[] data)
        {
#if ARM
            if (regvalue == null)
            {
                regvalue = "";
            }
            bool didSucceed = DevProgramReg.ReadMultiString(_devproghives[hive], key, regvalue, out data);
            if (didSucceed)
            {
                return REG_STATUS.SUCCESS;
            }
#endif
            data = null;
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegQueryVariableString(REG_HIVES hive, string key, string regvalue, out string data)
        {
#if ARM
            if (regvalue == null)
            {
                regvalue = "";
            }

            REG_STATUS result =
 RegQueryValue(hive, key, regvalue, REG_VALUE_TYPE.REG_EXPAND_SZ, out REG_VALUE_TYPE valtype, out byte[] datatmp);
            if (result == REG_STATUS.SUCCESS)
            {
                data = Encoding.Unicode.GetString(datatmp);
                return result;
            }
            data = "";
            return result;
#else
            data = "";
            return REG_STATUS.FAILED;
#endif
        }

        public REG_STATUS RegQueryDword(REG_HIVES hive, string key, string regvalue, out uint data)
        {
#if ARM
            if (regvalue == null)
            {
                regvalue = "";
            }

            bool didSucceed = DevProgramReg.ReadDWORD(_devproghives[hive], key, regvalue, out uint datatmp);
            if (didSucceed)
            {
                data = datatmp;
                return REG_STATUS.SUCCESS;
            }
#endif
            data = uint.MinValue;
            return REG_STATUS.FAILED;
        }

        public REG_KEY_STATUS RegQueryKeyStatus(REG_HIVES hive, string key)
        {
#if ARM
            if (!string.IsNullOrWhiteSpace(key))
            {
                DevProgramReg.GetValues(_devproghives[hive], key, out ValueInfo[] values);
                string errorcode = DevProgramReg.GetError().ToString();

                if (errorcode == "0")
                {
                    return REG_KEY_STATUS.FOUND;
                }
                else if (errorcode == "2")
                {
                    return REG_KEY_STATUS.NOT_FOUND;
                }
                else if (errorcode == "5")
                {
                    return REG_KEY_STATUS.ACCESS_DENIED;
                }
                else
                {
                    return REG_KEY_STATUS.UNKNOWN;
                }
            }
#endif
            return REG_KEY_STATUS.UNKNOWN;
        }

        public REG_STATUS RegQueryQword(REG_HIVES hive, string key, string regvalue, out ulong data)
        {
#if ARM
            if (regvalue == null)
            {
                regvalue = "";
            }

            bool didSucceed = DevProgramReg.ReadQWORD(_devproghives[hive], key, regvalue, out ulong datatmp);
            if (didSucceed)
            {
                data = datatmp;
                return REG_STATUS.SUCCESS;
            }
#endif
            data = ulong.MinValue;
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegQueryString(REG_HIVES hive, string key, string regvalue, out string data)
        {
#if ARM
            if (regvalue == null)
            {
                regvalue = "";
            }

            bool didSucceed = DevProgramReg.ReadString(_devproghives[hive], key, regvalue, out data);
            if (didSucceed)
            {
                return REG_STATUS.SUCCESS;
            }
#endif
            data = "";
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype,
            out REG_VALUE_TYPE outvaltype, out byte[] data)
        {
#if ARM
            if (regvalue == null)
            {
                regvalue = "";
            }

            bool didSucceed =
 DevProgramReg.QueryValue(_devproghives[hive], key, regvalue, out RegistryType type, out byte[] datatmp);
            if (didSucceed)
            {
                outvaltype = _devprogvaltypes.FirstOrDefault(x => x.Value == type).Key;
                data = datatmp;
                return REG_STATUS.SUCCESS;
            }
#endif
            outvaltype = REG_VALUE_TYPE.REG_NONE;
            data = new byte[0];
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegSetDword(REG_HIVES hive, string key, string regvalue, uint data)
        {
#if ARM
            if (regvalue == null)
            {
                regvalue = "";
            }

            bool didSucceed = DevProgramReg.WriteDWORD(_devproghives[hive], key, regvalue, data);
            if (didSucceed)
            {
                return REG_STATUS.SUCCESS;
            }
#endif
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegSetMultiString(REG_HIVES hive, string key, string regvalue, [ReadOnlyArray] string[] data)
        {
#if ARM
            if (regvalue == null)
            {
                regvalue = "";
            }

            bool didSucceed = DevProgramReg.WriteMultiString(_devproghives[hive], key, regvalue, data);
            if (didSucceed)
            {
                return REG_STATUS.SUCCESS;
            }
#endif
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegSetQword(REG_HIVES hive, string key, string regvalue, ulong data)
        {
#if ARM
            if (regvalue == null)
            {
                regvalue = "";
            }

            bool didSucceed = DevProgramReg.WriteQWORD(_devproghives[hive], key, regvalue, data);
            if (didSucceed)
            {
                return REG_STATUS.SUCCESS;
            }
#endif
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegSetString(REG_HIVES hive, string key, string regvalue, string data)
        {
#if ARM
            if (regvalue == null)
            {
                regvalue = "";
            }

            bool didSucceed = DevProgramReg.WriteString(_devproghives[hive], key, regvalue, data);
            if (didSucceed)
            {
                return REG_STATUS.SUCCESS;
            }
#endif
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype,
            [ReadOnlyArray] byte[] data)
        {
#if ARM
            if (regvalue == null)
            {
                regvalue = "";
            }

            bool didSucceed =
 DevProgramReg.SetValue(_devproghives[hive], key, regvalue, _devprogvaltypes[valtype], data);
            if (didSucceed)
            {
                return REG_STATUS.SUCCESS;
            }
#endif
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegSetVariableString(REG_HIVES hive, string key, string regvalue, string data)
        {
#if ARM
            if (regvalue == null)
            {
                regvalue = "";
            }

            REG_STATUS result =
 RegSetValue(hive, key, regvalue, REG_VALUE_TYPE.REG_QWORD, Encoding.Unicode.GetBytes(data));
            return result;
#else
            return REG_STATUS.FAILED;
#endif
        }

        public REG_STATUS RegRenameKey(REG_HIVES hive, string key, string newname)
        {
#if ARM
            bool didSucceed = DevProgramReg.RenameKey(_devproghives[hive], key, newname);
            if (didSucceed)
            {
                return REG_STATUS.SUCCESS;
            }
#endif
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegAddKey(REG_HIVES hive, string key)
        {
#if ARM
            bool didSucceed = DevProgramReg.CreateKey(_devproghives[hive], key);
            if (didSucceed)
            {
                return REG_STATUS.SUCCESS;
            }
#endif
            return REG_STATUS.FAILED;
        }

        public REG_STATUS RegQueryKeyLastModifiedTime(REG_HIVES hive, string key, out long lastmodified)
        {
            lastmodified = long.MinValue;
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, uint valtype, out uint outvaltype,
            out byte[] data)
        {
            outvaltype = 0;
            data = new byte[0];
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype,
            [ReadOnlyArray] byte[] data) => REG_STATUS.NOT_IMPLEMENTED;

        public REG_STATUS RegEnumKey(REG_HIVES? hive, string key, out IReadOnlyList<REG_ITEM_CUSTOM> items)
        {
            items = new List<REG_ITEM_CUSTOM>();
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegLoadHive(string FilePath, string mountpoint, bool inUser) => REG_STATUS.NOT_IMPLEMENTED;

        public REG_STATUS RegUnloadHive(string mountpoint, bool inUser) => REG_STATUS.NOT_IMPLEMENTED;
    }
}
