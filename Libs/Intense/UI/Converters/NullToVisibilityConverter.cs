using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

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
            var isNull = value == null;

            if (this.Inverse) {
                isNull = !isNull;
            }
            return isNull ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
