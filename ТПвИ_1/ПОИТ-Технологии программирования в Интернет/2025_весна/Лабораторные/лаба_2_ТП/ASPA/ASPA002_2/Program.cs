using Microsoft.Extensions.FileProviders;
using System.IO;

// Создаем строителя (Builder) для веб-приложения
// Этот объект позволяет настроить сервисы и конфигурацию
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Настраиваем список стартовых страниц
var defaultFilesOptions = new DefaultFilesOptions();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("Neumann.html");
app.UseDefaultFiles(defaultFilesOptions); // Подключаем middleware с нашими настройками

// Статические файлы из wwwroot (для / и /static)
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    RequestPath = "/static"
});

// Добавляем обслуживание папки Picture
app.UseStaticFiles(new StaticFileOptions
{
    // Указываем, где физически лежат файлы (в папке Picture в корне проекта)
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Picture")),
    RequestPath = "/Picture"
});

app.Run();