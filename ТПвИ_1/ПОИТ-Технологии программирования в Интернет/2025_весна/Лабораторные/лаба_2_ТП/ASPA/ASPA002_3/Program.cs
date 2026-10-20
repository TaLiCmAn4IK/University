using Microsoft.AspNetCore.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        // Создаем строителя веб-приложения с переданными аргументами командной строки
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddFilter("Microsoft.AspNetCore.Diagnostics", LogLevel.None); // настраиваем фильтр лоирования

        var app = builder.Build();

        app.UseExceptionHandler("/error");    // "/error" - путь, на который будет перенаправлен запрос при возникновении ошибки

        app.MapGet("/", () => "Start ");

        app.MapGet("/test1", () =>
        {
            throw new Exception("--- Exception Test ---"); // пользовательское исключение
        });

        app.MapGet("/test2", () =>
        {
            int x = 0, y = 5, z = 0;
            z = y / x; // деление на 0
            return "test2";
        });

        app.MapGet("/test3", () =>
        {
            int[] x = new int[3] { 1, 2, 3 };
            int y = x[3]; // выход за пределы массива
            return "test3";
        });

        app.Map("/error", async (ILogger<Program> logger, HttpContext context) =>
        {
            // Получаем информацию об исключении из Features коллекции контекста
            // IExceptionHandlerFeature содержит данные об исключении, сохраненные UseExceptionHandler
            IExceptionHandlerFeature? exobj = context.Features.Get<IExceptionHandlerFeature>();
            await context.Response.WriteAsync($"<h1>Oops!</h1>");
            logger.LogError(exobj?.Error, "ExceptionHandler");
        });

        app.Run();
    }
}