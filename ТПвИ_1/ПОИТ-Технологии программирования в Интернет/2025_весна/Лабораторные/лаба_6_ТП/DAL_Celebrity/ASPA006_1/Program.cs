using ASPA006_1.Config;
using DAL_Celebrity;
using DAL_Celebrity_MSSQL;

namespace ASPA006_1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. «агружаем конфигурацию из Celebrities.config.json
            builder.Configuration.AddJsonFile("Celebrities.config.json", optional: false, reloadOnChange: true);

            // 2. ѕрив€зываем конфигурацию к классу CelebritiesConfig через IOptions
            builder.Services.Configure<CelebritiesConfig>(builder.Configuration.GetSection("Celebrities"));

            // 3. –егистрируем репозиторий как Scoped-сервис один репозиторий на один HTTP запрос
            builder.Services.AddScoped<IRepository>(provider =>
            {
                var config = provider.GetRequiredService<Microsoft.Extensions.Options.IOptions<CelebritiesConfig>>().Value;
                return Repository.Create(config.ConnectionString);
            });

            // 4. ƒобавл€ем CORS (дл€ доступа с html-страницы)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // 5. Middleware дл€ обработки ошибок
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var errorResponse = new { error = ex.Message, stackTrace = ex.StackTrace };
                    await context.Response.WriteAsJsonAsync(errorResponse);
                }
            });

            app.UseCors("AllowAll");
            app.UseStaticFiles(); // дл€ доступа к html и js

            // ========== 6. Endpoints дл€ знаменитостей (Celebrities) ==========
            var celebrities = app.MapGroup("/api/Celebrities");

            // все знаменитости
            celebrities.MapGet("/", (IRepository repo) => repo.GetAllCelebrities());

            // знаменитость по ID
            celebrities.MapGet("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var celebrity = repo.GetCelebrityById(id);
                return celebrity is null ? Results.NotFound($"Celebrity with id {id} not found") : Results.Ok(celebrity);
            });

            // знаменитость по ID событи€
            celebrities.MapGet("/Lifeevents/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var celebrity = repo.GetCelebrityByLifeeventId(id);
                return celebrity is null ? Results.NotFound($"Celebrity for lifeevent {id} not found") : Results.Ok(celebrity);
            });

            // удалить знаменитость
            celebrities.MapDelete("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var success = repo.DelCelebrity(id);
                return success ? Results.Ok($"Celebrity {id} deleted") : Results.NotFound($"Celebrity {id} not found");
            });

            // добавить знаменитость
            celebrities.MapPost("/", (IRepository repo, Celebrity celebrity) =>
            {
                var success = repo.AddCelebrity(celebrity);
                return success ? Results.Ok(celebrity) : Results.BadRequest("Failed to add celebrity");
            });

            // изменить знаменитость
            celebrities.MapPut("/{id:int:min(1)}", (IRepository repo, int id, Celebrity celebrity) =>
            {
                var success = repo.UpdCelebrity(id, celebrity);
                return success ? Results.Ok(celebrity) : Results.NotFound($"Celebrity {id} not found");
            });

            // получить фото по имени файла
            celebrities.MapGet("/photo/{fname}", async (Microsoft.Extensions.Options.IOptions<CelebritiesConfig> iconfig, string fname) =>
            {
                var photosFolder = iconfig.Value.PhotosFolder;
                var filePath = Path.Combine(photosFolder, fname);

                if (!File.Exists(filePath))
                    return Results.NotFound($"Photo {fname} not found");

                var fileBytes = await File.ReadAllBytesAsync(filePath);
                var contentType = GetContentType(fname);
                return Results.File(fileBytes, contentType);
            });

            // ========== 7. Endpoints дл€ событий (Lifeevents) ==========
            var lifeevents = app.MapGroup("/api/Lifeevents");

            // все событи€
            lifeevents.MapGet("/", (IRepository repo) => repo.GetAllLifeevents());

            // событие по ID
            lifeevents.MapGet("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var lifeevent = repo.GetLifeevetById(id);
                return lifeevent is null ? Results.NotFound($"Lifeevent {id} not found") : Results.Ok(lifeevent);
            });

            // все событи€ по ID знаменитости
            lifeevents.MapGet("/Celebrities/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var events = repo.GetLifeeventsByCelebrityId(id);
                return Results.Ok(events);
            });

            // удалить событие
            lifeevents.MapDelete("/{id:int:min(1)}", (IRepository repo, int id) =>
            {
                var success = repo.DelLifeevent(id);
                return success ? Results.Ok($"Lifeevent {id} deleted") : Results.NotFound($"Lifeevent {id} not found");
            });

            // добавить событие
            lifeevents.MapPost("/", (IRepository repo, Lifeevent lifeevent) =>
            {
                var success = repo.AddLifeevent(lifeevent);
                return success ? Results.Ok(lifeevent) : Results.BadRequest("Failed to add lifeevent");
            });

            // изменить событие
            lifeevents.MapPut("/{id:int:min(1)}", (IRepository repo, int id, Lifeevent lifeevent) =>
            {
                var success = repo.UpdLifeevent(id, lifeevent);
                return success ? Results.Ok(lifeevent) : Results.NotFound($"Lifeevent {id} not found");
            });

            // 8. —тартова€ страница (Index.html)
            app.MapGet("/", () => Results.Redirect("/index.html"));

            // 9. ƒиагностика при запуске (проверка подключени€ к Ѕƒ)
            using (var scope = app.Services.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IRepository>();
                var count = repo.GetAllCelebrities().Count;
                Console.WriteLine($"Ќайдено знаменитостей в Ѕƒ: {count}");
            }

            app.Run();
        }

        private static string GetContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLower();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream"
            };
        }
    }
}