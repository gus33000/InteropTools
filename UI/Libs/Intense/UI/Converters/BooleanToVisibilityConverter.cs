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
    /// Converts a boolean to and from a visibility value.
    /// </summary>
    public class BooleanToVisibilityConverter
        : ValueConverter<Boolean, Visibility>
    {
        /// <summary>
        /// Determines whether an inverse conversion should take place.
        /// </summary>
        /// <remarks>If set, the value True results in <see cref="Visibility.Collapsed"/>, and false in <see cref="Visibility.Visible"/>.</remarks>
        public bool Inverse { get; set; }

        /// <summary>
        /// Converts a source value to the target type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        protected override Visibility Convert(bool value, object parameter, string language)
        {
            if (this.Inverse) {
                value = !value;
            }
            return value ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Converts a target value back to the source type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        protected override bool ConvertBack(Visibility value, object parameter, string language)
        {
            var result = value == Visibility.Visible;
            if (this.Inverse) {
                result = !result;
            }
            return result;
        }
    }
}
