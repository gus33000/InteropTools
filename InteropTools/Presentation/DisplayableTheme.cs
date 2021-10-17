// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Intense.Presentation;
using Windows.UI.Xaml;

namespace InteropTools.Presentation
{
    /// <summary>
    /// Represents a displayable theme.
    /// </summary>
    public class DisplayableTheme
        : Displayable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayableTheme"/>.
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="theme"></param>
        public DisplayableTheme(string displayName, ApplicationTheme? theme)
        {
            DisplayName = displayName;
            Theme = theme;
        }

        /// <summary>
        /// Gets the them.
        /// </summary>
        public ApplicationTheme? Theme { get; }
    }
}