using DAL004;
using Microsoft.AspNetCore.Diagnostics;

// Устанавливаем имя JSON-файла
IRepository.JSONFileName = "Celebrities.json";

// Создаём репозиторий
using (IRepository repository = Repository.Create("Celebrities"))
{
    // Устанавливаем ссылку на репозиторий для фильтров
    RepositoryHolder.Repository = repository;

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

    // Группа маршрутов для Celebrities
    var celebritiesGroup = app.MapGroup("/Celebrities");

    // GET: все записи
    celebritiesGroup.MapGet("/", () => repository.GetAllPersons());

    // GET: запись по ID
    celebritiesGroup.MapGet("/{id:int}", (int id) =>
    {
        var person = repository.GetPersonById(id);
        if (person == null) throw new FoundByIdException($"Celebrity Id = {id}");
        return person;
    });

    // POST: добавить запись с фильтрами
    celebritiesGroup.MapPost("/", (Person person) =>
    {
        int? id = repository.AddPerson(person);
        if (id == null) throw new Exception("Failed to add person");

        repository.SaveChanges();
        return Results.Ok(new Person((int)id, person.FirstName, person.Surname, person.PhotoPath));
    })
    .AddEndpointFilter<SurnameValidationFilter>()
    .AddEndpointFilter<UniqueSurnameFilter>()
    .AddEndpointFilter<PhotoExistsFilter>();

    // PUT: обновить запись с фильтром
    celebritiesGroup.MapPut("/{id:int}", async (int id, Person updatedPerson) =>
    {
        bool updated = repository.UpdatePersonById(id, updatedPerson);
        if (!updated) throw new Exception("Failed to update person");

        repository.SaveChanges();
        return Results.Ok(new Person(id, updatedPerson.FirstName, updatedPerson.Surname, updatedPerson.PhotoPath));
    })
    .AddEndpointFilter<PutValidationFilter>();

    // DELETE: удалить запись с фильтром
    celebritiesGroup.MapDelete("/{id:int}", (int id) =>
    {
        bool deleted = repository.DeletePersonById(id);
        if (!deleted) throw new Exception("Failed to delete person");

        repository.SaveChanges();
        return Results.Ok(new { message = $"Celebrity with Id = {id} deleted" });
    })
    .AddEndpointFilter<DeleteValidationFilter>();

    // Все остальные маршруты
    app.MapFallback((HttpContext ctx) =>
        Results.NotFound(new { error = $"path {ctx.Request.Path} not supported" }));

    app.Run();
}

// Статический класс для хранения ссылки на репозиторий
public static class RepositoryHolder
{
    public static IRepository? Repository { get; set; }
}

// Фильтр 1: проверка Surname (не null и длина >= 2)
public class SurnameValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
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
    }
}

// Фильтр 2: проверка на дублирование фамилии
public class UniqueSurnameFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
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

        var repository = RepositoryHolder.Repository;
        if (repository == null)
        {
            return Results.Problem(
                title: "ASPA005",
                detail: "Repository not initialized",
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
    }
}

// Фильтр 3: проверка существования файла фотографии
public class PhotoExistsFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Сначала выполняем основной обработчик (добавляем запись)
        var result = await next(context);

        // После добавления записи проверяем фото и добавляем заголовок
        var person = context.GetArgument<Person>(0);
        if (person != null)
        {
            string photoFileName = Path.GetFileName(person.PhotoPath);
            string photoPath = Path.Combine(Directory.GetCurrentDirectory(), "Celebrities", "Photo", photoFileName);

            if (!File.Exists(photoPath))
            {
                var httpContext = context.HttpContext;
                httpContext.Response.Headers.Append("X-Celebrity", $"NotFound={photoFileName}");
            }
        }

        return result;
    }
}

// Фильтр для PUT: проверка существования записи и корректности фамилии
public class PutValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Получаем id (первый аргумент)
        var id = context.GetArgument<int>(0);

        // Получаем Person (второй аргумент)
        var updatedPerson = context.GetArgument<Person>(1);

        var repository = RepositoryHolder.Repository;
        if (repository == null)
        {
            return Results.Problem(
                title: "ASPA005",
                detail: "Repository not initialized",
                statusCode: 500
            );
        }

        // Проверка существования записи
        var existingPerson = repository.GetPersonById(id);
        if (existingPerson == null)
        {
            return Results.Problem(
                title: "ASPA005",
                detail: $"Celebrity with Id = {id} not found",
                statusCode: 404
            );
        }

        // Проверка корректности фамилии
        if (updatedPerson == null || string.IsNullOrEmpty(updatedPerson.Surname) || updatedPerson.Surname.Length < 2)
        {
            return Results.Problem(
                title: "ASPA005",
                detail: "PUT /Celebrities error, Surname is wrong",
                statusCode: 409
            );
        }

        return await next(context);
    }
}

// Фильтр для DELETE: проверка существования записи
public class DeleteValidationFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var id = context.GetArgument<int>(0);

        var repository = RepositoryHolder.Repository;
        if (repository == null)
        {
            return Results.Problem(
                title: "ASPA005",
                detail: "Repository not initialized",
                statusCode: 500
            );
        }

        // Проверка существования записи
        var existingPerson = repository.GetPersonById(id);
        if (existingPerson == null)
        {
            return Results.Problem(
                title: "ASPA005",
                detail: $"Celebrity with Id = {id} not found",
                statusCode: 404
            );
        }

        return await next(context);
    }
}

// Исключение для ненайденной записи
public class FoundByIdException : Exception
{
    public FoundByIdException(string message) : base(message) { }
}