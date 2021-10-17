﻿// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

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
        private readonly NavigationItemCollection items;
        private string description;
        private string groupIcon;
        private string groupName;
        private string icon;
        private object pageParameter;
        private Type pageType;
        private NavigationItem parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationItem"/> class.
        /// </summary>
        public NavigationItem() => items = new NavigationItemCollection(this);

        /// <summary>
        /// Gets or sets a description of the item.
        /// </summary>
        public string Description
        {
            get => description;
            set => Set(ref description, value);
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
        /// Gets or sets the group.
        /// </summary>
        public string GroupName
        {
            get => groupName;
            set => Set(ref groupName, value);
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
        /// Gets the child navigation items.
        /// </summary>
        public NavigationItemCollection Items => items;

        /// <summary>
        /// Gets or set the parameter object that used when navigating to the page specified by <see cref="PageType"/>.
        /// </summary>
        public object PageParameter
        {
            get => pageParameter;
            set => Set(ref pageParameter, value);
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
        /// Gets the parent navigation item.
        /// </summary>
        public NavigationItem Parent
        {
            get => parent;
            internal set => Set(ref parent, value);
        }
    }
}
