using System;
using Windows.UI.Xaml.Data;

namespace Intense.UI.Converters
{
    /// <summary>
    /// The generic base implementation of a value converter.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TTarget">The target type.</typeparam>
    public abstract class ValueConverter<TSource, TTarget>
        : IValueConverter
    {
        /// <summary>
        /// Converts a source value to the target type.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TTarget Convert(TSource value)
        {
            return Convert(value, null, null);
        }

        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // CastExceptions will occur when invalid value, or target type provided.
            return Convert((TSource)value, parameter, language);
        }

        /// <summary>
        /// Converts a target value back to the source type.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public TSource ConvertBack(TTarget value)
        {
            return ConvertBack(value, null, null);
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // CastExceptions will occur when invalid value, or target type provided.
            return ConvertBack((TTarget)value, parameter, language);
        }

        /// <summary>
        /// Converts a source value to the target type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        protected virtual TTarget Convert(TSource value, object parameter, string language)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Converts a target value back to the source type.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        protected virtual TSource ConvertBack(TTarget value, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}