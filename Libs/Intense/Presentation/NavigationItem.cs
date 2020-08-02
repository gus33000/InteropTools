using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private NavigationItemCollection items;
        private NavigationItem parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationItem"/> class.
        /// </summary>
        public NavigationItem()
        {
            this.items = new NavigationItemCollection(this);
        }

        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        public string Icon
        {
            get { return this.icon; }
            set { Set(ref this.icon, value); }
        }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        public string GroupName
        {
            get { return this.groupName; }
            set { Set(ref this.groupName, value); }
        }

        /// <summary>
        /// Gets or sets the group icon.
        /// </summary>
        public string GroupIcon
        {
            get { return this.groupIcon; }
            set { Set(ref this.groupIcon, value); }
        }

        /// <summary>
        /// Gets or sets a description of the item.
        /// </summary>
        public string Description
        {
            get { return this.description; }
            set { Set(ref this.description, value); }
        }

        /// <summary>
        /// Gets or sets the page type associated with the item.
        /// </summary>
        public Type PageType
        {
            get { return this.pageType; }
            set { Set(ref this.pageType, value); }
        }

        /// <summary>
        /// Gets or set the parameter object that used when navigating to the page specified by <see cref="PageType"/>.
        /// </summary>
        public object PageParameter
        {
            get { return this.pageParameter; }
            set { Set(ref this.pageParameter, value); }
        }

        /// <summary>
        /// Gets the child navigation items.
        /// </summary>
        public NavigationItemCollection Items
        {
            get { return this.items; }
        }

        /// <summary>
        /// Gets the parent navigation item.
        /// </summary>
        public NavigationItem Parent
        {
            get { return this.parent; }
            internal set { Set(ref this.parent, value); }
        }
    }
}
