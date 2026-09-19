using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using NetworkSharp.Services;

namespace NetworkSharp.Converters
{
    /// <summary>
    /// Converts boolean to visibility
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue && boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }

    /// <summary>
    /// Converts zero integer to visibility (visible when zero)
    /// </summary>
    public class ZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is int intValue && intValue == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts standby status to visibility
    /// </summary>
    public class StandbyToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DeviceStatus status && status == DeviceStatus.Standby ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts new status to visibility
    /// </summary>
    public class NewToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is DeviceStatus status && status == DeviceStatus.New ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts port status to color
    /// </summary>
    public class PortStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PortStatus status)
            {
                return status switch
                {
                    PortStatus.Open => new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(166, 227, 161)),
                    PortStatus.Filtered => new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(249, 226, 175)),
                    PortStatus.Closed => new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(108, 112, 134)),
                    _ => new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(108, 112, 134))
                };
            }
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(108, 112, 134));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts signal strength to color
    /// </summary>
    public class SignalStrengthToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int strength)
            {
                if (strength >= 70)
                    return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(166, 227, 161));
                else if (strength >= 40)
                    return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(249, 226, 175));
                else
                    return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(243, 139, 168));
            }
            return new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(108, 112, 134));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts null to visibility (visible when not null)
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Inverts boolean value
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue && !boolValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue && !boolValue;
        }
    }

    /// <summary>
    /// Converts inverse boolean to visibility
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool boolValue && !boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
