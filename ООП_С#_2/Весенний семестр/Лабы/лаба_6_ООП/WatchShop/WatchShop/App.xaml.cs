using System;
using System.Windows;
using System.Windows.Threading;

namespace WatchShop
{
    public partial class App : Application
    {
        public App()
        {
            // обработка необработанных исключений в ui потоке
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // обработка необработанных исключений в других потоках
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"произошла ошибка:\n{e.Exception.Message}\n\n{e.Exception.StackTrace}",
                "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show($"критическая ошибка:\n{ex.Message}\n\n{ex.StackTrace}",
                    "ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}