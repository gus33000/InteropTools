using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InteropTools.Providers
{
	public enum HelperErrorCodes
	{
		SUCCESS,
		FAILED,
		ACCESS_DENIED,
		NOT_IMPLEMENTED
	}

	public enum KeyStatus
	{
		FOUND,
		NOT_FOUND,
		ACCESS_DENIED,
		UNKNOWN
	}

	public enum RegHives
	{
		HKEY_CLASSES_ROOT = int.MinValue,
		HKEY_CURRENT_USER = -2147483647,
		HKEY_LOCAL_MACHINE = -2147483646,
		HKEY_USERS = -2147483645,
		HKEY_PERFORMANCE_DATA = -2147483644,
		HKEY_CURRENT_CONFIG = -2147483643,
		HKEY_DYN_DATA = -2147483642,
		HKEY_CURRENT_USER_LOCAL_SETTINGS = -2147483641
	}

	public enum RegTypes
	{
		REG_ERROR = -1,
		REG_NONE = 0,
		REG_SZ = 1,
		REG_EXPAND_SZ = 2,
		REG_BINARY = 3,
		REG_DWORD = 4,
		REG_DWORD_BIG_ENDIAN = 5,
		REG_LINK = 6,
		REG_MULTI_SZ = 7,
		REG_RESOURCE_LIST = 8,
		REG_FULL_RESOURCE_DESCRIPTOR = 9,
		REG_RESOURCE_REQUIREMENTS_LIST = 10,
		REG_QWORD = 11
	}

	public enum RegistryItemType
	{
		HIVE,
		KEY,
		VALUE
	}

	public sealed class RegistryItem
	{
		public string Name { get; set; }
		public RegistryItemType Type { get; set; }

		public RegHives Hive { get; set; }

		public string Key { get; set; }

		public string Value { get; set; }
		public RegTypes ValueType { get; set; }
	}

	public sealed class RegistryItemCustom
	{
		public string Name { get; set; }
		public RegistryItemType Type { get; set; }

		public RegHives Hive { get; set; }

		public string Key { get; set; }

		public string Value { get; set; }
		public uint ValueType { get; set; }
    }

    public class GetKeyValueReturn
    {
        public HelperErrorCodes returncode { get; set; }
        public RegTypes regtype { get; set; }
        public string regvalue { get; set; }
    }

    public class GetKeyValueReturn2
    {
        public HelperErrorCodes returncode { get; set; }
        public uint regtype { get; set; }
        public string regvalue { get; set; }
    }

    public class GetKeyLastModifiedTime
    {
        public HelperErrorCodes returncode { get; set; }
        public DateTime LastModified { get; set; }
    }

    public interface IRegistryProvider
	{
		bool IsLocal();

		bool AllowsRegistryEditing();

		string GetFriendlyName();

		string GetHostName();

		string GetTitle();

		string GetDescription();

		string GetSymbol();

		Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type);

		Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type, string data);

		Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type);

		Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type, string data);

		Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue);

		Task<KeyStatus> GetKeyStatus(RegHives hive, string key);

		Task<HelperErrorCodes> AddKey(RegHives hive, string key);

		Task<GetKeyLastModifiedTime> GetKeyLastModifiedTime(RegHives hive, string key);

		Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive);

		Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname);

        //Task<IReadOnlyList<RegistryItem>> GetRegistryHives();

		//Task<IReadOnlyList<RegistryItem>> GetRegistryItems(RegHives hive, string key);

		Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2();

		Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key);

        Task<HelperErrorCodes> LoadHive(string FileName, string mountpoint, bool inUser);
        Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser);

        bool DoesFileExists(string path);

		string GetAppInstallationPath();
	}
}