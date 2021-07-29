namespace InteropTools.Providers.OSReboot.FlightingProvider
{
    internal enum REBOOT_OPERATION
    {
        SystemReboot
    }

    internal enum REBOOT_STATUS
    {
        SUCCESS,
        FAILED,
        ACCESS_DENIED,
        NOT_SUPPORTED,
    }
}