using ASPA008_1.Config;
using ASPA008_1.Services;
using DAL_Celebrity_MSSQL;
using Microsoft.Extensions.Options;

namespace ASPA008_1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration.AddJsonFile("Celebrities.config.json",
                                              optional: false, reloadOnChange: true);
            builder.Services.Configure<CelebritiesConfig>(
                builder.Configuration.GetSection("Celebrities"));

            builder.Services.AddScoped<IRepository>(provider =>
            {
                var config = provider.GetRequiredService<IOptions<CelebritiesConfig>>().Value;
                return Repository.Create(config.ConnectionString);
            });

            // Справочник стран 
            builder.Services.AddSingleton<ICountryCodesService, CountryCodesService>();

            // Заголовки представлений
            builder.Services.AddSingleton<CelebrityTitles>();

            // Action-фильтр Wikipedia 
            builder.Services.AddScoped<ASPA008_1.Filters.WikipediaLinksFilter>();

            // ASP.NET Core MVC
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Middleware-обработчик ошибок
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Celebrities/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            app.MapGet("/photo/{fname}",
                async (IOptions<CelebritiesConfig> iconfig, string fname) =>
                {
                    var filePath = Path.Combine(iconfig.Value.PhotosFolder, fname);
                    if (!File.Exists(filePath))
                        return Results.NotFound();

                    var bytes = await File.ReadAllBytesAsync(filePath);
                    var ext = Path.GetExtension(fname).ToLower();
                    var ct = ext switch
                    {
                        ".jpg" or ".jpeg" => "image/jpeg",
                        ".png" => "image/png",
                        ".gif" => "image/gif",
                        _ => "application/octet-stream"
                    };
                    return Results.File(bytes, ct);
                });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Celebrities}/{action=Index}/{id?}"); // имя контролера + имя действия + id

            try
            {
                using var scope = app.Services.CreateScope();
                var repo = scope.ServiceProvider.GetRequiredService<IRepository>();
                Console.WriteLine($"Знаменитостей в БД: {repo.GetAllCelebrities().Count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения к БД: {ex.Message}");
            }

            app.Run();
        }
    }
}
