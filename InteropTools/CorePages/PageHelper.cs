// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

namespace InteropTools.CorePages
{
    public static class PageHelper
    {
        public static string GetIconForPageGroup(PageGroup group) =>
            group switch
            {
                PageGroup.Bottom => "",
                PageGroup.Core => "",
                PageGroup.General => "",
                PageGroup.Registry => "",
                PageGroup.SSH => "",
                PageGroup.Tweaks => "",
                PageGroup.Unlock => "",
                _ => ""
            };

        public static string GetNameForPageGroup(PageGroup group) =>
            group switch
            {
                PageGroup.Bottom => "",
                PageGroup.Core => "Core",
                PageGroup.General => InteropTools.Resources.TextResources.Shell_GeneralGroupName,
                PageGroup.Registry => InteropTools.Resources.TextResources.Shell_RegistryGroupName,
                PageGroup.SSH => InteropTools.Resources.TextResources.Shell_SSHGroupName,
                PageGroup.Tweaks => InteropTools.Resources.TextResources.Shell_TweakGroupName,
                PageGroup.Unlock => InteropTools.Resources.TextResources.Shell_UnlockGroupName,
                _ => ""
            };
    }
}
