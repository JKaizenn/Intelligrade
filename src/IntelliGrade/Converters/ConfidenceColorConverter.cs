using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using IntelliGrade.App.DTOs;

namespace IntelliGrade.App.Converters;

/// <summary>
/// Converts AiConfidence enum to color for visual indicators.
/// High = Green, Medium = Yellow, Low = Red
/// </summary>
public class ConfidenceColorConverter : IValueConverter
{
    public static readonly ConfidenceColorConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is AiConfidence confidence)
        {
            return confidence switch
            {
                AiConfidence.High => Color.FromRgb(16, 185, 129),    // #10b981 - green
                AiConfidence.Medium => Color.FromRgb(245, 158, 11),  // #f59e0b - yellow
                AiConfidence.Low => Color.FromRgb(239, 68, 68),      // #ef4444 - red
                _ => Colors.Gray
            };
        }

        return Colors.Gray;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
