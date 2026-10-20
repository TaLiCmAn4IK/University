using DAL004;
using Microsoft.AspNetCore.Diagnostics;

// Устанавливаем имя JSON-файла
IRepository.JSONFileName = "Celebrities.json";

// Создаём репозиторий
using (IRepository repository = Repository.Create("Celebrities"))
{
    var builder = WebApplication.CreateBuilder(args);
    var app = builder.Build();

    app.UseHttpsRedirection();

    // Глобальный обработчик ошибок
    app.UseExceptionHandler(exceptionHandlerApp =>
    {
        exceptionHandlerApp.Run(async context =>
        {
            var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
            var ex = exceptionHandler?.Error;

            context.Response.ContentType = "application/json";

            if (ex is FoundByIdException)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            else if (ex is BadHttpRequestException)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            else if (ex != null)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new
                {
                    type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    title = "ASPA004",
                    status = 500,
                    detail = ex.Message,
                    instance = app.Environment.EnvironmentName
                });
            }
            else
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new
                {
                    type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    title = "ASPA004",
                    status = 500,
                    detail = "Panic",
                    instance = app.Environment.EnvironmentName
                });
            }
        });
    });

    // GET: все записи
    app.MapGet("/Celebrities", () => repository.GetAllPersons());

    // GET: запись по ID
    app.MapGet("/Celebrities/{id:int}", (int id) =>
    {
        var person = repository.GetPersonById(id);
        if (person == null) throw new FoundByIdException($"Celebrity Id = {id}");
        return person;
    });

    // POST: добавить запись
    app.MapPost("/Celebrities", async (HttpContext context) =>
    {
        var person = await context.Request.ReadFromJsonAsync<Person>();
        if (person == null) return Results.BadRequest("Invalid data");

        int? id = repository.AddPerson(person);
        if (id == null) throw new Exception("Failed to add person");

        repository.SaveChanges();
        return Results.Ok(new Person((int)id, person.FirstName, person.Surname, person.PhotoPath));
    });

    // DELETE - удаление записи
    app.MapDelete("/Celebrities/{id:int}", (int id) =>
    {
        var person = repository.GetPersonById(id);
        if (person == null) throw new FoundByIdException($"Celebrity Id = {id}");

        bool deleted = repository.DeletePersonById(id);
        if (!deleted) throw new Exception("Failed to delete person");

        repository.SaveChanges();
        return Results.Ok(new { message = $"Celebrity with Id = {id} deleted" });
    });

    // Все остальные маршруты
    app.MapFallback((HttpContext ctx) =>
        Results.NotFound(new { error = $"path {ctx.Request.Path} not supported" }));

    app.Run();
}

// Исключение для ненайденной записи
public class FoundByIdException : Exception
{
    public FoundByIdException(string message) : base(message) { }
}