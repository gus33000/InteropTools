using System;
using Windows.UI.Xaml.Markup;

namespace Intense.Presentation
{
    /// <summary>
    /// Represents a single item in a navigation hierarchy.
    /// </summary>
    [ContentProperty(Name = "Items")]
    public class NavigationItem
         : Displayable
    {
        private string icon;
        private string groupName;
        private string groupIcon;
        private string description;
        private Type pageType;
        private object pageParameter;

        private readonly NavigationItemCollection items;
        private NavigationItem parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationItem"/> class.
        /// </summary>
        public NavigationItem()
        {
            items = new NavigationItemCollection(this);
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public string Icon
        {
            get => icon;
            set => Set(ref icon, value);
        }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        public string GroupName
        {
            get => groupName;
            set => Set(ref groupName, value);
        }

        /// <summary>
        /// Gets or sets the group icon.
        /// </summary>
        public string GroupIcon
        {
            get => groupIcon;
            set => Set(ref groupIcon, value);
        }

        /// <summary>
        /// Gets or sets a description of the item.
        /// </summary>
        public string Description
        {
            get => description;
            set => Set(ref description, value);
        }

        /// <summary>
        /// Gets or sets the page type associated with the item.
        /// </summary>
        public Type PageType
        {
            get => pageType;
            set => Set(ref pageType, value);
        }

        /// <summary>
        /// Gets or set the parameter object that used when navigating to the page specified by <see cref="PageType"/>.
        /// </summary>
        public object PageParameter
        {
            get => pageParameter;
            set => Set(ref pageParameter, value);
        }

        /// <summary>
        /// Gets the child navigation items.
        /// </summary>
        public NavigationItemCollection Items => items;

        /// <summary>
        /// Gets the parent navigation item.
        /// </summary>
        public NavigationItem Parent
        {
            get => parent;
            internal set => Set(ref parent, value);
        }
    }
}
