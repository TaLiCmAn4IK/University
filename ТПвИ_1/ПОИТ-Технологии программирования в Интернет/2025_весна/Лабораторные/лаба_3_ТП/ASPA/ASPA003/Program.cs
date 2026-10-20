using DAL003;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;

IRepository.JSONFileName = "people.json";

using (IRepository repository = Repository.Create("Database"))
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddDirectoryBrowser();
    var app = builder.Build();

    app.UseHttpsRedirection();

    // Статические файлы из Database/Photo (для 4 задания) 
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "Database", "Photo")),
        RequestPath = "/Photo"
    });

    // DirectoryBrowser для просмотра папки (для 5 задания)
    app.UseDirectoryBrowser(new DirectoryBrowserOptions
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "Database", "Photo")),
        RequestPath = "/Photo"
    });

    // Эндпоинт для скачивания
    app.MapGet("/Download/{filename}", async (string filename) =>
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "Photo", filename);
        if (File.Exists(filePath))
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return Results.File(memory, "application/octet-stream", filename);
        }
        return Results.NotFound("Файл не найден");
    });

    app.MapGet("/", () => Results.Redirect("/Celebrities"));

    // Minimal API 
    app.MapGet("/Celebrities", () => repository.GetAllPersons());
    app.MapGet("/Celebrities/{id:int}", (int id) => repository.GetPersonById(id));
    app.MapGet("/Celebrities/BySurname/{surname}", (string surname) =>
        repository.GetPersonsBySurname(surname));
    app.MapGet("/Celebrities/PhotoPathById/{id:int}", (int id) =>
        repository.GetPhotoPathById(id)?.PhotoPath);
    app.MapGet("/Celebrities/All", () => repository.GetAllPersons());

    app.Run();
}