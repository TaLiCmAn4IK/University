using System;
using System.Globalization;
using System.Windows.Data;

namespace WatchShop.Services
{
    // конвертер для локализации текста в xaml
    public class LocalizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string key)
                return LocalizationService.GetString(key);
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}