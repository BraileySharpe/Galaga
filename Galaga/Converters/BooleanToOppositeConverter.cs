using System;
using Windows.UI.Xaml.Data;

namespace Galaga.Converters;

/// <summary>
///    Represents a converter that converts a boolean value to its opposite.
/// </summary>
public class BooleanToOppositeConverter : IValueConverter
{
    /// <summary>
    ///     Converts the specified value.
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <param name="targetType">
    ///     Type of the target.
    /// </param>
    /// <param name="parameter">
    ///     The parameter.
    /// </param>
    /// <param name="language">
    ///     The language.
    /// </param>
    /// <returns>
    ///     True if the value is false; otherwise, false.
    /// </returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var newValue = !(bool)value;
        return newValue;
    }

    /// <summary>
    ///     Converts the back.
    /// </summary>
    /// <param name="value">
    ///     The value to convert.
    /// </param>
    /// <param name="targetType">
    ///     Type of the target.
    /// </param>
    /// <param name="parameter">
    ///     The parameter.
    /// </param>
    /// <param name="language">
    ///     The language.
    /// </param>
    /// <returns>
    ///     True if the value is false; otherwise, false.
    /// </returns>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        var newValue = !(bool)value;
        return newValue;
    }
}