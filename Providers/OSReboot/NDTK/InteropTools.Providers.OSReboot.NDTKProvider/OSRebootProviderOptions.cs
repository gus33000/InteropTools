using InteropTools.Providers.OSReboot.Definition;
using System;

namespace InteropTools.Providers.OSReboot.NDTKProvider
{
    internal class OSRebootProviderOptions : Options
    {
        public static readonly Guid ID = new Guid("CF0E28B7-A0B3-4457-96CE-142271FD4AC7");

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
