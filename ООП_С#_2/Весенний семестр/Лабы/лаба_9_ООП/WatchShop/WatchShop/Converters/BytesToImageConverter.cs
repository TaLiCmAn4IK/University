using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace WatchShop.Converters
{
    /// <summary>
    /// Конвертирует byte[] (содержимое VARBINARY-поля Image из БД) в BitmapImage
    /// для отображения в WPF Image-элементах через биндинг.
    /// Используется в карточке товара и в окне деталей.
    /// </summary>
    public class BytesToImageConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not byte[] bytes || bytes.Length == 0) return null;

            try
            {
                var img = new BitmapImage();
                using var ms = new MemoryStream(bytes);
                img.BeginInit();
                img.CacheOption = BitmapCacheOption.OnLoad; // полностью загрузить в память
                img.StreamSource = ms;
                img.EndInit();
                img.Freeze();   // разрешает использование из другого потока
                return img;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type t, object p, CultureInfo c)
            => throw new NotImplementedException();
    }
}
