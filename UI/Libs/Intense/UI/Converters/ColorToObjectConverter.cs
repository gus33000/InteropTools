using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
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
            if (value == null) {
                return null;
            }
            var color = value as Color?;
            if (color != null) {
                return color;
            }
            var str = value as string;
            if (str != null) {
                var brush = XamlHelper.CreateSolidColorBrush(str);
                return brush.Color;
            }
            // no other conversions supported
            return null;
        }
    }
}
