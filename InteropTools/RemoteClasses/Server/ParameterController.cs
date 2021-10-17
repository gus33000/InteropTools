// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using InteropTools.Providers;
using Restup.Webserver.Attributes;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Windows.ApplicationModel;

namespace InteropTools.RemoteClasses.Server
{
    [RestController(InstanceCreationType.Singleton)]
    public class ParameterController
    {
        [UriFormat("/registry/addkey?hive={hive2}&key={key}")]
        public async Task<IGetResponse> AddKey(string hive2, string key)
        {
            Enum.TryParse(hive2, out RegHives hive);

            HelperErrorCodes resp = await App.MainRegistryHelper.AddKey(hive, key);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/deletekey?hive={hive2}&key={key}&recursive={recursive2}")]
        public async Task<IGetResponse> DeleteKey(string hive2, string key, string recursive2)
        {
            Enum.TryParse(hive2, out RegHives hive);
            bool.TryParse(recursive2, out bool recursive);

            HelperErrorCodes resp = await App.MainRegistryHelper.DeleteKey(hive, key, recursive);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/deletevalue?hive={hive2}&key={key}&valuename={valuename}")]
        public async Task<IGetResponse> DeleteValue(string hive2, string key, string valuename)
        {
            Enum.TryParse(hive2, out RegHives hive);

            HelperErrorCodes resp = await App.MainRegistryHelper.DeleteValue(hive, key, valuename);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/core/getappbuildstring")]
        public IGetResponse GetAppBuildString()
        {
            Assembly assembly = GetType().GetTypeInfo().Assembly;
            Stream resource = assembly.GetManifestResourceStream("InteropTools.Resources.BuildDate.txt");
            string builddate = new StreamReader(resource).ReadLine().Replace("\r", "");
            PackageVersion appver = Package.Current.Id.Version;
            string appverstr = string.Format("{0}.{1}.{2}.{3}", appver.Major, appver.Minor, appver.Build,
                appver.Revision);
            string buildString = appverstr + " (fbl_prerelease(gustavem)";
            Type myType = Type.GetType("InteropTools.ShellPages.Private.YourWindowsBuildPage");

            if (myType != null)
            {
                buildString += "/private";
            }

            buildString = buildString + "." + builddate + ")";

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                buildString);
        }

        [UriFormat("/deviceinfo/getdevicefamilly")]
        public IGetResponse GetDeviceFamilly()
        {
            string devicefamilly = Classes.DeviceInfo.Instance.DeviceFamily;

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                devicefamilly);
        }

        [UriFormat("/deviceinfo/getdevicehostname")]
        public IGetResponse GetDeviceHostname()
        {
            string hostname = Classes.DeviceInfo.Instance.FriendlyName;

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                hostname);
        }

        [UriFormat("/registry/getkeylastmodifiedtime?hive={hive2}&key={key}")]
        public async Task<IGetResponse> GetKeyLastModifiedTime(string hive2, string key)
        {
            Enum.TryParse(hive2, out RegHives hive);

            GetKeyLastModifiedTime resp = await App.MainRegistryHelper.GetKeyLastModifiedTime(hive, key);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/getkeystatus?hive={hive2}&key={key}")]
        public async Task<IGetResponse> GetKeyStatus(string hive2, string key)
        {
            Enum.TryParse(hive2, out RegHives hive);

            KeyStatus resp = await App.MainRegistryHelper.GetKeyStatus(hive, key);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/getkeyvalue?hive={hive2}&key={key}&valuename={valuename}&type={type2}")]
        public async Task<IGetResponse> GetKeyValue(string hive2, string key, string valuename, string type2)
        {
            Enum.TryParse(hive2, out RegHives hive);
            Enum.TryParse(type2, out RegTypes type);

            GetKeyValueReturn resp = await App.MainRegistryHelper.GetKeyValue(hive, key, valuename, type);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/getkeyvalue2?hive={hive2}&key={key}&valuename={valuename}&type={type2}")]
        public async Task<IGetResponse> GetKeyValue2(string hive2, string key, string valuename, string type2)
        {
            Enum.TryParse(hive2, out RegHives hive);
            uint.TryParse(type2, out uint type);

            GetKeyValueReturn2 resp = await App.MainRegistryHelper.GetKeyValue(hive, key, valuename, type);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/getregistryhives2")]
        public async Task<IGetResponse> GetRegistryHives2()
        {
            IReadOnlyList<RegistryItemCustom> resp = await App.MainRegistryHelper.GetRegistryHives2();

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/getregistryitems2?hive={hive2}&key={key}")]
        public async Task<IGetResponse> GetRegistryItems2(string hive2, string key)
        {
            Enum.TryParse(hive2, out RegHives hive);

            IReadOnlyList<RegistryItemCustom> resp = await App.MainRegistryHelper.GetRegistryItems2(hive, key);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/renamekey?hive={hive2}&key={key}&newname={newname}")]
        public async Task<IGetResponse> RenameKey(string hive2, string key, string newname)
        {
            Enum.TryParse(hive2, out RegHives hive);

            HelperErrorCodes resp = await App.MainRegistryHelper.RenameKey(hive, key, newname);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat(
            "/registry/setkeyvalue?hive={hive2}&key={key}&valuename={valuename}&type={type2}&valuedata={valuedata}")]
        public async Task<IGetResponse> SetKeyValue(string hive2, string key, string valuename, string type2,
            string valuedata)
        {
            Enum.TryParse(hive2, out RegHives hive);
            Enum.TryParse(type2, out RegTypes type);

            HelperErrorCodes resp = await App.MainRegistryHelper.SetKeyValue(hive, key, valuename, type, valuedata);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat(
            "/registry/setkeyvalue2?hive={hive2}&key={key}&valuename={valuename}&type={type2}&valuedata={valuedata}")]
        public async Task<IGetResponse> SetKeyValue2(string hive2, string key, string valuename, string type2,
            string valuedata)
        {
            Enum.TryParse(hive2, out RegHives hive);
            uint.TryParse(type2, out uint type);

            HelperErrorCodes resp = await App.MainRegistryHelper.SetKeyValue(hive, key, valuename, type, valuedata);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }
    }
}
