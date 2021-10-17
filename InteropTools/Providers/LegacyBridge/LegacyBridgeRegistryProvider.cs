// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InteropTools.ContentDialogs.Providers;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Core;

namespace InteropTools.Providers
{
    public class LegacyBridgeRegistryProvider : IRegistryProvider
    {
        public bool UsesPlugins;
        public IRegProvider LocalProvider;
        private bool IsInitialized;

        public async Task InitializeAsync()
        {
            IRegProvider result = null;
            bool HasUIAccess = false;

            try
            {
                HasUIAccess = CoreWindow.GetForCurrentThread().Dispatcher.HasThreadAccess;
            }
            catch
            {
            }

            if (HasUIAccess)
            {
                result = await new SelectRegistryProviderContentDialog().AskUserForProvider();
            }
            else
            {
                result = await DispatcherHelper.ExecuteOnUIThreadAsync(async () =>
                    await new SelectRegistryProviderContentDialog().AskUserForProvider());
            }

            if (result == null)
            {
                UsesPlugins = true;
                LocalProvider = null;
            }
            else
            {
                UsesPlugins = false;
                LocalProvider = result;
            }

            IsInitialized = true;
        }

        public async Task<HelperErrorCodes>
            ExecuteAction(Func<IRegProvider, Task<HelperErrorCodes>> providerFunctionCall) =>
            await ExecuteAction(providerFunctionCall, (t) => t, (t) => t, (t) => t);

        public async Task<T1> ExecuteAction<T1, T2>(Func<IRegProvider, Task<T2>> providerFunctionCall,
            Func<T2, T1> typeConverterCall, Func<T2, HelperErrorCodes> typeStatusConverter,
            Func<HelperErrorCodes, T1> statusTypeConverter, bool SecondCall = false)
        {
            try
            {
                if (!IsInitialized)
                {
                    await InitializeAsync();
                }

                if (!UsesPlugins)
                {
                    T2 result = await providerFunctionCall(LocalProvider);

                    if (typeStatusConverter(result) == HelperErrorCodes.NotImplemented && !SecondCall)
                    {
                        await InitializeAsync();
                        return await ExecuteAction(providerFunctionCall, typeConverterCall, typeStatusConverter,
                            statusTypeConverter, true);
                    }
                    else
                    {
                        return typeConverterCall(result);
                    }
                }
                else
                {
                    bool hadaccessdenied = false;
                    bool hadfailed = false;

                    using (AppPlugin.PluginList.PluginList<string, string, double> reglist =
                        await Registry.Definition.RegistryProvidersWithOptions.ListAsync(Registry.Definition
                            .RegistryProvidersWithOptions.PLUGIN_NAME))
                    {
                        foreach (AppPlugin.PluginList.PluginList<string, string, double>.PluginProvider plugin in
                            reglist.Plugins)
                        {
                            RegistryProvider provider = new(plugin);

                            T2 result = await providerFunctionCall(provider);

                            if (typeStatusConverter(result) == HelperErrorCodes.Success)
                            {
                                reglist.Dispose();
                                return typeConverterCall(result);
                            }

                            if (typeStatusConverter(result) == HelperErrorCodes.NotImplemented)
                            {
                                continue;
                            }

                            if (typeStatusConverter(result) == HelperErrorCodes.AccessDenied)
                            {
                                hadaccessdenied = true;
                                continue;
                            }

                            if (typeStatusConverter(result) == HelperErrorCodes.Failed)
                            {
                                hadfailed = true;
                                continue;
                            }
                        }
                    }

                    if (hadaccessdenied)
                    {
                        return statusTypeConverter(HelperErrorCodes.AccessDenied);
                    }

                    if (hadfailed)
                    {
                        return statusTypeConverter(HelperErrorCodes.Failed);
                    }

                    return statusTypeConverter(HelperErrorCodes.NotImplemented);
                }
            }
            catch
            {
            }

            return statusTypeConverter(HelperErrorCodes.Failed);
        }

        public async Task<HelperErrorCodes> AddKey(RegHives hive, string key) =>
            await ExecuteAction((t) => t.RegAddKey(hive, key));

        public bool AllowsRegistryEditing() => true;

        public async Task<HelperErrorCodes> DeleteKey(RegHives hive, string key, bool recursive) =>
            await ExecuteAction((t) => t.RegDeleteKey(hive, key, recursive));

        public async Task<HelperErrorCodes> DeleteValue(RegHives hive, string key, string keyvalue) =>
            await ExecuteAction((t) => t.RegDeleteValue(hive, key, keyvalue));

        public bool DoesFileExists(string path)
        {
            bool fileexists;
            try
            {
                fileexists = File.Exists(path);
            }
            catch (InvalidOperationException)
            {
                fileexists = true;
            }

            return fileexists;
        }

        public string GetAppInstallationPath() => Windows.ApplicationModel.Package.Current.InstalledLocation.Path;

        public string GetDescription() => "This device (through provider extensions)";

        public string GetFriendlyName() => "This device (through provider extensions)";

        public string GetHostName() => "127.0.0.1";

        public async Task<GetKeyLastModifiedTime> GetKeyLastModifiedTime(RegHives hive, string key) =>
            await ExecuteAction((t) => t.RegQueryKeyLastModifiedTime(hive, key),
                (t) => new GetKeyLastModifiedTime()
                {
                    LastModified = new DateTime(t.LastModified), returncode = t.returncode
                }, (t) => t.returncode,
                (t) => new GetKeyLastModifiedTime() {LastModified = new DateTime(), returncode = t});

        public async Task<KeyStatus> GetKeyStatus(RegHives hive, string key) =>
            await ExecuteAction(
                (Func<IRegProvider, Task<KeyStatus>>)((t) =>
                    (Task<KeyStatus>)t.RegQueryKeyStatus((RegHives)hive, (string)key)), (t) => t,
                (Func<KeyStatus, HelperErrorCodes>)((t) =>
                {
                    switch (t)
                    {
                        case KeyStatus.Found:
                            return HelperErrorCodes.Success;
                        case KeyStatus.NotFound:
                            return HelperErrorCodes.Failed;
                        case KeyStatus.AccessDenied:
                            return HelperErrorCodes.AccessDenied;
                        case KeyStatus.Unknown:
                            return HelperErrorCodes.NotImplemented;
                    }

                    return HelperErrorCodes.NotImplemented;
                }), (Func<HelperErrorCodes, KeyStatus>)((t) =>
                {
                    switch (t)
                    {
                        case HelperErrorCodes.Success:
                            return (KeyStatus)KeyStatus.Found;
                        case HelperErrorCodes.Failed:
                            return (KeyStatus)KeyStatus.NotFound;
                        case HelperErrorCodes.AccessDenied:
                            return (KeyStatus)KeyStatus.AccessDenied;
                        case HelperErrorCodes.NotImplemented:
                            return (KeyStatus)KeyStatus.Unknown;
                    }

                    return (KeyStatus)KeyStatus.Unknown;
                }));

        public async Task<GetKeyValueReturn> GetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type) =>
            await ExecuteAction((t) => t.RegQueryValue(hive, key, keyvalue, type),
                (t) => new GetKeyValueReturn() {regtype = t.regtype, regvalue = t.regvalue, returncode = t.returncode},
                (t) => t.returncode,
                (t) => new GetKeyValueReturn() {regtype = RegTypes.REG_ERROR, regvalue = "", returncode = t});

        public async Task<GetKeyValueReturn2> GetKeyValue(RegHives hive, string key, string keyvalue, uint type) =>
            await ExecuteAction((t) => t.RegQueryValue(hive, key, keyvalue, type),
                (t) => new GetKeyValueReturn2() {regtype = t.regtype, regvalue = t.regvalue, returncode = t.returncode},
                (t) => t.returncode, (t) => new GetKeyValueReturn2() {regtype = 0, regvalue = "", returncode = t});

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryHives2() => await ExecuteAction(
            (t) => t.RegEnumKey(null, ""), (t) => t.items, (t) => t.returncode, (t) => new List<RegistryItemCustom>());

        public async Task<IReadOnlyList<RegistryItemCustom>> GetRegistryItems2(RegHives hive, string key) =>
            await ExecuteAction((t) => t.RegEnumKey(hive, key), (t) => t.items, (t) => t.returncode,
                (t) => new List<RegistryItemCustom>());

        public string GetSymbol() => "";

        public string GetTitle() => "This device (through provider extensions)";

        public bool IsLocal() => true;

        public async Task<HelperErrorCodes> LoadHive(string FileName, string mountpoint, bool inUser) =>
            await ExecuteAction((t) => t.RegLoadHive(FileName, mountpoint, inUser));

        public async Task<HelperErrorCodes> RenameKey(RegHives hive, string key, string newname) =>
            await ExecuteAction((t) => t.RegRenameKey(hive, key, newname));

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, RegTypes type,
            string data) => await ExecuteAction((t) => t.RegSetValue(hive, key, keyvalue, type, data));

        public async Task<HelperErrorCodes> SetKeyValue(RegHives hive, string key, string keyvalue, uint type,
            string data) => await ExecuteAction((t) => t.RegSetValue(hive, key, keyvalue, type, data));

        public async Task<HelperErrorCodes> UnloadHive(string mountpoint, bool inUser) =>
            await ExecuteAction((t) => t.RegUnloadHive(mountpoint, inUser));
    }
}
