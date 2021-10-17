// Copyright 2015-2021 (c) Interop Tools Development Team
// This file is licensed to you under the MIT license.

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace Intense.UI.Converters
{
    /// <summary>
    /// Converts a color or string object to and from a nullable color value.
    /// </summary>
    public class ColorToObjectConverter
        : ToObjectConverter<Color?>
    {
        /// <summary>
        /// Converts a target value back to the source type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        protected override Color? ConvertBack(object value, object parameter, string language)
        {
            if (value == null)
            {
                return null;
            }

            Color? color = value as Color?;
            if (color != null)
            {
                return color;
            }

            if (value is string str)
            {
                SolidColorBrush brush = XamlHelper.CreateSolidColorBrush(str);
                return brush.Color;
            }

            // no other conversions supported
            return null;
        }
    }
}
