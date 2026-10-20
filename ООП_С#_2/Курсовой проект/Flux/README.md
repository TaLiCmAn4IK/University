# Flux

A simplified Faceit-style desktop application built with WPF, EF Core (SQLite), and MVVM.
Supports **CS2 match simulation (5v5)** and **fully playable Chess (1v1)** with realistic Elo, matchmaking, ready-check, admin panel, bilingual UI (English / Russian), and a black-and-yellow e-sports theme.

---

## Requirements

- **.NET 8 SDK** (`net8.0-windows`)
- **Visual Studio 2022** (17.8+) or **JetBrains Rider** 2023.3+
- Windows 10/11 (WPF target)

No SQL Server, no cloud, no manual DB setup — SQLite file is created automatically on first launch.

---

## How to build & run

1. Open `Flux.sln` in Visual Studio.
2. Restore NuGet packages (automatic on first build).
3. Set **Flux** as the startup project (there's only one).
4. Press **F5**.

On first launch the app:

- Creates `faceit.db` (SQLite) next to the executable in `bin/Debug/net8.0-windows/`.
- Seeds one admin account (`admin` / `admin123`) and ~15 bot players (CS2 & chess names like *s1mple*, *Magnus*, etc.) so matchmaking has partners.
- Starts the matchmaking background timer.

### Alternative: command line

```bash
dotnet restore
dotnet build -c Debug
dotnet run --project Flux.csproj
```

---

## Default credentials

| Role   | Login    | Password   |
|--------|----------|------------|
| Admin  | `admin`  | `admin123` |

Bot accounts share password `password`, e.g. login `s1mple` — only useful if you want to test a second human-style account.

---

## What to try

1. **Log in** as `admin` (or register a new account).
2. Click **Play** (yellow nav), select **CS2** or **Chess**, then **Find Match**.
3. Wait a few seconds — the matchmaker will auto-fill with bots whose Elo is within `±200` of yours (widens over time).
4. Hit **Ready** within 10 seconds when the ready-check pops up.
5. **CS2 flow**: server pick → map pick → scoreboard with full K/D/ADR/HLTV stats and per-player Elo delta.
6. **Chess flow**: click a piece to see legal moves (yellow dots), click a target to move. Castling, en passant, promotion (auto-queen), check/mate/stalemate all detected. Bot opponent moves automatically.
7. Visit **Profile** to see Elo, W/L/D, and avg stats over your last 30 CS2 matches.
8. **Match History** shows every match; click **Details** for the full scoreboard or move list.
9. **Admin** tab (visible only for admin role): ban/unban players, view all matches.
10. Toggle language with the **EN / RU** button at any moment — every label refreshes instantly.

---

## Architecture

```
Flux/
├── Models/             # EF Core entities: User, Match, MatchPlayer, QueueEntry
├── DTOs/               # CS2MatchDetails, ChessMatchDetails (serialized to Match.ResultDetails)
├── Data/
│   ├── AppDbContext.cs          # EF Core DbContext + model configuration
│   ├── DatabaseInitializer.cs   # EnsureCreated + admin/bot seed
│   └── Repositories/            # Generic IRepository<T> + typed repositories
├── Services/
│   ├── AuthService.cs           # BCrypt login/register
│   ├── RatingService.cs         # Classic Elo (K=32)
│   ├── CS2GameService.cs        # Match simulation with realistic stats
│   ├── ChessGameService.cs      # Chess match persistence + Elo update
│   ├── MatchmakingService.cs    # Timer-polling matchmaker (1.5 s tick)
│   └── Chess/                   # Piece, Move, ChessBoard (full engine)
├── ViewModels/         # 11 view-models (Login, Register, Main, Queue,
│                       # CS2Match, ChessBoard, Profile, History,
│                       # MatchDetails, PlayerSearch, Admin)
├── Views/              # Matching UserControls for each VM (XAML, no code-behind
│                       # except minimal password sync)
├── Themes/
│   └── DarkYellowTheme.xaml     # Colors, typography, button / textbox / datagrid styles
├── Converters/         # BoolToVisibility, EloDeltaToColor, EnumEqualsVisibility, ...
├── Localization/
│   ├── Strings.resx             # English keys (~100)
│   ├── Strings.ru.resx          # Russian translations
│   └── LanguageService.cs       # Singleton + `{loc:Loc Key}` markup extension
├── Infrastructure/     # NavigationService, DialogService, SessionService, ViewModelBase
├── App.xaml(.cs)       # DI container wiring (Microsoft.Extensions.DependencyInjection)
└── MainWindow.xaml(.cs) # Shell; DataTemplates map each VM to its View
```

### Key design decisions

- **Pure MVVM**: Views contain only `InitializeComponent()` and (for two login/register screens) a tiny `PasswordBox → VM.Password` handler (`PasswordBox` cannot safely DataBind its password).
- **Navigation**: a single `ContentControl.Content` bound to `INavigationService.CurrentViewModel`; a `DataTemplate` dictionary in `MainWindow.xaml` picks the right view.
- **Localization at runtime**: the custom `LocExtension` creates a `Binding` to `LanguageService.Instance[Key]`. Toggling language raises `PropertyChanged("")` which makes **every** localized binding refresh — no restart, no flicker.
- **Matchmaking**: `System.Threading.Timer` polls the `QueueEntry` table every 1.5 s. When enough players with Elo in-window are present (±200 initially, widening at 15 Elo/s up to 1000), it snake-drafts them into two teams and raises `MatchFound`.
   *SignalR alternative:* replace the timer with a `HubConnection` that invokes `QueueChanged` and broadcasts `MatchFound`. The `IMatchmakingService` interface already exposes the same events, so the consumer ViewModels don't change.
- **CS2 simulation**: winner chosen with Elo-weighted probability (`0.25..0.85`), score generated as either MR12 (`13:x`) or OT (`16:12..14`), per-player stats scaled so winners have K/D 1.2-1.6 and losers 0.55-0.9.
- **Chess engine**: bitboard-free, array-based `ChessBoard` with pseudo-legal generation + self-check filtering via board-clone trial. Castling respects attack squares; en-passant tracks a single target square; promotion asks the board for the piece type.
- **Logging**: Serilog writes to console and `logs/faceit-YYYY-MM-DD.log`.

---

## Switching to real EF Core migrations (optional)

The seeder uses `EnsureCreatedAsync()` for frictionless first-run. For production-style migrations:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

Then in `Data/DatabaseInitializer.cs` swap:

```csharp
await db.Database.EnsureCreatedAsync();
// ↓ replace with
await db.Database.MigrateAsync();
```

---

## SignalR instead of polling (optional)

The matchmaker is already behind `IMatchmakingService`. To move to real-time:

1. Add `Microsoft.AspNetCore.SignalR.Client` NuGet.
2. Create a lightweight host (ASP.NET Core Worker Service) exposing a `MatchmakingHub`.
3. Replace the `Timer` in `MatchmakingService` with:

```csharp
var conn = new HubConnectionBuilder().WithUrl("https://localhost:5001/hub/matchmaking").Build();
conn.On<MatchFoundPayload>("MatchFound", p => MatchFound?.Invoke(this, MapPayload(p)));
await conn.StartAsync();
```

4. Push `Enqueue` / `Dequeue` server-side; the hub broadcasts `QueueChanged` to everyone.

The ViewModel layer is unchanged.

---

## Troubleshooting

- **`SqliteException: unable to open database file`** → the working directory isn't writable. Run VS as your normal user; the DB is created at `AppContext.BaseDirectory`.
- **Empty scoreboard after CS2 match** → check the log; deserialization of `ResultDetails` failed. Delete `faceit.db` to reseed.
- **Language toggle doesn't refresh some text** → you used `{Binding Source={x:Static loc:Strings.SomeKey}}` instead of `{loc:Loc SomeKey}`. Only the markup extension refreshes on language change.
- **Bot never moves in chess** → the `DispatcherTimer` requires a running Dispatcher; you'll see this only if you instantiate the VM outside the UI thread.

---

## Credits

- Unicode chess glyphs (U+2654 … U+265F)
- CS2 map & server names are used as pure labels for simulation; no game integration.
- Colors inspired by the official Faceit identity (`#FFCC00` + near-black).
