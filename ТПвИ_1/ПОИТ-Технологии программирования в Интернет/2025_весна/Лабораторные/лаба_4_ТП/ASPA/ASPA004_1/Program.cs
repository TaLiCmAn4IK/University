using DAL004;
using Microsoft.AspNetCore.Diagnostics;

IRepository.JSONFileName = "Celebrities.json";

IRepository repository = null;
Exception? creationException = null;

try
{
    repository = Repository.Create("Celebrities");
}
catch (Exception ex)
{
    creationException = ex;
    Console.WriteLine($"Ошибка при создании репозитория: {ex.Message}");
}

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.UseHttpsRedirection(); // Включаем перенаправление HTTP на HTTPS для безопасности

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

// Если репозиторий не создан, возвращаем ошибку
if (repository == null || creationException != null)
{
    string errorDetail = creationException?.Message ?? "Неизвестная ошибка";

    app.MapGet("/Celebrities", () => Results.Problem(
        type: "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        title: "ASPA004",
        statusCode: 500,
        detail: errorDetail,
        instance: app.Environment.EnvironmentName
    ));

    app.MapGet("/Celebrities/{id:int}", (int id) => Results.Problem(
        type: "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        title: "ASPA004",
        statusCode: 500,
        detail: errorDetail,
        instance: app.Environment.EnvironmentName
    ));

    app.MapPost("/Celebrities", () => Results.Problem(
        type: "https://tools.ietf.org/html/rfc7231#section-6.6.1",
        title: "ASPA004",
        statusCode: 500,
        detail: errorDetail,
        instance: app.Environment.EnvironmentName
    ));
}
else
{
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
}

// Все остальные маршруты
app.MapFallback((HttpContext ctx) =>
    Results.NotFound(new { error = $"path {ctx.Request.Path} not supported" }));

app.Run();

// Исключение для ненайденной записи
public class FoundByIdException : Exception
{
    public FoundByIdException(string message) : base(message) { }
}