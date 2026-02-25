using SonicBoost.Core.Tweaks.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SonicBoost.Helpers;

public class RiskToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is TweakRisk risk ? risk switch
        {
            TweakRisk.Safe => new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0xC8, 0x53)),
            TweakRisk.Moderate => new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0xA0, 0x00)),
            TweakRisk.Advanced => new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0x17, 0x44)),
            _ => Brushes.Transparent
        } : Brushes.Transparent;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class RiskToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is TweakRisk risk && risk != TweakRisk.Safe ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class StringToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return !string.IsNullOrEmpty(value?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class RiskToRussianConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is TweakRisk risk ? risk switch
        {
            TweakRisk.Safe => "Безопасно",
            TweakRisk.Moderate => "Умеренно",
            TweakRisk.Advanced => "Продвинуто",
            _ => ""
        } : "";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class ServiceStatusToRussianConverter : IValueConverter
{
    private static string ToRussian(string? s)
    {
        return s switch
        {
            "Running" => "Работает",
            "Stopped" => "Остановлена",
            "Automatic" => "Автоматически",
            "Manual" => "Вручную",
            "Disabled" => "Отключено",
            _ => s ?? ""
        };
    }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ToRussian(value?.ToString());
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class BoolToStatusTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? "✓ Включено" : "✗ Выключено";
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class BoolToStatusBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true 
            ? new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0xE6, 0x76)) 
            : new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0x55, 0x55));
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class InverseBoolToStatusTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true ? "✗ Отключена" : "✓ Работает";
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}

public class InverseBoolToStatusBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is true 
            ? new SolidColorBrush(Color.FromArgb(0x33, 0xFF, 0x55, 0x55)) 
            : new SolidColorBrush(Color.FromArgb(0x33, 0x00, 0xE6, 0x76));
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotSupportedException();
}
