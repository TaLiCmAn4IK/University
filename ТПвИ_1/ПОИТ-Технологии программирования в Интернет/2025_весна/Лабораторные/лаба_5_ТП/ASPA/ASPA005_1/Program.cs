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

            if (ex is FoundByIdException) // (запись не найдена)
            {
                context.Response.StatusCode = 404;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            else if (ex is BadHttpRequestException) // (неверный формат запроса)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsJsonAsync(new { error = ex.Message });
            }
            else if (ex != null) // другое исключение
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsJsonAsync(new
                {
                    type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    title = "ASPA005",
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
                    title = "ASPA005",
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

    // POST: добавить запись с фильтрами
    app.MapPost("/Celebrities", (Person person) =>
    {
        int? id = repository.AddPerson(person);
        if (id == null) throw new Exception("Failed to add person");

        repository.SaveChanges();
        return Results.Ok(new Person((int)id, person.FirstName, person.Surname, person.PhotoPath));
    })
    .AddEndpointFilter(async (context, next) =>
    {
        var person = context.GetArgument<Person>(0);
        if (person == null)
        {
            return Results.Problem(
                title: "ASPA005",
                detail: "POST /Celebrities error, person is null",
                statusCode: 500
            );
        }

        if (string.IsNullOrEmpty(person.Surname) || person.Surname.Length < 2)
        {
            return Results.Problem(
                title: "ASPA005",
                detail: "POST /Celebrities error, Surname is wrong",
                statusCode: 409
            );
        }

        return await next(context);
    })
    .AddEndpointFilter(async (context, next) =>
    {
        var person = context.GetArgument<Person>(0);
        if (person == null)
        {
            return Results.Problem(
                title: "ASPA005",
                detail: "POST /Celebrities error, person is null",
                statusCode: 500
            );
        }

        var existing = repository.GetAllPersons().FirstOrDefault(p =>
            p.Surname.Equals(person.Surname, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
        {
            return Results.Problem(
                title: "ASPA005",
                detail: "POST /Celebrities error, Surname is doubled",
                statusCode: 409
            );
        }

        return await next(context);
    })
    .AddEndpointFilter(async (context, next) =>
    {
        var person = context.GetArgument<Person>(0);
        if (person == null)
        {
            return Results.Problem(
                title: "ASPA005",
                detail: "POST /Celebrities error, person is null",
                statusCode: 500
            );
        }

        string photoFileName = Path.GetFileName(person.PhotoPath);
        string photoPath = Path.Combine(Directory.GetCurrentDirectory(), "Celebrities", "Photo", photoFileName);

        if (!File.Exists(photoPath))
        {
            var httpContext = context.HttpContext;
            httpContext.Response.Headers.Append("X-Celebrity", $"NotFound={photoFileName}");
            return Results.Ok(new { message = "Celebrity added but photo file not found" });
        }

        return await next(context);
    });

    // DELETE: удалить запись
    app.MapDelete("/Celebrities/{id:int}", (int id) =>
    {
        var person = repository.GetPersonById(id);
        if (person == null) throw new FoundByIdException($"Celebrity Id = {id}");

        bool deleted = repository.DeletePersonById(id);
        if (!deleted) throw new Exception("Failed to delete person");

        repository.SaveChanges();
        return Results.Ok(new { message = $"Celebrity with Id = {id} deleted" });
    });

    // PUT - обновить запись
    app.MapPut("/Celebrities/{id:int}", async (int id, HttpContext context) =>
    {
        var updatedPerson = await context.Request.ReadFromJsonAsync<Person>();
        if (updatedPerson == null) return Results.BadRequest("Invalid data");

        var existingPerson = repository.GetPersonById(id);
        if (existingPerson == null) throw new FoundByIdException($"Celebrity Id = {id}");

        bool updated = repository.UpdatePersonById(id, updatedPerson);
        if (!updated) throw new Exception("Failed to update person");

        repository.SaveChanges();
        return Results.Ok(new Person(id, updatedPerson.FirstName, updatedPerson.Surname, updatedPerson.PhotoPath));
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