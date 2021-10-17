// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using InteropTools.Providers.OSReboot.Definition;

namespace InteropTools.Providers.OSReboot.NDTKProvider
{
    internal class OSRebootProviderOptions : Options
    {
        public static readonly Guid ID = new("CF0E28B7-A0B3-4457-96CE-142271FD4AC7");

        private readonly AbstractOption[] abstractOption;

        public OSRebootProviderOptions()
        {
            abstractOption = new AbstractOption[]
            {
            };
        }

        public OSRebootProviderOptions(Options o)
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