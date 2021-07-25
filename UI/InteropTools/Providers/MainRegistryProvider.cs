using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.Core;

namespace InteropTools.Providers
{
    public class MainRegistryProvider : IRegistryProvider
    {
        public class HistoryItem
        {
            // Function name
            public HistoryOperation Operation { get; set; }

            // Function in objects
            public RegHives? Hive { get; set; }
            public string Key { get; set; }
            public string ValueName { get; set; }
            public RegTypes? Type { get; set; }
            public uint Type2 { get; set; }
            public string Data { get; set; }
            public string NewKeyName { get; set; }
            public bool? DeleteKeyRecursive { get; set; }

            // Function out objects
            public RegTypes? RetType { get; set; }
            public uint RetType2 { get; set; }
            public string RetData { get; set; }
            public DateTime RetLastModified { get; set; }

            // Function return objects
            public HelperErrorCodes? RetErrorCode { get; set; }
            public IReadOnlyList<RegistryItem> RetRegistryItems { get; set; }
            public IReadOnlyList<RegistryItemCustom> RetRegistryItems2 { get; set; }
        }

        public enum HistoryOperation
        {
            AddKey,
            DeleteKey,
            DeleteValue,
            GetKeyLastModifiedTime,
            GetKeyStatus,
            GetKeyValue,
            GetRegistryHives,
            GetRegistryItems,
            RenameKey,
            SetKeyValue
        }

        public bool IsLocal()
        {
            return App.RegistryHelper.IsLocal();
        }

        public bool AllowsRegistryEditing()
        {
            try
            {
                return App.RegistryHelper.AllowsRegistryEditing();
            }
            catch
            {
                return false;
            }
        }

        public string GetFriendlyName()
        {
            return App.RegistryHelper.GetFriendlyName();
        }

        public string GetHostName()
        {
            return App.RegistryHelper.GetHostName();
        }

        public string GetTitle()
        {
            return App.RegistryHelper.GetTitle();
        }

        public string GetDescription()
        {
            return App.RegistryHelper.GetDescription();
        }

        public string GetSymbol()
        {
            return App.RegistryHelper.GetSymbol();
        }

        private async void ShowStatusBarInfo(string text, bool show)
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                bool isinuithread = false;

                try
                {
                    isinuithread = CoreWindow.GetForCurrentThread().Dispatcher.HasThreadAccess;
                }
                catch
                {

                }

                if (show)
                {
                    if (isinuithread)
                    {

                        await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
#if DEBUG
                        Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.Text = "DEBUG: " + text;
#else
                        Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.Text = "Working...";
#endif
                    }
                    else
                    {
                        await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                        {
                            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
#if DEBUG
                            Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.Text = "DEBUG: " + text;
#else
                            Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.Text = "Working...";
#endif
                        });
                    }
                }
                else
                {
                    if (isinuithread)
                    {
                        await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
                    }
                    else
                    {
                        await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                        {
                            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
                        });
                    }
                }
            }
        }

        public async Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type)
        {
            ShowStatusBarInfo("GetKeyValue", true);

            GetKeyValueReturn ret = await App.RegistryHelper.GetKeyValue(hive, key, keyvalue, type);
            _ = new HistoryItem()
            {
                Operation = HistoryOperation.GetKeyValue,
                Hive = hive,
                Key = key,
                ValueName = keyvalue,
                Type = type,
                RetType = ret.regtype,
                RetData = ret.regvalue,
                RetErrorCode = ret.returncode
            };

            ShowStatusBarInfo(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type, string data)
        {
            ShowStatusBarInfo("SetKeyValue", true);

            HelperErrorCodes ret = await App.RegistryHelper.SetKeyValue(hive, key, keyvalue, type, data);

            ShowStatusBarInfo(null, false);

            return ret;
        }

        public async Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type)
        {
            ShowStatusBarInfo("GetKeyValue", true);

            GetKeyValueReturn2 ret = await App.RegistryHelper.GetKeyValue(hive, key, keyvalue, type);

            ShowStatusBarInfo(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type, string data)
        {
            ShowStatusBarInfo("SetKeyValue", true);

            HelperErrorCodes ret = await App.RegistryHelper.SetKeyValue(hive, key, keyvalue, type, data);

            ShowStatusBarInfo(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue)
        {
            ShowStatusBarInfo("DeleteValue", true);

            HelperErrorCodes ret = await App.RegistryHelper.DeleteValue(hive, key, keyvalue);

            ShowStatusBarInfo(null, false);

            return ret;
        }

        public async Task<KeyStatus> GetKeyStatus(RegHives hive, string key)
        {
            ShowStatusBarInfo("GetKeyStatus", true);

            KeyStatus ret = await App.RegistryHelper.GetKeyStatus(hive, key);

            ShowStatusBarInfo(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> AddKey(RegHives hive, string key)
        {
            ShowStatusBarInfo("AddKey", true);

            HelperErrorCodes ret = await App.RegistryHelper.AddKey(hive, key);

            ShowStatusBarInfo(null, false);

            return ret;
        }

        public async Task<GetKeyLastModifiedTime> GetKeyLastModifiedTime(RegHives hive, string key)
        {
            ShowStatusBarInfo("GetKeyLastModifiedTime", true);

            GetKeyLastModifiedTime ret = await App.RegistryHelper.GetKeyLastModifiedTime(hive, key);

            ShowStatusBarInfo(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive)
        {
            ShowStatusBarInfo("DeleteKey", true);

            HelperErrorCodes ret = await App.RegistryHelper.DeleteKey(hive, key, recursive);

            ShowStatusBarInfo(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname)
        {
            ShowStatusBarInfo("RenameKey", true);

            HelperErrorCodes ret = await App.RegistryHelper.RenameKey(hive, key, newname);

            ShowStatusBarInfo(null, false);

            return ret;
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2()
        {
            ShowStatusBarInfo("GetRegistryHives2", true);

            IReadOnlyList<RegistryItemCustom> ret = await App.RegistryHelper.GetRegistryHives2();

            ShowStatusBarInfo(null, false);

            return ret;
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key)
        {
            ShowStatusBarInfo("GetRegistryItems2", true);

            IReadOnlyList<RegistryItemCustom> ret = await App.RegistryHelper.GetRegistryItems2(hive, key);

            ShowStatusBarInfo(null, false);

            return ret;
        }

        public bool DoesFileExists(string path)
        {
            return App.RegistryHelper.DoesFileExists(path);
        }

        public string GetAppInstallationPath()
        {
            return App.RegistryHelper.GetAppInstallationPath();
        }

        public async Task<HelperErrorCodes> LoadHive(string FileName, string mountpoint, bool inUser)
        {
            ShowStatusBarInfo("LoadHive", true);

            HelperErrorCodes ret = await App.RegistryHelper.LoadHive(FileName, mountpoint, inUser);

            ShowStatusBarInfo(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser)
        {
            ShowStatusBarInfo("UnloadHive", true);

            HelperErrorCodes ret = await App.RegistryHelper.UnloadHive(mountpoint, inUser);

            ShowStatusBarInfo(null, false);

            return ret;
        }
    }
}
