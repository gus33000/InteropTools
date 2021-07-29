using RegistryHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;

namespace InteropTools.Providers
{
    public sealed class CNativeRegistryProvider : IRegistryProvider
    {
        private readonly CRegistryHelper helper = new();
        public bool Initialized;

        private static readonly Dictionary<RegHives, RegistryHelper.REG_HIVES> _hives = new()
        {
            { RegHives.HKEY_CLASSES_ROOT, RegistryHelper.REG_HIVES.HKEY_CLASSES_ROOT },
            { RegHives.HKEY_CURRENT_USER, RegistryHelper.REG_HIVES.HKEY_CURRENT_USER },
            { RegHives.HKEY_LOCAL_MACHINE, RegistryHelper.REG_HIVES.HKEY_LOCAL_MACHINE },
            { RegHives.HKEY_USERS, RegistryHelper.REG_HIVES.HKEY_USERS },
            { RegHives.HKEY_PERFORMANCE_DATA, RegistryHelper.REG_HIVES.HKEY_PERFORMANCE_DATA },
            { RegHives.HKEY_CURRENT_CONFIG, RegistryHelper.REG_HIVES.HKEY_CURRENT_CONFIG },
            { RegHives.HKEY_DYN_DATA, RegistryHelper.REG_HIVES.HKEY_DYN_DATA },
            { RegHives.HKEY_CURRENT_USER_LOCAL_SETTINGS, RegistryHelper.REG_HIVES.HKEY_CURRENT_USER_LOCAL_SETTINGS }
        };

        private static readonly Dictionary<RegTypes, RegistryHelper.REG_VALUE_TYPE> _valtypes = new()
        {
            { RegTypes.REG_NONE, RegistryHelper.REG_VALUE_TYPE.REG_NONE },
            { RegTypes.REG_SZ, RegistryHelper.REG_VALUE_TYPE.REG_SZ },
            { RegTypes.REG_EXPAND_SZ, RegistryHelper.REG_VALUE_TYPE.REG_EXPAND_SZ },
            { RegTypes.REG_BINARY, RegistryHelper.REG_VALUE_TYPE.REG_BINARY },
            { RegTypes.REG_DWORD, RegistryHelper.REG_VALUE_TYPE.REG_DWORD },
            { RegTypes.REG_DWORD_BIG_ENDIAN, RegistryHelper.REG_VALUE_TYPE.REG_DWORD_BIG_ENDIAN },
            { RegTypes.REG_LINK, RegistryHelper.REG_VALUE_TYPE.REG_LINK },
            { RegTypes.REG_MULTI_SZ, RegistryHelper.REG_VALUE_TYPE.REG_MULTI_SZ },
            { RegTypes.REG_RESOURCE_LIST, RegistryHelper.REG_VALUE_TYPE.REG_RESOURCE_LIST },
            { RegTypes.REG_FULL_RESOURCE_DESCRIPTOR, RegistryHelper.REG_VALUE_TYPE.REG_FULL_RESOURCE_DESCRIPTOR },
            { RegTypes.REG_RESOURCE_REQUIREMENTS_LIST, RegistryHelper.REG_VALUE_TYPE.REG_RESOURCE_REQUIREMENTS_LIST },
            { RegTypes.REG_QWORD, RegistryHelper.REG_VALUE_TYPE.REG_QWORD }
        };

        public string GetAppInstallationPath()
        {
            return Package.Current.InstalledLocation.Path;
        }

        public bool IsLocal()
        {
            return true;
        }

        public bool AllowsRegistryEditing()
        {
            return true;
        }

        public string GetFriendlyName()
        {
            return ResourceManager.Current.MainResourceMap.GetValue("Resources/This_device", ResourceContext.GetForCurrentView()).ValueAsString;
        }

        public string GetTitle()
        {
#if !STORE
            return ResourceManager.Current.MainResourceMap.GetValue("Resources/This_device", ResourceContext.GetForCurrentView()).ValueAsString;
#endif
#if STORE
            return ResourceManager.Current.MainResourceMap.GetValue("Resources/This_device", ResourceContext.GetForCurrentView()).ValueAsString + " (S EXPERIMENTAL)";
#endif
        }

        public string GetDescription()
        {
            return
              ResourceManager.Current.MainResourceMap.GetValue("Resources/Connects_to_this_device_natively_using_different_runtime_components__Provides_system_level_access_for_Nokia__excluding_x50s___Samsung_and_LG_devices__Other_devices_get_normal_access_to_the_registry",
                  ResourceContext.GetForCurrentView()).ValueAsString;
        }

        public string GetSymbol()
        {
            return "";
        }

        public async Task<HelperErrorCodes> AddKey(RegHives hive, string key)
        {
            RegistryHelper.REG_HIVES tmphive = _hives[hive];
            HelperErrorCodes result = (HelperErrorCodes)(uint)helper.RegAddKey(tmphive, key);
            return result;
        }

        public async Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive)
        {
            RegistryHelper.REG_HIVES tmphive = _hives[hive];
            HelperErrorCodes result = (HelperErrorCodes)(uint)helper.RegDeleteKey(tmphive, key, recursive);
            return result;
        }

        public async Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue)
        {
            RegistryHelper.REG_HIVES tmphive = _hives[hive];
            HelperErrorCodes result = (HelperErrorCodes)(uint)helper.RegDeleteValue(tmphive, key, keyvalue);
            return result;
        }

        public bool DoesFileExists(string path)
        {
            return helper.DoesFileExists(path);
        }

        public async Task<KeyStatus> GetKeyStatus(RegHives hive, string key)
        {
            RegistryHelper.REG_HIVES tmphive = _hives[hive];
            KeyStatus result = (KeyStatus)(uint)helper.RegQueryKeyStatus(tmphive, key);
            return result;
        }

        public async Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type)
        {
            GetKeyValueReturn ret = new();

            RegistryHelper.REG_HIVES tmphive = _hives[hive];
            RegistryHelper.REG_VALUE_TYPE tmptype = _valtypes[type];
            HelperErrorCodes result =
              (HelperErrorCodes)
              (uint)helper.RegQueryValue(tmphive, key, keyvalue, tmptype, out RegistryHelper.REG_VALUE_TYPE tmpregtype, out string regvalue);

            ret.regtype = (RegTypes)(uint)tmpregtype;
            ret.regvalue = regvalue;
            ret.returncode = result;

            return ret;
        }

        public async Task<IReadOnlyList<RegistryItem>> GetRegistryHives()
        {
            List<RegistryItem> itemsList = new();
            helper.RegEnumKey(null, null, out IReadOnlyList<RegistryHelper.REG_ITEM> items);

            foreach (RegistryHelper.REG_ITEM item in items)
            {
                itemsList.Add(new RegistryItem
                {
                    Hive = _hives.FirstOrDefault(x => x.Value == item.Hive).Key,
                    Key = item.Key,
                    Name = item.Name,
                    Type = (RegistryItemType)(uint)item.Type,
                    Value = item.DataAsString,
                    ValueType = _valtypes.FirstOrDefault(x => x.Value == item.ValueType).Key
                });
            }

            return itemsList;
        }

        public async Task<IReadOnlyList<RegistryItem>> GetRegistryItems(RegHives hive, string key)
        {
            RegistryHelper.REG_HIVES tmphive = _hives[hive];
            List<RegistryItem> itemsList = new();
            helper.RegEnumKey(tmphive, key, out IReadOnlyList<RegistryHelper.REG_ITEM> items);

            foreach (RegistryHelper.REG_ITEM item in items)
            {
                itemsList.Add(new RegistryItem
                {
                    Hive = _hives.FirstOrDefault(x => x.Value == item.Hive).Key,
                    Key = item.Key,
                    Name = item.Name,
                    Type = (RegistryItemType)(uint)item.Type,
                    Value = item.DataAsString,
                    ValueType = _valtypes.FirstOrDefault(x => x.Value == item.ValueType).Key
                });
            }

            return itemsList;
        }

        public async Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname)
        {
            RegistryHelper.REG_HIVES tmphive = _hives[hive];
            HelperErrorCodes result = (HelperErrorCodes)(uint)helper.RegRenameKey(tmphive, key, newname);
            return result;
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type, string data)
        {
            RegistryHelper.REG_HIVES tmphive = _hives[hive];
            RegistryHelper.REG_VALUE_TYPE tmptype = _valtypes[type];
            HelperErrorCodes result = (HelperErrorCodes)(uint)helper.RegSetValue(tmphive, key, keyvalue, tmptype, data);
            return result;
        }

        public string GetHostName()
        {
            return "127.0.0.1";
        }

        public async Task<GetKeyLastModifiedTime> GetKeyLastModifiedTime(RegHives hive, string key)
        {
            GetKeyLastModifiedTime ret = new();

            try
            {
                RegistryHelper.REG_HIVES tmphive = _hives[hive];
                HelperErrorCodes result = (HelperErrorCodes)(uint)helper.RegQueryKeyLastModifiedTime(tmphive, key, out long time);
                ret.LastModified = DateTime.FromFileTime(time);
                ret.returncode = HelperErrorCodes.SUCCESS;
            }
            catch
            {
                ret.LastModified = new DateTime();
                ret.returncode = HelperErrorCodes.FAILED;
            }

            return ret;
        }

        public async Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type)
        {
            GetKeyValueReturn2 ret = new();

            RegistryHelper.REG_HIVES tmphive = _hives[hive];
            HelperErrorCodes result =
              (HelperErrorCodes)
              (uint)helper.RegQueryValue(tmphive, key, keyvalue, type, out uint regtype, out string regvalue);

            ret.regtype = regtype;
            ret.regvalue = regvalue;
            ret.returncode = result;
            return ret;
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type, string data)
        {
            RegistryHelper.REG_HIVES tmphive = _hives[hive];
            HelperErrorCodes result = (HelperErrorCodes)(uint)helper.RegSetValue(tmphive, key, keyvalue, type, data);
            return result;
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2()
        {
            List<RegistryItemCustom> itemsList = new();
            helper.RegEnumKey(null, null, out IReadOnlyList<REG_ITEM_CUSTOM> items);

            foreach (REG_ITEM_CUSTOM item in items)
            {
                itemsList.Add(new RegistryItemCustom
                {
                    Hive = _hives.FirstOrDefault(x => x.Value == item.Hive).Key,
                    Key = item.Key,
                    Name = item.Name,
                    Type = (RegistryItemType)(uint)item.Type,
                    Value = item.DataAsString,
                    ValueType = item.ValueType == null ? 0 : (uint)item.ValueType
                });
            }

            return itemsList;
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key)
        {
            RegistryHelper.REG_HIVES tmphive = _hives[hive];
            List<RegistryItemCustom> itemsList = new();
            helper.RegEnumKey(tmphive, key, out IReadOnlyList<REG_ITEM_CUSTOM> items);

            foreach (REG_ITEM_CUSTOM item in items)
            {
                itemsList.Add(new RegistryItemCustom
                {
                    Hive = _hives.FirstOrDefault(x => x.Value == item.Hive).Key,
                    Key = item.Key,
                    Name = item.Name,
                    Type = (RegistryItemType)(uint)item.Type,
                    Value = item.DataAsString,
                    ValueType = item.ValueType == null ? 0 : (uint)item.ValueType
                });
            }

            return itemsList;
        }

        public async Task<HelperErrorCodes> LoadHive(string FileName, string mountpoint, bool inUser)
        {
            return (HelperErrorCodes)(uint)helper.RegLoadHive(FileName, mountpoint, inUser);
        }

        public async Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser)
        {
            return (HelperErrorCodes)(uint)helper.RegUnloadHive(mountpoint, inUser);
        }
    }
}