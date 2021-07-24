using InteropTools.Providers.OSReboot.Definition;
using System;

namespace InteropTools.Providers.OSReboot.FlightingProvider
{
    internal class OSRebootProviderOptions : Options
    {
        public static readonly Guid ID = new Guid("CAB45A10-6BC7-4BCF-B246-62E1EC7679E0");

        private readonly AbstractOption[] abstractOption;

        public OSRebootProviderOptions()
        {
            abstractOption = new AbstractOption[]
            {

            };
        }


        public OSRebootProviderOptions(Options o)
        {
            if (o.OptionsIdentifier != ID)
            {
                throw new ArgumentException();
            }

            abstractOption = o.Settings;
        }

        public override Guid OptionsIdentifier => ID;

        protected override AbstractOption[] GetSettings()
        {
            return abstractOption;
        }
    }
}
