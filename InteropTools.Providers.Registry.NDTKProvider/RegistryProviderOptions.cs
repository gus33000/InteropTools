using InteropTools.Providers.Registry.Definition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InteropTools.Providers.Registry.NDTKProvider
{

    class RegistryProviderOptions : Options
    {
        public static readonly Guid ID = new Guid("AB577183-9A64-47E0-B4B6-E8B5D309F537");

        private readonly AbstractOption[] abstractOption;

        public RegistryProviderOptions()
        {
            this.abstractOption = new AbstractOption[]
            {
                
            };
        }


        public RegistryProviderOptions(Options o)
        {
            if (o.OptionsIdentifier != ID)
                throw new ArgumentException();
            this.abstractOption = o.Settings;
        }
        
        public override Guid OptionsIdentifier => ID;

        protected override AbstractOption[] GetSettings() => this.abstractOption;

    }
}
