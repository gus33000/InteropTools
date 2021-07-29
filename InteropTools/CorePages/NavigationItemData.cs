using Intense.Presentation;
using static InteropTools.CorePages.Shell;

namespace InteropTools.CorePages
{
    public class NavigationItemData
    {
        public string Description
        {
            get
            {
                if (IsGroup)
                { return GroupItem.DisplayName; }

                return NavigationItem.Description;
            }
        }

        public GroupItem GroupItem { get; set; }

        public string Icon
        {
            get
            {
                if (IsGroup)
                { return GroupItem.Icon; }

                return NavigationItem.Icon;
            }
        }

        public bool IsGroup => GroupItem != null;

        public string Name
        {
            get
            {
                if (IsGroup)
                { return GroupItem.DisplayName; }

                return NavigationItem.DisplayName;
            }
        }

        public NavigationItem NavigationItem { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is not NavigationItemData eqobj)
            {
                return false;
            }

            return eqobj.Name + eqobj.Icon == Name + Icon;
        }

        public override int GetHashCode()
        {
            int hash = 23;

            if (Name != null)
            {
                hash = (hash * 31) + Name.GetHashCode() + Icon.GetHashCode();
            }

            return hash;
        }
    }
}