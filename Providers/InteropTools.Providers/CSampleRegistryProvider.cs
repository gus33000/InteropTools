// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;

namespace InteropTools.Providers
{
    public sealed class CSampleRegistryProvider : IRegistryProvider
    {
        public async Task<HelperErrorCodes> AddKey(RegHives hive, string key) => HelperErrorCodes.Success;

        public bool AllowsRegistryEditing() => true;

        public async Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive) =>
            HelperErrorCodes.Success;

        public async Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue) =>
            HelperErrorCodes.Success;

        public bool DoesFileExists(string path) => true;

        public string GetAppInstallationPath() => Package.Current.InstalledLocation.Path;

        public string GetDescription() => ResourceManager.Current.MainResourceMap
            .GetValue("Resources/For_testing_purposes_only", ResourceContext.GetForCurrentView()).ValueAsString;

        public string GetFriendlyName() => ResourceManager.Current.MainResourceMap
            .GetValue("Resources/Fake_device", ResourceContext.GetForCurrentView()).ValueAsString;

        public string GetHostName() => "127.0.0.1";

        public async Task<GetKeyLastModifiedTime> GetKeyLastModifiedTime(RegHives hive, string key)
        {
            GetKeyLastModifiedTime ret = new() {LastModified = DateTime.Now, returncode = HelperErrorCodes.Success};
            return ret;
        }

        public async Task<KeyStatus> GetKeyStatus(RegHives hive, string key) => KeyStatus.Found;

        public async Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type)
        {
            GetKeyValueReturn ret = new()
            {
                regtype = RegTypes.REG_SZ, regvalue = "Test", returncode = HelperErrorCodes.Success
            };
            return ret;
        }

        public async Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type)
        {
            GetKeyValueReturn2 ret = new()
            {
                regtype = (uint)RegTypes.REG_SZ, regvalue = "Test", returncode = HelperErrorCodes.Success
            };
            return ret;
        }

        public async Task<IReadOnlyList<RegistryItem>> GetRegistryHives()
        {
            List<RegistryItem> itemList = new()
            {
                new RegistryItem
                {
                    Name = "HKEY_CLASSES_ROOT",
                    Hive = RegHives.HKEY_CLASSES_ROOT,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_CURRENT_CONFIG",
                    Hive = RegHives.HKEY_CURRENT_CONFIG,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_CURRENT_USER",
                    Hive = RegHives.HKEY_CURRENT_USER,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_CURRENT_USER_LOCAL_SETTINGS",
                    Hive = RegHives.HKEY_CURRENT_USER_LOCAL_SETTINGS,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_DYN_DATA",
                    Hive = RegHives.HKEY_DYN_DATA,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_LOCAL_MACHINE",
                    Hive = RegHives.HKEY_LOCAL_MACHINE,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_PERFORMANCE_DATA",
                    Hive = RegHives.HKEY_PERFORMANCE_DATA,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "HKEY_USERS",
                    Hive = RegHives.HKEY_USERS,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                }
            };
            return itemList;
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2()
        {
            List<RegistryItemCustom> itemList = new()
            {
                new RegistryItemCustom
                {
                    Name = "HKEY_CLASSES_ROOT",
                    Hive = RegHives.HKEY_CLASSES_ROOT,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_CURRENT_CONFIG",
                    Hive = RegHives.HKEY_CURRENT_CONFIG,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_CURRENT_USER",
                    Hive = RegHives.HKEY_CURRENT_USER,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_CURRENT_USER_LOCAL_SETTINGS",
                    Hive = RegHives.HKEY_CURRENT_USER_LOCAL_SETTINGS,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_DYN_DATA",
                    Hive = RegHives.HKEY_DYN_DATA,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_LOCAL_MACHINE",
                    Hive = RegHives.HKEY_LOCAL_MACHINE,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_PERFORMANCE_DATA",
                    Hive = RegHives.HKEY_PERFORMANCE_DATA,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_USERS",
                    Hive = RegHives.HKEY_USERS,
                    Key = null,
                    Type = RegistryItemType.Hive,
                    Value = null,
                    ValueType = 0
                }
            };
            return itemList;
        }

        public async Task<IReadOnlyList<RegistryItem>> GetRegistryItems(RegHives hive, string key)
        {
            List<RegistryItem> itemList = new()
            {
                new RegistryItem
                {
                    Name = "Test 1",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Key,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "est 2",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Key,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "st 3",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Key,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "t 4",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Key,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "Test 5",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Key,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "est 6",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Key,
                    Value = null,
                    ValueType = RegTypes.REG_ERROR
                },
                new RegistryItem
                {
                    Name = "st 7",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Value,
                    Value = "Test value",
                    ValueType = RegTypes.REG_SZ
                },
                new RegistryItem
                {
                    Name = "t 8",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Value,
                    Value = "Test value",
                    ValueType = RegTypes.REG_SZ
                },
                new RegistryItem
                {
                    Name = "Test 9",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Value,
                    Value = "Test value",
                    ValueType = RegTypes.REG_SZ
                },
                new RegistryItem
                {
                    Name = "est 10",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Value,
                    Value = "Test value",
                    ValueType = RegTypes.REG_SZ
                },
                new RegistryItem
                {
                    Name = "st 11",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Value,
                    Value = "Test value",
                    ValueType = RegTypes.REG_SZ
                },
                new RegistryItem
                {
                    Name = "t 12",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Value,
                    Value = "Test value",
                    ValueType = RegTypes.REG_SZ
                }
            };
            return itemList;
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key)
        {
            List<RegistryItemCustom> itemList = new()
            {
                new RegistryItemCustom
                {
                    Name = "Test 1",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Key,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "est 2",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Key,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "st 3",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Key,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "t 4",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Key,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "Test 5",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Key,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "est 6",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Key,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "st 7",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Value,
                    Value = "Test value",
                    ValueType = 1
                },
                new RegistryItemCustom
                {
                    Name = "t 8",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Value,
                    Value = "Test value",
                    ValueType = 1
                },
                new RegistryItemCustom
                {
                    Name = "Test 9",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Value,
                    Value = "Test value",
                    ValueType = 1
                },
                new RegistryItemCustom
                {
                    Name = "est 10",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Value,
                    Value = "Test value",
                    ValueType = 1
                },
                new RegistryItemCustom
                {
                    Name = "st 11",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Value,
                    Value = "Test value",
                    ValueType = 1
                },
                new RegistryItemCustom
                {
                    Name = "t 12",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.Value,
                    Value = "Test value",
                    ValueType = 1
                }
            };
            return itemList;
        }

        public string GetSymbol() => "";

        public string GetTitle() => ResourceManager.Current.MainResourceMap
            .GetValue("Resources/Test_Provider", ResourceContext.GetForCurrentView()).ValueAsString;

        public bool IsLocal() => true;

        public Task<HelperErrorCodes> LoadHive(string FileName, string mountpoint, bool inUser) =>
            throw new NotImplementedException();

        public async Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname) =>
            HelperErrorCodes.Success;

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type,
            string data) => HelperErrorCodes.Success;

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type,
            string data) => HelperErrorCodes.Success;

        public Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser) => throw new NotImplementedException();
    }
}
