using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flux.Data;
using Flux.Infrastructure;
using Flux.Localization;
using Flux.Models;
using Flux.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.ViewModels;

public partial class LobbySlotVm : ObservableObject
{
    [ObservableProperty] private bool _isEmpty = true;
    [ObservableProperty] private bool _isLeader;
    [ObservableProperty] private int _userId;
    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private int _elo;
    [ObservableProperty] private int _level;
    [ObservableProperty] private string _initials = string.Empty;
    [ObservableProperty] private byte[]? _avatarBytes;
}

public partial class FriendInviteEntry : ObservableObject
{
    [ObservableProperty] private int _userId;
    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private int _eloCS2;
    [ObservableProperty] private bool _isOnline;
    [ObservableProperty] private bool _isInParty;
}

public partial class LevelRow : ObservableObject
{
    public int Level { get; set; }
    public string Range { get; set; } = string.Empty;
    public bool IsCurrent { get; set; }
}

public partial class QueueViewModel : ViewModelBase, INavigationPage
{
    private readonly IMatchmakingService _mm;
    private readonly ISessionService _session;
    private readonly IDialogService _dialog;
    private readonly IServiceProvider _sp;
    private readonly INavigationService _nav;
    private readonly IPartyService _partyService;
    private readonly IFriendsService _friends;
    private readonly IInvitationService _invitations;
    private readonly IMatchProposalSync _proposalSync;

    private int? _pendingMatchId;
    private MatchFoundEventArgs? _proposal;
    private DispatcherTimer? _readyTimer;
    private bool _subscribed;

    private const int ReadyDurationSeconds = 20;

    [ObservableProperty] private GameType _selectedGame = GameType.CS2;
    [ObservableProperty] private string _selectedGameLabel = "CS2";
    [ObservableProperty] private bool _isInQueue;
    [ObservableProperty] private string _statusText = string.Empty;
    [ObservableProperty] private int _queueSizeCS2;
    [ObservableProperty] private int _queueSizeChess;

    partial void OnSelectedGameChanged(GameType value)
    {
        SelectedGameLabel = value == GameType.CS2
            ? LanguageService.Instance["CS2"]
            : LanguageService.Instance["Chess"];
    }

    // Accept-popup state
    [ObservableProperty] private bool _acceptPopupVisible;
    [ObservableProperty] private bool _userAcceptedMatch;
    [ObservableProperty] private int _readyCountdown;
    [ObservableProperty] private int _totalToAccept;
    [ObservableProperty] private int _acceptedCount;

    // Friends flyout
    [ObservableProperty] private bool _friendsFlyoutVisible;

    // Current Elo + level
    [ObservableProperty] private int _currentElo;
    [ObservableProperty] private int _currentLevel;
    [ObservableProperty] private int _eloToNext;
    [ObservableProperty] private int _levelMin;
    [ObservableProperty] private int _levelMax;

    // Level tooltip
    [ObservableProperty] private bool _levelTooltipVisible;
    public ObservableCollection<LevelRow> LevelRows { get; } = new();

    // Match-readiness
    [ObservableProperty] private bool _canFindMatch;
    [ObservableProperty] private string _findMatchLabel = string.Empty;
    [ObservableProperty] private bool _isLeader = true;

    public ObservableCollection<LobbySlotVm> Slots { get; } = new();
    public ObservableCollection<FriendInviteEntry> Friends { get; } = new();

    public ICommand SelectCS2Command { get; }
    public ICommand SelectChessCommand { get; }
    public ICommand FindMatchCommand { get; }
    public ICommand LeaveQueueCommand { get; }
    public ICommand AcceptMatchCommand { get; }
    public ICommand ToggleFriendsCommand { get; }
    public ICommand InviteFriendCommand { get; }
    public ICommand KickFromSlotCommand { get; }
    public ICommand ToggleLevelTooltipCommand { get; }

    public QueueViewModel(IMatchmakingService mm, ISessionService session, IDialogService dialog,
        IServiceProvider sp, INavigationService nav, IPartyService partyService, IFriendsService friends,
        IInvitationService invitations, IMatchProposalSync proposalSync)
    {
        _mm = mm; _session = session; _dialog = dialog; _sp = sp; _nav = nav;
        _partyService = partyService; _friends = friends; _invitations = invitations;
        _proposalSync = proposalSync;

        SelectCS2Command   = new RelayCommand(() => ChangeGame(GameType.CS2), () => !IsInQueue);
        SelectChessCommand = new RelayCommand(() => ChangeGame(GameType.Chess), () => !IsInQueue);
        FindMatchCommand   = new AsyncRelayCommand(FindMatchAsync, () => !IsInQueue && CanFindMatch);
        LeaveQueueCommand  = new AsyncRelayCommand(LeaveAsync, () => IsInQueue);
        AcceptMatchCommand = new AsyncRelayCommand(OnAcceptAsync, () => !UserAcceptedMatch);
        ToggleFriendsCommand = new AsyncRelayCommand(ToggleFriendsAsync);
        InviteFriendCommand = new RelayCommand<FriendInviteEntry>(OnInviteFriend);
        KickFromSlotCommand = new RelayCommand<LobbySlotVm>(OnKickSlot);
        ToggleLevelTooltipCommand = new RelayCommand(() => LevelTooltipVisible = !LevelTooltipVisible);

        PopulateLevelRows();

        // Обновляем строки, зависящие от выбранного языка (например «Найти матч (3/5)»)
        LanguageService.Instance.LanguageChanged += (_, _) =>
        {
            UpdateFindMatchState();
            PopulateLevelRows();
            // Подсказать обновить SelectedGameLabel: триггерим setter заново
            SelectedGameLabel = SelectedGame == GameType.CS2
                ? LanguageService.Instance["CS2"]
                : LanguageService.Instance["Chess"];
        };

        // Инициализация лейбла на старте
        SelectedGameLabel = LanguageService.Instance["CS2"];

        // Когда партия меняется извне (например, MainVM добавил игрока после accept приглашения,
        // или follower вышел из лобби), перерисовываем слоты и пересчитываем состояние кнопки.
        // Если состав уменьшился во время очереди — выходим из очереди (отменяем поиск).
        int lastCount = _partyService.Party.Count;
        _partyService.Party.Changed += async (_, _) =>
        {
            var newCount = _partyService.Party.Count;
            bool shrank = newCount < lastCount;
            lastCount = newCount;

            // Если состав уменьшился И мы сейчас в очереди — отменяем поиск.
            if (shrank && IsInQueue)
            {
                try { await LeaveAsync(); } catch { /* лучшее усилие */ }
            }

            Application.Current?.Dispatcher.Invoke(() =>
            {
                RebuildSlots();
                UpdateFindMatchState();
            });
        };
    }

    public void OnNavigatedFrom() { }

    public void OnNavigatedTo() => OnActivate();

    public async void OnActivate()
    {
        // Subscribe once — avoid duplicate subscriptions if OnActivate fires multiple times.
        if (!_subscribed)
        {
            _mm.MatchProposed   += OnMatchProposed;
            _mm.MatchFound      += OnMatchFound;
            _mm.MatchCancelled  += OnMatchCancelled;
            _mm.QueueUpdated    += OnQueueUpdated;
            _mm.AcceptProgress  += OnAcceptProgress;
            // Подписки на синхронизированные через БД события матча (для второго клиента)
            _proposalSync.IncomingProposal  += OnIncomingProposal;
            _proposalSync.ProgressChanged   += OnProposalProgress;
            _proposalSync.ProposalConfirmed += OnProposalConfirmed;
            _proposalSync.ProposalCancelled += OnProposalCancelled;
            _subscribed = true;
        }

        var me = _session.CurrentUser!;
        await _partyService.EnsureLeaderAsync(me);
        RebuildSlots();
        UpdateEloAndLevel();
        UpdateFindMatchState();

        QueueSizeCS2 = await _mm.GetQueueSizeAsync(GameType.CS2);
        QueueSizeChess = await _mm.GetQueueSizeAsync(GameType.Chess);
    }

    private void OnQueueUpdated(object? sender, EventArgs e)
    {
        Application.Current?.Dispatcher.BeginInvoke(new Action(async () =>
        {
            QueueSizeCS2 = await _mm.GetQueueSizeAsync(GameType.CS2);
            QueueSizeChess = await _mm.GetQueueSizeAsync(GameType.Chess);
        }));
    }

    private void ChangeGame(GameType g)
    {
        SelectedGame = g;
        // Не трогаем состав партии при переключении игры — лобби CS2 сохраняется.
        // В шахматах партия игнорируется (поиск идёт через EnqueueAsync(me)), но запись в БД
        // не удаляется, чтобы при возврате к CS2 пользователь снова увидел своих приглашённых.
        RebuildSlots();
        UpdateEloAndLevel();
        UpdateFindMatchState();
    }

    // ---------- slots ----------

    private void RebuildSlots()
    {
        Slots.Clear();
        int totalSlots = SelectedGame == GameType.CS2 ? 5 : 1;
        var party = _partyService.Party;

        for (int i = 0; i < totalSlots; i++)
        {
            if (i < party.Count)
            {
                var u = party.Members[i];
                int elo = SelectedGame == GameType.CS2 ? u.EloCS2 : u.EloChess;
                Slots.Add(new LobbySlotVm
                {
                    IsEmpty = false,
                    IsLeader = i == 0,
                    UserId = u.Id,
                    Nickname = u.Nickname,
                    Elo = elo,
                    Level = EloLevels.GetInfo(elo).Level,
                    Initials = u.Nickname.Length > 0 ? u.Nickname.Substring(0, 1).ToUpper() : "?",
                    AvatarBytes = u.AvatarBytes
                });
            }
            else
            {
                Slots.Add(new LobbySlotVm { IsEmpty = true });
            }
        }
    }

    private void UpdateEloAndLevel()
    {
        var me = _session.CurrentUser!;
        CurrentElo = SelectedGame == GameType.CS2 ? me.EloCS2 : me.EloChess;
        var info = EloLevels.GetInfo(CurrentElo);
        CurrentLevel = info.Level;
        LevelMin = info.LevelMin;
        LevelMax = info.LevelMax;
        EloToNext = info.Level == 10 ? 0 : System.Math.Max(0, info.NextLevelAt - CurrentElo);

        // refresh the "current" flag in the tooltip table
        foreach (var r in LevelRows) r.IsCurrent = r.Level == info.Level;
    }

    private void PopulateLevelRows()
    {
        LevelRows.Clear();
        foreach (var (lvl, min, max) in EloLevels.All)
        {
            string range = max == int.MaxValue ? $"{min}+" : $"{min} – {max}";
            LevelRows.Add(new LevelRow { Level = lvl, Range = range });
        }
    }

    private void UpdateFindMatchState()
    {
        int membersCount = _partyService.Party.Members.Count;
        // Если партия пуста (поллер ещё не зачитал DB или я только что залогинился) —
        // считаем что я сам в лобби, и я же лидер. Это позволяет нажать «Найти матч»
        // сразу после входа, не дожидаясь поллера.
        int have = Math.Max(membersCount, 1);
        int target = SelectedGame == GameType.CS2 ? 5 : 1;
        // Лидер: пустая партия (только я неявно) ИЛИ я первый в составе
        bool isLeader = membersCount == 0
                        || (_session.CurrentUser != null
                            && _partyService.Party.Members[0].Id == _session.CurrentUser.Id);
        IsLeader = isLeader;
        CanFindMatch = isLeader;
        FindMatchLabel = SelectedGame == GameType.CS2
            ? (isLeader
                ? $"{LanguageService.Instance["FindMatch"]}  ({have}/{target})"
                : LanguageService.Instance["WaitingForLeader"])
            : LanguageService.Instance["FindMatch"];
        ((AsyncRelayCommand)FindMatchCommand).NotifyCanExecuteChanged();
    }

    // ---------- friends ----------

    private async Task ToggleFriendsAsync()
    {
        if (!FriendsFlyoutVisible)
            await ReloadFriendsAsync();
        FriendsFlyoutVisible = !FriendsFlyoutVisible;
    }

    private async Task ReloadFriendsAsync()
    {
        Friends.Clear();
        var me = _session.CurrentUser!;
        var list = await _friends.GetFriendsAsync(me.Id);
        var partyIds = _partyService.Party.Members.Select(x => x.Id).ToHashSet();
        foreach (var u in list)
        {
            Friends.Add(new FriendInviteEntry
            {
                UserId = u.Id,
                Nickname = u.Nickname,
                EloCS2 = u.EloCS2,
                IsOnline = _friends.IsOnline(u.Id),
                IsInParty = partyIds.Contains(u.Id)
            });
        }
    }

    private async void OnInviteFriend(FriendInviteEntry? f)
    {
        if (f == null) return;
        if (SelectedGame != GameType.CS2) return;
        if (f.IsInParty) return;
        if (_session.CurrentUser == null) return;

        // Поведение: не добавляем игрока в лобби сразу. Отправляем приглашение через
        // InvitationService (он пишет запись в таблицу LobbyInvitations). Второй клиент,
        // запущенный под этим пользователем, увидит её через свой поллинг и покажет
        // тост в MainView. Когда получатель нажмёт "Принять", статус станет Accepted —
        // наш собственный поллинг (тоже в InvitationService у нас как у отправителя)
        // поднимет событие InvitationAccepted, MainViewModel добавит его в Party,
        // а лобби автоматически перерисуется через подписку RebuildSlots на Party.Changed.
        try
        {
            await _invitations.SendAsync(_session.CurrentUser.Id, f.UserId, GameType.CS2);
            // Маркируем "приглашён" чтобы кнопка "+" не нажималась повторно
            f.IsInParty = true;
        }
        catch
        {
            // лучшее усилие
        }
    }

    private async void OnKickSlot(LobbySlotVm? slot)
    {
        if (slot == null || slot.IsEmpty || slot.IsLeader) return;
        await _partyService.RemoveAsync(slot.UserId);
        var f = Friends.FirstOrDefault(x => x.UserId == slot.UserId);
        if (f != null) f.IsInParty = false;
        // RebuildSlots / UpdateFindMatchState вызовутся через Party.Changed-подписку
    }

    // ---------- matchmaking ----------

    private async Task FindMatchAsync()
    {
        var me = _session.CurrentUser!;
        if (me.IsBanned)
        {
            _dialog.ShowError(LanguageService.Instance["UserBanned"]);
            return;
        }

        bool ok;
        if (SelectedGame == GameType.CS2 && _partyService.Party.Count > 1)
            ok = await _mm.EnqueuePartyAsync(_partyService.Party.Members, SelectedGame);
        else
            ok = await _mm.EnqueueAsync(me, SelectedGame);
        if (!ok) return;

        IsInQueue = true;
        StatusText = LanguageService.Instance["SearchingOpponents"];
        ((AsyncRelayCommand)FindMatchCommand).NotifyCanExecuteChanged();
        ((AsyncRelayCommand)LeaveQueueCommand).NotifyCanExecuteChanged();
    }

    private async Task LeaveAsync()
    {
        var me = _session.CurrentUser!;
        if (SelectedGame == GameType.CS2 && _partyService.Party.Count > 1)
            await _mm.DequeueManyAsync(_partyService.Party.Members.Select(u => u.Id));
        else
            await _mm.DequeueAsync(me.Id);

        IsInQueue = false;
        StatusText = string.Empty;
        ((AsyncRelayCommand)FindMatchCommand).NotifyCanExecuteChanged();
        ((AsyncRelayCommand)LeaveQueueCommand).NotifyCanExecuteChanged();
    }

    // ---------- Ready check (timer + accept) ----------

    private void OnMatchProposed(object? sender, MatchFoundEventArgs e)
    {
        var me = _session.CurrentUser!;
        bool iAmIn = e.Team1.Any(u => u.Id == me.Id) || e.Team2.Any(u => u.Id == me.Id);
        if (!iAmIn) return;

        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            _pendingMatchId = e.PendingMatchId;
            _proposal = e;
            UserAcceptedMatch = false;
            ReadyCountdown = ReadyDurationSeconds;
            TotalToAccept = e.TotalPlayers;
            // Everyone (including bots) starts as NOT accepted; they fill in over time via AcceptProgress.
            AcceptedCount = 0;
            AcceptPopupVisible = true;
            StatusText = LanguageService.Instance["MatchFound"];
            ((AsyncRelayCommand)AcceptMatchCommand).NotifyCanExecuteChanged();

            StartReadyTimer();
        }));
    }

    /// <summary>Создаёт и запускает заново таймер обратного отсчёта готовности.
    /// Всегда пересоздаёт таймер, чтобы исключить дублирование подписок на Tick.</summary>
    private void StartReadyTimer()
    {
        StopReadyTimer();
        _readyTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _readyTimer.Tick += ReadyTimerTick;
        _readyTimer.Start();
    }

    /// <summary>Останавливает и полностью освобождает таймер обратного отсчёта.</summary>
    private void StopReadyTimer()
    {
        if (_readyTimer == null) return;
        _readyTimer.Stop();
        _readyTimer.Tick -= ReadyTimerTick;
        _readyTimer = null;
    }

    private void OnAcceptProgress(object? sender, AcceptProgressEventArgs e)
    {
        if (_pendingMatchId != e.PendingMatchId) return;
        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            AcceptedCount = e.Accepted;
            TotalToAccept = e.Total;
        }));
    }

    private async void ReadyTimerTick(object? sender, EventArgs e)
    {
        if (ReadyCountdown > 0) ReadyCountdown--;
        if (ReadyCountdown > 0) return;

        _readyTimer?.Stop();
        // Окно подтверждения истекло. Если пользователь не принял — отклоняем матч
        // (это отменит его у всех клиентов). Если принял — ждём терминального
        // события сервиса/БД-синхронизации, которое закроет окно.
        if (_pendingMatchId.HasValue && !UserAcceptedMatch)
        {
            var id = _pendingMatchId.Value;
            try { await _mm.DeclineMatchAsync(id, _session.CurrentUser!.Id); }
            catch { /* лучшее усилие */ }
        }
    }

    private async Task OnAcceptAsync()
    {
        if (_pendingMatchId == null) return;
        UserAcceptedMatch = true;
        ((AsyncRelayCommand)AcceptMatchCommand).NotifyCanExecuteChanged();
        await _mm.AcceptMatchAsync(_pendingMatchId.Value, _session.CurrentUser!.Id);
    }

    private void OnMatchCancelled(object? sender, int pendingId)
    {
        if (_pendingMatchId != pendingId) return;
        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            // Полный сброс состояния лобби при срыве матча — без зависшего окна и счётчика.
            StopReadyTimer();
            _pendingMatchId = null;
            _proposal = null;
            AcceptPopupVisible = false;
            UserAcceptedMatch = false;
            AcceptedCount = 0;
            TotalToAccept = 0;
            // При отмене матча сервис убирает из очереди ВСЕХ участников
            // (и принявших, и нет) — поэтому всегда выходим из состояния очереди.
            IsInQueue = false;
            StatusText = string.Empty;
            ((AsyncRelayCommand)FindMatchCommand).NotifyCanExecuteChanged();
            ((AsyncRelayCommand)LeaveQueueCommand).NotifyCanExecuteChanged();
        }));
    }

    private void OnMatchFound(object? sender, MatchFoundEventArgs e)
    {
        var me = _session.CurrentUser!;
        bool iAmIn = e.Team1.Any(u => u.Id == me.Id) || e.Team2.Any(u => u.Id == me.Id);
        if (!iAmIn) return;

        Application.Current.Dispatcher.BeginInvoke(new Action(async () =>
        {
            StopReadyTimer();
            _pendingMatchId = null;
            _proposal = null;
            AcceptPopupVisible = false;
            UserAcceptedMatch = false;
            IsInQueue = false;
            StatusText = string.Empty;
            ((AsyncRelayCommand)FindMatchCommand).NotifyCanExecuteChanged();
            ((AsyncRelayCommand)LeaveQueueCommand).NotifyCanExecuteChanged();

            if (e.GameType == GameType.CS2)
            {
                var vm = _sp.GetRequiredService<CS2MatchViewModel>();
                await vm.InitializeAsync(e.Team1, e.Team2, e.PendingMatchId, e.RealUserIds);
                _nav.NavigateRoot(vm);
            }
            else
            {
                var vm = _sp.GetRequiredService<ChessBoardViewModel>();
                vm.Initialize(e.Team1[0], e.Team2[0]);
                _nav.NavigateRoot(vm);
            }
        }));
    }

    // ---------- Cross-client proposal sync ----------

    /// <summary>
    /// Срабатывает у второго клиента (не лидера), когда первый клиент сформировал матч.
    /// Показываем тот же match-found UI что и у лидера.
    /// </summary>
    private void OnIncomingProposal(object? sender, IncomingProposalEventArgs e)
    {
        // Если это мой собственный proposal (я же его сформировал) — игнорируем DB-event,
        // т.к. локальный _mm.MatchProposed уже показал у меня popup.
        if (e.OwnerToken == _proposalSync.MyToken) return;

        // Не дублируем popup: если у нас уже идёт этот pending — выходим.
        if (_pendingMatchId == e.PendingMatchId && AcceptPopupVisible) return;
        // Если я не в этом матче — игнорируем (на всякий случай)
        var me = _session.CurrentUser;
        if (me == null) return;
        bool iAmIn = e.Team1.Any(u => u.Id == me.Id) || e.Team2.Any(u => u.Id == me.Id);
        if (!iAmIn) return;

        Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
        {
            _pendingMatchId = e.PendingMatchId;
            _proposal = new MatchFoundEventArgs
            {
                GameType = e.GameType, Team1 = e.Team1, Team2 = e.Team2,
                PendingMatchId = e.PendingMatchId,
                RealUserIds = e.RealUserIds?.ToList() ?? new List<int>()
            };
            UserAcceptedMatch = false;
            ReadyCountdown = ReadyDurationSeconds;
            TotalToAccept = e.Team1.Count + e.Team2.Count;
            AcceptedCount = 0;
            AcceptPopupVisible = true;
            StatusText = LanguageService.Instance["MatchFound"];
            ((AsyncRelayCommand)AcceptMatchCommand).NotifyCanExecuteChanged();

            StartReadyTimer();
        }));
    }

    private void OnProposalProgress(object? sender, ProposalProgressEventArgs e)
    {
        if (_pendingMatchId != e.PendingMatchId) return;
        Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
        {
            AcceptedCount = e.Accepted;
            TotalToAccept = e.Total;
        }));
    }

    /// <summary>Все приняли — стартуем матч (второй клиент тоже получит это через БД).</summary>
    private void OnProposalConfirmed(object? sender, IncomingProposalEventArgs e)
    {
        // Дублируем поведение OnMatchFound, но только если локальный _mm.MatchFound не сработал
        // (т.е. это второй клиент). Сравниваем по PendingMatchId — если у нас локальный pending
        // уже сброшен в null, значит подтверждение уже обработали.
        if (_pendingMatchId != e.PendingMatchId) return;
        var args = new MatchFoundEventArgs
        {
            GameType = e.GameType, Team1 = e.Team1, Team2 = e.Team2,
            PendingMatchId = e.PendingMatchId,
            RealUserIds = e.RealUserIds?.ToList() ?? new List<int>()
        };
        OnMatchFound(this, args);
    }

    private void OnProposalCancelled(object? sender, int pendingId)
    {
        if (_pendingMatchId != pendingId) return;
        OnMatchCancelled(this, pendingId);
    }
}
