using System.Collections.Generic;

namespace InteropTools.Providers.OSReboot.FlightingProvider
{
    internal interface IRebootProvider
    {
        bool IsSupported(REBOOT_OPERATION operation);

        REBOOT_STATUS SystemReboot();
    }
}
