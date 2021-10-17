// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Windows.UI.Xaml;

namespace Intense.UI.Converters
{
    /// <summary>
    /// Converts a null value to and from a visibility value.
    /// </summary>
    public class NullToVisibilityConverter
        : ValueConverter<object, Visibility>
    {
        /// <summary>
        /// Determines whether an inverse conversion should take place.
        /// </summary>
        /// <remarks>If set, the value null results in <see cref="Visibility.Visible"/>, and not null in <see cref="Visibility.Collapsed"/>.</remarks>
        public bool Inverse { get; set; }

        /// <summary>
        /// Converts a source value to the target type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        protected override Visibility Convert(object value, object parameter, string language)
        {
            bool isNull = value == null;

            if (Inverse)
            {
                isNull = !isNull;
            }
            return isNull ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}