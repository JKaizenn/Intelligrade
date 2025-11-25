using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace IntelliGrade.App.Converters;

/// <summary>
/// Returns true if object is null, false otherwise.
/// Useful for IsVisible bindings where you want to show something when data is absent.
/// </summary>
public class ObjectNullConverter : IValueConverter
{
    public static readonly ObjectNullConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value == null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// Returns true if object is not null, false otherwise.
/// Useful for IsVisible bindings where you want to show something when data is present.
/// </summary>
public class ObjectNotNullConverter : IValueConverter
{
    public static readonly ObjectNotNullConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
