using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace BatchPay.Frontend.Converters;

public sealed class ReferenceEqualsConverter : IValueConverter
{
    public object Convert( object? value, Type targetType, object? parameter, CultureInfo culture )
        => ReferenceEquals( value, parameter );

    public object ConvertBack( object? value, Type targetType, object? parameter, CultureInfo culture )
        => throw new NotSupportedException();
}