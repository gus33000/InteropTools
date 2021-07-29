using System.Collections.Generic;
using System.Threading.Tasks;

namespace InteropTools.Providers
{
    public interface IRegProvider
    {
        Task<REG_STATUS> RegAddKey(REG_HIVES hive, string key);

        string RegBufferToString(uint valtype, byte[] data);

        Task<REG_STATUS> RegDeleteKey(REG_HIVES hive, string key, bool recursive);

        Task<REG_STATUS> RegDeleteValue(REG_HIVES hive, string key, string name);

        [Windows.Foundation.Metadata.DefaultOverload()]
        Task<RegEnumKey> RegEnumKey(REG_HIVES? hive, string key);

        Task<REG_STATUS> RegLoadHive(string hivepath, string mountedname, bool InUser);

        Task<RegQueryKeyLastModifiedTime> RegQueryKeyLastModifiedTime(REG_HIVES hive, string key);

        Task<REG_KEY_STATUS> RegQueryKeyStatus(REG_HIVES hive, string key);

        Task<RegQueryValue> RegQueryValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype);

        Task<RegQueryValue1> RegQueryValue(REG_HIVES hive, string key, string regvalue, uint valtype);

        Task<REG_STATUS> RegRenameKey(REG_HIVES hive, string key, string newname);

        [Windows.Foundation.Metadata.DefaultOverload()]
        Task<REG_STATUS> RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, byte[] data);

        Task<REG_STATUS> RegSetValue(REG_HIVES hive, string key, string regvalue, REG_VALUE_TYPE valtype, string data);

        Task<REG_STATUS> RegSetValue(REG_HIVES hive, string key, string regvalue, uint valtype, string data);

        byte[] RegStringToBuffer(uint valtype, string val);

        Task<REG_STATUS> RegUnloadHive(string mountedname, bool InUser);
    }

    public class RegEnumKey
    {
        public IReadOnlyList<REG_ITEM> items { get; set; }
        public REG_STATUS returncode { get; set; }
    }

    public class RegQueryKeyLastModifiedTime
    {
        public long LastModified { get; set; }
        public REG_STATUS returncode { get; set; }
    }

    public class RegQueryValue
    {
        public REG_VALUE_TYPE regtype { get; set; }
        public string regvalue { get; set; }
        public REG_STATUS returncode { get; set; }
    }

    public class RegQueryValue1
    {
        public uint regtype { get; set; }
        public string regvalue { get; set; }
        public REG_STATUS returncode { get; set; }
    }

    public class RegQueryValue2
    {
        public uint regtype { get; set; }
        public byte[] regvalue { get; set; }
        public REG_STATUS returncode { get; set; }
    }
}