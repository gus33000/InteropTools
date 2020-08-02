using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

namespace RegistryHelper
{
    public interface IRegistryProvider
    {
        bool IsSupported();

        REG_STATUS RegQueryKeyLastModifiedTime(REG_HIVES hive, string key, out long lastmodified);

        REG_STATUS RegEnumKey(REG_HIVES? hive, string key, out IReadOnlyList<REG_ITEM> items);

        REG_STATUS RegAddKey(REG_HIVES hive, string key);

        REG_STATUS RegDeleteKey(REG_HIVES hive, string key, bool recursive);
        REG_STATUS RegDeleteValue(REG_HIVES hive, string key, string name);

        REG_STATUS RegRenameKey(REG_HIVES hive, string key, string newname);

        REG_KEY_STATUS RegQueryKeyStatus(REG_HIVES hive, string key);

        REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype, out REG_VALUE_TYPE outvaltype, out byte[] data);
        REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype, [ReadOnlyArray] byte[] data);

        // Customs
        [Windows.Foundation.Metadata.DefaultOverloadAttribute()]
        REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, uint valtype, out uint outvaltype, out byte[] data);
        [Windows.Foundation.Metadata.DefaultOverloadAttribute()]
        REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, [ReadOnlyArray] byte[] data);
        [Windows.Foundation.Metadata.DefaultOverloadAttribute()]
        REG_STATUS RegEnumKey(REG_HIVES? hive, string key, out IReadOnlyList<REG_ITEM_CUSTOM> items);


        REG_STATUS RegQueryString(REG_HIVES hive, string key, string regvalue, out string data);
        REG_STATUS RegSetString(REG_HIVES hive, string key, string regvalue, string data);

        REG_STATUS RegQueryMultiString(REG_HIVES hive, string key, string regvalue, out string[] data);
        REG_STATUS RegSetMultiString(REG_HIVES hive, string key, string regvalue, [ReadOnlyArray] string[] data);

        REG_STATUS RegQueryVariableString(REG_HIVES hive, string key, string regvalue, out string data);
        REG_STATUS RegSetVariableString(REG_HIVES hive, string key, string regvalue, string data);

        REG_STATUS RegQueryDword(REG_HIVES hive, string key, string regvalue, out uint data);
        REG_STATUS RegSetDword(REG_HIVES hive, string key, string regvalue, uint data);

        REG_STATUS RegQueryQword(REG_HIVES hive, string key, string regvalue, out ulong data);
        REG_STATUS RegSetQword(REG_HIVES hive, string key, string regvalue, ulong data);

        REG_STATUS RegLoadHive(string FilePath, string mountpoint, bool inUser);
        REG_STATUS RegUnloadHive(string mountpoint, bool inUser);
    }
}
