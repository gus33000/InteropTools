// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System.Threading.Tasks;

namespace InteropTools.Providers
{
    public interface IRegProvider
    {
        Task<HelperErrorCodes> RegAddKey(RegHives hive, string key);

        string RegBufferToString(uint valtype, byte[] data);

        Task<HelperErrorCodes> RegDeleteKey(RegHives hive, string key, bool recursive);

        Task<HelperErrorCodes> RegDeleteValue(RegHives hive, string key, string name);

        [Windows.Foundation.Metadata.DefaultOverload()]
        Task<RegEnumKey> RegEnumKey(RegHives? hive, string key);

        Task<HelperErrorCodes> RegLoadHive(string hivepath, string mountedname, bool InUser);

        Task<RegQueryKeyLastModifiedTime> RegQueryKeyLastModifiedTime(RegHives hive, string key);

        Task<KeyStatus> RegQueryKeyStatus(RegHives hive, string key);

        Task<RegQueryValue> RegQueryValue(RegHives hive, string key, string regvalue, RegTypes valtype);

        Task<RegQueryValue1> RegQueryValue(RegHives hive, string key, string regvalue, uint valtype);

        Task<HelperErrorCodes> RegRenameKey(RegHives hive, string key, string newname);

        [Windows.Foundation.Metadata.DefaultOverload()]
        Task<HelperErrorCodes> RegSetValue(RegHives hive, string key, string regvalue, uint valtype, byte[] data);

        Task<HelperErrorCodes> RegSetValue(RegHives hive, string key, string regvalue, RegTypes valtype, string data);

        Task<HelperErrorCodes> RegSetValue(RegHives hive, string key, string regvalue, uint valtype, string data);

        byte[] RegStringToBuffer(uint valtype, string val);

        Task<HelperErrorCodes> RegUnloadHive(string mountedname, bool InUser);
    }
}
