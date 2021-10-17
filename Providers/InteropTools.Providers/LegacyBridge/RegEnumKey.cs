using System.Collections.Generic;

namespace InteropTools.Providers
{
    public class RegEnumKey
    {
        public IReadOnlyList<RegistryItemCustom> items { get; set; }
        public HelperErrorCodes returncode { get; set; }
    }
}