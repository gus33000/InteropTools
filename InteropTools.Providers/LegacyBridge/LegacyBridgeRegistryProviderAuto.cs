using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace InteropTools.Providers
{
    public class LegacyBridgeRegistryProviderAuto : IRegistryProvider
    {
        public IRegProvider provider;

        bool initialized = false;

        public RegHives ConvertToOldHive(REG_HIVES hive)
        {
            return (RegHives)Enum.Parse(typeof(RegHives), hive.ToString());
        }

        public REG_HIVES ConvertToNewHive(RegHives hive)
        {
            return (REG_HIVES)Enum.Parse(typeof(REG_HIVES), hive.ToString());
        }

        public RegTypes ConvertToOldValType(REG_VALUE_TYPE type)
        {
            return (RegTypes)Enum.Parse(typeof(RegTypes), type.ToString());
        }

        public REG_TYPE ConvertToNewValType(RegistryItemType type)
        {
            return (REG_TYPE)Enum.Parse(typeof(REG_TYPE), type.ToString());
        }

        public RegistryItemType ConvertToOldType(REG_TYPE type)
        {
            return (RegistryItemType)Enum.Parse(typeof(RegistryItemType), type.ToString());
        }

        public REG_VALUE_TYPE ConvertToNewType(RegTypes type)
        {
            return (REG_VALUE_TYPE)Enum.Parse(typeof(REG_VALUE_TYPE), type.ToString());
        }

        public HelperErrorCodes ConvertToOldStatus(REG_STATUS status)
        {
            switch (status)
            {
                case REG_STATUS.ACCESS_DENIED:
                    return HelperErrorCodes.ACCESS_DENIED;
                case REG_STATUS.FAILED:
                    return HelperErrorCodes.FAILED;
                case REG_STATUS.NOT_SUPPORTED:
                    return HelperErrorCodes.NOT_IMPLEMENTED;
                case REG_STATUS.SUCCESS:
                    return HelperErrorCodes.SUCCESS;
            }
            return HelperErrorCodes.NOT_IMPLEMENTED;
        }

        public KeyStatus ConvertToOldKeyStatus(REG_KEY_STATUS status)
        {
            return (KeyStatus)Enum.Parse(typeof(KeyStatus), status.ToString());
        }

        public async Task<HelperErrorCodes> AddKey(RegHives hive, string key)
        {
            bool hadaccessdenied = false;
            bool hadfailed = false;

            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                var result = await provider.RegAddKey(ConvertToNewHive(hive), key);

                if (result == REG_STATUS.SUCCESS)
                {
                    reglist.Dispose();
                    return ConvertToOldStatus(result);
                }

                if (result == REG_STATUS.NOT_SUPPORTED)
                {
                    continue;
                }

                if (result == REG_STATUS.ACCESS_DENIED)
                {
                    hadaccessdenied = true;
                    continue;
                }

                if (result == REG_STATUS.FAILED)
                {
                    hadfailed = true;
                    continue;
                }
            }

            reglist.Dispose();

            if (hadaccessdenied)
            {
                return HelperErrorCodes.ACCESS_DENIED;
            }

            if (hadfailed)
            {
                return HelperErrorCodes.FAILED;
            }

            return HelperErrorCodes.NOT_IMPLEMENTED;
        }

        public bool AllowsRegistryEditing()
        {
            return true;
        }

        public async Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive)
        {
            bool hadaccessdenied = false;
            bool hadfailed = false;

            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                var result = await provider.RegDeleteKey(ConvertToNewHive(hive), key, recursive);

                if (result == REG_STATUS.SUCCESS)
                {
                    reglist.Dispose();
                    return ConvertToOldStatus(result);
                }

                if (result == REG_STATUS.NOT_SUPPORTED)
                {
                    continue;
                }

                if (result == REG_STATUS.ACCESS_DENIED)
                {
                    hadaccessdenied = true;
                    continue;
                }

                if (result == REG_STATUS.FAILED)
                {
                    hadfailed = true;
                    continue;
                }
            }

            reglist.Dispose();

            if (hadaccessdenied)
            {
                return HelperErrorCodes.ACCESS_DENIED;
            }

            if (hadfailed)
            {
                return HelperErrorCodes.FAILED;
            }

            return HelperErrorCodes.NOT_IMPLEMENTED;
        }

        public async Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue)
        {
            bool hadaccessdenied = false;
            bool hadfailed = false;

            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                var result = await provider.RegDeleteValue(ConvertToNewHive(hive), key, keyvalue);

                if (result == REG_STATUS.SUCCESS)
                {
                    reglist.Dispose();
                    return ConvertToOldStatus(result);
                }

                if (result == REG_STATUS.NOT_SUPPORTED)
                {
                    continue;
                }

                if (result == REG_STATUS.ACCESS_DENIED)
                {
                    hadaccessdenied = true;
                    continue;
                }

                if (result == REG_STATUS.FAILED)
                {
                    hadfailed = true;
                    continue;
                }
            }

            reglist.Dispose();

            if (hadaccessdenied)
            {
                return HelperErrorCodes.ACCESS_DENIED;
            }

            if (hadfailed)
            {
                return HelperErrorCodes.FAILED;
            }

            return HelperErrorCodes.NOT_IMPLEMENTED;
        }

        public bool DoesFileExists(string path)
        {
            bool fileexists = false;

            try
            {
                fileexists = File.Exists(path);
            }

            catch (InvalidOperationException)
            {
                fileexists = true;
            }

            return fileexists;
        }

        public string GetAppInstallationPath()
        {
            return Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
        }

        public string GetDescription()
        {
            return "This device (through provider extensions)";
        }

        public string GetFriendlyName()
        {
            return "This device (through provider extensions)";
        }

        public string GetHostName()
        {
            return "127.0.0.1";
        }

        public async Task<GetKeyLastModifiedTime> GetKeyLastModifiedTime(RegHives hive, string key)
        {
            var ret = new GetKeyLastModifiedTime();
            bool hadaccessdenied = false;
            bool hadfailed = false;

            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                long modified;
                var result = await provider.RegQueryKeyLastModifiedTime(ConvertToNewHive(hive), key);

                modified = result.LastModified;

                ret.LastModified = new DateTime(modified);

                if (result.returncode == REG_STATUS.SUCCESS)
                {
                    reglist.Dispose();
                    ret.returncode = ConvertToOldStatus(result.returncode);
                    return ret;
                }

                if (result.returncode == REG_STATUS.NOT_SUPPORTED)
                {
                    continue;
                }

                if (result.returncode == REG_STATUS.ACCESS_DENIED)
                {
                    hadaccessdenied = true;
                    continue;
                }

                if (result.returncode == REG_STATUS.FAILED)
                {
                    hadfailed = true;
                    continue;
                }
            }

            reglist.Dispose();

            if (hadaccessdenied)
            {
                ret.LastModified = new DateTime();
                ret.returncode = HelperErrorCodes.ACCESS_DENIED;
                return ret;
            }

            if (hadfailed)
            {
                ret.LastModified = new DateTime();
                ret.returncode = HelperErrorCodes.FAILED;
                return ret;
            }

            ret.LastModified = new DateTime();
            ret.returncode = HelperErrorCodes.NOT_IMPLEMENTED;
            return ret;
        }

        public async Task<KeyStatus> GetKeyStatus(RegHives hive, string key)
        {
            bool hadaccessdenied = false;
            bool hadfailed = false;

            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                var result = await provider.RegQueryKeyStatus(ConvertToNewHive(hive), key);

                if (result == REG_KEY_STATUS.FOUND)
                {
                    reglist.Dispose();
                    return ConvertToOldKeyStatus(result);
                }

                if (result == REG_KEY_STATUS.UNKNOWN)
                {
                    continue;
                }

                if (result == REG_KEY_STATUS.ACCESS_DENIED)
                {
                    hadaccessdenied = true;
                    continue;
                }

                if (result == REG_KEY_STATUS.NOT_FOUND)
                {
                    hadfailed = true;
                    continue;
                }
            }

            reglist.Dispose();

            if (hadaccessdenied)
            {
                return KeyStatus.ACCESS_DENIED;
            }

            if (hadfailed)
            {
                return KeyStatus.NOT_FOUND;
            }

            return KeyStatus.UNKNOWN;
        }

        public async Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type)
        {
            var ret = new GetKeyValueReturn();
            bool hadaccessdenied = false;
            bool hadfailed = false;

            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                Debug.WriteLine(plugin.UniqueId);

                REG_VALUE_TYPE newregtype;
                string regvalue;
                var result = await provider.RegQueryValue(ConvertToNewHive(hive), key, keyvalue, ConvertToNewType(type));

                regvalue = result.regvalue;
                newregtype = result.regtype;

                ret.regvalue = regvalue;

                ret.regtype = ConvertToOldValType(newregtype);

                if (result.returncode == REG_STATUS.SUCCESS)
                {
                    reglist.Dispose();
                    ret.returncode = ConvertToOldStatus(result.returncode);
                    return ret;
                }

                if (result.returncode == REG_STATUS.NOT_SUPPORTED)
                {
                    continue;
                }

                if (result.returncode == REG_STATUS.ACCESS_DENIED)
                {
                    hadaccessdenied = true;
                    continue;
                }

                if (result.returncode == REG_STATUS.FAILED)
                {
                    hadfailed = true;
                    continue;
                }
            }

            reglist.Dispose();

            if (hadaccessdenied)
            {
                ret.regtype = RegTypes.REG_ERROR;
                ret.regvalue = "";
                ret.returncode = HelperErrorCodes.ACCESS_DENIED;
                return ret;
            }

            if (hadfailed)
            {
                ret.regtype = RegTypes.REG_ERROR;
                ret.regvalue = "";
                ret.returncode = HelperErrorCodes.FAILED;
                return ret;
            }

            ret.regtype = RegTypes.REG_ERROR;
            ret.regvalue = "";
            ret.returncode = HelperErrorCodes.NOT_IMPLEMENTED;
            return ret;
        }

        public async Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type)
        {
            var ret = new GetKeyValueReturn2();
            bool hadaccessdenied = false;
            bool hadfailed = false;

            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                uint regtype;
                string regvalue;
                var result = await provider.RegQueryValue(ConvertToNewHive(hive), key, keyvalue, type);

                regtype = result.regtype;
                regvalue = result.regvalue;

                ret.regvalue = regvalue;

                ret.regtype = regtype;

                if (result.returncode == REG_STATUS.SUCCESS)
                {
                    reglist.Dispose();
                    ret.returncode = ConvertToOldStatus(result.returncode);
                    return ret;
                }

                if (result.returncode == REG_STATUS.NOT_SUPPORTED)
                {
                    continue;
                }

                if (result.returncode == REG_STATUS.ACCESS_DENIED)
                {
                    hadaccessdenied = true;
                    continue;
                }

                if (result.returncode == REG_STATUS.FAILED)
                {
                    hadfailed = true;
                    continue;
                }
            }

            reglist.Dispose();

            if (hadaccessdenied)
            {
                ret.regtype = 0;
                ret.regvalue = "";
                ret.returncode = HelperErrorCodes.ACCESS_DENIED;
                return ret;
            }

            if (hadfailed)
            {
                ret.regtype = 0;
                ret.regvalue = "";
                ret.returncode = HelperErrorCodes.FAILED;
                return ret;
            }

            ret.regtype = 0;
            ret.regvalue = "";
            ret.returncode = HelperErrorCodes.NOT_IMPLEMENTED;
            return ret;
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2()
        {
            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                IReadOnlyList<REG_ITEM> items;
                var result = await provider.RegEnumKey(null, "");

                items = result.items;

                if (result.returncode == REG_STATUS.SUCCESS)
                {
                    reglist.Dispose();
                    return ConvertFromNewToOldListItems(items);
                }
            }

            reglist.Dispose();

            return new List<RegistryItemCustom>();
        }

        public string RegBufferToString(uint valtype, byte[] data)
        {
            switch (valtype)
            {
                case (uint)REG_VALUE_TYPE.REG_DWORD:
                    {
                        return data.Length == 0 ? "" : BitConverter.ToUInt32(data, 0).ToString();
                    }
                case (uint)REG_VALUE_TYPE.REG_QWORD:
                    {
                        return data.Length == 0 ? "" : BitConverter.ToUInt64(data, 0).ToString();
                    }
                case (uint)REG_VALUE_TYPE.REG_MULTI_SZ:
                    {
                        string strNullTerminated = Encoding.Unicode.GetString(data);
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
                        return String.Join("\n", strNullTerminated.Split('\0'));
                    }
                case (uint)REG_VALUE_TYPE.REG_SZ:
                    {
                        return Encoding.Unicode.GetString(data).TrimEnd('\0');
                    }
                case (uint)REG_VALUE_TYPE.REG_EXPAND_SZ:
                    {
                        return Encoding.Unicode.GetString(data).TrimEnd('\0');
                    }
                default:
                    {
                        return ByteArrayToHexViaLookup32(data);
                    }
            }
        }


        private static readonly uint[] _lookup32 = CreateLookup32();

        private static uint[] CreateLookup32()
        {
            var result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }
            return result;
        }

        private static string ByteArrayToHexViaLookup32(byte[] bytes)
        {
            try
            {
                var lookup32 = _lookup32;
                var result = new char[bytes.Length * 2];
                for (int i = 0; i < bytes.Length; i++)
                {
                    var val = lookup32[bytes[i]];
                    result[2 * i] = (char)val;
                    result[2 * i + 1] = (char)(val >> 16);
                }
                return new string(result);
            }
            catch
            {
                return "";
            }
        }



        private IReadOnlyList<RegistryItemCustom> ConvertFromNewToOldListItems(IReadOnlyList<REG_ITEM> items)
        {
            List<RegistryItemCustom> itemlist = new List<RegistryItemCustom>();

            foreach (var item in items)
            {
                var itm = new RegistryItemCustom();
                itm.Hive = item.Hive.HasValue ? ConvertToOldHive(item.Hive.Value) : RegHives.HKEY_LOCAL_MACHINE;
                itm.Key = item.Key;
                itm.Name = item.Name;
                itm.Type = item.Type.HasValue ? ConvertToOldType(item.Type.Value) : RegistryItemType.HIVE;
                itm.Value = RegBufferToString(item.ValueType.HasValue ? item.ValueType.Value : 0, item.Data);
                itm.ValueType = item.ValueType.HasValue ? item.ValueType.Value : 0;

                itemlist.Add(itm);
            }

            return itemlist;
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key)
        {
            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                IReadOnlyList<REG_ITEM> items;
                var result = await provider.RegEnumKey(ConvertToNewHive(hive), key);

                items = result.items;

                if (result.returncode == REG_STATUS.SUCCESS)
                {
                    reglist.Dispose();
                    return ConvertFromNewToOldListItems(items);
                }
            }

            reglist.Dispose();

            return new List<RegistryItemCustom>();
        }

        public string GetSymbol()
        {
            return "";
        }

        public string GetTitle()
        {
            return "This device (through provider extensions)";
        }

        public bool IsLocal()
        {
            return true;
        }

        public async Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname)
        {
            bool hadaccessdenied = false;
            bool hadfailed = false;

            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                var result = await provider.RegRenameKey(ConvertToNewHive(hive), key, newname);

                if (result == REG_STATUS.SUCCESS)
                {
                    reglist.Dispose();
                    return ConvertToOldStatus(result);
                }

                if (result == REG_STATUS.NOT_SUPPORTED)
                {
                    continue;
                }

                if (result == REG_STATUS.ACCESS_DENIED)
                {
                    hadaccessdenied = true;
                    continue;
                }

                if (result == REG_STATUS.FAILED)
                {
                    hadfailed = true;
                    continue;
                }
            }

            reglist.Dispose();

            if (hadaccessdenied)
            {
                return HelperErrorCodes.ACCESS_DENIED;
            }

            if (hadfailed)
            {
                return HelperErrorCodes.FAILED;
            }

            return HelperErrorCodes.NOT_IMPLEMENTED;
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type, string data)
        {
            bool hadaccessdenied = false;
            bool hadfailed = false;

            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                var result = await provider.RegSetValue(ConvertToNewHive(hive), key, keyvalue, ConvertToNewType(type), data);

                if (result == REG_STATUS.SUCCESS)
                {
                    reglist.Dispose();
                    return ConvertToOldStatus(result);
                }

                if (result == REG_STATUS.NOT_SUPPORTED)
                {
                    continue;
                }

                if (result == REG_STATUS.ACCESS_DENIED)
                {
                    hadaccessdenied = true;
                    continue;
                }

                if (result == REG_STATUS.FAILED)
                {
                    hadfailed = true;
                    continue;
                }
            }

            reglist.Dispose();

            if (hadaccessdenied)
            {
                return HelperErrorCodes.ACCESS_DENIED;
            }

            if (hadfailed)
            {
                return HelperErrorCodes.FAILED;
            }

            return HelperErrorCodes.NOT_IMPLEMENTED;
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type, string data)
        {
            bool hadaccessdenied = false;
            bool hadfailed = false;

            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                var result = await provider.RegSetValue(ConvertToNewHive(hive), key, keyvalue, type, data);

                if (result == REG_STATUS.SUCCESS)
                {
                    reglist.Dispose();
                    return ConvertToOldStatus(result);
                }

                if (result == REG_STATUS.NOT_SUPPORTED)
                {
                    continue;
                }

                if (result == REG_STATUS.ACCESS_DENIED)
                {
                    hadaccessdenied = true;
                    continue;
                }

                if (result == REG_STATUS.FAILED)
                {
                    hadfailed = true;
                    continue;
                }
            }

            reglist.Dispose();

            if (hadaccessdenied)
            {
                return HelperErrorCodes.ACCESS_DENIED;
            }

            if (hadfailed)
            {
                return HelperErrorCodes.FAILED;
            }

            return HelperErrorCodes.NOT_IMPLEMENTED;
        }

        public async Task<HelperErrorCodes> LoadHive(string FileName, string mountpoint, bool inUser)
        {
            bool hadaccessdenied = false;
            bool hadfailed = false;

            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                var result = await provider.RegLoadHive(FileName, mountpoint, inUser);

                if (result == REG_STATUS.SUCCESS)
                {
                    reglist.Dispose();
                    return ConvertToOldStatus(result);
                }

                if (result == REG_STATUS.NOT_SUPPORTED)
                {
                    continue;
                }

                if (result == REG_STATUS.ACCESS_DENIED)
                {
                    hadaccessdenied = true;
                    continue;
                }

                if (result == REG_STATUS.FAILED)
                {
                    hadfailed = true;
                    continue;
                }
            }

            reglist.Dispose();

            if (hadaccessdenied)
            {
                return HelperErrorCodes.ACCESS_DENIED;
            }

            if (hadfailed)
            {
                return HelperErrorCodes.FAILED;
            }

            return HelperErrorCodes.NOT_IMPLEMENTED;
        }

        public async Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser)
        {
            bool hadaccessdenied = false;
            bool hadfailed = false;

            var reglist = await InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.ListAsync(InteropTools.Providers.Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME);

            foreach (var plugin in reglist.Plugins)
            {
                var provider = new RegistryProvider(plugin);

                var result = await provider.RegUnloadHive(mountpoint, inUser);

                if (result == REG_STATUS.SUCCESS)
                {
                    reglist.Dispose();
                    return ConvertToOldStatus(result);
                }

                if (result == REG_STATUS.NOT_SUPPORTED)
                {
                    continue;
                }

                if (result == REG_STATUS.ACCESS_DENIED)
                {
                    hadaccessdenied = true;
                    continue;
                }

                if (result == REG_STATUS.FAILED)
                {
                    hadfailed = true;
                    continue;
                }
            }

            reglist.Dispose();

            if (hadaccessdenied)
            {
                return HelperErrorCodes.ACCESS_DENIED;
            }

            if (hadfailed)
            {
                return HelperErrorCodes.FAILED;
            }

            return HelperErrorCodes.NOT_IMPLEMENTED;
        }
    }
}
