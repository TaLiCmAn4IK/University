using System;
using System.IO;

namespace WatchShop.Services
{
    // сервис для логирования событий в файл
    public static class LoggerService
    {
        private static readonly string logFilePath;

        static LoggerService()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "WatchShop");

            if (!Directory.Exists(appDataPath))
                Directory.CreateDirectory(appDataPath);

            logFilePath = Path.Combine(appDataPath, "log.txt");
        }

        // запись события в лог
        public static void Log(string message)
        {
            try
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                File.AppendAllText(logFilePath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ошибка логирования: {ex.Message}");
            }
        }

        // запись события о низком количестве товара
        public static void LogLowStock(string productName, int quantity)
        {
            Log($"низкий остаток товара '{productName}': осталось {quantity} шт.");
        }

        // запись события о заказе
        public static void LogOrder(string productName, int quantity, decimal total)
        {
            Log($"оформлен заказ: '{productName}', {quantity} шт., на сумму {total:C}");
        }

        // запись события о входе
        public static void LogLogin(string username)
        {
            Log($"пользователь '{username}' вошел в систему");
        }

        // запись события о выходе
        public static void LogLogout(string username)
        {
            Log($"пользователь '{username}' вышел из системы");
        }

        // запись события об изменении товара
        public static void LogProductChange(string action, string productName)
        {
            Log($"действие '{action}' с товаром '{productName}'");
        }

        // чтение лога
        public static string ReadLog()
        {
            try
            {
                if (File.Exists(logFilePath))
                {
                    return File.ReadAllText(logFilePath);
                }
                return "лог пуст";
            }
            catch (Exception ex)
            {
                return $"ошибка чтения лога: {ex.Message}";
            }
        }

        // очистка лога
        public static void ClearLog()
        {
            try
            {
                if (File.Exists(logFilePath))
                {
                    File.WriteAllText(logFilePath, "");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ошибка очистки лога: {ex.Message}");
            }
        }
    }
}