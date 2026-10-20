// Объявление класса Program, содержащего точку входа приложения
internal class Program
{
    // Метод Main — стандартная точка входа для консольного и веб‑приложения .NET
    private static void Main(string[] args)
    {
        // Создаём объект WebApplicationBuilder,
        // который настраивает веб‑хост, конфигурацию и контейнер служб
        var builder = WebApplication.CreateBuilder(args);

        // Регистрируем службу HTTP‑логирования
        builder.Services.AddHttpLogging(options =>
        {
            // Логируем основные данные запроса и ответа:
            // метод, путь, заголовки, статус‑код и т.д.
            options.LoggingFields =
                Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.Request |
                Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.Response;
        });

        // На основе настроек builder строим объект веб‑приложения WebApplication
        var app = builder.Build();

        // Включаем middleware HTTP‑логирования в конвейере
        app.UseHttpLogging();

        // Регистрируем обработчик HTTP‑запроса GET по адресу "/"
        // При обращении к корню сайта возвращается строка "Моё первое ASPA"
        app.MapGet("/", () => "Моё первое ASPA");

        // Запускаем веб‑сервер Kestrel и начинаем обработку входящих HTTP‑запросов
        app.Run();
    }
}
