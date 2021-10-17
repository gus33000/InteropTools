// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using InteropTools.Providers;

namespace InteropTools.CorePages
{
    public abstract class ShellPage
    {
        public abstract PageGroup PageGroup { get; }
        public abstract string PageName { get; }

        public IRegistryProvider RegistryProvider
        {
            get => SessionManager.Sessions[viewid].Helper;

            set => SessionManager.Sessions[viewid].Helper = value;
        }

        public int viewid => SessionManager.CurrentSession.Value;
    }
}