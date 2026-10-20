using System;
using System.Globalization;
using System.Windows.Data;

namespace WatchShop.Converters
{
    /// <summary>
    /// Возвращает true, если на складе осталось от 1 до 3 шт. — "мало товара".
    /// При 0 → false (для этого случая показывается оверлей "нет в наличии").
    /// При 4+ → false.
    /// </summary>
    public class LowStockConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int q) return q > 0 && q <= 3;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
