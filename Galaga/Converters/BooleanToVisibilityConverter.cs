using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Galaga.Converters
{
    /// <summary>
    ///     Converts a boolean value to a Visibility value.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        ///    Converts a boolean value to a Visibility value.
        /// </summary>
        /// <param name="value">
        ///     the boolean value to convert.
        /// </param>
        /// <param name="targetType">
        ///     the type of the target property.
        /// </param>
        /// <param name="parameter">
        ///     the parameter to use for the conversion.
        /// </param>
        /// <param name="language">
        ///     the language to use for the conversion.
        /// </param>
        /// <returns>
        ///     Visibility.Visible if the value is true, Visibility.Collapsed otherwise.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        ///    Converts a Visibility value to a boolean value.
        /// </summary>
        /// <param name="value">
        ///     the Visibility value to convert.
        /// </param>
        /// <param name="targetType">
        ///     the type of the target property.
        /// </param>
        /// <param name="parameter">
        ///     the parameter to use for the conversion.
        /// </param>
        /// <param name="language">
        ///     the language to use for the conversion.
        /// </param>
        /// <returns>
        ///     True if the value is Visibility.Visible, false otherwise.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }
}
