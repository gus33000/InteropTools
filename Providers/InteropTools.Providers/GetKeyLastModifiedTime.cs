// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;

namespace InteropTools.Providers
{
    public class GetKeyLastModifiedTime
    {
        public DateTime LastModified { get; set; }
        public HelperErrorCodes returncode { get; set; }
    }
}