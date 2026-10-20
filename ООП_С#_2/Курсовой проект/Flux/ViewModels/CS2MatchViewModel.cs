using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flux.Data;
using Flux.DTOs;
using Flux.Infrastructure;
using Flux.Localization;
using Flux.Models;
using Flux.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.ViewModels;

public enum CS2Phase { ServerPick, MapBan, ResultScreen, CaptainSyncWait }

public partial class CS2PlayerRow : ObservableObject
{
    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private int _elo;
    [ObservableProperty] private int _kills;
    [ObservableProperty] private int _deaths;
    [ObservableProperty] private int _assists;
    [ObservableProperty] private int _headshots;
    [ObservableProperty] private double _adr;
    [ObservableProperty] private double _hltv;
    [ObservableProperty] private int _eloChange;
    [ObservableProperty] private bool _isSelf;
    [ObservableProperty] private int _team;
}

public partial class MapBanItem : ObservableObject
{
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private bool _isBanned;
    [ObservableProperty] private int _bannedByTeam;  // 0 = not banned, 1 or 2 otherwise
}

public partial class CS2MatchViewModel : ViewModelBase
{
    private readonly ICS2GameService _cs2;
    private readonly ISessionService _session;
    private readonly INavigationService _nav;
    private readonly IServiceProvider _sp;
    private readonly IMatchProposalSync _proposalSync;
    private readonly IPartyService _party;

    private List<User> _team1 = new();
    private List<User> _team2 = new();
    private int _userTeam;
    private int _currentBanningTeam;
    private int _serverPickerTeam;
    private int _mapBanFirstTeam;

    private int _pendingMatchId;
    private int _captainId;
    private int _captainTeam;
    /// <summary>Несколько реальных людей в матче — один капитан симулирует, остальные ждут БД.</summary>
    private bool _multiHuman;
    /// <summary>Этот клиент выполняет пик и SimulateAndSaveAsync.</summary>
    private bool _isCaptainAuthority;
    private DispatcherTimer? _pollTimer;

    private bool IsCaptain => _session.CurrentUser != null && _session.CurrentUser.Id == _captainId;

    [ObservableProperty] private CS2Phase _phase = CS2Phase.ServerPick;
    [ObservableProperty] private string _phaseTitle = string.Empty;
    [ObservableProperty] private string _phaseSubtitle = string.Empty;
    [ObservableProperty] private bool _canActNow;
    [ObservableProperty] private string? _selectedServer;
    [ObservableProperty] private string? _finalMap;
    [ObservableProperty] private int _mapsRemaining;

    public ObservableCollection<string> Servers { get; }
    public ObservableCollection<MapBanItem> Maps { get; } = new();

    public ObservableCollection<CS2PlayerRow> Team1Rows { get; } = new();
    public ObservableCollection<CS2PlayerRow> Team2Rows { get; } = new();

    [ObservableProperty] private int _scoreTeam1;
    [ObservableProperty] private int _scoreTeam2;
    [ObservableProperty] private string _resultText = string.Empty;
    [ObservableProperty] private bool _isVictory;
    [ObservableProperty] private string _mapDisplay = string.Empty;
    [ObservableProperty] private string _serverDisplay = string.Empty;

    public ICommand PickServerCommand { get; }
    public ICommand BanMapCommand { get; }
    public ICommand ReturnCommand { get; }

    [ObservableProperty] private string _team1LeaderName = string.Empty;
    [ObservableProperty] private string _team2LeaderName = string.Empty;
    [ObservableProperty] private string _currentActorLabel = string.Empty;

    public CS2MatchViewModel(ICS2GameService cs2, ISessionService session, INavigationService nav,
        IServiceProvider sp, IMatchProposalSync proposalSync, IPartyService party)
    {
        _cs2 = cs2;
        _session = session;
        _nav = nav;
        _sp = sp;
        _proposalSync = proposalSync;
        _party = party;
        Servers = new ObservableCollection<string>(cs2.ServerPool);

        PickServerCommand = new AsyncRelayCommand<string>(async s => await OnPickServer(s!));
        BanMapCommand     = new AsyncRelayCommand<MapBanItem>(async m => await OnBanMap(m!));
        ReturnCommand     = new RelayCommand(ReturnToMenu);
    }

    public Task InitializeAsync(List<User> team1, List<User> team2, int pendingMatchId, IReadOnlyList<int> realUserIds)
    {
        _team1 = team1;
        _team2 = team2;
        _pendingMatchId = pendingMatchId;
        var me = _session.CurrentUser!;
        _userTeam = team1.Exists(u => u.Id == me.Id) ? 1 : 2;

        var real = realUserIds?.Where(id => id > 0).Distinct().OrderBy(id => id).ToList() ?? new List<int>();
        if (real.Count == 0)
            real = new List<int> { me.Id };

        _multiHuman = real.Count > 1;
        var lobbyLeader = _party.CurrentLeaderId;
        var partyIds = _party.Party.Members.Select(m => m.Id).ToHashSet();
        bool allRealHumansAreOurLobbyMates = real.All(id => partyIds.Contains(id));
        _captainId = _multiHuman && allRealHumansAreOurLobbyMates && real.Contains(lobbyLeader)
            ? lobbyLeader
            : real.Min();
        _captainTeam = team1.Any(u => u.Id == _captainId) ? 1 : 2;
        _isCaptainAuthority = !_multiHuman || me.Id == _captainId;

        var capUser = team1.Concat(team2).FirstOrDefault(u => u.Id == _captainId);
        var captainNick = capUser?.Nickname ?? $"#{_captainId}";

        Team1LeaderName = team1.Count > 0 ? team1[0].Nickname : "";
        Team2LeaderName = team2.Count > 0 ? team2[0].Nickname : "";

        _serverPickerTeam = _captainTeam;
        _mapBanFirstTeam = _captainTeam;

        Maps.Clear();
        foreach (var m in _cs2.MapPool)
            Maps.Add(new MapBanItem { Name = m });
        MapsRemaining = Maps.Count;

        if (!_isCaptainAuthority)
        {
            Phase = CS2Phase.CaptainSyncWait;
            PhaseTitle = LanguageService.Instance["CaptainSyncWaitTitle"];
            PhaseSubtitle = string.Format(LanguageService.Instance["CaptainSyncWaitSubtitle"], captainNick);
            CurrentActorLabel = captainNick;
            CanActNow = false;
            StartPollForCaptainMatch();
            return Task.CompletedTask;
        }

        Phase = CS2Phase.ServerPick;
        PhaseTitle = LanguageService.Instance["ServerPickPhase"];
        CurrentActorLabel = string.Format(
            LanguageService.Instance["ServerPickBy"],
            _serverPickerTeam == 1 ? Team1LeaderName : Team2LeaderName);
        CanActNow = true;
        PhaseSubtitle = LanguageService.Instance["YourTeamPicks"];
        return Task.CompletedTask;
    }

    private void StartPollForCaptainMatch()
    {
        _pollTimer?.Stop();
        _pollTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(450) };
        _pollTimer.Tick += async (_, _) => await PollCaptainMatchTickAsync();
        _pollTimer.Start();
    }

    private async Task PollCaptainMatchTickAsync()
    {
        if (_pendingMatchId == 0) return;
        try
        {
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var id = await db.Matches.AsNoTracking()
                .Where(m => m.SourcePendingMatchId == _pendingMatchId)
                .Select(m => m.Id)
                .FirstOrDefaultAsync();
            if (id == 0) return;

            _pollTimer?.Stop();
            _pollTimer = null;
            await LoadResultFromDatabaseAsync(id);
        }
        catch
        {
        }
    }

    private async Task OnPickServer(string server)
    {
        if (Phase != CS2Phase.ServerPick) return;
        if (!IsCaptain) return;

        SelectedServer = server;
        Phase = CS2Phase.MapBan;
        PhaseTitle = LanguageService.Instance["BanPhase"];
        _currentBanningTeam = _mapBanFirstTeam;
        await ProcessBanTurnAsync();
    }

    private async Task ProcessBanTurnAsync()
    {
        UpdatePhaseSubtitle();
        string leaderName = _currentBanningTeam == 1 ? Team1LeaderName : Team2LeaderName;
        CurrentActorLabel = string.Format(LanguageService.Instance["MapBanBy"], leaderName);

        var remaining = Maps.Where(m => !m.IsBanned).ToList();
        MapsRemaining = remaining.Count;

        if (remaining.Count <= 1)
        {
            FinalMap = remaining.FirstOrDefault()?.Name ?? _cs2.MapPool[0];
            CanActNow = false;
            await SimulateAsync(FinalMap!);
            return;
        }

        if (_currentBanningTeam == _captainTeam && IsCaptain)
        {
            CanActNow = true;
        }
        else
        {
            CanActNow = false;
            await Task.Delay(1000 + Random.Shared.Next(700));
            var choice = remaining[Random.Shared.Next(remaining.Count)];
            await OnBanMap(choice);
        }
    }

    private void UpdatePhaseSubtitle()
    {
        if (_currentBanningTeam == _captainTeam)
        {
            PhaseSubtitle = IsCaptain
                ? LanguageService.Instance["YourTeamBans"]
                : LanguageService.Instance["WaitingForLeader"];
        }
        else
        {
            PhaseSubtitle = LanguageService.Instance["EnemyTeamBans"];
        }
    }

    private async Task OnBanMap(MapBanItem map)
    {
        if (Phase != CS2Phase.MapBan) return;
        if (map.IsBanned) return;
        if (_currentBanningTeam == _captainTeam && !IsCaptain) return;

        map.IsBanned = true;
        map.BannedByTeam = _currentBanningTeam;

        _currentBanningTeam = _currentBanningTeam == 1 ? 2 : 1;
        await ProcessBanTurnAsync();
    }

    private async Task SimulateAsync(string map)
    {
        var match = await _cs2.SimulateAndSaveAsync(_team1, _team2, map, SelectedServer!, _mapBanFirstTeam, _serverPickerTeam,
            _multiHuman ? _pendingMatchId : null);

        try
        {
            if (_multiHuman)
                await _proposalSync.CleanupAsync(_pendingMatchId);
        }
        catch { /* уже могли удалить */ }

        await LoadResultFromDatabaseAsync(match.Id);
    }

    private async Task LoadResultFromDatabaseAsync(int matchId)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var saved = await db.Matches
            .Include(m => m.Players)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(m => m.Id == matchId);
        if (saved == null) return;

        var details = System.Text.Json.JsonSerializer.Deserialize<CS2MatchDetails>(saved.ResultDetails)!;
        ScoreTeam1 = details.ScoreTeam1;
        ScoreTeam2 = details.ScoreTeam2;
        MapDisplay = details.Map;
        ServerDisplay = details.Server;

        Team1Rows.Clear();
        Team2Rows.Clear();
        var me = _session.CurrentUser!;
        foreach (var p in saved.Players.OrderByDescending(p => p.HltvRating))
        {
            var row = new CS2PlayerRow
            {
                Nickname = p.User.Nickname,
                Elo = p.EloAfter,
                Kills = p.Kills,
                Deaths = p.Deaths,
                Assists = p.Assists,
                Headshots = p.Headshots,
                Adr = p.Adr,
                Hltv = p.HltvRating,
                EloChange = p.EloAfter - p.EloBefore,
                IsSelf = p.UserId == me.Id,
                Team = p.Team == TeamSide.Team1 ? 1 : 2
            };
            if (p.Team == TeamSide.Team1) Team1Rows.Add(row); else Team2Rows.Add(row);
        }

        bool userWon = (_userTeam == 1 && ScoreTeam1 > ScoreTeam2) ||
                       (_userTeam == 2 && ScoreTeam2 > ScoreTeam1);
        IsVictory = userWon;
        ResultText = userWon ? LanguageService.Instance["Victory"] : LanguageService.Instance["Defeat"];
        Phase = CS2Phase.ResultScreen;

        var updated = await db.Users.FirstOrDefaultAsync(u => u.Id == me.Id);
        if (updated != null) me.EloCS2 = updated.EloCS2;
    }

    private void ReturnToMenu()
    {
        _pollTimer?.Stop();
        _pollTimer = null;
        if (_pendingMatchId != 0 && _multiHuman)
            _ = TryCleanupProposalAsync();
        _nav.NavigateRoot<MainViewModel>();
    }

    private async Task TryCleanupProposalAsync()
    {
        try { await _proposalSync.CleanupAsync(_pendingMatchId); }
        catch { /* */ }
    }
}
