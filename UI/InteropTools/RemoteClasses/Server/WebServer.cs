using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restup.Webserver.Attributes;
using Restup.Webserver.File;
using Restup.Webserver.Http;
using Restup.Webserver.Models.Contracts;
using Restup.Webserver.Models.Schemas;
using Restup.Webserver.Rest;
using System.Threading.Tasks;
using InteropTools.Providers;
using Windows.Foundation;
using Windows.ApplicationModel;
using System.IO;
using System.Reflection;

namespace InteropTools.RemoteClasses.Server
{
    public class WebServer
    {
        public async Task Run()
        {
            var restRouteHandler = new RestRouteHandler();
            restRouteHandler.RegisterController<ParameterController>();

            var configuration = new HttpServerConfiguration()
              .ListenOnPort(8800)
              .RegisterRoute("api", restRouteHandler)
              .EnableCors()
              .RegisterRoute(new StaticFileRouteHandler(@"Web"));

            var httpServer = new HttpServer(configuration);
            await httpServer.StartServerAsync();

            // now make sure the app won't stop after this (eg use a BackgroundTaskDeferral)
        }
    }

    public class DataReceived
    {
        public int ID { get; set; }
        public string PropName { get; set; }
    }

    [RestController(InstanceCreationType.Singleton)]
    public class ParameterController
    {
        [UriFormat("/registry/getkeyvalue?hive={hive2}&key={key}&valuename={valuename}&type={type2}")]
        public async Task<IGetResponse> GetKeyValue(string hive2, string key, string valuename, string type2)
        {
            RegHives hive;
            Enum.TryParse(hive2, out hive);
            RegTypes type;
            Enum.TryParse(type2, out type);

            var resp = await App.MainRegistryHelper.GetKeyValue(hive, key, valuename, type);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/setkeyvalue?hive={hive2}&key={key}&valuename={valuename}&type={type2}&valuedata={valuedata}")]
        public async Task<IGetResponse> SetKeyValue(string hive2, string key, string valuename, string type2, string valuedata)
        {
            RegHives hive;
            Enum.TryParse(hive2, out hive);
            RegTypes type;
            Enum.TryParse(type2, out type);

            var resp = await App.MainRegistryHelper.SetKeyValue(hive, key, valuename, type, valuedata);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/getkeystatus?hive={hive2}&key={key}")]
        public async Task<IGetResponse> GetKeyStatus(string hive2, string key)
        {
            RegHives hive;
            Enum.TryParse(hive2, out hive);

            var resp = await App.MainRegistryHelper.GetKeyStatus(hive, key);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/deletevalue?hive={hive2}&key={key}&valuename={valuename}")]
        public async Task<IGetResponse> DeleteValue(string hive2, string key, string valuename)
        {
            RegHives hive;
            Enum.TryParse(hive2, out hive);

            var resp = await App.MainRegistryHelper.DeleteValue(hive, key, valuename);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/deletekey?hive={hive2}&key={key}&recursive={recursive2}")]
        public async Task<IGetResponse> DeleteKey(string hive2, string key, string recursive2)
        {
            RegHives hive;
            Enum.TryParse(hive2, out hive);
            bool recursive;
            bool.TryParse(recursive2, out recursive);

            var resp = await App.MainRegistryHelper.DeleteKey(hive, key, recursive);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }
        
        [UriFormat("/registry/addkey?hive={hive2}&key={key}")]
        public async Task<IGetResponse> AddKey(string hive2, string key)
        {
            RegHives hive;
            Enum.TryParse(hive2, out hive);

            var resp = await App.MainRegistryHelper.AddKey(hive, key);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }
        
        [UriFormat("/registry/renamekey?hive={hive2}&key={key}&newname={newname}")]
        public async Task<IGetResponse> RenameKey(string hive2, string key, string newname)
        {
            RegHives hive;
            Enum.TryParse(hive2, out hive);

            var resp = await App.MainRegistryHelper.RenameKey(hive, key, newname);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/getregistryhives2")]
        public async Task<IGetResponse> GetRegistryHives2()
        {
            var resp = await App.MainRegistryHelper.GetRegistryHives2();

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/getregistryitems2?hive={hive2}&key={key}")]
        public async Task<IGetResponse> GetRegistryItems2(string hive2, string key)
        {
            RegHives hive;
            Enum.TryParse(hive2, out hive);

            var resp = await App.MainRegistryHelper.GetRegistryItems2(hive, key);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/getkeyvalue2?hive={hive2}&key={key}&valuename={valuename}&type={type2}")]
        public async Task<IGetResponse> GetKeyValue2(string hive2, string key, string valuename, string type2)
        {
            RegHives hive;
            Enum.TryParse(hive2, out hive);
            uint type;
            uint.TryParse(type2, out type);

            var resp = await App.MainRegistryHelper.GetKeyValue(hive, key, valuename, type);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/setkeyvalue2?hive={hive2}&key={key}&valuename={valuename}&type={type2}&valuedata={valuedata}")]
        public async Task<IGetResponse> SetKeyValue2(string hive2, string key, string valuename, string type2, string valuedata)
        {
            RegHives hive;
            Enum.TryParse(hive2, out hive);
            uint type;
            uint.TryParse(type2, out type);

            var resp = await App.MainRegistryHelper.SetKeyValue(hive, key, valuename, type, valuedata);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }

        [UriFormat("/registry/getkeylastmodifiedtime?hive={hive2}&key={key}")]
        public async Task<IGetResponse> GetKeyLastModifiedTime(string hive2, string key)
        {
            RegHives hive;
            Enum.TryParse(hive2, out hive);

            var resp = await App.MainRegistryHelper.GetKeyLastModifiedTime(hive, key);

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                resp);
        }
        
        [UriFormat("/core/getappbuildstring")]
        public IGetResponse GetAppBuildString()
        {
            var buildString = "";
            var assembly = GetType().GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceStream("InteropTools.Resources.BuildDate.txt");
            var builddate = new StreamReader(resource).ReadLine().Replace("\r", "");
            var appver = Package.Current.Id.Version;
            var appverstr = string.Format("{0}.{1}.{2}.{3}", appver.Major, appver.Minor, appver.Build, appver.Revision);
            buildString = appverstr + " (fbl_prerelease(gustavem)";
            var myType = Type.GetType("InteropTools.ShellPages.Private.YourWindowsBuildPage");

            if (myType != null)
            {
                buildString = buildString + "/private";
            }

            buildString = buildString + "." + builddate + ")";

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                buildString);
        }

        [UriFormat("/deviceinfo/getdevicehostname")]
        public IGetResponse GetDeviceHostname()
        {
            var hostname = InteropTools.Classes.DeviceInfo.Instance.FriendlyName;

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                hostname);
        }

        [UriFormat("/deviceinfo/getdevicefamilly")]
        public IGetResponse GetDeviceFamilly()
        {
            var devicefamilly = InteropTools.Classes.DeviceInfo.Instance.DeviceFamily;

            return new GetResponse(
                GetResponse.ResponseStatus.OK,
                devicefamilly);
        }
    }
}
