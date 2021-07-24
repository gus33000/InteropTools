using InteropTools.Providers.Registry.Definition;
using System;

namespace InteropTools.Providers.Registry.RegistryRTProvider
{
    internal class RegistryProviderOptions : Options
    {
        public static readonly Guid ID = new Guid("AB577183-9A64-47E0-B4B6-E8B5D309F537");

        private readonly AbstractOption[] abstractOption;

        public RegistryProviderOptions()
        {
            abstractOption = new AbstractOption[]
            {

            };
        }


        public RegistryProviderOptions(Options o)
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
