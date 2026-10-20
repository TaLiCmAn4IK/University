using System;
using System.Globalization;
using System.Windows.Data;

namespace WatchShop.Converters
{
    /// <summary>
    /// Инвертирует булево значение. Используется например в StarRating
    /// для IsHitTestVisible="not(IsReadOnly)".
    /// </summary>
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !(value is bool b && b);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => !(value is bool b && b);
    }
}
