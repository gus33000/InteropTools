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
        public async Task<HelperErrorCodes> AddKey(RegHives hive, string key)
        {
            return HelperErrorCodes.NOT_IMPLEMENTED;
        }

        public bool AllowsRegistryEditing()
        {
            return false;
        }

        public async Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive)
        {
            return HelperErrorCodes.NOT_IMPLEMENTED;
        }

        public async Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue)
        {
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
            return Package.Current.InstalledLocation.Path;
        }

        public string GetDescription()
        {
            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Launch_the_app_without_the_registry_features_enabled", ResourceContext.GetForCurrentView()).ValueAsString;
        }

        public string GetFriendlyName()
        {
            return ResourceManager.Current.MainResourceMap.GetValue("Resources/This_device", ResourceContext.GetForCurrentView()).ValueAsString;
        }

        public string GetHostName()
        {
            return "127.0.0.1";
        }

        public async Task<GetKeyLastModifiedTime> GetKeyLastModifiedTime(RegHives hive, string key)
        {
            GetKeyLastModifiedTime ret = new GetKeyLastModifiedTime
            {
                LastModified = new DateTime(),
                returncode = HelperErrorCodes.NOT_IMPLEMENTED
            };
            return ret;
        }

        public async Task<KeyStatus> GetKeyStatus(RegHives hive, string key)
        {
            return KeyStatus.UNKNOWN;
        }

        public async Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type)
        {
            GetKeyValueReturn2 ret = new GetKeyValueReturn2
            {
                regtype = 0,
                regvalue = "",
                returncode = HelperErrorCodes.NOT_IMPLEMENTED
            };
            return ret;
        }

        public async Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type)
        {
            GetKeyValueReturn ret = new GetKeyValueReturn
            {
                regtype = RegTypes.REG_ERROR,
                regvalue = "",
                returncode = HelperErrorCodes.NOT_IMPLEMENTED
            };
            return ret;
        }

        public async Task<IReadOnlyList<RegistryItem>> GetRegistryHives()
        {
            return new List<RegistryItem>();
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2()
        {
            return new List<RegistryItemCustom>();
        }

        public async Task<IReadOnlyList<RegistryItem>> GetRegistryItems(RegHives hive, string key)
        {
            return new List<RegistryItem>();
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key)
        {
            return new List<RegistryItemCustom>();
        }

        public string GetSymbol()
        {
            return "";
        }

        public string GetTitle()
        {
            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Registry_less_provider", ResourceContext.GetForCurrentView()).ValueAsString;
        }

        public bool IsLocal()
        {
            return true;
        }

        public Task<HelperErrorCodes> LoadHive(string FileName, string mountpoint, bool inUser)
        {
            throw new NotImplementedException();
        }

        public async Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname)
        {
            return HelperErrorCodes.NOT_IMPLEMENTED;
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type, string data)
        {
            return HelperErrorCodes.NOT_IMPLEMENTED;
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type, string data)
        {
            return HelperErrorCodes.NOT_IMPLEMENTED;
        }

        public Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser)
        {
            throw new NotImplementedException();
        }
    }
}