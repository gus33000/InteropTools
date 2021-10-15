using InteropTools.ContentDialogs.Providers;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace InteropTools.Providers
{
    public class LegacyBridgeRegistryProvider : IRegistryProvider
    {
        public bool UsesPlugins;
        public IRegProvider LocalProvider;
        private bool IsInitialized;

        private static readonly uint[] _lookup32 = CreateLookup32();

        public async Task InitializeAsync()
        {
            IRegProvider result = null;
            bool HasUIAccess = false;

            try
            {
                HasUIAccess = CoreWindow.GetForCurrentThread().Dispatcher.HasThreadAccess;
            }
            catch { }

            if (HasUIAccess)
            {
                result = await new SelectRegistryProviderContentDialog().AskUserForProvider();
            }
            else
            {
                result = await DispatcherHelper.ExecuteOnUIThreadAsync(async () => await new SelectRegistryProviderContentDialog().AskUserForProvider());
            }

            if (result == null)
            {
                UsesPlugins = true;
                LocalProvider = null;
            }
            else
            {
                UsesPlugins = false;
                LocalProvider = result;
            }

            IsInitialized = true;
        }

        public async Task<HelperErrorCodes> ExecuteAction(Func<IRegProvider, Task<REG_STATUS>> providerFunctionCall)
        {
            return await ExecuteAction(providerFunctionCall, RegStatusToHelperErrorCodes, (t) => t, (t) => t);
        }

        public async Task<T1> ExecuteAction<T1,T2>(Func<IRegProvider, Task<T2>> providerFunctionCall, Func<T2, T1> typeConverterCall, Func<T2, REG_STATUS> typeStatusConverter, Func<HelperErrorCodes, T1> statusTypeConverter, bool SecondCall = false)
        {
            try
            {
                if (!IsInitialized)
                {
                    await InitializeAsync();
                }

                if (!UsesPlugins)
                {
                    T2 result = await providerFunctionCall(LocalProvider);

                    if (typeStatusConverter(result) == REG_STATUS.NOT_SUPPORTED && !SecondCall)
                    {
                        await InitializeAsync();
                        return await ExecuteAction(providerFunctionCall, typeConverterCall, typeStatusConverter, statusTypeConverter, true);
                    }
                    else
                    {
                        return typeConverterCall(result);
                    }
                }
                else
                {
                    bool hadaccessdenied = false;
                    bool hadfailed = false;

                    using (AppPlugin.PluginList.PluginList<string, string, double> reglist = await Registry.Definition.RegistryProvidersWithOptions.ListAsync(Registry.Definition.RegistryProvidersWithOptions.PLUGIN_NAME))
                    {
                        foreach (AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider plugin in reglist.Plugins)
                        {
                            RegistryProvider provider = new(plugin);

                            T2 result = await providerFunctionCall(provider);

                            if (typeStatusConverter(result) == REG_STATUS.SUCCESS)
                            {
                                reglist.Dispose();
                                return typeConverterCall(result);
                            }

                            if (typeStatusConverter(result) == REG_STATUS.NOT_SUPPORTED)
                            {
                                continue;
                            }

                            if (typeStatusConverter(result) == REG_STATUS.ACCESS_DENIED)
                            {
                                hadaccessdenied = true;
                                continue;
                            }

                            if (typeStatusConverter(result) == REG_STATUS.FAILED)
                            {
                                hadfailed = true;
                                continue;
                            }
                        }
                    }

                    if (hadaccessdenied)
                    {
                        return statusTypeConverter(HelperErrorCodes.AccessDenied);
                    }

                    if (hadfailed)
                    {
                        return statusTypeConverter(HelperErrorCodes.Failed);
                    }

                    return statusTypeConverter(HelperErrorCodes.NotImplemented);
                }
            }
            catch { }

            return statusTypeConverter(HelperErrorCodes.Failed);
        }

        public REG_HIVES ConvertToNewHive(RegHives hive)
        {
            return (REG_HIVES)Enum.Parse(typeof(REG_HIVES), hive.ToString());
        }

        public REG_VALUE_TYPE ConvertToNewType(RegTypes type)
        {
            return (REG_VALUE_TYPE)Enum.Parse(typeof(REG_VALUE_TYPE), type.ToString());
        }

        public REG_TYPE ConvertToNewValType(RegistryItemType type)
        {
            return (REG_TYPE)Enum.Parse(typeof(REG_TYPE), type.ToString());
        }

        public RegHives ConvertToOldHive(REG_HIVES hive)
        {
            return (RegHives)Enum.Parse(typeof(RegHives), hive.ToString());
        }

        public KeyStatus ConvertToOldKeyStatus(REG_KEY_STATUS status)
        {
            return (KeyStatus)Enum.Parse(typeof(KeyStatus), status.ToString());
        }

        public HelperErrorCodes RegStatusToHelperErrorCodes(REG_STATUS status)
        {
            return status switch
            {
                REG_STATUS.ACCESS_DENIED => HelperErrorCodes.AccessDenied,
                REG_STATUS.FAILED => HelperErrorCodes.Failed,
                REG_STATUS.NOT_SUPPORTED => HelperErrorCodes.NotImplemented,
                REG_STATUS.SUCCESS => HelperErrorCodes.Success,
                _ => HelperErrorCodes.NotImplemented,
            };
        }

        public RegistryItemType ConvertToOldType(REG_TYPE type)
        {
            return (RegistryItemType)Enum.Parse(typeof(RegistryItemType), type.ToString());
        }

        public RegTypes ConvertToOldValType(REG_VALUE_TYPE type)
        {
            return (RegTypes)Enum.Parse(typeof(RegTypes), type.ToString());
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
                        return string.Join("\n", strNullTerminated.Split('\0'));
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

        private static string ByteArrayToHexViaLookup32(byte[] bytes)
        {
            try
            {
                uint[] lookup32 = _lookup32;
                char[] result = new char[bytes.Length * 2];
                for (int i = 0; i < bytes.Length; i++)
                {
                    uint val = lookup32[bytes[i]];
                    result[2 * i] = (char)val;
                    result[(2 * i) + 1] = (char)(val >> 16);
                }
                return new string(result);
            }
            catch
            {
                return "";
            }
        }

        private static uint[] CreateLookup32()
        {
            uint[] result = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                string s = i.ToString("X2");
                result[i] = s[0] + ((uint)s[1] << 16);
            }
            return result;
        }

        private IReadOnlyList<RegistryItemCustom> ConvertFromNewToOldListItems(IReadOnlyList<REG_ITEM> items)
        {
            List<RegistryItemCustom> itemlist = new();

            foreach (REG_ITEM item in items)
            {
                RegistryItemCustom itm = new()
                {
                    Hive = item.Hive.HasValue ? ConvertToOldHive(item.Hive.Value) : RegHives.HKEY_LOCAL_MACHINE,
                    Key = item.Key,
                    Name = item.Name,
                    Type = item.Type.HasValue ? ConvertToOldType(item.Type.Value) : RegistryItemType.Hive,
                    Value = RegBufferToString(item.ValueType ?? 0, item.Data),
                    ValueType = item.ValueType ?? 0
                };

                itemlist.Add(itm);
            }

            return itemlist;
        }

        public async Task<HelperErrorCodes> AddKey(RegHives hive, string key)
        {
            return await ExecuteAction((t) => t.RegAddKey(ConvertToNewHive(hive), key));
        }

        public bool AllowsRegistryEditing()
        {
            return true;
        }

        public async Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive)
        {
            return await ExecuteAction((t) => t.RegDeleteKey(ConvertToNewHive(hive), key, recursive));
        }

        public async Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue)
        {
            return await ExecuteAction((t) => t.RegDeleteValue(ConvertToNewHive(hive), key, keyvalue));
        }

        public bool DoesFileExists(string path)
        {
            bool fileexists;
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
            return await ExecuteAction((t) => t.RegQueryKeyLastModifiedTime(ConvertToNewHive(hive), key), (t) => new Providers.GetKeyLastModifiedTime() {  LastModified = new DateTime(t.LastModified), returncode = RegStatusToHelperErrorCodes(t.returncode) }, (t) => t.returncode, (t) => new Providers.GetKeyLastModifiedTime() { LastModified = new DateTime(), returncode = t });
        }

        public async Task<KeyStatus> GetKeyStatus(RegHives hive, string key)
        {
            return await ExecuteAction((t) => t.RegQueryKeyStatus(ConvertToNewHive(hive), key), ConvertToOldKeyStatus, (t) =>
            { 
                switch (t)
                {
                    case REG_KEY_STATUS.FOUND:
                        return REG_STATUS.SUCCESS;
                    case REG_KEY_STATUS.NOT_FOUND:
                        return REG_STATUS.FAILED;
                    case REG_KEY_STATUS.ACCESS_DENIED:
                        return REG_STATUS.ACCESS_DENIED;
                    case REG_KEY_STATUS.UNKNOWN:
                        return REG_STATUS.NOT_SUPPORTED;
                }

                return REG_STATUS.NOT_SUPPORTED;
            }, (t) =>
            {
                switch (t)
                {
                    case HelperErrorCodes.Success:
                        return KeyStatus.Found;
                    case HelperErrorCodes.Failed:
                        return KeyStatus.NotFound;
                    case HelperErrorCodes.AccessDenied:
                        return KeyStatus.AccessDenied;
                    case HelperErrorCodes.NotImplemented:
                        return KeyStatus.Unknown;
                }

                return KeyStatus.Unknown;
            });
        }

        public async Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type)
        {
            return await ExecuteAction((t) => t.RegQueryValue(ConvertToNewHive(hive), key, keyvalue, ConvertToNewType(type)), (t) => new Providers.GetKeyValueReturn() { regtype = ConvertToOldValType(t.regtype), regvalue = t.regvalue, returncode = RegStatusToHelperErrorCodes(t.returncode) }, (t) => t.returncode, (t) => new Providers.GetKeyValueReturn() { regtype = RegTypes.REG_ERROR, regvalue = "", returncode = t });
        }

        public async Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type)
        {
            return await ExecuteAction((t) => t.RegQueryValue(ConvertToNewHive(hive), key, keyvalue, type), (t) => new Providers.GetKeyValueReturn2() { regtype = t.regtype, regvalue = t.regvalue, returncode = RegStatusToHelperErrorCodes(t.returncode) }, (t) => t.returncode, (t) => new Providers.GetKeyValueReturn2() { regtype = 0, regvalue = "", returncode = t });
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2()
        {
            return await ExecuteAction((t) => t.RegEnumKey(null, ""), (t) => ConvertFromNewToOldListItems(t.items), (t) => t.returncode, (t) => new List<RegistryItemCustom>());
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key)
        {
            return await ExecuteAction((t) => t.RegEnumKey(ConvertToNewHive(hive), key), (t) => ConvertFromNewToOldListItems(t.items), (t) => t.returncode, (t) => new List<RegistryItemCustom>());
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

        public async Task<HelperErrorCodes> LoadHive(string FileName, string mountpoint, bool inUser)
        {
            return await ExecuteAction((t) => t.RegLoadHive(FileName, mountpoint, inUser));
        }

        public async Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname)
        {
            return await ExecuteAction((t) => t.RegRenameKey(ConvertToNewHive(hive), key, newname));
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type, string data)
        {
            return await ExecuteAction((t) => t.RegSetValue(ConvertToNewHive(hive), key, keyvalue, ConvertToNewType(type), data));
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type, string data)
        {
            return await ExecuteAction((t) => t.RegSetValue(ConvertToNewHive(hive), key, keyvalue, type, data));
        }

        public async Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser)
        {
            return await ExecuteAction((t) => t.RegUnloadHive(mountpoint, inUser));
        }
    }
}
