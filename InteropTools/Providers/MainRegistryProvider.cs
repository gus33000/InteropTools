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

        private bool ProgressShown;

        private async Task ShowStatusBarInfoAsync(string text, bool show)
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

                if (show && !ProgressShown)
                {
                    if (isinuithread)
                    {
                        Windows.UI.ViewManagement.StatusBar currentView = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();

                        await currentView.ProgressIndicator.ShowAsync();
#if DEBUG
                        currentView.ProgressIndicator.Text = "DEBUG: " + text;
#else
                        currentView.ProgressIndicator.Text = "Working...";
#endif
                    }
                    else
                    {
                        await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                        {
                            Windows.UI.ViewManagement.StatusBar currentView = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();

                            await currentView.ProgressIndicator.ShowAsync();
#if DEBUG
                            currentView.ProgressIndicator.Text = "DEBUG: " + text;
#else
                            currentView.ProgressIndicator.Text = "Working...";
#endif
                        });
                    }
                }
                else if (!show && ProgressShown)
                {
                    if (isinuithread)
                    {
                        await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
                    }
                    else
                    {
                        await DispatcherHelper.ExecuteOnUIThreadAsync(async () => await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.HideAsync());
                    }
                }

                ProgressShown = show;
            }
        }

        public async Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type)
        {
            await ShowStatusBarInfoAsync("GetKeyValue", true);

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

            await ShowStatusBarInfoAsync(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type, string data)
        {
            await ShowStatusBarInfoAsync("SetKeyValue", true);

            HelperErrorCodes ret = await App.RegistryHelper.SetKeyValue(hive, key, keyvalue, type, data);

            await ShowStatusBarInfoAsync(null, false);

            return ret;
        }

        public async Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type)
        {
            await ShowStatusBarInfoAsync("GetKeyValue", true);

            GetKeyValueReturn2 ret = await App.RegistryHelper.GetKeyValue(hive, key, keyvalue, type);

            await ShowStatusBarInfoAsync(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type, string data)
        {
            await ShowStatusBarInfoAsync("SetKeyValue", true);

            HelperErrorCodes ret = await App.RegistryHelper.SetKeyValue(hive, key, keyvalue, type, data);

            await ShowStatusBarInfoAsync(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue)
        {
            await ShowStatusBarInfoAsync("DeleteValue", true);

            HelperErrorCodes ret = await App.RegistryHelper.DeleteValue(hive, key, keyvalue);

            await ShowStatusBarInfoAsync(null, false);

            return ret;
        }

        public async Task<KeyStatus> GetKeyStatus(RegHives hive, string key)
        {
            await ShowStatusBarInfoAsync("GetKeyStatus", true);

            KeyStatus ret = await App.RegistryHelper.GetKeyStatus(hive, key);

            await ShowStatusBarInfoAsync(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> AddKey(RegHives hive, string key)
        {
            await ShowStatusBarInfoAsync("AddKey", true);

            HelperErrorCodes ret = await App.RegistryHelper.AddKey(hive, key);

            await ShowStatusBarInfoAsync(null, false);

            return ret;
        }

        public async Task<GetKeyLastModifiedTime> GetKeyLastModifiedTime(RegHives hive, string key)
        {
            await ShowStatusBarInfoAsync("GetKeyLastModifiedTime", true);

            GetKeyLastModifiedTime ret = await App.RegistryHelper.GetKeyLastModifiedTime(hive, key);

            await ShowStatusBarInfoAsync(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive)
        {
            await ShowStatusBarInfoAsync("DeleteKey", true);

            HelperErrorCodes ret = await App.RegistryHelper.DeleteKey(hive, key, recursive);

            await ShowStatusBarInfoAsync(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname)
        {
            await ShowStatusBarInfoAsync("RenameKey", true);

            HelperErrorCodes ret = await App.RegistryHelper.RenameKey(hive, key, newname);

            await ShowStatusBarInfoAsync(null, false);

            return ret;
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2()
        {
            await ShowStatusBarInfoAsync("GetRegistryHives2", true);

            IReadOnlyList<RegistryItemCustom> ret = await App.RegistryHelper.GetRegistryHives2();

            await ShowStatusBarInfoAsync(null, false);

            return ret;
        }

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key)
        {
            await ShowStatusBarInfoAsync("GetRegistryItems2", true);

            IReadOnlyList<RegistryItemCustom> ret = await App.RegistryHelper.GetRegistryItems2(hive, key);

            await ShowStatusBarInfoAsync(null, false);

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
            await ShowStatusBarInfoAsync("LoadHive", true);

            HelperErrorCodes ret = await App.RegistryHelper.LoadHive(FileName, mountpoint, inUser);

            await ShowStatusBarInfoAsync(null, false);

            return ret;
        }

        public async Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser)
        {
            await ShowStatusBarInfoAsync("UnloadHive", true);

            HelperErrorCodes ret = await App.RegistryHelper.UnloadHive(mountpoint, inUser);

            await ShowStatusBarInfoAsync(null, false);

            return ret;
        }
    }
}