using System;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;
using WatchShop.Services;

namespace WatchShop
{
    public partial class App : Application
    {
        public App()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // ===== Инициализация БД на старте =====
            // Если БД WatchShopDB не существует — создаём и накатываем
            // схему (таблицы, view, триггеры, хранимые процедуры).
            try
            {
                var initializer = new DatabaseInitializer();
                bool justCreated = initializer.EnsureDatabaseExists();

                bool importJson = bool.TryParse(
                    ConfigurationManager.AppSettings["ImportFromJsonOnFirstRun"], out var v) && v;
                if (justCreated && importJson)
                {
                    var importer = new JsonImporter();
                    importer.ImportAll();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Не удалось подключиться к SQL Server или создать БД.\n\n" +
                    $"Проверьте:\n" +
                    $"  1) Что SQL Server запущен.\n" +
                    $"  2) Строка подключения в App.config корректна\n" +
                    $"     (имя сервера, инстанс).\n\n" +
                    $"Подробности:\n{ex.Message}",
                    "Ошибка инициализации БД",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Shutdown();
                return;
            }

            base.OnStartup(e);
        }

        private void OnDispatcherUnhandledException(object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"Произошла ошибка:\n{e.Exception.Message}",
                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
                MessageBox.Show($"Критическая ошибка:\n{ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
