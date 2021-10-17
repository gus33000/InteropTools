// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using Windows.ApplicationModel.AppService;

namespace AppPlugin.Exceptions
{
    public class ConnectionFailureException : Exception
    {
        private readonly AppServiceConnection connection;
        private readonly AppServiceConnectionStatus status;

        internal ConnectionFailureException(AppServiceResponseStatus status) : base(GenerateMessage(status))
        {
            Status = status;
        }

        internal ConnectionFailureException(AppServiceConnectionStatus status, AppServiceConnection connection) : base(GenerateMessage(status, connection))
        {
            this.status = status;
            this.connection = connection;
        }

        public AppServiceResponseStatus Status { get; }

        private static string GenerateMessage(AppServiceResponseStatus status)
        {
            switch (status)
            {
                case AppServiceResponseStatus.Success:
                    throw new ArgumentException("Success sollte keine Exception auslösen.", nameof(status));
                case AppServiceResponseStatus.Failure:
                case AppServiceResponseStatus.ResourceLimitsExceeded:
                case AppServiceResponseStatus.Unknown:
                case AppServiceResponseStatus.RemoteSystemUnavailable:
                case AppServiceResponseStatus.MessageSizeTooLarge:
                    return "";

                default:
                    return "Unknown failure";
            }
        }

        private static string GenerateMessage(AppServiceConnectionStatus status, AppServiceConnection connection)
        {
            switch (status)
            {
                case AppServiceConnectionStatus.Success:
                    throw new ArgumentException("Success sollte keine Exception auslösen.", nameof(status));
                case AppServiceConnectionStatus.AppNotInstalled:
                    return "The app AppServicesProvider is not installed. Deploy AppServicesProvider to this device and try again.";

                case AppServiceConnectionStatus.AppUnavailable:
                    return "The app AppServicesProvider is not available. This could be because it is currently being updated or was installed to a removable device that is no longer available.";

                case AppServiceConnectionStatus.AppServiceUnavailable:
                    return string.Format("The app AppServicesProvider is installed but it does not provide the app service {0}.", connection.AppServiceName);

                case AppServiceConnectionStatus.Unknown:
                    return "An unkown error occurred while we were trying to open an AppServiceConnection.";

                case AppServiceConnectionStatus.RemoteSystemUnavailable:
                    return "The remote system is unavailable.";

                case AppServiceConnectionStatus.RemoteSystemNotSupportedByApp:
                    return "The Remote System is not supported by the app.";

                case AppServiceConnectionStatus.NotAuthorized:
                    return "You are not authorized.";

                default:
                    return "Unknown failure";
            }
        }
    }
}