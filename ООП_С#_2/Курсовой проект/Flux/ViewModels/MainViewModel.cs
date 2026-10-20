using System;
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

public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _nav;
    private readonly ISessionService _session;
    private readonly ISessionPersistence _persistence;
    private readonly IFriendsService _friendsService;
    private readonly IInvitationService _invitations;
    private readonly IPartyService _party;
    private readonly IMatchProposalSync _proposalSync;
    private readonly IDialogService _dialog;
    private readonly IServiceProvider _sp;

    /// <summary>Таймер периодической проверки блокировки текущего аккаунта.</summary>
    private DispatcherTimer? _banWatchTimer;
    /// <summary>Защита от повторного завершения сессии (logout / принудительный бан).</summary>
    private bool _sessionEnded;

    [ObservableProperty] private ViewModelBase? _currentPage;

    // -- Invitation toast (входящее приглашение в лобби) ---------------------
    [ObservableProperty] private bool _invitationVisible;
    [ObservableProperty] private string _invitationFromNickname = string.Empty;
    [ObservableProperty] private string _invitationGameLabel = string.Empty;
    private int _pendingInvitationId;

    public string Nickname => _session.CurrentUser?.Nickname ?? "";
    public bool IsAdmin => _session.IsAdmin;

    public ICommand NavigateQueueCommand { get; }
    public ICommand NavigateProfileCommand { get; }
    public ICommand NavigateSearchCommand { get; }
    public ICommand NavigateAdminCommand { get; }
    public ICommand NavigateFriendsCommand { get; }
    public ICommand NavigateLeaderboardCommand { get; }
    public ICommand NavigateTournamentsCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand ToggleLanguageCommand { get; }
    public ICommand AcceptInvitationCommand { get; }
    public ICommand DeclineInvitationCommand { get; }

    public MainViewModel(INavigationService nav, ISessionService session, ISessionPersistence persistence,
        IFriendsService friendsService, IInvitationService invitations, IPartyService party,
        IMatchProposalSync proposalSync, IDialogService dialog,
        IServiceProvider sp,
        QueueViewModel queueVm, ProfileViewModel profileVm,
        PlayerSearchViewModel searchVm, AdminViewModel adminVm, FriendsViewModel friendsVm,
        LeaderboardViewModel leaderboardVm, TournamentsViewModel tournamentsVm)
    {
        _nav = nav; _session = session; _persistence = persistence; _friendsService = friendsService;
        _invitations = invitations; _party = party; _proposalSync = proposalSync; _dialog = dialog; _sp = sp;

        // Текущий пользователь онлайн на время сессии
        if (_session.CurrentUser != null)
            _friendsService.Online(_session.CurrentUser.Id);

        // При смене вкладки — OnNavigatedFrom у текущей, затем смена страницы.
        Action<ViewModelBase> goTo = vm =>
        {
            if (CurrentPage is INavigationPage oldNav) oldNav.OnNavigatedFrom();
            CurrentPage = vm;
        };

        NavigateQueueCommand   = new RelayCommand(() => { goTo(queueVm);   (queueVm as INavigationPage)?.OnNavigatedTo(); });
        NavigateProfileCommand = new RelayCommand(() => { goTo(profileVm); _ = profileVm.LoadAsync(); });
        NavigateSearchCommand  = new RelayCommand(() => { goTo(searchVm); });
        NavigateFriendsCommand = new RelayCommand(() => { goTo(friendsVm); _ = friendsVm.LoadAsync(); });
        NavigateLeaderboardCommand = new RelayCommand(() => { goTo(leaderboardVm); _ = leaderboardVm.LoadAsync(); });
        NavigateTournamentsCommand = new RelayCommand(() =>
        {
            goTo(tournamentsVm);
            (tournamentsVm as INavigationPage)?.OnNavigatedTo();
        });
        NavigateAdminCommand   = new RelayCommand(() => { goTo(adminVm);  _ = adminVm.LoadAsync(); }, () => _session.IsAdmin);

        AcceptInvitationCommand  = new AsyncRelayCommand(AcceptInvitationAsync);
        DeclineInvitationCommand = new AsyncRelayCommand(DeclineInvitationAsync);

        LogoutCommand = new RelayCommand(async () => await PerformLogoutAsync());
        ToggleLanguageCommand = new RelayCommand(() => LanguageService.Instance.ToggleLanguage());

        // Подписки на службу приглашений (приходят из фонового потока — диспатчим в UI)
        _invitations.InvitationReceived += OnInvitationReceived;
        _invitations.InvitationAccepted += OnInvitationAccepted;

        if (_session.CurrentUser != null)
        {
            _invitations.Start(_session.CurrentUser.Id);
            _party.Start(_session.CurrentUser.Id);
            _proposalSync.Start(_session.CurrentUser.Id);

            // Слежение за блокировкой: если администратор забанит пользователя
            // во время сессии, он будет принудительно разлогинен в течение
            // нескольких секунд — не дожидаясь перезахода в аккаунт.
            _banWatchTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
            _banWatchTimer.Tick += async (_, _) => await CheckBanAsync();
            _banWatchTimer.Start();
        }

        CurrentPage = queueVm;
        (queueVm as INavigationPage)?.OnNavigatedTo();
    }

    private void OnInvitationReceived(object? sender, InvitationReceivedEventArgs e)
    {
        Application.Current?.Dispatcher.Invoke(() =>
        {
            // Если уже что-то висит — не перебиваем (новые игнорируем до ответа на текущее).
            if (InvitationVisible) return;

            _pendingInvitationId = e.InvitationId;
            InvitationFromNickname = e.FromNickname;
            InvitationGameLabel = e.GameType == GameType.CS2
                ? LanguageService.Instance["CS2"]
                : LanguageService.Instance["Chess"];
            InvitationVisible = true;
        });
    }

    private void OnInvitationAccepted(object? sender, InvitationAcceptedEventArgs e)
    {
    }

    private async Task AcceptInvitationAsync()
    {
        if (_pendingInvitationId == 0) return;
        var inviteId = _pendingInvitationId;
        _pendingInvitationId = 0;
        InvitationVisible = false;

        if (await IsCurrentUserBannedAsync())
        {
            try { await _invitations.DeclineAsync(inviteId); } catch { }
            _dialog.ShowError(LanguageService.Instance["InvitationBanned"]);
            return;
        }

        await _invitations.AcceptAsync(inviteId);

        try
        {
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var inv = await db.LobbyInvitations
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == inviteId);
            if (inv != null && _session.CurrentUser != null)
            {
                await _party.JoinAsync(inv.FromUserId, _session.CurrentUser);
            }
        }
        catch {  }
    }

    private async Task PerformLogoutAsync()
    {
        if (_sessionEnded) return;
        _sessionEnded = true;

        _banWatchTimer?.Stop();
        _invitations.Stop();
        _proposalSync.Stop();
        if (_session.CurrentUser != null)
        {
            try
            {
                var myId = _session.CurrentUser.Id;
                if (_party.CurrentLeaderId == myId)
                    await _party.DisbandAsync();
                else
                    await _party.RemoveAsync(myId);
            }
            catch {  }
            _friendsService.Offline(_session.CurrentUser.Id);
        }
        _party.Stop();
        _session.Clear();
        _persistence.Clear();
        _nav.NavigateRoot<LoginViewModel>();
    }

    /// <summary>Периодическая проверка блокировки аккаунта. При обнаружении бана —
    /// сообщение пользователю и немедленный принудительный выход из аккаунта.</summary>
    private async Task CheckBanAsync()
    {
        if (_sessionEnded || _session.CurrentUser == null) return;
        if (!await IsCurrentUserBannedAsync()) return;

        _banWatchTimer?.Stop();
        _dialog.ShowError(LanguageService.Instance["UserBanned"]);
        await PerformLogoutAsync();
    }

    /// <summary>Проверяет, заблокирован ли текущий пользователь (актуальный статус из БД).</summary>
    private async Task<bool> IsCurrentUserBannedAsync()
    {
        if (_session.CurrentUser == null) return false;
        try
        {
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var u = await db.Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == _session.CurrentUser.Id);
            if (u != null)
            {
                _session.CurrentUser.IsBanned = u.IsBanned; // синхронизируем in-memory сессию
                return u.IsBanned;
            }
        }
        catch { /* лучшее усилие — при ошибке не блокируем */ }
        return false;
    }

    private async Task DeclineInvitationAsync()
    {
        if (_pendingInvitationId == 0) return;
        var inviteId = _pendingInvitationId;
        _pendingInvitationId = 0;
        InvitationVisible = false;
        await _invitations.DeclineAsync(inviteId);
    }
}
