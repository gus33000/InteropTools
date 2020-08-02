using InteropTools.Providers.Applications.Definition;
using System;

namespace InteropTools.Providers.Applications.WinRTProvider
{

    class OSRebootProviderOptions : Options
    {
        public static readonly Guid ID = new Guid("CF0E28B7-A0B3-4457-96CE-142271FD4AC7");

        private readonly AbstractOption[] abstractOption;

        public OSRebootProviderOptions()
        {
            this.abstractOption = new AbstractOption[]
            {
                
            };
        }


        public OSRebootProviderOptions(Options o)
        {
            if (o.OptionsIdentifier != ID)
                throw new ArgumentException();
            this.abstractOption = o.Settings;
        }
        
        public override Guid OptionsIdentifier => ID;

        protected override AbstractOption[] GetSettings() => this.abstractOption;

    }
}
