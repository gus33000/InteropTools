namespace InteropTools.Providers.OSReboot.NDTKProvider
{
    internal interface IRebootProvider
    {
        bool IsSupported(REBOOT_OPERATION operation);

        REBOOT_STATUS SystemReboot();
    }
}