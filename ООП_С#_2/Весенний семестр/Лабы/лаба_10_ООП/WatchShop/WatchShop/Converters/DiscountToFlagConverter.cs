using System;
using System.Globalization;
using System.Windows.Data;

namespace WatchShop.Converters
{
    /// <summary>
    /// Возвращает true если входное число > 0.
    /// Используется в DataTrigger карточки товара для определения,
    /// действует ли скидка (Discount > 0).
    /// </summary>
    public class DiscountToFlagConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                double d = System.Convert.ToDouble(value);
                return d > 0;
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
