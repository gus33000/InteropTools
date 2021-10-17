// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Windows.Management.Deployment;

namespace InteropTools.Providers.Applications.WinRTProvider
{
    internal interface IApplicationsProvider
    {
        APPLICATIONS_STATUS AddPackage(DeploymentOptions deployop, string pathtopackage);

        bool IsSupported(APPLICATIONS_OPERATION operation);

        APPLICATIONS_STATUS RegisterPackage(DeploymentOptions deployop, string pathtopackage);

        APPLICATIONS_STATUS RemovePackage(RemovalOptions removaloptions, string packagefullname);

        APPLICATIONS_STATUS UpdatePackage(DeploymentOptions deployop, string pathtopackage);
    }
}
