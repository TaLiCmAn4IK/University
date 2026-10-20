using Serilog;

var builder = WebApplication.CreateBuilder(args);

// заставит приложение использовать нашу конфигурацию
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Регистрация нашего сервиса очередей(Теперь нужно сказать ASP.NET Core,
// чтобы он создал этот сервис как Singleton и умел внедрять его в наши будущие контроллеры.)
builder.Services.AddSingleton<ASPA0011_1.Services.ChannelService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
