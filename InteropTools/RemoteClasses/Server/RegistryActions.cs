using InteropTools.Providers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace InteropTools.RemoteClasses.Server
{
    public sealed class RegistryActions
    {
        private readonly IRegistryProvider _helper = App.MainRegistryHelper;

        public async Task<string> RegistryAction(string json, string hostname)
        {
            try
            {
                RootObject data = JsonConvert.DeserializeObject<RootObject>(json);
                bool allowed = false;
                bool denied = false;

                foreach (SessionManager.Remote item in SessionManager.AllowedRemotes)
                {
                    if (string.Equals(item.Hostname, hostname, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (string.Equals(item.SessionID, data.SessionID, StringComparison.CurrentCultureIgnoreCase))
                        {
                            allowed = true;
                        }
                    }
                }

                foreach (SessionManager.Remote item in SessionManager.DeniedRemotes)
                {
                    if (string.Equals(item.Hostname, hostname, StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (string.Equals(item.SessionID, data.SessionID, StringComparison.CurrentCultureIgnoreCase))
                        {
                            denied = true;
                        }
                    }
                }

                if (!allowed)
                {
                    if (!denied)
                    {
                        if (data.Operation == "Authentificate")
                        {
                            Windows.Foundation.IAsyncAction tsk = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                bool result = await new ContentDialogs.Core.DualMessageDialogContentDialog().ShowDualMessageDialog(SessionManager.RemoteLoc, "You can allow it to access it or deny it\n\nIP address: " + hostname,
                                             SessionManager.RemoteAllowLoc, SessionManager.RemoteDenyLoc);

                                if (result)
                                {
                                    SessionManager.AllowedRemotes.Add(new SessionManager.Remote { Hostname = hostname, SessionID = data.SessionID });
                                    allowed = true;
                                }
                                else
                                {
                                    SessionManager.DeniedRemotes.Add(new SessionManager.Remote { Hostname = hostname, SessionID = data.SessionID });
                                }
                            });

                            while (tsk.Status != Windows.Foundation.AsyncStatus.Completed)
                            {
                            }
                        }
                    }
                }

                if (allowed)
                {
                    switch (data.Operation)
                    {
                        case "Authentificate":
                            {
                                try
                                {
                                    Result result = new() { Status = "SUCCESS" };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "GetKeyValue":
                            {
                                try
                                {
                                    Enum.TryParse(data.Hive, out RegHives hive);
                                    Enum.TryParse(data.ValueType, out RegTypes type);
                                    GetKeyValueReturn error = await _helper.GetKeyValue(hive, data.Key, data.ValueName, type);

                                    string valuedata = error.regvalue;

                                    Result result = new()
                                    {
                                        Error = error.returncode.ToString(),
                                        ValueData = valuedata,
                                        ValueType = type.ToString()
                                    };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "SetKeyValue":
                            {
                                try
                                {
                                    Enum.TryParse(data.Hive, out RegHives hive);
                                    Enum.TryParse(data.ValueType, out RegTypes type);
                                    HelperErrorCodes error = await _helper.SetKeyValue(hive, data.Key, data.ValueName, type, data.ValueData);
                                    Result result = new() { Error = error.ToString() };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "GetKeyStatus":
                            {
                                try
                                {
                                    Enum.TryParse(data.Hive, out RegHives hive);
                                    KeyStatus status = await _helper.GetKeyStatus(hive, data.Key);
                                    Result result = new() { Status = status.ToString() };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "DeleteValue":
                            {
                                try
                                {
                                    Enum.TryParse(data.Hive, out RegHives hive);
                                    HelperErrorCodes error = await _helper.DeleteValue(hive, data.Key, data.ValueName);
                                    Result result = new() { Error = error.ToString() };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "DeleteKey":
                            {
                                try
                                {
                                    Enum.TryParse(data.Hive, out RegHives hive);
                                    HelperErrorCodes error = await _helper.DeleteKey(hive, data.Key, data.Recursive);
                                    Result result = new() { Error = error.ToString() };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "AddKey":
                            {
                                try
                                {
                                    Enum.TryParse(data.Hive, out RegHives hive);
                                    HelperErrorCodes error = await _helper.AddKey(hive, data.Key);
                                    Result result = new() { Error = error.ToString() };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "RenameKey":
                            {
                                try
                                {
                                    Enum.TryParse(data.Hive, out RegHives hive);
                                    HelperErrorCodes error = await _helper.RenameKey(hive, data.Key, data.NewName);
                                    Result result = new() { Error = error.ToString() };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "GetRegistryHives2":
                            {
                                try
                                {
                                    IReadOnlyList<RegistryItemCustom> regitems = await _helper.GetRegistryHives2();
                                    Result result = new();
                                    List<Item> items = regitems.Select(item => new Item
                                    {
                                        Hive = item.Hive.ToString(),
                                        Key = item.Key,
                                        Name = item.Name,
                                        Type = item.Type.ToString(),
                                        Value = item.Value,
                                        ValueType2 = item.ValueType
                                    }).ToList();
                                    result.Items = items;
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "GetRegistryItems2":
                            {
                                try
                                {
                                    Enum.TryParse(data.Hive, out RegHives hive);
                                    IReadOnlyList<RegistryItemCustom> regitems = await _helper.GetRegistryItems2(hive, data.Key);
                                    Result result = new();
                                    List<Item> items = regitems.Select(item => new Item
                                    {
                                        Hive = item.Hive.ToString(),
                                        Key = item.Key,
                                        Name = item.Name,
                                        Type = item.Type.ToString(),
                                        Value = item.Value,
                                        ValueType2 = item.ValueType
                                    }).ToList();
                                    result.Items = items;
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "GetKeyValue2":
                            {
                                try
                                {
                                    Enum.TryParse(data.Hive, out RegHives hive);
                                    uint type = data.ValueType2;
                                    GetKeyValueReturn2 error = await _helper.GetKeyValue(hive, data.Key, data.ValueName, type);
                                    string valuedata = error.regvalue;
                                    Result result = new()
                                    {
                                        Error = error.returncode.ToString(),
                                        ValueData = valuedata,
                                        ValueType2 = type
                                    };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "SetKeyValue2":
                            {
                                try
                                {
                                    Enum.TryParse(data.Hive, out RegHives hive);
                                    uint type = data.ValueType2;
                                    HelperErrorCodes error = await _helper.SetKeyValue(hive, data.Key, data.ValueName, type, data.ValueData);
                                    Result result = new() { Error = error.ToString() };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "DoesFileExists":
                            {
                                try
                                {
                                    Result result = new() { Exists = _helper.DoesFileExists(data.Path) };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "GetAppInstallationPath":
                            {
                                try
                                {
                                    Result result = new() { AppInstallationPath = _helper.GetAppInstallationPath() };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        case "GetKeyLastModifiedTime":
                            {
                                try
                                {
                                    Enum.TryParse(data.Hive, out RegHives hive);
                                    GetKeyLastModifiedTime error = await _helper.GetKeyLastModifiedTime(hive, data.Key);
                                    Result result = new() { Error = error.returncode.ToString(), LastModifiedTime = error.LastModified };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }
                        case "LoadHive":
                            {
                                try
                                {
                                    HelperErrorCodes error = await _helper.LoadHive(data.FilePath, data.mountpoint, data.inUser);
                                    Result result = new() { Error = error.ToString() };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }
                        case "UnloadHive":
                            {
                                try
                                {
                                    HelperErrorCodes error = await _helper.UnloadHive(data.mountpoint, data.inUser);
                                    Result result = new() { Error = error.ToString() };
                                    data.Result = result;
                                    string resultjson = JsonConvert.SerializeObject(data);
                                    return resultjson;
                                }
                                catch
                                {
                                    // ignored
                                }

                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }
                }
                else
                {
                    try
                    {
                        Result result = new() { Status = "FAILED" };
                        data.Result = result;
                        string resultjson = JsonConvert.SerializeObject(data);
                        return resultjson;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
            catch
            {
                // ignored
            }

            return "";
        }

        private class Item
        {
            public string Hive { get; set; }
            public string Key { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
            public string ValueType { get; set; }
            public uint ValueType2 { get; set; }
        }

        private class Result
        {
            public string AppInstallationPath { get; set; }
            public string Error { get; set; }
            public bool Exists { get; set; }
            public List<Item> Items { get; set; }
            public DateTime LastModifiedTime { get; set; }
            public string Status { get; set; }
            public string ValueData { get; set; }
            public string ValueType { get; set; }
            public uint ValueType2 { get; set; }
        }

        private class RootObject
        {
            public string FilePath { get; set; }
            public string Hive { get; set; }
            public bool inUser { get; set; }
            public string Key { get; set; }
            public string mountpoint { get; set; }
            public string NewName { get; set; }
            public string Operation { get; set; }
            public string Path { get; set; }
            public bool Recursive { get; set; }
            public Result Result { get; set; }
            public string SessionID { get; set; }
            public string ValueData { get; set; }
            public string ValueName { get; set; }
            public string ValueType { get; set; }
            public uint ValueType2 { get; set; }
        }
    }
}