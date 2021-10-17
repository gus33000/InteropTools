// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;

namespace InteropTools.Providers
{
    public sealed class CLocalOnlyProvider : IRegistryProvider
    {
        public async Task<HelperErrorCodes> AddKey(RegHives hive, string key) => HelperErrorCodes.NotImplemented;

        public bool AllowsRegistryEditing() => false;

        public async Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive) =>
            HelperErrorCodes.NotImplemented;

        public async Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue) =>
            HelperErrorCodes.NotImplemented;

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

        public string GetAppInstallationPath() => Package.Current.InstalledLocation.Path;

        public string GetDescription() => ResourceManager.Current.MainResourceMap
            .GetValue("Resources/Launch_the_app_without_the_registry_features_enabled",
                ResourceContext.GetForCurrentView()).ValueAsString;

        public string GetFriendlyName() => ResourceManager.Current.MainResourceMap
            .GetValue("Resources/This_device", ResourceContext.GetForCurrentView()).ValueAsString;

        public string GetHostName() => "127.0.0.1";

        public async Task<GetKeyLastModifiedTime> GetKeyLastModifiedTime(RegHives hive, string key)
        {
            GetKeyLastModifiedTime ret = new()
            {
                LastModified = new DateTime(), returncode = HelperErrorCodes.NotImplemented
            };
            return ret;
        }

        public async Task<KeyStatus> GetKeyStatus(RegHives hive, string key) => KeyStatus.Unknown;

        public async Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type)
        {
            GetKeyValueReturn2 ret = new() {regtype = 0, regvalue = "", returncode = HelperErrorCodes.NotImplemented};
            return ret;
        }

        public async Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type)
        {
            GetKeyValueReturn ret = new()
            {
                regtype = RegTypes.REG_ERROR, regvalue = "", returncode = HelperErrorCodes.NotImplemented
            };
            return ret;
        }

        public async Task<IReadOnlyList<RegistryItem>> GetRegistryHives() => new List<RegistryItem>();

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2() => new List<RegistryItemCustom>();

        public async Task<IReadOnlyList<RegistryItem>> GetRegistryItems(RegHives hive, string key) =>
            new List<RegistryItem>();

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key) =>
            new List<RegistryItemCustom>();

        public string GetSymbol() => "";

        public string GetTitle() => ResourceManager.Current.MainResourceMap
            .GetValue("Resources/Registry_less_provider", ResourceContext.GetForCurrentView()).ValueAsString;

        public bool IsLocal() => true;

        public Task<HelperErrorCodes> LoadHive(string FileName, string mountpoint, bool inUser) =>
            throw new NotImplementedException();

        public async Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname) =>
            HelperErrorCodes.NotImplemented;

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type,
            string data) => HelperErrorCodes.NotImplemented;

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type,
            string data) => HelperErrorCodes.NotImplemented;

        public Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser) => throw new NotImplementedException();
    }
}
