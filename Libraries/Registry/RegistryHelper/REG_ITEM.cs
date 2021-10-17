// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

namespace RegistryHelper
{
    public sealed class REG_ITEM
    {
        public string DataAsString { get; internal set; }
        public REG_HIVES Hive { get; internal set; }
        public string Key { get; internal set; }
        public string Name { get; internal set; }
        public REG_TYPE Type { get; internal set; }
        public REG_VALUE_TYPE? ValueType { get; internal set; }
    }
}