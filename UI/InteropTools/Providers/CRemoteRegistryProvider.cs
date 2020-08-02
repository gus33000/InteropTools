using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InteropTools.RemoteClasses.Client;
using Newtonsoft.Json;
using Windows.ApplicationModel.Resources.Core;
using Windows.UI.Xaml;

namespace InteropTools.Providers
{
    public sealed class CRemoteRegistryProvider : IRegistryProvider
    {
        private readonly RemoteClient _client;
        private readonly CCMDProvider _cmdprov = new CCMDProvider();

        private readonly string _hostname;
        private bool _initialized;
        private readonly int _portnumber;

        private bool _useCmd;

        public CRemoteRegistryProvider(string hostname, int portnumber)
        {
            this._hostname = hostname;
            this._portnumber = portnumber;
            _client = new RemoteClient(hostname, portnumber);
        }

        public string GetTitle()
        {
            return ResourceManager.Current.MainResourceMap.GetValue("Resources/Remote_Device", ResourceContext.GetForCurrentView()).ValueAsString;
        }

        public string GetDescription()
        {
            return
              ResourceManager.Current.MainResourceMap.GetValue("Resources/Connects_to_a_remote_device_which_has_remote_access_enabled__Level_of_access_is_subject_to_the_remote_device",
                  ResourceContext.GetForCurrentView()).ValueAsString;
        }

        public string GetSymbol()
        {
            return "";
        }

        public async Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type)
        {
            var ret = new GetKeyValueReturn();
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "GetKeyValue",
                Hive = hive.ToString(),
                Key = key,
                ValueName = keyvalue,
                ValueType = type.ToString()
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata != null)
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                    ret.regtype = (RegTypes)Enum.Parse(typeof(RegTypes), data.Result.ValueType);
                    ret.regvalue = data.Result.ValueData;
                    ret.returncode = (HelperErrorCodes)Enum.Parse(typeof(HelperErrorCodes), data.Result.Error);
                    return ret;
                }

                catch
                {
                    ret.regtype = RegTypes.REG_ERROR;
                    ret.regvalue = "";
                    ret.returncode = HelperErrorCodes.FAILED;
                    return ret;
                }
            }

            ret.regtype = RegTypes.REG_ERROR;
            ret.regvalue = "";
            ret.returncode = HelperErrorCodes.FAILED;
            return ret;
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type, string data)
        {
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "SetKeyValue",
                Hive = hive.ToString(),
                Key = key,
                ValueName = keyvalue,
                ValueType = type.ToString(),
                ValueData = data
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return HelperErrorCodes.FAILED;
            }

            try
            {
                var data_ = JsonConvert.DeserializeObject<RootObject>(replydata);
                return (HelperErrorCodes)Enum.Parse(typeof(HelperErrorCodes), data_.Result.Error);
            }

            catch
            {
                return HelperErrorCodes.FAILED;
            }
        }

        public async Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue)
        {
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "DeleteValue",
                Hive = hive.ToString(),
                Key = key,
                ValueName = keyvalue
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return HelperErrorCodes.FAILED;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                return (HelperErrorCodes)Enum.Parse(typeof(HelperErrorCodes), data.Result.Error);
            }

            catch
            {
                return HelperErrorCodes.FAILED;
            }
        }

        public async Task<KeyStatus> GetKeyStatus(RegHives hive, string key)
        {
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "GetKeyStatus",
                Hive = hive.ToString(),
                Key = key
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return KeyStatus.UNKNOWN;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                return (KeyStatus)Enum.Parse(typeof(KeyStatus), data.Result.Status);
            }

            catch
            {
                return KeyStatus.UNKNOWN;
            }
        }

        public async Task<HelperErrorCodes> AddKey(RegHives hive, string key)
        {
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "AddKey",
                Hive = hive.ToString(),
                Key = key
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return HelperErrorCodes.FAILED;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                return (HelperErrorCodes)Enum.Parse(typeof(HelperErrorCodes), data.Result.Error);
            }

            catch
            {
                return HelperErrorCodes.FAILED;
            }
        }

        public async Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive)
        {
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "DeleteKey",
                Hive = hive.ToString(),
                Recursive = recursive,
                Key = key
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return HelperErrorCodes.FAILED;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                return (HelperErrorCodes)Enum.Parse(typeof(HelperErrorCodes), data.Result.Error);
            }

            catch
            {
                return HelperErrorCodes.FAILED;
            }
        }

        public async Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname)
        {
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "RenameKey",
                Hive = hive.ToString(),
                Key = key,
                NewName = newname
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return HelperErrorCodes.FAILED;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                return (HelperErrorCodes)Enum.Parse(typeof(HelperErrorCodes), data.Result.Error);
            }

            catch
            {
                return HelperErrorCodes.FAILED;
            }
        }

        public async Task<IReadOnlyList<RegistryItem>> GetRegistryHives()
        {
            var itemList = new List<RegistryItem>();
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "GetRegistryHives"
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return itemList;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                itemList.AddRange(data.Result.Items.Select(item => new RegistryItem
                {
                    Hive = (RegHives)Enum.Parse(typeof(RegHives), item.Hive),
                    Key = item.Key,
                    Name = item.Name,
                    Type = (RegistryItemType)Enum.Parse(typeof(RegistryItemType), item.Type),
                    Value = item.Value,
                    ValueType = (RegTypes)Enum.Parse(typeof(RegTypes), item.ValueType)
                }));
            }

            catch
            {
                // ignored
            }

            return itemList;
        }

        public async Task<IReadOnlyList<RegistryItem>> GetRegistryItems(RegHives hive, string key)
        {
            if (!_initialized)
            {
                _useCmd = await App.IsCMDSupported();
                _initialized = true;
            }

            if (_useCmd)
            {
                var itemsList = await _cmdprov.GetRegistryItems(hive, key);
                return itemsList;
            }

            var itemList = new List<RegistryItem>();
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "GetRegistryItems",
                Hive = hive.ToString(),
                Key = key
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return itemList;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                itemList.AddRange(data.Result.Items.Select(item => new RegistryItem
                {
                    Hive = (RegHives)Enum.Parse(typeof(RegHives), item.Hive),
                    Key = item.Key,
                    Name = item.Name,
                    Type = (RegistryItemType)Enum.Parse(typeof(RegistryItemType), item.Type),
                    Value = item.Value,
                    ValueType = (RegTypes)Enum.Parse(typeof(RegTypes), item.ValueType)
                }));
            }

            catch
            {
                // ignored
            }

            return itemList;
        }

        public bool DoesFileExists(string path)
        {
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "DoesFileExists",
                Path = path
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return false;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                return data.Result.Exists;
            }

            catch
            {
                return false;
            }
        }

        public string GetAppInstallationPath()
        {
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "GetAppInstallationPath"
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return "";
            }

            try
            {
                var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                return data.Result.AppInstallationPath;
            }

            catch
            {
                return "";
            }
        }

        public string GetHostName()
        {
            return _hostname;
        }

        public bool IsLocal()
        {
            return false;
        }

        public bool AllowsRegistryEditing()
        {
            return true;
        }

        public string GetFriendlyName()
        {
            return _hostname + ":" + _portnumber;
        }

        public async Task<GetKeyLastModifiedTime> GetKeyLastModifiedTime(RegHives hive, String key)
        {
            var ret = new GetKeyLastModifiedTime();
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "GetKeyLastModifiedTime",
                Hive = hive.ToString(),
                Key = key
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                ret.LastModified = new DateTime();
                ret.returncode = HelperErrorCodes.FAILED;
                return ret;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                ret.LastModified = data.Result.LastModifiedTime;
                ret.returncode = (HelperErrorCodes)Enum.Parse(typeof(HelperErrorCodes), data.Result.Error);
                return ret;
            }

            catch
            {
                ret.LastModified = new DateTime();
                ret.returncode = HelperErrorCodes.FAILED;
                return ret;
            }
        }

        public async Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type)
        {
            var ret = new GetKeyValueReturn2();
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "GetKeyValue2",
                Hive = hive.ToString(),
                Key = key,
                ValueName = keyvalue,
                ValueType = type.ToString()
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata != null)
            {
                try
                {
                    var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                    ret.regtype = data.Result.ValueType2;
                    ret.regvalue = data.Result.ValueData;
                    ret.returncode = (HelperErrorCodes)Enum.Parse(typeof(HelperErrorCodes), data.Result.Error);
                    return ret;
                }

                catch
                {
                    ret.regtype = 0;
                    ret.regvalue = "";
                    ret.returncode = HelperErrorCodes.FAILED;
                    return ret;
                }
            }

            ret.regtype = 0;
            ret.regvalue = "";
            ret.returncode = HelperErrorCodes.FAILED;
            return ret;
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type, string data)
        {
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "SetKeyValue2",
                Hive = hive.ToString(),
                Key = key,
                ValueName = keyvalue,
                ValueType2 = type,
                ValueData = data
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return HelperErrorCodes.FAILED;
            }

            try
            {
                var data_ = JsonConvert.DeserializeObject<RootObject>(replydata);
                return (HelperErrorCodes)Enum.Parse(typeof(HelperErrorCodes), data_.Result.Error);
            }

            catch
            {
                return HelperErrorCodes.FAILED;
            }
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2()
        {
            var itemList = new List<RegistryItemCustom>();
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "GetRegistryHives2"
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return itemList;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                itemList.AddRange(data.Result.Items.Select(item => new RegistryItemCustom
                {
                    Hive = (RegHives)Enum.Parse(typeof(RegHives), item.Hive),
                    Key = item.Key,
                    Name = item.Name,
                    Type = (RegistryItemType)Enum.Parse(typeof(RegistryItemType), item.Type),
                    Value = item.Value,
                    ValueType = item.ValueType2
                }));
            }

            catch
            {
                // ignored
            }

            return itemList;
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key)
        {
            if (!_initialized)
            {
                _useCmd = await App.IsCMDSupported();
                _initialized = true;
            }

            if (_useCmd)
            {
                var itemsList = await _cmdprov.GetRegistryItems2(hive, key);
                return itemsList;
            }

            var itemList = new List<RegistryItemCustom>();
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "GetRegistryItems2",
                Hive = hive.ToString(),
                Key = key
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return itemList;
            }

            try
            {
                var data = JsonConvert.DeserializeObject<RootObject>(replydata);
                itemList.AddRange(data.Result.Items.Select(item => new RegistryItemCustom
                {
                    Hive = (RegHives)Enum.Parse(typeof(RegHives), item.Hive),
                    Key = item.Key,
                    Name = item.Name,
                    Type = (RegistryItemType)Enum.Parse(typeof(RegistryItemType), item.Type),
                    Value = item.Value,
                    ValueType = item.ValueType2
                }));
            }

            catch
            {
                // ignored
            }

            return itemList;
        }

        public async Task<HelperErrorCodes> LoadHive(string FileName, string mountpoint, bool inUser)
        {
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "LoadHive",
                FilePath = FileName,
                mountpoint = mountpoint,
                inUser = inUser
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return HelperErrorCodes.FAILED;
            }

            try
            {
                var data_ = JsonConvert.DeserializeObject<RootObject>(replydata);
                return (HelperErrorCodes)Enum.Parse(typeof(HelperErrorCodes), data_.Result.Error);
            }

            catch
            {
                return HelperErrorCodes.FAILED;
            }
        }

        public async Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser)
        {
            var jsonObject = new RootObject
            {
                SessionId = App.SessionId,
                Operation = "UnloadHive",
                mountpoint = mountpoint,
                inUser = inUser
            };
            var replydata = AsyncHelper.RunSync(() => _client.GetData(JsonConvert.SerializeObject(jsonObject)));

            if (replydata == null)
            {
                return HelperErrorCodes.FAILED;
            }

            try
            {
                var data_ = JsonConvert.DeserializeObject<RootObject>(replydata);
                return (HelperErrorCodes)Enum.Parse(typeof(HelperErrorCodes), data_.Result.Error);
            }

            catch
            {
                return HelperErrorCodes.FAILED;
            }
        }

        private class Item
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Hive { get; set; }
            public string Key { get; set; }
            public string Value { get; set; }
            public string ValueType { get; set; }
            public uint ValueType2 { get; set; }
        }

        private class Result
        {
            public string Error { get; set; }
            public bool Exists { get; set; }
            public string ValueData { get; set; }
            public string ValueType { get; set; }
            public uint ValueType2 { get; set; }
            public string Status { get; set; }

            public List<Item> Items { get; set; }

            public string AppInstallationPath { get; set; }

            public DateTime LastModifiedTime { get; set; }
        }

        private class RootObject
        {
            public string SessionId { get; set; }
            public string Operation { get; set; }
            public string Path { get; set; }
            public string Hive { get; set; }
            public string Key { get; set; }
            public string ValueName { get; set; }
            public string ValueType { get; set; }
            public uint ValueType2 { get; set; }
            public string ValueData { get; set; }
            public bool Recursive { get; set; }
            public string NewName { get; set; }
            public Result Result { get; set; }
            public string FilePath { get; set; }
            public string mountpoint { get; set; }
            public bool inUser { get; set; }
        }

        private static class AsyncHelper
        {
            private static readonly TaskFactory MyTaskFactory = new
            TaskFactory(CancellationToken.None,
                        TaskCreationOptions.None,
                        TaskContinuationOptions.None,
                        TaskScheduler.Default);

            public static TResult RunSync<TResult>(Func<Task<TResult>> func)
            {
                return AsyncHelper.MyTaskFactory
                       .StartNew<Task<TResult>>(func)
                       .Unwrap<TResult>()
                       .GetAwaiter()
                       .GetResult();
            }
        }
    }
}