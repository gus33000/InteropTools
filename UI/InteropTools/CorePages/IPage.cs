using InteropTools.Providers;

namespace InteropTools.CorePages
{
    public enum PageGroup
    {
        Core,
        Registry,
        General,
        Tweaks,
        SSH,
        Unlock,
        Bottom
    }

    public static class PageHelper
    {
        public static string GetIconForPageGroup(PageGroup group)
        {
            return group switch
            {
                PageGroup.Bottom => "",
                PageGroup.Core => "",
                PageGroup.General => "",
                PageGroup.Registry => "",
                PageGroup.SSH => "",
                PageGroup.Tweaks => "",
                PageGroup.Unlock => "",
                _ => "",
            };
        }

        public static string GetNameForPageGroup(PageGroup group)
        {
            return group switch
            {
                PageGroup.Bottom => "",
                PageGroup.Core => "Core",
                PageGroup.General => Resources.TextResources.Shell_GeneralGroupName,
                PageGroup.Registry => Resources.TextResources.Shell_RegistryGroupName,
                PageGroup.SSH => Resources.TextResources.Shell_SSHGroupName,
                PageGroup.Tweaks => Resources.TextResources.Shell_TweakGroupName,
                PageGroup.Unlock => Resources.TextResources.Shell_UnlockGroupName,
                _ => "",
            };
        }
    }

    public abstract class ShellPage
    {
        public int viewid => SessionManager.CurrentSession.Value;

        public abstract PageGroup PageGroup { get; }
        public abstract string PageName { get; }

        public IRegistryProvider RegistryProvider
        {
            get => SessionManager.Sessions[viewid].Helper;

            set => SessionManager.Sessions[viewid].Helper = value;
        }
    }
}
