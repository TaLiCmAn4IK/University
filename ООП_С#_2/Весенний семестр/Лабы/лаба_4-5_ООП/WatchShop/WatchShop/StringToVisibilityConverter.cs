using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WatchShop
{
    // конвертер для отображения заглушки при отсутствии изображения
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // если строка пустая или null - показываем заглушку
            // если строка не пустая - скрываем заглушку
            return string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}