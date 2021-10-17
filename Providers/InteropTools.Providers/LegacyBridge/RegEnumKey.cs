// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System.Collections.Generic;

namespace InteropTools.Providers
{
    public class RegEnumKey
    {
        public IReadOnlyList<RegistryItemCustom> items { get; set; }
        public HelperErrorCodes returncode { get; set; }
    }
}
