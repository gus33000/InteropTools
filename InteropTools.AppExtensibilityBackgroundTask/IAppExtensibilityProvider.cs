using System.Collections.Generic;

namespace InteropTools.AppExtensibilityBackgroundTask
{
    internal interface IAppExtensibilityProvider
    {
        REG_STATUS RegQueryKeyLastModifiedTime(REG_HIVES hive, string key, out long lastmodified);

        REG_STATUS RegAddKey(REG_HIVES hive, string key);

        REG_STATUS RegDeleteKey(REG_HIVES hive, string key, bool recursive);
        REG_STATUS RegDeleteValue(REG_HIVES hive, string key, string name);

        REG_STATUS RegRenameKey(REG_HIVES hive, string key, string newname);

        REG_KEY_STATUS RegQueryKeyStatus(REG_HIVES hive, string key);

        [Windows.Foundation.Metadata.DefaultOverloadAttribute()]
        REG_STATUS RegQueryValue(REG_HIVES hive, string key, string regvalue, uint valtype, out uint outvaltype, out byte[] data);
        [Windows.Foundation.Metadata.DefaultOverloadAttribute()]
        REG_STATUS RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, byte[] data);
        [Windows.Foundation.Metadata.DefaultOverloadAttribute()]
        REG_STATUS RegEnumKey(REG_HIVES? hive, string key, out IReadOnlyList<REG_ITEM> items);
        REG_STATUS RegLoadHive(string hivepath, string mountedname, bool InUser);
        REG_STATUS RegUnloadHive(string mountedname, bool InUser);
    }
}
