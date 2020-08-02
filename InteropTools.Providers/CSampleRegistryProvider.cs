using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Resources.Core;

namespace InteropTools.Providers
{
	public sealed class CSampleRegistryProvider : IRegistryProvider
	{
		public string GetAppInstallationPath()
		{
			return Package.Current.InstalledLocation.Path;
		}

		public async Task<HelperErrorCodes> AddKey(RegHives hive, string key)
		{
			return HelperErrorCodes.SUCCESS;
		}

		public async Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive)
		{
			return HelperErrorCodes.SUCCESS;
		}

		public async Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue)
		{
			return HelperErrorCodes.SUCCESS;
		}

		public bool DoesFileExists(string path)
		{
			return true;
		}

		public string GetDescription()
		{
			return ResourceManager.Current.MainResourceMap.GetValue("Resources/For_testing_purposes_only", ResourceContext.GetForCurrentView()).ValueAsString;
		}

		public async Task<KeyStatus> GetKeyStatus(RegHives hive, string key)
		{
			return KeyStatus.FOUND;
		}

		public async Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type)
		{
            var ret = new GetKeyValueReturn();
			ret.regtype = RegTypes.REG_SZ;
			ret.regvalue = "Test";
			ret.returncode = HelperErrorCodes.SUCCESS;
            return ret;
		}

		public async Task<IReadOnlyList<RegistryItem>> GetRegistryHives()
		{
			var itemList = new List<RegistryItem>
			{
				new RegistryItem
				{
					Name = "HKEY_CLASSES_ROOT",
					Hive = RegHives.HKEY_CLASSES_ROOT,
					Key = null,
					Type = RegistryItemType.HIVE,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				},
				new RegistryItem
				{
					Name = "HKEY_CURRENT_CONFIG",
					Hive = RegHives.HKEY_CURRENT_CONFIG,
					Key = null,
					Type = RegistryItemType.HIVE,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				},
				new RegistryItem
				{
					Name = "HKEY_CURRENT_USER",
					Hive = RegHives.HKEY_CURRENT_USER,
					Key = null,
					Type = RegistryItemType.HIVE,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				},
				new RegistryItem
				{
					Name = "HKEY_CURRENT_USER_LOCAL_SETTINGS",
					Hive = RegHives.HKEY_CURRENT_USER_LOCAL_SETTINGS,
					Key = null,
					Type = RegistryItemType.HIVE,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				},
				new RegistryItem
				{
					Name = "HKEY_DYN_DATA",
					Hive = RegHives.HKEY_DYN_DATA,
					Key = null,
					Type = RegistryItemType.HIVE,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				},
				new RegistryItem
				{
					Name = "HKEY_LOCAL_MACHINE",
					Hive = RegHives.HKEY_LOCAL_MACHINE,
					Key = null,
					Type = RegistryItemType.HIVE,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				},
				new RegistryItem
				{
					Name = "HKEY_PERFORMANCE_DATA",
					Hive = RegHives.HKEY_PERFORMANCE_DATA,
					Key = null,
					Type = RegistryItemType.HIVE,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				},
				new RegistryItem
				{
					Name = "HKEY_USERS",
					Hive = RegHives.HKEY_USERS,
					Key = null,
					Type = RegistryItemType.HIVE,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				}
			};
			return itemList;
		}

		public async Task<IReadOnlyList<RegistryItem>> GetRegistryItems(RegHives hive, string key)
		{
			var itemList = new List<RegistryItem>
			{
				new RegistryItem
				{
					Name = "Test 1",
					Hive = hive,
					Key = key,
					Type = RegistryItemType.KEY,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				},
				new RegistryItem
				{
					Name = "est 2",
					Hive = hive,
					Key = key,
					Type = RegistryItemType.KEY,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				},
				new RegistryItem
				{
					Name = "st 3",
					Hive = hive,
					Key = key,
					Type = RegistryItemType.KEY,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				},
				new RegistryItem
				{
					Name = "t 4",
					Hive = hive,
					Key = key,
					Type = RegistryItemType.KEY,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				},
				new RegistryItem
				{
					Name = "Test 5",
					Hive = hive,
					Key = key,
					Type = RegistryItemType.KEY,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				},
				new RegistryItem
				{
					Name = "est 6",
					Hive = hive,
					Key = key,
					Type = RegistryItemType.KEY,
					Value = null,
					ValueType = RegTypes.REG_ERROR
				},
				new RegistryItem
				{
					Name = "st 7",
					Hive = hive,
					Key = key,
					Type = RegistryItemType.VALUE,
					Value = "Test value",
					ValueType = RegTypes.REG_SZ
				},
				new RegistryItem
				{
					Name = "t 8",
					Hive = hive,
					Key = key,
					Type = RegistryItemType.VALUE,
					Value = "Test value",
					ValueType = RegTypes.REG_SZ
				},
				new RegistryItem
				{
					Name = "Test 9",
					Hive = hive,
					Key = key,
					Type = RegistryItemType.VALUE,
					Value = "Test value",
					ValueType = RegTypes.REG_SZ
				},
				new RegistryItem
				{
					Name = "est 10",
					Hive = hive,
					Key = key,
					Type = RegistryItemType.VALUE,
					Value = "Test value",
					ValueType = RegTypes.REG_SZ
				},
				new RegistryItem
				{
					Name = "st 11",
					Hive = hive,
					Key = key,
					Type = RegistryItemType.VALUE,
					Value = "Test value",
					ValueType = RegTypes.REG_SZ
				},
				new RegistryItem
				{
					Name = "t 12",
					Hive = hive,
					Key = key,
					Type = RegistryItemType.VALUE,
					Value = "Test value",
					ValueType = RegTypes.REG_SZ
				}
			};
			return itemList;
		}

		public string GetSymbol()
		{
			return "";
		}

		public string GetTitle()
		{
			return ResourceManager.Current.MainResourceMap.GetValue("Resources/Test_Provider", ResourceContext.GetForCurrentView()).ValueAsString;
		}

		public async Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname)
		{
			return HelperErrorCodes.SUCCESS;
		}

		public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type, string data)
		{
			return HelperErrorCodes.SUCCESS;
		}

		public string GetHostName()
		{
			return "127.0.0.1";
		}

		public bool IsLocal()
		{
			return true;
		}

		public bool AllowsRegistryEditing()
		{
			return true;
		}

		public string GetFriendlyName()
		{
			return ResourceManager.Current.MainResourceMap.GetValue("Resources/Fake_device", ResourceContext.GetForCurrentView()).ValueAsString;
		}

		public async Task<GetKeyLastModifiedTime> GetKeyLastModifiedTime(RegHives hive, String key)
		{
            var ret = new GetKeyLastModifiedTime();
			ret.LastModified = DateTime.Now;
			ret.returncode = HelperErrorCodes.SUCCESS;
            return ret;
		}

		public async Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type)
		{
            var ret = new GetKeyValueReturn2();
            ret.regtype = (uint)RegTypes.REG_SZ;
            ret.regvalue = "Test";
            ret.returncode = HelperErrorCodes.SUCCESS;
            return ret;
        }

		public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type, string data)
		{
            return HelperErrorCodes.SUCCESS;
        }

		public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2()
		{
            var itemList = new List<RegistryItemCustom>
            {
                new RegistryItemCustom
                {
                    Name = "HKEY_CLASSES_ROOT",
                    Hive = RegHives.HKEY_CLASSES_ROOT,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_CURRENT_CONFIG",
                    Hive = RegHives.HKEY_CURRENT_CONFIG,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_CURRENT_USER",
                    Hive = RegHives.HKEY_CURRENT_USER,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_CURRENT_USER_LOCAL_SETTINGS",
                    Hive = RegHives.HKEY_CURRENT_USER_LOCAL_SETTINGS,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_DYN_DATA",
                    Hive = RegHives.HKEY_DYN_DATA,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_LOCAL_MACHINE",
                    Hive = RegHives.HKEY_LOCAL_MACHINE,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_PERFORMANCE_DATA",
                    Hive = RegHives.HKEY_PERFORMANCE_DATA,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "HKEY_USERS",
                    Hive = RegHives.HKEY_USERS,
                    Key = null,
                    Type = RegistryItemType.HIVE,
                    Value = null,
                    ValueType = 0
                }
            };
            return itemList;
        }

		public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key)
		{
            var itemList = new List<RegistryItemCustom>
            {
                new RegistryItemCustom
                {
                    Name = "Test 1",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.KEY,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "est 2",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.KEY,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "st 3",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.KEY,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "t 4",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.KEY,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "Test 5",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.KEY,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "est 6",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.KEY,
                    Value = null,
                    ValueType = 0
                },
                new RegistryItemCustom
                {
                    Name = "st 7",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.VALUE,
                    Value = "Test value",
                    ValueType = 1
                },
                new RegistryItemCustom
                {
                    Name = "t 8",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.VALUE,
                    Value = "Test value",
                    ValueType = 1
                },
                new RegistryItemCustom
                {
                    Name = "Test 9",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.VALUE,
                    Value = "Test value",
                    ValueType = 1
                },
                new RegistryItemCustom
                {
                    Name = "est 10",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.VALUE,
                    Value = "Test value",
                    ValueType = 1
                },
                new RegistryItemCustom
                {
                    Name = "st 11",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.VALUE,
                    Value = "Test value",
                    ValueType = 1
                },
                new RegistryItemCustom
                {
                    Name = "t 12",
                    Hive = hive,
                    Key = key,
                    Type = RegistryItemType.VALUE,
                    Value = "Test value",
                    ValueType = 1
                }
            };
            return itemList;
        }

        public Task<HelperErrorCodes> LoadHive(string FileName, string mountpoint, bool inUser)
        {
            throw new NotImplementedException();
        }

        public Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser)
        {
            throw new NotImplementedException();
        }
    }
}