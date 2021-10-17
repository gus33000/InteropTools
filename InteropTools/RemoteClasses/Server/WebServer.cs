// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System.Threading.Tasks;
using Restup.Webserver.File;
using Restup.Webserver.Http;
using Restup.Webserver.Rest;

namespace InteropTools.RemoteClasses.Server
{
    public class WebServer
    {
        public async Task Run()
        {
            RestRouteHandler restRouteHandler = new();
            restRouteHandler.RegisterController<ParameterController>();

            HttpServerConfiguration configuration = new HttpServerConfiguration()
                .ListenOnPort(8800)
                .RegisterRoute("api", restRouteHandler)
                .EnableCors()
                .RegisterRoute(new StaticFileRouteHandler("Web"));

            HttpServer httpServer = new(configuration);
            await httpServer.StartServerAsync();

            // now make sure the app won't stop after this (eg use a BackgroundTaskDeferral)
        }
    }
}
