using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WatchShop.Converters
{
    /// <summary>
    /// bool -> Visibility: true = Visible, false = Collapsed.
    /// Если параметр "Invert" — наоборот.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool b = value is bool flag && flag;
            if (parameter is string s && s.Equals("Invert", StringComparison.OrdinalIgnoreCase))
                b = !b;
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
