using Intense.Presentation;
using static InteropTools.CorePages.Shell;

namespace InteropTools.CorePages
{
    public class NavigationItemData
    {
        public string Name
        {
            get
            {
                if (IsGroup)
                { return GroupItem.DisplayName; }

                return NavigationItem.DisplayName;
            }
        }

        public string Icon
        {
            get
            {
                if (IsGroup)
                { return GroupItem.Icon; }

                return NavigationItem.Icon;
            }
        }

        public string Description
        {
            get
            {
                if (IsGroup)
                { return GroupItem.DisplayName; }

                return NavigationItem.Description;
            }
        }

        public bool IsGroup => GroupItem != null;

        public GroupItem GroupItem { get; set; }
        public NavigationItem NavigationItem { get; set; }

        public override bool Equals(object obj)
        {
            NavigationItemData eqobj = obj as NavigationItemData;

            if (eqobj == null)
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
                hash = hash * 31 + Name.GetHashCode() + Icon.GetHashCode();
            }

            return hash;
        }
    }
}
