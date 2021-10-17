// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using System;
using InteropTools.AppExtensibilityDefinition;

namespace InteropTools.AppExtensibilityBackgroundTask
{
    internal class AppExtensibilityProviderOptions : Options
    {
        public static readonly Guid ID = new("AB577183-9A64-47E0-B4B6-E8B5D309F537");

        private readonly AbstractOption[] abstractOption;

        public AppExtensibilityProviderOptions() =>
            abstractOption = new AbstractOption[] { };

        public AppExtensibilityProviderOptions(Options o)
        {
            if (o.OptionsIdentifier != ID)
            {
                throw new ArgumentException();
            }

            abstractOption = o.Settings;
        }

        public override Guid OptionsIdentifier => ID;

        protected override AbstractOption[] GetSettings() => abstractOption;
    }
}
