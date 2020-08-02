using InteropTools.AppExtensibilityDefinition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace InteropTools.AppExtensibilityBackgroundTask
{

    class AppExtensibilityProviderOptions : Options
    {
        public static readonly Guid ID = new Guid("AB577183-9A64-47E0-B4B6-E8B5D309F537");

        private readonly AbstractOption[] abstractOption;

        public AppExtensibilityProviderOptions()
        {
            this.abstractOption = new AbstractOption[]
            {
                
            };
        }


        public AppExtensibilityProviderOptions(Options o)
        {
            if (o.OptionsIdentifier != ID)
                throw new ArgumentException();
            this.abstractOption = o.Settings;
        }
        
        public override Guid OptionsIdentifier => ID;

        protected override AbstractOption[] GetSettings() => this.abstractOption;

    }
}
