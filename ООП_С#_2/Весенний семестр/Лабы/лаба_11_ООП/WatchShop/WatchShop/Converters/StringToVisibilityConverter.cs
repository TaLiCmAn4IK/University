using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WatchShop.Converters
{
    /// <summary>
    /// Если строка пустая — Visible (показать заглушку),
    /// если строка не пустая — Collapsed (заглушка скрыта).
    /// Используется для отображения иконки часов, когда нет картинки.
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.IsNullOrEmpty(value as string) ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
