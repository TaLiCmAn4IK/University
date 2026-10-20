using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flux.Data.Repositories;
using Flux.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flux.Services;

public class MatchFoundEventArgs : EventArgs
{
    public GameType GameType { get; init; }
    public List<User> Team1 { get; init; } = new();
    public List<User> Team2 { get; init; } = new();
    public int PendingMatchId { get; init; }
    /// <summary>Реальные пользователи в матче (не боты). Один и тот же набор на всех клиентах.</summary>
    public List<int> RealUserIds { get; init; } = new();
    /// <summary>Total players in the match (team1.Count + team2.Count).</summary>
    public int TotalPlayers => Team1.Count + Team2.Count;
    /// <summary>Number of users (including bots) that still need to accept at proposal time.</summary>
    public int NeedAcceptCount { get; init; }
}

public class AcceptProgressEventArgs : EventArgs
{
    public int PendingMatchId { get; init; }
    public int Accepted { get; init; }
    public int Total { get; init; }
}

public interface IMatchmakingService
{
    /// <summary>Fired when a candidate match is formed; the UI should show an Accept popup to the user.</summary>
    event EventHandler<MatchFoundEventArgs>? MatchProposed;
    /// <summary>Fired once ALL users have accepted; the UI should transition into the match.</summary>
    event EventHandler<MatchFoundEventArgs>? MatchFound;
    event EventHandler? QueueUpdated;
    event EventHandler<int>? MatchCancelled;
    /// <summary>Fired as players accept one by one so the UI can update "Ready: X / Y".</summary>
    event EventHandler<AcceptProgressEventArgs>? AcceptProgress;

    Task<bool> EnqueueAsync(User user, GameType gameType);
    Task<bool> EnqueuePartyAsync(IReadOnlyList<User> members, GameType gameType);
    Task DequeueAsync(int userId);
    Task DequeueManyAsync(IEnumerable<int> userIds);
    Task<bool> IsInQueueAsync(int userId);
    Task<int> GetQueueSizeAsync(GameType gameType);

    /// <summary>Called by the user's client to accept the found match. When every real user has accepted,
    /// the match is confirmed. Bots always auto-accept.</summary>
    Task AcceptMatchAsync(int pendingMatchId, int userId);

    /// <summary>Decline → cancel the pending match; all non-decliners are returned to queue.</summary>
    Task DeclineMatchAsync(int pendingMatchId, int userId);

    void Start();
    void Stop();
}

/// <summary>
/// Timer-polling matchmaker. Every tick it scans the QueueEntry table and tries to form
/// a match for each game type where players are within ±elo window (widening over time).
/// To keep things lively without real multiplayer, it auto-adds bot users to fill slots
/// after a short wait.
/// </summary>
public class MatchmakingService : IMatchmakingService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MatchmakingService> _logger;
    private Timer? _timer;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly Guid _ownerToken = Guid.NewGuid(); // уникальный per-процесс — для distributed lock

    public event EventHandler<MatchFoundEventArgs>? MatchFound;
    public event EventHandler? QueueUpdated;
    public event EventHandler<int>? MatchCancelled;
    public event EventHandler<AcceptProgressEventArgs>? AcceptProgress;

    // Pending match state (in-memory). Key = pending match id.
    private readonly Dictionary<int, PendingMatch> _pending = new();
    private int _nextPendingId = 1;

    private const int TickMs = 600;
    private const int InitialEloWindow = 200;
    private const int WindowGrowthPerSecond = 15;
    private const int MaxEloWindow = 1000;
    private const double AutoFillAfterSeconds = 0.5;

    private class PendingMatch
    {
        public int Id;
        public GameType GameType;
        public List<User> Team1 = new();
        public List<User> Team2 = new();
        public HashSet<int> NeedAccept = new();     
        public HashSet<int> RealUserIds = new();     
        public HashSet<int> BotUserIds = new();     
        public HashSet<int> AllUserIds = new();
        public DateTime CreatedAt;
        public int TotalSeconds = ReadyTimeoutSeconds;
        public System.Threading.CancellationTokenSource Cts = new();
        public volatile bool AllHumansAccepted;
    }

    private const int ReadyTimeoutSeconds = 20;

    // Параметры симуляции принятия матча ботами (~95% success, большинство — в пределах 2 секунд)
    private const double BotDeclineChance = 0.01;     // 1% ботов никогда не принимают
    private const double FastAcceptShare   = 0.92;    // 92% ботов принимают в «быстром» окне
    private const int FastWindowMsMin = 100;
    private const int FastWindowMsMax = 1100;         // 0.1..1.1s — основная масса
    private const int SlowWindowMsMin = 1100;
    private const int SlowWindowMsMax = 2400;         // 1.1..2.4s — отстающие
    private const int BetweenBotsMsMin = 80;
    private const int BetweenBotsMsMax = 350;

    public MatchmakingService(IServiceScopeFactory scopeFactory, ILogger<MatchmakingService> logger,
        IMatchProposalSync proposalSync)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
        _proposalSync = proposalSync;

        _proposalSync.ProposalConfirmed += (_, e) => DropPending(e.PendingMatchId);
        _proposalSync.ProposalCancelled += (_, pid) => DropPending(pid);
    }

    /// <summary>Снять ожидающий матч с in-memory учёта и остановить связанные с ним фоновые задачи.</summary>
    private void DropPending(int pendingMatchId)
    {
        PendingMatch? p;
        lock (_pending)
        {
            if (!_pending.TryGetValue(pendingMatchId, out p)) return;
            _pending.Remove(pendingMatchId);
        }
        try { p.Cts.Cancel(); } catch { }
    }

    /// <summary>Удалить ack-записи предложения с задержкой — чтобы все клиенты успели
    /// увидеть финальный статус (подтверждение/отмену) через свой поллинг.</summary>
    private void ScheduleAckCleanup(int pendingMatchId)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                // Запас по времени, кратно превышающий интервал поллинга клиентов (1 сек),
                // чтобы все они гарантированно увидели финальный статус до удаления записей.
                await Task.Delay(TimeSpan.FromSeconds(6));
                await _proposalSync.CleanupAsync(pendingMatchId);
            }
            catch { /* лучшее усилие */ }
        });
    }

    private readonly IMatchProposalSync _proposalSync;

    public void Start()
    {
        _timer ??= new Timer(async _ => await TickAsync(), null, TickMs, TickMs);
    }

    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }

    public async Task<bool> EnqueueAsync(User user, GameType gameType)
    {
        using var scope = _scopeFactory.CreateScope();
        var queue = scope.ServiceProvider.GetRequiredService<IRepository<QueueEntry>>();
        if (await queue.Query().AnyAsync(q => q.UserId == user.Id))
            return false;

        int elo = gameType == GameType.CS2 ? user.EloCS2 : user.EloChess;
        await queue.AddAsync(new QueueEntry
        {
            UserId = user.Id,
            GameType = gameType,
            EloAtQueue = elo,
            CreatedAt = DateTime.UtcNow
        });
        await queue.SaveChangesAsync();
        QueueUpdated?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public async Task<bool> EnqueuePartyAsync(IReadOnlyList<User> members, GameType gameType)
    {
        if (members == null || members.Count == 0) return false;
        using var scope = _scopeFactory.CreateScope();
        var queue = scope.ServiceProvider.GetRequiredService<IRepository<QueueEntry>>();

        var memberIds = members.Select(m => m.Id).ToList();
        if (await queue.Query().AnyAsync(q => memberIds.Contains(q.UserId)))
            return false;

        var now = DateTime.UtcNow;
        foreach (var u in members)
        {
            int elo = gameType == GameType.CS2 ? u.EloCS2 : u.EloChess;
            await queue.AddAsync(new QueueEntry
            {
                UserId = u.Id,
                GameType = gameType,
                EloAtQueue = elo,
                CreatedAt = now
            });
        }
        await queue.SaveChangesAsync();
        QueueUpdated?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public async Task DequeueAsync(int userId)
    {
        using var scope = _scopeFactory.CreateScope();
        var queue = scope.ServiceProvider.GetRequiredService<IRepository<QueueEntry>>();
        var entries = await queue.FindAsync(q => q.UserId == userId);
        foreach (var e in entries) await queue.DeleteAsync(e);
        await queue.SaveChangesAsync();
        QueueUpdated?.Invoke(this, EventArgs.Empty);
    }

    public async Task DequeueManyAsync(IEnumerable<int> userIds)
    {
        using var scope = _scopeFactory.CreateScope();
        var queue = scope.ServiceProvider.GetRequiredService<IRepository<QueueEntry>>();
        var ids = userIds.ToList();
        var entries = await queue.FindAsync(q => ids.Contains(q.UserId));
        foreach (var e in entries) await queue.DeleteAsync(e);
        await queue.SaveChangesAsync();
        QueueUpdated?.Invoke(this, EventArgs.Empty);
    }

    public async Task<bool> IsInQueueAsync(int userId)
    {
        using var scope = _scopeFactory.CreateScope();
        var queue = scope.ServiceProvider.GetRequiredService<IRepository<QueueEntry>>();
        return await queue.Query().AnyAsync(q => q.UserId == userId);
    }

    public async Task<int> GetQueueSizeAsync(GameType gameType)
    {
        using var scope = _scopeFactory.CreateScope();
        var queue = scope.ServiceProvider.GetRequiredService<IRepository<QueueEntry>>();
        return await queue.Query().CountAsync(q => q.GameType == gameType);
    }

    private async Task TickAsync()
    {
        if (!await _lock.WaitAsync(0)) return;
        try
        {
            // Матчи формирует только ОДИН клиент в сети — владелец
            if (!await TryAcquireMatchmakerLockAsync())
            {
                QueueUpdated?.Invoke(this, EventArgs.Empty);
                return;
            }

            await TryFormMatch(GameType.CS2, 10);
            await TryFormMatch(GameType.Chess, 2);
            QueueUpdated?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Matchmaking tick failed");
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <summary>
    /// Пытается взять / продлить эксклюзивный лок формирования матчей.
    /// Возвращает true если этот процесс владеет локом (или только что захватил).
    /// </summary>
    private async Task<bool> TryAcquireMatchmakerLockAsync()
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();

            var now = DateTime.UtcNow;
            var ttl = TimeSpan.FromSeconds(5);
            var renewedExpiry = now.Add(ttl);

            // Лок — это «строка с наименьшим Id». Id НЕ задаём вручную: колонка
            // автоинкрементная, значение назначает БД.
            var row = await db.MatchmakerLocks.OrderBy(l => l.Id).FirstOrDefaultAsync();
            if (row == null)
            {
                // Первый раз — создаём (Id назначит сервер).
                db.MatchmakerLocks.Add(new Models.MatchmakerLock
                {
                    OwnerToken = _ownerToken,
                    AcquiredAt = now,
                    ExpiresAt = renewedExpiry
                });
                await db.SaveChangesAsync();
                return true;
            }

            // Если лок мой — продлеваем
            if (row.OwnerToken == _ownerToken)
            {
                row.ExpiresAt = renewedExpiry;
                await db.SaveChangesAsync();
                return true;
            }

            // Лок чужой и не истёк — пропускаем тик
            if (row.ExpiresAt > now) return false;

            // Лок истёк — захватываем
            row.OwnerToken = _ownerToken;
            row.AcquiredAt = now;
            row.ExpiresAt = renewedExpiry;
            await db.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            // Другой клиент захватил раньше нас — пропускаем тик
            return false;
        }
        catch (Exception ex)
        {
            // Непредвиденная ошибка механизма лока не должна полностью останавливать
            // матчмейкинг — работаем дальше (в худшем случае без защиты от дублей).
            _logger.LogWarning(ex, "Сбой матчмейкер-лока — продолжаем формировать матчи без него");
            return true;
        }
    }

    private async Task ReQueueMembersExceptAsync(PendingMatch p, HashSet<int> excludeUserIds)
    {
        using var scope = _scopeFactory.CreateScope();
        var queue = scope.ServiceProvider.GetRequiredService<IRepository<QueueEntry>>();
        var userRepo = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
        var now = DateTime.UtcNow;

        foreach (var userId in p.AllUserIds)
        {
            if (excludeUserIds.Contains(userId)) continue;
            var user = await userRepo.Query().FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) continue;
            // Skip if already in queue (shouldn't be, but guard)
            if (await queue.Query().AnyAsync(q => q.UserId == userId)) continue;
            int elo = p.GameType == GameType.CS2 ? user.EloCS2 : user.EloChess;
            await queue.AddAsync(new QueueEntry
            {
                UserId = userId,
                GameType = p.GameType,
                EloAtQueue = elo,
                CreatedAt = now
            });
        }
        await queue.SaveChangesAsync();
    }

    public async Task AcceptMatchAsync(int pendingMatchId, int userId)
    {
        PendingMatch? p;
        lock (_pending)
        {
            if (!_pending.TryGetValue(pendingMatchId, out p)) p = null;
        }
        if (p != null)
        {
            lock (_pending)
            {
                p.NeedAccept.Remove(userId);
                if (!p.AllHumansAccepted && p.NeedAccept.All(id => p.BotUserIds.Contains(id)))
                    p.AllHumansAccepted = true;
            }
            RaiseAcceptProgress(p);
            CheckAndConfirm(p);
        }

        try { await _proposalSync.AcceptAsync(pendingMatchId, userId); } catch { }
    }

    public async Task DeclineMatchAsync(int pendingMatchId, int userId)
    {
        try { await _proposalSync.DeclineAsync(pendingMatchId, userId); } catch { }
        PendingMatch? p;
        lock (_pending)
        {
            if (!_pending.TryGetValue(pendingMatchId, out p)) return;
        }
        p.NeedAccept.Add(userId);
        await CancelMatchAsync(p, acceptedReturnToQueue: true);
        _logger.LogInformation("Pending match {Id} declined by user {User}", pendingMatchId, userId);
    }
    // Основной алгоритм формирования матча
    private async Task TryFormMatch(GameType game, int needed)
    {
        using var scope = _scopeFactory.CreateScope();
        var queueRepo = scope.ServiceProvider.GetRequiredService<IRepository<QueueEntry>>();
        var userRepo = scope.ServiceProvider.GetRequiredService<IRepository<User>>();

        var entries = await queueRepo.Query()
            .Include(q => q.User)
            .Where(q => q.GameType == game)
            .OrderBy(q => q.CreatedAt)
            .ToListAsync();

        if (entries.Count == 0) return;

        var waitTime = (DateTime.UtcNow - entries[0].CreatedAt).TotalSeconds;
        if (entries.Count < needed && waitTime >= AutoFillAfterSeconds)
        {
            int window = Math.Min(MaxEloWindow, InitialEloWindow + (int)(waitTime * WindowGrowthPerSecond));
            int anchor = entries[0].EloAtQueue;
            var existingIds = entries.Select(e => e.UserId).ToList();

            var bots = await userRepo.Query()
                .Where(u => !u.IsBanned && u.Role == UserRole.Player && !existingIds.Contains(u.Id))
                .ToListAsync();

            var pool = bots
                .Select(u => new { U = u, Elo = game == GameType.CS2 ? u.EloCS2 : u.EloChess })
                .Where(x => Math.Abs(x.Elo - anchor) <= window)
                .OrderBy(_ => Random.Shared.Next())
                .ToList();

            int slotsNeeded = needed - entries.Count;
            foreach (var p in pool.Take(slotsNeeded))
            {
                entries.Add(new QueueEntry
                {
                    UserId = p.U.Id,
                    User = p.U,
                    EloAtQueue = p.Elo,
                    GameType = game,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        if (entries.Count < needed) return;

        var matched = entries.Take(needed).ToList();

        // Делим на команды «пачками». Участники одного лобби попадают в очередь
        // одним вызовом EnqueuePartyAsync с общим CreatedAt — по нему и определяем пати,
        // чтобы игроки из одной пати ВСЕГДА оказывались в одной команде.
        // Боты и одиночные игроки — каждый сам по себе (своя пачка из одного человека).
        int Elo(User u) => game == GameType.CS2 ? u.EloCS2 : u.EloChess;
        int TeamElo(List<User> team) => team.Sum(Elo);

        // Ключ пачки: реальные участники одной пати делят CreatedAt (общий для вызова
        // EnqueuePartyAsync), поэтому ключ "real:<ticks>". Боты (autofill, Id == 0) и
        // одиночные игроки получают уникальный ключ — каждый сам по себе.
        var groups = matched
            .GroupBy(e => e.Id > 0 ? "real:" + e.CreatedAt.Ticks : "bot:" + e.UserId)
            .Select(g => g.Select(e => e.User).ToList())
            // Сначала размещаем большие пачки — так они гарантированно помещаются целиком.
            .OrderByDescending(g => g.Count)
            .ThenByDescending(g => g.Sum(Elo))
            .ToList();

        int half = needed / 2;
        var t1 = new List<User>();
        var t2 = new List<User>();
        foreach (var group in groups)
        {
            // По умолчанию — в команду, где сейчас меньше игроков
            // (при равенстве — где суммарный elo ниже, для баланса).
            bool toT1 = t1.Count != t2.Count ? t1.Count < t2.Count
                                             : TeamElo(t1) <= TeamElo(t2);
            // Не переполняем команду, если пачка целиком влезает в другую.
            if (toT1 && t1.Count + group.Count > half && t2.Count + group.Count <= half)
                toT1 = false;
            else if (!toT1 && t2.Count + group.Count > half && t1.Count + group.Count <= half)
                toT1 = true;
            (toT1 ? t1 : t2).AddRange(group);
        }

        // Remove from DB queue (only persisted entries)
        foreach (var e in matched.Where(e => e.Id > 0))
            await queueRepo.DeleteAsync(e);
        await queueRepo.SaveChangesAsync();

        // Реальные игроки (не боты) должны принимать вручную.
        // Игроки попадают в БД-очередь через EnqueueAsync или EnqueuePartyAsync — у них Id > 0.
        // Бот-добавления (autofill) создавались выше с Id == 0 и в БД не сохранены.
        var realUserIds = matched.Where(e => e.Id > 0).Select(e => e.UserId).ToHashSet();
        var allIds = matched.Select(e => e.UserId).ToHashSet();
        var botIds = allIds.Where(id => !realUserIds.Contains(id)).ToHashSet();

        // ID должен быть уникален между двумя клиентами, поэтому берём max из БД + 1.
        int pendingId;
        using (var dbScope = _scopeFactory.CreateScope())
        {
            var db = dbScope.ServiceProvider.GetRequiredService<Data.AppDbContext>();
            var maxInDb = await db.MatchAcks.MaxAsync(a => (int?)a.PendingMatchId) ?? 0;
            pendingId = Math.Max(_nextPendingId, maxInDb + 1);
            _nextPendingId = pendingId + 1;
        }

        var pending = new PendingMatch
        {
            Id = pendingId,
            GameType = game,
            Team1 = t1,
            Team2 = t2,
            NeedAccept = new HashSet<int>(allIds), // EVERYONE needs to accept
            RealUserIds = realUserIds,
            BotUserIds = botIds,
            AllUserIds = allIds,
            CreatedAt = DateTime.UtcNow,
            TotalSeconds = ReadyTimeoutSeconds
        };
        _pending[pendingId] = pending;

        try
        {
            await _proposalSync.PublishProposalAsync(new ProposalSnapshot
            {
                PendingMatchId = pendingId,
                GameType = game,
                Team1 = t1.Select(u => u.Id).ToList(),
                Team2 = t2.Select(u => u.Id).ToList(),
                OwnerToken = _proposalSync.MyToken,
                RealUserIds = realUserIds.ToList()
            }, botIds);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Не удалось опубликовать proposal в БД (продолжим локально)");
        }

        if (realUserIds.Count == 0)
        {
            _ = Task.Run(async () =>
            {
                await Task.Delay(2000);
                ConfirmMatch(pending);
            });
            MatchProposed?.Invoke(this, new MatchFoundEventArgs
            {
                GameType = game, Team1 = t1, Team2 = t2, PendingMatchId = pendingId,
                NeedAcceptCount = pending.NeedAccept.Count,
                RealUserIds = realUserIds.ToList()
            });
            return;
        }

        MatchProposed?.Invoke(this, new MatchFoundEventArgs
        {
            GameType = game, Team1 = t1, Team2 = t2, PendingMatchId = pendingId,
            NeedAcceptCount = pending.NeedAccept.Count,
            RealUserIds = realUserIds.ToList()
        });

        _ = SimulateBotAcceptsAsync(pending);
        _ = EnforceReadyTimeoutAsync(pending);
    }
    //Имитирует реалистичное принятие матча ботами
    private async Task SimulateBotAcceptsAsync(PendingMatch p)
    {
        var rng = new Random();

        // Заранее «бросаем кубики» для каждого бота, потом обрабатываем по очереди.
        var botPlan = p.BotUserIds
            .Select(id =>
            {
                double roll = rng.NextDouble();
                BotBehavior beh;
                int delayMs;

                if (roll < BotDeclineChance)
                {
                    beh = BotBehavior.Decline;
                    delayMs = rng.Next(2000, 4500); // отказывается быстро — матч сразу отменяется
                }
                else if (roll < BotDeclineChance + FastAcceptShare)
                {
                    beh = BotBehavior.FastAccept;
                    delayMs = rng.Next(FastWindowMsMin, FastWindowMsMax); // 0.4..3.5s
                }
                else
                {
                    beh = BotBehavior.SlowAccept;
                    delayMs = rng.Next(SlowWindowMsMin, SlowWindowMsMax); // 3.5..5.5s
                }

                return new { BotId = id, Behavior = beh, DelayMs = delayMs };
            })
            .OrderBy(_ => rng.Next())
            .ToList();

        foreach (var bot in botPlan)
        {
            if (p.Cts.IsCancellationRequested) return;

            int wait = bot.DelayMs;

            // Ускоряем оставшихся ботов, если все реальные пользователи уже приняли
            if (p.AllHumansAccepted)
                wait = Math.Min(wait, rng.Next(200, 700));

            // Небольшой джиттер между ботами, чтобы они не активировались синхронно
            wait = Math.Max(wait, rng.Next(BetweenBotsMsMin, BetweenBotsMsMax));

            try { await Task.Delay(wait, p.Cts.Token); }
            catch (OperationCanceledException) { return; }

            if (p.Cts.IsCancellationRequested) return;

            // Бот-отказник: явно отклоняем матч — он сразу отменяется у всех клиентов.
            if (bot.Behavior == BotBehavior.Decline)
            {
                await DeclineMatchAsync(p.Id, bot.BotId);
                return;
            }

            lock (_pending)
            {
                if (!_pending.ContainsKey(p.Id)) return;
                p.NeedAccept.Remove(bot.BotId);
            }
            // Записываем accept бота в БД, чтобы второй клиент тоже видел прогресс
            try { await _proposalSync.AcceptAsync(p.Id, bot.BotId); } catch { }
            RaiseAcceptProgress(p);
            CheckAndConfirm(p);
        }
    }

    private enum BotBehavior { Decline, FastAccept, SlowAccept }

    private async Task EnforceReadyTimeoutAsync(PendingMatch p)
    {
        // Через 5 секунд ускоряем оставшихся ботов, чтобы матч не висел из-за них.
        try { await Task.Delay(TimeSpan.FromSeconds(5), p.Cts.Token); }
        catch (OperationCanceledException) { return; }

        lock (_pending) { if (!_pending.ContainsKey(p.Id)) return; }
        p.AllHumansAccepted = true; // флаг ускоряет симуляцию принятия ботами

        // Дожидаемся конца окна подтверждения.
        try { await Task.Delay(TimeSpan.FromSeconds(p.TotalSeconds - 5), p.Cts.Token); }
        catch (OperationCanceledException) { return; }

        lock (_pending) { if (!_pending.ContainsKey(p.Id)) return; }

        // Окно истекло, приняли не все — отменяем матч.
        await CancelMatchAsync(p, acceptedReturnToQueue: false);
    }

    private void RaiseAcceptProgress(PendingMatch p)
    {
        int total = p.AllUserIds.Count;
        int accepted = total - p.NeedAccept.Count;
        AcceptProgress?.Invoke(this, new AcceptProgressEventArgs
        {
            PendingMatchId = p.Id,
            Accepted = accepted,
            Total = total
        });
    }

    private void CheckAndConfirm(PendingMatch p)
    {
        bool allIn;
        lock (_pending)
        {
            if (!_pending.ContainsKey(p.Id)) return;
            allIn = p.NeedAccept.Count == 0;
        }
        if (allIn) ConfirmMatch(p);
    }

    private void ConfirmMatch(PendingMatch p)
    {
        lock (_pending)
        {
            if (!_pending.Remove(p.Id)) return;
        }
        try { p.Cts.Cancel(); } catch { }
        MatchFound?.Invoke(this, new MatchFoundEventArgs
        {
            GameType = p.GameType,
            Team1 = p.Team1,
            Team2 = p.Team2,
            PendingMatchId = p.Id,
            RealUserIds = p.RealUserIds.ToList()
        });
        // CS2 с несколькими людьми
        bool deferAckCleanup = p.GameType == GameType.CS2 && p.RealUserIds.Count > 1;
        if (!deferAckCleanup)
            ScheduleAckCleanup(p.Id);
    }

    /// <summary>Отмена ожидающего матча. Все игроки (включая принявших) убираются из очереди
    /// — пользователю нужно снова нажать «Найти матч», чтобы начать поиск заново.</summary>
    private async Task CancelMatchAsync(PendingMatch p, bool acceptedReturnToQueue)
    {
        lock (_pending) { _pending.Remove(p.Id); }
        try { p.Cts.Cancel(); } catch { }

        // Удаляем ВСЕХ участников отменённого матча из очереди. Никаких авто-возвратов.
        await ReQueueMembersExceptAsync(p, new HashSet<int>(p.AllUserIds));
        MatchCancelled?.Invoke(this, p.Id);
        // Чистим ack-записи с задержкой — чтобы остальные клиенты успели увидеть отмену.
        ScheduleAckCleanup(p.Id);
    }

    public event EventHandler<MatchFoundEventArgs>? MatchProposed;
}
