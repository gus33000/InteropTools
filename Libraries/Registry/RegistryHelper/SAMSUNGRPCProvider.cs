using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

#if ARM
using RPCComponent;
#endif

namespace RegistryHelper
{
    public sealed class SAMSUNGRPCProvider : IRegistryProvider
    {
        private bool _initialized;

        private static readonly Dictionary<REG_HIVES, uint> _srpchives = new()
        {
            { REG_HIVES.HKEY_CLASSES_ROOT, 2147483648 },
            { REG_HIVES.HKEY_CURRENT_USER, 2147483649 },
            { REG_HIVES.HKEY_LOCAL_MACHINE, 2147483650 },
            { REG_HIVES.HKEY_USERS, 2147483651 },
            { REG_HIVES.HKEY_PERFORMANCE_DATA, 2147483652 },
            { REG_HIVES.HKEY_CURRENT_CONFIG, 2147483653 },
            { REG_HIVES.HKEY_DYN_DATA, 2147483654 },
            { REG_HIVES.HKEY_CURRENT_USER_LOCAL_SETTINGS, 2147483655 }
        };

        public bool IsSupported()
        {
            return Initialize();
        }

        private bool Initialize()
        {
#if ARM
            if (_initialized)
            {
                return true;
            }

            EasClientDeviceInformation deviceInformation = new EasClientDeviceInformation();
            string SystemManufacturer = deviceInformation.SystemManufacturer;

            if (SystemManufacturer.ToLower().Contains("samsung"))
            {
                try
                {
                    CRPCComponent.Initialize();

                    _initialized = true;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
#endif
            return false;
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
            data = new string[0];
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryVariableString(REG_HIVES hive, string key, string regvalue, out string data)
        {
            data = "";
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryDword(REG_HIVES hive, string key, string regvalue, out uint data)
        {
#if ARM
            try
            {
                bool res = Initialize();
                if (!res)
                {
                    data = uint.MinValue;
                    return REG_STATUS.FAILED;
                }

                CRPCComponent.Registry_GetDWORD(_srpchives[hive], key, regvalue, out uint udata);
                data = udata;
                return REG_STATUS.SUCCESS;
            }
            catch
            {
            }
#endif
            data = uint.MinValue;
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_KEY_STATUS RegQueryKeyStatus(REG_HIVES hive, string key)
        {
            return REG_KEY_STATUS.UNKNOWN;
        }

        public REG_STATUS RegQueryQword(REG_HIVES hive, string key, string regvalue, out ulong data)
        {
            data = ulong.MinValue;
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryString(REG_HIVES hive, string key, string regvalue, out string data)
        {
#if ARM
            try
            {
                bool res = Initialize();
                if (!res)
                {
                    data = "";
                    return REG_STATUS.FAILED;
                }

                data = CRPCComponent.Registry_GetString(_srpchives[hive], key, regvalue, out uint udata);
                return REG_STATUS.SUCCESS;
            }
            catch
            {
            }
#endif
            data = "";
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype, out REG_VALUE_TYPE outvaltype, out byte[] data)
        {
            data = new byte[0];
            outvaltype = REG_VALUE_TYPE.REG_NONE;
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetDword(REG_HIVES hive, string key, string regvalue, uint data)
        {
#if ARM
            try
            {
                bool res = Initialize();
                if (!res)
                {
                    return REG_STATUS.FAILED;
                }

                CRPCComponent.Registry_SetDWORD(_srpchives[hive], key, regvalue, data, out uint udata);
                return REG_STATUS.SUCCESS;
            }
            catch
            {
            }
#endif
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetMultiString(REG_HIVES hive, string key, string regvalue, [ReadOnlyArray] string[] data)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetQword(REG_HIVES hive, string key, string regvalue, ulong data)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetString(REG_HIVES hive, string key, string regvalue, string data)
        {
#if ARM
            try
            {
                bool res = Initialize();
                if (!res)
                {
                    return REG_STATUS.FAILED;
                }

                CRPCComponent.Registry_SetString(_srpchives[hive], key, regvalue, data, out uint udata);
                return REG_STATUS.SUCCESS;
            }
            catch
            {
            }
#endif
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype, [ReadOnlyArray] byte[] data)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetVariableString(REG_HIVES hive, string key, string regvalue, string data)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
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