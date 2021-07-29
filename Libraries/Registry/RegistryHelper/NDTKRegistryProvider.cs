using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;

#if ARM
using ndtklib;
#endif

namespace RegistryHelper
{
    public sealed class NDTKRegistryProvider : IRegistryProvider
    {
#if ARM
        private NRPC _nrpc;
#endif

        private static readonly Dictionary<REG_HIVES, uint> _ndtkhives = new()
        {
            { REG_HIVES.HKEY_CLASSES_ROOT, 0 },
            { REG_HIVES.HKEY_CURRENT_USER, 2 },
            { REG_HIVES.HKEY_LOCAL_MACHINE, 1 },
            { REG_HIVES.HKEY_USERS, 4 },
            { REG_HIVES.HKEY_PERFORMANCE_DATA, 5 },
            { REG_HIVES.HKEY_CURRENT_CONFIG, 3 },
            { REG_HIVES.HKEY_DYN_DATA, 6 },
            { REG_HIVES.HKEY_CURRENT_USER_LOCAL_SETTINGS, 7 }
        };

        private static readonly Dictionary<REG_VALUE_TYPE, uint> _ndtkvaltypes = new()
        {
            { REG_VALUE_TYPE.REG_NONE, 0 },
            { REG_VALUE_TYPE.REG_SZ, 1 },
            { REG_VALUE_TYPE.REG_EXPAND_SZ, 2 },
            { REG_VALUE_TYPE.REG_BINARY, 3 },
            { REG_VALUE_TYPE.REG_DWORD, 4 },
            { REG_VALUE_TYPE.REG_DWORD_BIG_ENDIAN, 5 },
            { REG_VALUE_TYPE.REG_LINK, 6 },
            { REG_VALUE_TYPE.REG_MULTI_SZ, 7 },
            { REG_VALUE_TYPE.REG_RESOURCE_LIST, 8 },
            { REG_VALUE_TYPE.REG_FULL_RESOURCE_DESCRIPTOR, 9 },
            { REG_VALUE_TYPE.REG_RESOURCE_REQUIREMENTS_LIST, 10 },
            { REG_VALUE_TYPE.REG_QWORD, 11 }
        };

        private bool Initialize()
        {
#if ARM
            if (_nrpc != null)
            {
                return true;
            }

            try
            {
                _nrpc = new NRPC();
                uint ret = _nrpc.Initialize();
                return ret == 0;
            }
            catch
            {
                _nrpc = null;
                return false;
            }
#else
            return false;
#endif
        }

        public bool IsSupported()
        {
            return Initialize();
        }

        public REG_STATUS RegDeleteKey(REG_HIVES hive, string key, bool recursive)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegDeleteValue(REG_HIVES hive, string key, string name)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegEnumKey(REG_HIVES? hive, string key, out IReadOnlyList<REG_ITEM> items)
        {
            items = new List<REG_ITEM>();
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryMultiString(REG_HIVES hive, string key, string regvalue, out string[] data)
        {
            try
            {
                REG_STATUS result = RegQueryValue(hive, key, regvalue, REG_VALUE_TYPE.REG_MULTI_SZ, out REG_VALUE_TYPE regtype, out byte[] buffer);
                if (result != REG_STATUS.SUCCESS)
                {
                    data = new string[0];
                    return result;
                }
                string strNullTerminated = Encoding.Unicode.GetString(buffer);
                if (strNullTerminated.Substring(strNullTerminated.Length - 3) == "\0\0")
                {
                    // The REG_MULTI_SZ is properly terminated.
                    // Remove the array terminator, and the final string terminator.
                    strNullTerminated = strNullTerminated.Substring(0, strNullTerminated.Length - 2);
                }
                else if (strNullTerminated.Substring(strNullTerminated.Length - 2) == "\0")
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
                data = new string[0];
                return REG_STATUS.FAILED;
            }
        }

        public REG_STATUS RegQueryVariableString(REG_HIVES hive, string key, string regvalue, out string data)
        {
            try
            {
                REG_STATUS result = RegQueryValue(hive, key, regvalue, REG_VALUE_TYPE.REG_EXPAND_SZ, out REG_VALUE_TYPE regtype, out byte[] buffer);
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

        public REG_STATUS RegQueryDword(REG_HIVES hive, string key, string regvalue, out uint data)
        {
            try
            {
                REG_STATUS result = RegQueryValue(hive, key, regvalue, REG_VALUE_TYPE.REG_DWORD, out REG_VALUE_TYPE regtype, out byte[] buffer);
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

        public REG_KEY_STATUS RegQueryKeyStatus(REG_HIVES hive, string key)
        {
            return REG_KEY_STATUS.UNKNOWN;
        }

        public REG_STATUS RegQueryQword(REG_HIVES hive, string key, string regvalue, out ulong data)
        {
            try
            {
                REG_STATUS result = RegQueryValue(hive, key, regvalue, REG_VALUE_TYPE.REG_QWORD, out REG_VALUE_TYPE regtype, out byte[] buffer);
                if (result != REG_STATUS.SUCCESS)
                {
                    data = ulong.MinValue;
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

        public REG_STATUS RegQueryString(REG_HIVES hive, string key, string regvalue, out string data)
        {
            try
            {
                REG_STATUS result = RegQueryValue(hive, key, regvalue, REG_VALUE_TYPE.REG_SZ, out REG_VALUE_TYPE regtype, out byte[] buffer);
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

        public REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype, out REG_VALUE_TYPE outvaltype, out byte[] data)
        {
#if !ARM
            data = new byte[0];
            outvaltype = REG_VALUE_TYPE.REG_NONE;
            return REG_STATUS.FAILED;
#else
            try
            {
                bool res = Initialize();
                if (!res)
                {
                    data = new byte[0];
                    outvaltype = REG_VALUE_TYPE.REG_NONE;
                    return REG_STATUS.FAILED;
                }

                int length = 0;
                byte[] buffer;
                uint returncode;
                uint dataType = _ndtkvaltypes[valtype];

                do
                {
                    length += 1;
                    buffer = new byte[length];
                    try
                    {
                        returncode = _nrpc.RegQueryValueW(_ndtkhives[hive], key, regvalue, dataType, buffer);
                    }
                    catch (Exception e)
                    {
                        returncode = (uint)e.HResult;
                        if (returncode != 0x800700ea)
                        {
                            // rethrow if not ERROR_MORE_DATA
                            data = new byte[0];
                            outvaltype = REG_VALUE_TYPE.REG_NONE;
                            return REG_STATUS.FAILED;
                        }
                    }
                    // throw if an error occured that's not ERROR_MORE_DATA
                    if ((returncode != 0) && (returncode != 0x800700ea))
                    {
                        data = new byte[0];
                        outvaltype = REG_VALUE_TYPE.REG_NONE;
                        return REG_STATUS.FAILED;
                    }
                } while (returncode == 0x800700ea);

                data = buffer;
                outvaltype = _ndtkvaltypes.FirstOrDefault(x => x.Value == dataType).Key;

                return REG_STATUS.SUCCESS;
            }
            catch
            {
                data = new byte[0];
                outvaltype = REG_VALUE_TYPE.REG_NONE;
                return REG_STATUS.FAILED;
            }
#endif
        }

        public REG_STATUS RegSetDword(REG_HIVES hive, string key, string regvalue, uint data)
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

        public REG_STATUS RegSetMultiString(REG_HIVES hive, string key, string regvalue, [ReadOnlyArray] string[] data)
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

        public REG_STATUS RegSetQword(REG_HIVES hive, string key, string regvalue, ulong data)
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

        public REG_STATUS RegSetString(REG_HIVES hive, string key, string regvalue, string data)
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

        public REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype, [ReadOnlyArray] byte[] data)
        {
#if !ARM
            return REG_STATUS.FAILED;
#else
            try
            {
                bool res = Initialize();
                if (!res)
                {
                    return REG_STATUS.FAILED;
                }

                try
                {
                    uint returncode = _nrpc.RegSetValueW(_ndtkhives[hive], key, regvalue, _ndtkvaltypes[valtype], data);
                    if (returncode != 0)
                    {
                        return REG_STATUS.FAILED;
                    }
                }
                catch
                {
                }
                return REG_STATUS.SUCCESS;
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
#endif
        }

        public REG_STATUS RegSetVariableString(REG_HIVES hive, string key, string regvalue, string data)
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

        public REG_STATUS RegRenameKey(REG_HIVES hive, string key, string newname)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegAddKey(REG_HIVES hive, string key)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryKeyLastModifiedTime(REG_HIVES hive, string key, out long lastmodified)
        {
            lastmodified = long.MinValue;
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, uint valtype, out uint outvaltype, out byte[] data)
        {
#if !ARM
            data = new byte[0];
            outvaltype = 0;
            return REG_STATUS.FAILED;
#else
            try
            {
                bool res = Initialize();
                if (!res)
                {
                    data = new byte[0];
                    outvaltype = 0;
                    return REG_STATUS.FAILED;
                }

                int length = 0;
                byte[] buffer;
                uint returncode;
                uint dataType = valtype;

                do
                {
                    length += 1;
                    buffer = new byte[length];
                    try
                    {
                        returncode = _nrpc.RegQueryValueW(_ndtkhives[hive], key, regvalue, dataType, buffer);
                    }
                    catch (Exception e)
                    {
                        returncode = (uint)e.HResult;
                        if (returncode != 0x800700ea)
                        {
                            // rethrow if not ERROR_MORE_DATA
                            data = new byte[0];
                            outvaltype = 0;
                            return REG_STATUS.FAILED;
                        }
                    }
                    // throw if an error occured that's not ERROR_MORE_DATA
                    if ((returncode != 0) && (returncode != 0x800700ea))
                    {
                        data = new byte[0];
                        outvaltype = 0;
                        return REG_STATUS.FAILED;
                    }
                } while (returncode == 0x800700ea);

                data = buffer;
                outvaltype = dataType;

                return REG_STATUS.SUCCESS;
            }
            catch
            {
                data = new byte[0];
                outvaltype = 0;
                return REG_STATUS.FAILED;
            }
#endif
        }

        public REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, [ReadOnlyArray] byte[] data)
        {
#if !ARM
            return REG_STATUS.FAILED;
#else
            try
            {
                bool res = Initialize();
                if (!res)
                {
                    return REG_STATUS.FAILED;
                }

                try
                {
                    uint returncode = _nrpc.RegSetValueW(_ndtkhives[hive], key, regvalue, valtype, data);
                    if (returncode != 0)
                    {
                        return REG_STATUS.FAILED;
                    }
                }
                catch
                {
                }
                return REG_STATUS.SUCCESS;
            }
            catch
            {
                return REG_STATUS.FAILED;
            }
#endif
        }

        public REG_STATUS RegEnumKey(REG_HIVES? hive, string key, out IReadOnlyList<REG_ITEM_CUSTOM> items)
        {
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