using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

/// <summary>
/// Compares a value with the converter parameter for equality.
/// Returns true if they are equal, false otherwise.
/// For CriterionLevel objects, compares by Points property.
/// </summary>
public class EqualityConverter : IValueConverter
{
    public static readonly EqualityConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null && parameter == null)
            return true;

        if (value == null || parameter == null)
            return false;

        // Handle CriterionLevel comparisons
        if (value is IntelliGrade.App.Models.CriterionLevel level1 &&
            parameter is IntelliGrade.App.Models.CriterionLevel level2)
            return level1.Points == level2.Points && level1.Label == level2.Label;

        // Handle numeric comparisons
        if (value is int intValue && parameter is int intParam)
            return intValue == intParam;

        // Direct reference equality
        if (ReferenceEquals(value, parameter))
            return true;

        // Try converting both to strings for comparison
        return value.ToString() == parameter.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// MultiValue converter that compares two values for equality.
/// Used with MultiBinding to compare values from different sources.
/// </summary>
public class MultiValueEqualityConverter : IMultiValueConverter
{
    public static readonly MultiValueEqualityConverter Instance = new();

    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Count < 2)
            return false;

        var first = values[0];
        var second = values[1];

        if (first == null && second == null)
            return true;

        if (first == null || second == null)
            return false;

        // Handle int? (nullable int) comparisons
        if (first is int intFirst && second is int intSecond)
            return intFirst == intSecond;

        // Direct reference equality
        if (ReferenceEquals(first, second))
            return true;

        return first.Equals(second);
    }
}
