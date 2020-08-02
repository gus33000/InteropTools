using InteropTools.Providers;
using Windows.UI.Xaml.Controls;

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
            switch (group)
            {
                case PageGroup.Bottom:
                    return "";
                case PageGroup.Core:
                    return "";
                case PageGroup.General:
                    return "";
                case PageGroup.Registry:
                    return "";
                case PageGroup.SSH:
                    return "";
                case PageGroup.Tweaks:
                    return "";
                case PageGroup.Unlock:
                    return "";
                default:
                    return "";
            }
        }

        public static string GetNameForPageGroup(PageGroup group)
        {
            switch (group)
            {
                case PageGroup.Bottom:
                    return "";
                case PageGroup.Core:
                    return "Core";
                case PageGroup.General:
                    return InteropTools.Resources.TextResources.Shell_GeneralGroupName;
                case PageGroup.Registry:
                    return InteropTools.Resources.TextResources.Shell_RegistryGroupName;
                case PageGroup.SSH:
                    return InteropTools.Resources.TextResources.Shell_SSHGroupName;
                case PageGroup.Tweaks:
                    return InteropTools.Resources.TextResources.Shell_TweakGroupName;
                case PageGroup.Unlock:
                    return InteropTools.Resources.TextResources.Shell_UnlockGroupName;
                default:
                    return "";
            }
        }
    }

    public abstract class ShellPage
    {
        public int viewid => App.CurrentSession.Value;
        
        public abstract PageGroup PageGroup { get; }
        public abstract string PageName { get; }

        public IRegistryProvider RegistryProvider {
            get
            {
                return App.Sessions[viewid].Helper;
            }

            set
            {
                App.Sessions[viewid].Helper = value;
            }
        }
    }
}
