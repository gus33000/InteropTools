// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

namespace InteropTools.Providers
{
    public class RegQueryValue2
    {
        public uint regtype { get; set; }
        public byte[] regvalue { get; set; }
        public HelperErrorCodes returncode { get; set; }
    }
}