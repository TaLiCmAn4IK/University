using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace WatchShop.Services
{
    /// <summary>
    /// Проверяет наличие БД WatchShopDB на SQL Server и создаёт её, если нет.
    /// Запускается при старте приложения. SQL-скрипт создания берётся из
    /// встроенного ресурса Database/InitDatabase.sql.
    /// </summary>
    public class DatabaseInitializer
    {
        public string DatabaseName { get; }
        public string MasterConnectionString { get; }
        public string AppConnectionString { get; }

        public DatabaseInitializer()
        {
            DatabaseName = ConfigurationManager.AppSettings["DatabaseName"] ?? "WatchShopDB";

            MasterConnectionString = ConfigurationManager.ConnectionStrings["MasterConnection"]?.ConnectionString
                ?? throw new InvalidOperationException("MasterConnection не найдена в App.config");
            AppConnectionString = ConfigurationManager.ConnectionStrings["WatchShopConnection"]?.ConnectionString
                ?? throw new InvalidOperationException("WatchShopConnection не найдена в App.config");
        }

        /// <summary>
        /// Возвращает true, если БД создана прямо сейчас (значит, нужен импорт JSON).
        /// </summary>
        public bool EnsureDatabaseExists()
        {
            bool justCreated = false;

            using (var conn = new SqlConnection(MasterConnectionString)) // создаём подключение к системной базе master
            {
                conn.Open();

                using var cmdCheck = new SqlCommand(
                    "SELECT database_id FROM sys.databases WHERE name = @name", conn);
                cmdCheck.Parameters.AddWithValue("@name", DatabaseName); // добавляет параметр
                var result = cmdCheck.ExecuteScalar(); // возвращает первое значение первой строки

                if (result == null || result == DBNull.Value)
                {
                    using var cmdCreate = new SqlCommand(
                        $"CREATE DATABASE [{DatabaseName}]", conn);
                    cmdCreate.ExecuteNonQuery();
                    justCreated = true;
                }
            }

            if (justCreated)
                ExecuteInitScript();

            return justCreated;
        }

        /// <summary>
        /// Выполняет встроенный SQL-скрипт. SQL Server использует "GO" как
        /// разделитель пакетов; ADO.NET его не понимает, поэтому делим скрипт
        /// сами и выполняем по одному пакету.
        /// </summary>
        private void ExecuteInitScript()
        {
            string script = LoadEmbeddedScript();

            var batches = Regex.Split(script, @"^\s*GO\s*$",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

            using var conn = new SqlConnection(AppConnectionString);
            conn.Open();

            foreach (var batch in batches)
            {
                if (string.IsNullOrWhiteSpace(batch)) continue;
                using var cmd = new SqlCommand(batch, conn) { CommandTimeout = 60 };
                cmd.ExecuteNonQuery();
            }
        }

        private static string LoadEmbeddedScript()
        {
            var asm = Assembly.GetExecutingAssembly();
            const string name = "WatchShop.Database.InitDatabase.sql";
            using var stream = asm.GetManifestResourceStream(name)
                ?? throw new FileNotFoundException($"Ресурс {name} не найден в сборке");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
