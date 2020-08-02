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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Windows.ApplicationModel.Background;
using InteropTools.Providers.OSReboot.Definition;
using Windows.ApplicationModel.AppService;

namespace InteropTools.Providers.OSReboot.FlightingProvider
{

    public sealed class OSRebootProvider : IBackgroundTask
    {
        private IBackgroundTask internalTask = new OSRebootProviderIntern();
        public void Run(IBackgroundTaskInstance taskInstance)
         => this.internalTask.Run(taskInstance);
    }

    internal class OSRebootProviderIntern : OSRebootProvidersWithOptions
    {
        // Define your provider class here
        IRebootProvider provider = new FlightingRebootProvider();

        protected override async Task<string> ExecuteAsync(AppServiceConnection sender, string input, IProgress<double> progress, CancellationToken cancelToken)
        {
            var arr = input.Split(new string[] { "Q+q:8rKwjyVG\"~@<],TNH!@kcn/qUv:=3=Zs)+gU$Efc:[&Ku^qn,U}&yrRY{}byf<4DV&W!mF>R@Z8uz=>kgj~F[KeB{,]'[Veb" }, StringSplitOptions.None);

            var operation = arr.First();
            REBOOT_OPERATION operationenum;
            Enum.TryParse(operation, true, out operationenum);

            List<List<string>> returnvalue = new List<List<string>>();
            List<string> returnvalue2 = new List<string>();

            if (provider.IsSupported(operationenum))
            {
                switch (operationenum)
                {
                    case REBOOT_OPERATION.SystemReboot:
                        {
                            var ret = provider.SystemReboot();

                            returnvalue2.Add(ret.ToString());

                            returnvalue.Add(returnvalue2);
                            break;
                        }
                }
            }
            else
            {
                returnvalue2.Add(REBOOT_STATUS.NOT_SUPPORTED.ToString());

                returnvalue.Add(returnvalue2);
            }

            var returnstr = "";

            foreach (var str in returnvalue)
            {
                var str2 = string.Join("*[Pp)8/P'=Tu(pm\"fYNh#*7w27V~>bubdt#\"AF~'\\}{jwAE2uY5,~bEVfBZ2%xx+UK?c&Xr@)C6/}j?5rjuB=8+egU\\D@\"; T3M<%", str);
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
            => Task.FromResult<Options>(new OSRebootProviderOptions());

        protected override Guid GetOptionsGuid() => OSRebootProviderOptions.ID;

    }
}
