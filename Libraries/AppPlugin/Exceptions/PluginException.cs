// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;

namespace AppPlugin.Exceptions
{
    internal class PluginException : Exception
    {
        internal PluginException(string message) : base(message)
        {
        }
    }
}