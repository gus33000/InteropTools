namespace InteropTools.Providers.Applications.WinRTProvider
{
    internal enum APPLICATIONS_OPERATION
    {
        AddPackage,
        RegisterPackage,
        UpdatePackage,
        RemovePackage,
        QueryApplicationVolumes,
        QueryApplications
    }

    internal enum APPLICATIONS_STATUS
    {
        SUCCESS,
        FAILED,
        ACCESS_DENIED,
        NOT_SUPPORTED,
    }
}