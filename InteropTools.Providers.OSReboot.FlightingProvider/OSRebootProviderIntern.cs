/*++

Copyright (c) 2016  Interop Tools Development Team
Copyright (c) 2017  Gustave M.

Module Name:

    Plugin.cs

Abstract:

    This module implements the NDTK Registry Provider Plugin.

Author:

    Gustave M.     (gus33000)       20-Mar-2017

Revision History:

    Gustave M. (gus33000) 20-Mar-2017

        Initial Implementation.

--*/

using InteropTools.Providers.OSReboot.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;

namespace InteropTools.Providers.OSReboot.FlightingProvider
{
    public sealed class OSRebootProvider : IBackgroundTask
    {
        private readonly IBackgroundTask internalTask = new OSRebootProviderIntern();
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            internalTask.Run(taskInstance);
        }
    }

    internal class OSRebootProviderIntern : OSRebootProvidersWithOptions
    {
        // Define your provider class here
        private readonly IRebootProvider provider = new FlightingRebootProvider();

        protected override async Task<string> ExecuteAsync(AppServiceConnection sender, string input, IProgress<double> progress, CancellationToken cancelToken)
        {
            string[] arr = input.Split(new string[] { "Q+q:8rKwjyVG\"~@<],TNH!@kcn/qUv:=3=Zs)+gU$Efc:[&Ku^qn,U}&yrRY{}byf<4DV&W!mF>R@Z8uz=>kgj~F[KeB{,]'[Veb" }, StringSplitOptions.None);

            string operation = arr[0];
            Enum.TryParse(operation, true, out REBOOT_OPERATION operationenum);

            List<List<string>> returnvalue = new();
            List<string> returnvalue2 = new();

            if (provider.IsSupported(operationenum))
            {
                switch (operationenum)
                {
                    case REBOOT_OPERATION.SystemReboot:
                        {
                            REBOOT_STATUS ret = provider.SystemReboot();

                            returnvalue2.Add(ret.ToString());

                            returnvalue.Add(returnvalue2);
                            break;
                        }
                }
            }
            else
            {
                returnvalue2.Add(nameof(REBOOT_STATUS.NOT_SUPPORTED));

                returnvalue.Add(returnvalue2);
            }

            string returnstr = "";

            foreach (List<string> str in returnvalue)
            {
                string str2 = string.Join("*[Pp)8/P'=Tu(pm\"fYNh#*7w27V~>bubdt#\"AF~'\\}{jwAE2uY5,~bEVfBZ2%xx+UK?c&Xr@)C6/}j?5rjuB=8+egU\\D@\"; T3M<%", str);
                if (string.IsNullOrEmpty(returnstr))
                {
                    returnstr = str2;
                }
                else
                {
                    returnstr += "Q+q:8rKwjyVG\"~@<],TNH!@kcn/qUv:=3=Zs)+gU$Efc:[&Ku^qn,U}&yrRY{}byf<4DV&W!mF>R@Z8uz=>kgj~F[KeB{,]'[Veb" + str2;
                }
            }

            return returnstr;
        }

        protected override Task<Options> GetOptions()
        {
            return Task.FromResult<Options>(new OSRebootProviderOptions());
        }

        protected override Guid GetOptionsGuid()
        {
            return OSRebootProviderOptions.ID;
        }
    }
}
