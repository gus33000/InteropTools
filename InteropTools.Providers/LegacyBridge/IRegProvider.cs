using System.Collections.Generic;
using System.Threading.Tasks;

namespace InteropTools.Providers
{
    public class RegQueryValue
    {
        public REG_STATUS returncode { get; set; }
        public REG_VALUE_TYPE regtype { get; set; }
        public string regvalue { get; set; }
    }

    public class RegQueryValue1
    {
        public REG_STATUS returncode { get; set; }
        public uint regtype { get; set; }
        public string regvalue { get; set; }
    }

    public class RegQueryValue2
    {
        public REG_STATUS returncode { get; set; }
        public uint regtype { get; set; }
        public byte[] regvalue { get; set; }
    }

    public class RegQueryKeyLastModifiedTime
    {
        public REG_STATUS returncode { get; set; }
        public long LastModified { get; set; }
    }

    public class RegEnumKey
    {
        public REG_STATUS returncode { get; set; }
        public IReadOnlyList<REG_ITEM> items { get; set; }
    }

    public interface IRegProvider
    {
        Task<RegQueryKeyLastModifiedTime> RegQueryKeyLastModifiedTime(REG_HIVES hive, string key);

        Task<REG_STATUS> RegAddKey(REG_HIVES hive, string key);

        Task<REG_STATUS> RegDeleteKey(REG_HIVES hive, string key, bool recursive);
        Task<REG_STATUS> RegDeleteValue(REG_HIVES hive, string key, string name);

        Task<REG_STATUS> RegRenameKey(REG_HIVES hive, string key, string newname);

        Task<REG_KEY_STATUS> RegQueryKeyStatus(REG_HIVES hive, string key);

        [Windows.Foundation.Metadata.DefaultOverload()]
        Task<REG_STATUS> RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, byte[] data);
        [Windows.Foundation.Metadata.DefaultOverload()]
        Task<RegEnumKey> RegEnumKey(REG_HIVES? hive, string key);

        string RegBufferToString(uint valtype, byte[] data);
        byte[] RegStringToBuffer(uint valtype, string val);

        Task<REG_STATUS> RegSetValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype, string data);
        Task<RegQueryValue> RegQueryValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype);

        Task<REG_STATUS> RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, string data);
        Task<RegQueryValue1> RegQueryValue(REG_HIVES hive, string key, string regvalue, uint valtype);
        Task<REG_STATUS> RegLoadHive(string hivepath, string mountedname, bool InUser);
        Task<REG_STATUS> RegUnloadHive(string mountedname, bool InUser);
    }
}
