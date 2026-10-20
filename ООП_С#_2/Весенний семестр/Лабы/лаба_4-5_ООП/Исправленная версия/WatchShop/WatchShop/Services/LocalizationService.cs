using System;
using System.Linq;
using System.Windows;

namespace WatchShop.Services
{
    /// <summary>
    /// Сервис динамической локализации на основе Resource Dictionary.
    /// При смене языка подменяет словарь Strings.{lang}.xaml в Application.Current.Resources.
    /// Все XAML-строки привязаны через {DynamicResource Key} — обновятся автоматически.
    /// </summary>
    public static class LocalizationService
    {
        public const string LanguageRu = "ru-RU";
        public const string LanguageEn = "en-US";

        private static readonly Uri RuDictUri =
            new Uri("/WatchShop;component/Localization/Strings.ru.xaml", UriKind.Relative);
        private static readonly Uri EnDictUri =
            new Uri("/WatchShop;component/Localization/Strings.en.xaml", UriKind.Relative);

        public static string CurrentLanguage { get; private set; } = LanguageRu;

        /// <summary>
        /// Срабатывает после смены языка — нужен только тем местам, где
        /// перевод используется в коде (например, форматирование MessageBox).
        /// </summary>
        public static event EventHandler? LanguageChanged;

        /// <summary>
        /// Установить язык. Удаляет старый словарь Strings.* и подгружает новый.
        /// </summary>
        public static void ChangeLanguage(string language)
        {
            if (language != LanguageRu && language != LanguageEn)
                language = LanguageRu;

            var resources = Application.Current.Resources.MergedDictionaries;

            // удаляем существующий словарь локализации (если есть)
            var existing = resources.FirstOrDefault(d =>
                d.Source != null &&
                (d.Source.OriginalString.Contains("Strings.ru.xaml") ||
                 d.Source.OriginalString.Contains("Strings.en.xaml")));

            if (existing != null)
                resources.Remove(existing);

            // загружаем новый словарь и вставляем ПЕРВЫМ, чтобы стили его не перекрывали
            var newDict = new ResourceDictionary
            {
                Source = language == LanguageRu ? RuDictUri : EnDictUri
            };
            resources.Insert(0, newDict);

            CurrentLanguage = language;
            LanguageChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Получить строку из текущего словаря (для использования в C#-коде).
        /// В XAML напрямую используется {DynamicResource Key}.
        /// </summary>
        public static string GetString(string key)
        {
            var value = Application.Current.TryFindResource(key) as string;
            return value ?? key;
        }

        /// <summary>
        /// Форматированная строка из ресурсов (для сообщений с подстановкой).
        /// </summary>
        public static string Format(string key, params object[] args)
        {
            try
            {
                return string.Format(GetString(key), args);
            }
            catch
            {
                return GetString(key);
            }
        }
    }
}
