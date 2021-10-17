// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

namespace Intense.Presentation
{
    /// <summary>
    /// Provides a base implementation for objects that are displayed in the UI.
    /// </summary>
    public class Displayable
        : NotifyPropertyChanged
    {
        private string displayName;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName
        {
            get => displayName;
            set
            {
                if (Set(ref displayName, value))
                {
                    OnPropertyChanged("DisplayNameUppercase");
                }
            }
        }

        /// <summary>
        /// Get the uppercase variant of the display name.
        /// </summary>
        public string DisplayNameUppercase => displayName?.ToUpper();

        /// <summary>
        /// Gets a string representation of this instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => displayName;
    }
}
