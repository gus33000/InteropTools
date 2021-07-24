using Windows.Management.Deployment;

namespace InteropTools.Providers.Applications.WinRTProvider
{
    internal interface IApplicationsProvider
    {
        bool IsSupported(APPLICATIONS_OPERATION operation);

        APPLICATIONS_STATUS AddPackage(DeploymentOptions deployop, string pathtopackage);
        APPLICATIONS_STATUS RegisterPackage(DeploymentOptions deployop, string pathtopackage);
        APPLICATIONS_STATUS UpdatePackage(DeploymentOptions deployop, string pathtopackage);
        APPLICATIONS_STATUS RemovePackage(RemovalOptions removaloptions, string packagefullname);
    }
}
