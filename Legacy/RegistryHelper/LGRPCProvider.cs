using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Security.ExchangeActiveSyncProvisioning;
#if ARM
using LGRuntimeComponent;
#endif

namespace RegistryHelper
{
    public sealed class LGRPCProvider : IRegistryProvider
    {
#if ARM
        RegistryWrapper _lgrpcreg;
        RpcWrapper _lgrpc;
#endif

        public Boolean IsSupported()
        {
            return Initialize();
        }

        private bool Initialize()
        {
#if ARM
            if (_lgrpc != null)
            {
                return true;
            }

            var deviceInformation = new EasClientDeviceInformation();
            string SystemManufacturer = deviceInformation.SystemManufacturer;

            if (SystemManufacturer.ToLower().Contains("lg"))
            {
                try
                {
                    _lgrpc = RpcWrapper.GetRpcWrapper();
                    _lgrpcreg = new RegistryWrapper();
                    return true;
                }
                catch
                {
                    _lgrpc = null;
                    _lgrpcreg = null;
                    return false;
                }
            }
#endif
            return false;
        }

        public REG_STATUS RegDeleteKey(REG_HIVES hive, String key, bool recursive)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegDeleteValue(REG_HIVES hive, String key, String name)
        {
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegEnumKey(REG_HIVES? hive, String key, out IReadOnlyList<REG_ITEM> items)
        {
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
            try
            {
                var res = Initialize();
                if (!res)
                {
                    data = uint.MinValue;
                    return REG_STATUS.FAILED;
                }

                if (hive == REG_HIVES.HKEY_LOCAL_MACHINE)
                {
                    data = BitConverter.ToUInt32(BitConverter.GetBytes(_lgrpcreg.GetRegistryDWORDValue(key, regvalue)), 0);
                    return REG_STATUS.SUCCESS;
                }
            }
            catch
            {

            }
#endif
            data = uint.MinValue;
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_KEY_STATUS RegQueryKeyStatus(REG_HIVES hive, String key)
        {
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
            try
            {
                var res = Initialize();
                if (!res)
                {
                    data = "";
                    return REG_STATUS.FAILED;
                }

                if (hive == REG_HIVES.HKEY_LOCAL_MACHINE)
                {
                    data = _lgrpcreg.GetRegistryValue(key, regvalue);
                    return REG_STATUS.SUCCESS;
                }
            }
            catch
            {

            }
#endif
            data = "";
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegQueryValue(REG_HIVES hive, String key, String regvalue, REG_VALUE_TYPE valtype, out REG_VALUE_TYPE outvaltype, out Byte[] data)
        {
            data = new byte[0];
            outvaltype = REG_VALUE_TYPE.REG_NONE;
            return REG_STATUS.NOT_IMPLEMENTED;
        }

        public REG_STATUS RegSetDword(REG_HIVES hive, String key, String regvalue, UInt32 data)
        {
#if ARM
            try
            {
                var res = Initialize();
                if (!res)
                {
                    return REG_STATUS.FAILED;
                }

                if (hive == REG_HIVES.HKEY_LOCAL_MACHINE)
                {
                    _lgrpc.RpcSetRegistryDWORDValue(key, regvalue, BitConverter.ToInt32(BitConverter.GetBytes(data), 0));
                    return REG_STATUS.SUCCESS;
                }
            }
            catch
            {

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
            try
            {
                var res = Initialize();
                if (!res)
                {
                    return REG_STATUS.FAILED;
                }

                if (hive == REG_HIVES.HKEY_LOCAL_MACHINE)
                {
                    _lgrpcreg.AddRegKey(key, regvalue, data);
                    return REG_STATUS.SUCCESS;
                }
            }
            catch
            {

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
