using System;
using System.Windows;
using System.Windows.Threading;

namespace WatchShop
{
    public partial class App : Application
    {
        public App()
        {
            // Глобальный обработчик ошибок UI-потока
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            // Глобальный обработчик ошибок других потоков
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        private void OnDispatcherUnhandledException(object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                $"Произошла ошибка:\n{e.Exception.Message}",
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            e.Handled = true;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show(
                    $"Критическая ошибка:\n{ex.Message}",
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
