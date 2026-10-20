using System;
using System.IO;
using System.Windows;
using Flux.Data;
using Flux.Data.Repositories;
using Flux.Infrastructure;
using Flux.Models;
using Flux.Services;
using Flux.ViewModels;
using Flux.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Windows.Input;

namespace Flux;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        LoadCursors();

        // Базовый курсор для каждого окна
        EventManager.RegisterClassHandler(
            typeof(Window),
            FrameworkElement.LoadedEvent,
            new RoutedEventHandler((s, _) =>
            {
                if (s is Window w && CursorNormal != null)
                    w.Cursor = CursorNormal;
            }));

        EventManager.RegisterClassHandler(
            typeof(System.Windows.Controls.TextBox),
            UIElement.MouseEnterEvent,
            new MouseEventHandler((s, _) =>
            {
                if (s is FrameworkElement fe && CursorText != null)
                    fe.Cursor = CursorText;
            }));

        EventManager.RegisterClassHandler(
            typeof(System.Windows.Controls.PasswordBox),
            UIElement.MouseEnterEvent,
            new MouseEventHandler((s, _) =>
            {
                if (s is FrameworkElement fe && CursorText != null)
                    fe.Cursor = CursorText;
            }));

        // Курсор-«ссылка» для кнопок: ставим в MouseEnter а не в Loaded.
        // Loaded срабатывает один раз — но WPF может пересоздавать визуальное дерево
        // (смена шаблона при наведении/нажатии), и значение Cursor «слетает».
        // MouseEnter гарантированно срабатывает каждый раз при входе курсора в кнопку.
        EventManager.RegisterClassHandler(
            typeof(System.Windows.Controls.Primitives.ButtonBase),
            UIElement.MouseEnterEvent,
            new MouseEventHandler((s, _) =>
            {
                if (s is FrameworkElement fe && CursorLink != null)
                    fe.Cursor = CursorLink;
            }));

        // Global exception handlers — log instead of crashing silently.
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            Log.Error(args.ExceptionObject as Exception, "Unhandled domain exception");
        DispatcherUnhandledException += (_, args) =>
        {
            Log.Error(args.Exception, "Unhandled dispatcher exception");
            args.Handled = true;
        };
        System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (_, args) =>
        {
            Log.Error(args.Exception, "Unobserved task exception");
            args.SetObserved();
        };

        // Логи кладём в %LOCALAPPDATA%\Flux\logs — этот путь всегда доступен на запись,
        // в отличие от папки установки (Program Files) при запуске без прав администратора.
        var logDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Flux", "logs");
        Directory.CreateDirectory(logDir);
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(Path.Combine(logDir, "faceit-.log"), rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var services = new ServiceCollection();
        ConfigureServices(services);
        Services = services.BuildServiceProvider();

        // Create DB + seed. Ошибку показываем пользователю и корректно завершаемся —
        // иначе процесс «висел» бы без окна (как было при недоступном SQL Server).
        try
        {
            using var scope = Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await DatabaseInitializer.InitializeAsync(db);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Не удалось инициализировать базу данных");
            MessageBox.Show(
                "Не удалось подготовить базу данных приложения.\n\n" + ex.Message,
                "Flux — ошибка запуска", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
            return;
        }

        // Start matchmaker
        var mm = Services.GetRequiredService<IMatchmakingService>();
        mm.Start();

        // Start tournament auto-runner (фоновая логика: автозапуск турниров, разыгрывание матчей)
        var ts = Services.GetRequiredService<ITournamentService>();
        ts.StartAutoRunner();

        // Show main window
        var main = Services.GetRequiredService<MainWindow>();
        main.Show();
    }

    public static Cursor? CursorNormal { get; private set; }
    public static Cursor? CursorLink { get; private set; }
    public static Cursor? CursorBusy { get; private set; }
    public static Cursor? CursorText { get; private set; }
    public static Cursor? CursorPrecision { get; private set; }
    public static Cursor? CursorUnavailable { get; private set; }

    private static Cursor? TryLoadCursor(string fileName)
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Cursors", fileName);
            return File.Exists(path) ? new Cursor(path) : null;
        }
        catch { return null; }
    }

    private static void LoadCursors()
    {
        CursorNormal = TryLoadCursor("Usagi Location.ani");
        CursorLink = TryLoadCursor("Usagi Link.ani");
        CursorBusy = TryLoadCursor("Usagi Busy.ani");
        CursorText = TryLoadCursor("Usagi Text.ani");
        CursorPrecision = TryLoadCursor("Usagi Precision.ani");
        CursorUnavailable = TryLoadCursor("Usagi Unavailable.ani");
    }

    protected override void OnExit(ExitEventArgs e)
    {
        Services.GetService<IMatchmakingService>()?.Stop();
        Services.GetService<ITournamentService>()?.StopAutoRunner();
        Log.CloseAndFlush();
        base.OnExit(e);
    }

    private void ConfigureServices(IServiceCollection services)
    {
        // Logging
        services.AddLogging(b =>
        {
            b.ClearProviders();
            b.AddSerilog(dispose: true);
        });

        // Конфигурация необязательна — по умолчанию БД создаётся автоматически.
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();

        // БД — локальный файл SQLite: внешний сервер не нужен, приложение работает
        // на любом компьютере «из коробки». Файл хранится в %LOCALAPPDATA%\Flux
        // (доступен на запись без прав администратора; два клиента на одном ПК
        // используют один и тот же файл и видят очередь/лобби друг друга).
        // appsettings.json может переопределить путь, задав строку подключения SQLite.
        var connStr = configuration.GetConnectionString("Flux");
        if (string.IsNullOrWhiteSpace(connStr))
        {
            var dataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Flux");
            Directory.CreateDirectory(dataDir);
            connStr = $"Data Source={Path.Combine(dataDir, "flux.db")}";
        }

        services.AddDbContext<AppDbContext>(
            o => o.UseSqlite(connStr).AddInterceptors(new SqlitePragmaInterceptor()),
            contextLifetime: ServiceLifetime.Scoped);

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Services
        services.AddSingleton<ISessionService, SessionService>();
        services.AddSingleton<ISessionPersistence, JsonSessionPersistence>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IPartyService, PartyService>();
        services.AddSingleton<IFriendsService, FriendsService>();

        // Game / auth services create their own DbContext scopes internally (IServiceScopeFactory),
        // so they are safe to register as singletons.
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IRatingService, RatingService>();
        services.AddSingleton<ICS2GameService, CS2GameService>();
        services.AddSingleton<IChessGameService, ChessGameService>();
        services.AddSingleton<IMatchmakingService, MatchmakingService>();
        services.AddSingleton<IAvatarService, AvatarService>();
        services.AddSingleton<IInvitationService, InvitationService>();
        services.AddSingleton<IMatchProposalSync, MatchProposalSync>();
        services.AddSingleton<ITournamentService, TournamentService>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddTransient<RegisterViewModel>();
        services.AddTransient<MainViewModel>();
        services.AddTransient<QueueViewModel>();
        services.AddTransient<CS2MatchViewModel>();
        services.AddTransient<ChessBoardViewModel>();
        services.AddTransient<ProfileViewModel>();
        services.AddTransient<MatchDetailsViewModel>();
        services.AddTransient<PlayerSearchViewModel>();
        services.AddTransient<AdminViewModel>();
        services.AddTransient<FriendsViewModel>();
        services.AddSingleton<LeaderboardViewModel>();
        services.AddSingleton<TournamentsViewModel>();

        // Windows
        services.AddSingleton<MainWindow>();
    }
}
