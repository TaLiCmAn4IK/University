using System;
using System.Linq;
using System.Windows;

namespace WatchShop.Services
{
    /// <summary>
    /// Сервис переключения тем оформления.
    /// Подменяет ResourceDictionary темы в Application.Current.Resources.
    /// Все элементы UI, использующие {DynamicResource ...Brush}, обновятся автоматически.
    /// </summary>
    public static class ThemeService
    {
        public const string ThemeLight = "Light";
        public const string ThemeDark = "Dark";
        public const string ThemePink = "Pink";

        private static readonly Uri LightUri =
            new Uri("/WatchShop;component/Themes/Theme.Light.xaml", UriKind.Relative);
        private static readonly Uri DarkUri =
            new Uri("/WatchShop;component/Themes/Theme.Dark.xaml", UriKind.Relative);
        private static readonly Uri PinkUri =
            new Uri("/WatchShop;component/Themes/Theme.Pink.xaml", UriKind.Relative);

        public static string CurrentTheme { get; private set; } = ThemeLight;

        public static event EventHandler? ThemeChanged;

        /// <summary>
        /// Переключает тему. Удаляет текущий словарь темы из MergedDictionaries
        /// и подключает новый. Делается строго первым в коллекции, чтобы стили
        /// (Styles.xaml) могли использовать кисти отсюда.
        /// </summary>
        public static void ChangeTheme(string theme)
        {
            Uri uri = theme switch
            {
                ThemeDark => DarkUri,
                ThemePink => PinkUri,
                _ => LightUri,
            };

            var mergedDicts = Application.Current.Resources.MergedDictionaries;

            // ищем активный словарь темы по характерному пути
            var existing = mergedDicts.FirstOrDefault(d =>
                d.Source != null && d.Source.OriginalString.Contains("Themes/Theme."));

            var newDict = new ResourceDictionary { Source = uri };

            if (existing != null)
            {
                int idx = mergedDicts.IndexOf(existing);
                mergedDicts[idx] = newDict;
            }
            else
            {
                mergedDicts.Insert(0, newDict);
            }

            CurrentTheme = theme switch
            {
                ThemeDark => ThemeDark,
                ThemePink => ThemePink,
                _ => ThemeLight,
            };

            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}
