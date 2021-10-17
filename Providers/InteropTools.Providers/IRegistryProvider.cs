// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System.Collections.Generic;
using System.Threading.Tasks;

namespace InteropTools.Providers
{
    public interface IRegistryProvider
    {
        Task<HelperErrorCodes> AddKey(RegHives hive, string key);

        bool AllowsRegistryEditing();

        Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive);

        Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue);

        bool DoesFileExists(string path);

        string GetAppInstallationPath();

        string GetDescription();

        string GetFriendlyName();

        string GetHostName();

        Task<GetKeyLastModifiedTime> GetKeyLastModifiedTime(RegHives hive, string key);

        Task<KeyStatus> GetKeyStatus(RegHives hive, string key);

        Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type);

        Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type);

        Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2();

        //Task<IReadOnlyList<RegistryItem>> GetRegistryItems(RegHives hive, string key);
        Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key);

        string GetSymbol();

        string GetTitle();

        bool IsLocal();

        //Task<IReadOnlyList<RegistryItem>> GetRegistryHives();
        Task<HelperErrorCodes> LoadHive(string FileName, string mountpoint, bool inUser);

        Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname);

        Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type, string data);

        Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type, string data);

        Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser);
    }
}