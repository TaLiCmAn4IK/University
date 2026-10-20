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
using Flux.Infrastructure;
using Flux.Localization;
using Flux.Models;
using Flux.Services;

namespace Flux.ViewModels;

public partial class TournamentRow : ObservableObject
{
    public int Id { get; set; }
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _gameLabel = string.Empty;
    [ObservableProperty] private int _players;
    [ObservableProperty] private int _maxPlayers;
    [ObservableProperty] private string _statusLabel = string.Empty;
    [ObservableProperty] private string _countdown = string.Empty;
    [ObservableProperty] private string _participantList = string.Empty;
    [ObservableProperty] private string _winnerName = string.Empty;
    [ObservableProperty] private bool _isFinished;
    [ObservableProperty] private bool _canJoin;
    [ObservableProperty] private bool _canLeave;
    [ObservableProperty] private bool _canCancel;

    public string Capacity => $"{Players}/{MaxPlayers}";
}

public partial class TournamentsViewModel : ViewModelBase, INavigationPage
{
    private readonly ITournamentService _tournaments;
    private readonly ISessionService _session;
    private readonly IDialogService _dialog;
    private DispatcherTimer? _timer;

    public ObservableCollection<TournamentRow> Items { get; } = new();

    [ObservableProperty] private bool _isAdmin;
    [ObservableProperty] private bool _isCreating;
    [ObservableProperty] private string _newName = string.Empty;
    [ObservableProperty] private GameType _newGameType = GameType.CS2;
    [ObservableProperty] private int _newMaxPlayers = 8;
    [ObservableProperty] private int _newDurationMinutes = 2;
    [ObservableProperty] private bool _hasItems;

    /// <summary>Текущее значение NewGameType в виде локализованной метки (для отображения в форме).</summary>
    public string SelectedGameLabel => NewGameType == GameType.CS2
        ? LanguageService.Instance["CS2"]
        : LanguageService.Instance["Chess"];

    partial void OnNewGameTypeChanged(GameType value)
        => OnPropertyChanged(nameof(SelectedGameLabel));

    public ICommand ShowCreateCommand { get; }
    public ICommand CancelCreateCommand { get; }
    public ICommand SelectCS2Command { get; }
    public ICommand SelectChessCommand { get; }
    public IAsyncRelayCommand CreateCommand { get; }
    public IAsyncRelayCommand RefreshCommand { get; }
    public IAsyncRelayCommand<TournamentRow> JoinCommand { get; }
    public IAsyncRelayCommand<TournamentRow> LeaveCommand { get; }
    public IAsyncRelayCommand<TournamentRow> CancelTournamentCommand { get; }

    public TournamentsViewModel(ITournamentService tournaments, ISessionService session, IDialogService dialog)
    {
        _tournaments = tournaments;
        _session = session;
        _dialog = dialog;

        ShowCreateCommand   = new RelayCommand(() => IsCreating = true);
        CancelCreateCommand = new RelayCommand(() => { IsCreating = false; NewName = string.Empty; });
        SelectCS2Command    = new RelayCommand(() => NewGameType = GameType.CS2);
        SelectChessCommand  = new RelayCommand(() => NewGameType = GameType.Chess);

        CreateCommand            = new AsyncRelayCommand(CreateAsync, () => !string.IsNullOrWhiteSpace(NewName));
        RefreshCommand           = new AsyncRelayCommand(LoadAsync);
        JoinCommand              = new AsyncRelayCommand<TournamentRow>(JoinAsync!);
        LeaveCommand             = new AsyncRelayCommand<TournamentRow>(LeaveAsync!);
        CancelTournamentCommand  = new AsyncRelayCommand<TournamentRow>(CancelAsync!);

        // Перечитываем строки при смене языка
        LanguageService.Instance.LanguageChanged += async (_, _) =>
        {
            OnPropertyChanged(nameof(SelectedGameLabel));
            await LoadAsync();
        };
    }

    partial void OnNewNameChanged(string value)
        => CreateCommand.NotifyCanExecuteChanged();

    public void OnNavigatedFrom() => OnDeactivate();

    public void OnNavigatedTo() => OnActivate();

    public void OnActivate()
    {
        IsAdmin = _session.IsAdmin;
        _ = LoadAsync();
        _timer?.Stop();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
        _timer.Tick += async (_, _) => await LoadAsync();
        _timer.Start();
    }

    public void OnDeactivate()
    {
        _timer?.Stop();
        _timer = null;
    }

    public async Task LoadAsync()
    {
        try
        {
            var list = await _tournaments.GetAllAsync();
            int? myId = _session.CurrentUser?.Id;
            bool admin = _session.IsAdmin;

            var rows = list.Select(t =>
            {
                var participants = t.Participants?.ToList() ?? new List<TournamentParticipant>();
                bool amParticipant = myId.HasValue && participants.Any(p => p.UserId == myId.Value);

                var row = new TournamentRow
                {
                    Id = t.Id,
                    Name = t.Name,
                    GameLabel = t.GameType == GameType.CS2
                        ? LanguageService.Instance["CS2"]
                        : LanguageService.Instance["Chess"],
                    Players = participants.Count,
                    MaxPlayers = t.MaxPlayers,
                    IsFinished = t.Status == TournamentStatus.Finished,
                    WinnerName = t.Winner?.Nickname ?? string.Empty,
                    ParticipantList = string.Join(", ",
                        participants.Select(p => p.User?.Nickname ?? $"#{p.UserId}"))
                };

                if (t.Status == TournamentStatus.Registration)
                {
                    var rem = t.EndTime - DateTime.UtcNow;
                    row.StatusLabel = LanguageService.Instance["TournamentStatusRegistration"];
                    row.Countdown = rem.TotalSeconds > 0 ? FormatRemaining(rem)
                                                        : LanguageService.Instance["StartingNow"];
                    row.CanJoin   = !admin && !amParticipant && participants.Count < t.MaxPlayers;
                    row.CanLeave  = !admin && amParticipant;
                    row.CanCancel = admin;
                }
                else if (t.Status == TournamentStatus.Finished)
                {
                    row.StatusLabel = LanguageService.Instance["TournamentStatusFinished"];
                    row.Countdown = string.Empty;
                    row.CanCancel = admin;
                }
                return row;
            }).ToList();

            Application.Current?.Dispatcher.Invoke(() =>
            {
                Items.Clear();
                foreach (var r in rows) Items.Add(r);
                HasItems = Items.Count > 0;
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[Tournaments] LoadAsync error: {ex}");
        }
    }

    private static string FormatRemaining(TimeSpan ts)
    {
        var min = LanguageService.Instance["MinutesShort"];
        var sec = LanguageService.Instance["SecondsShort"];
        if (ts.TotalMinutes >= 1) return $"{(int)ts.TotalMinutes} {min} {ts.Seconds} {sec}";
        return $"{ts.Seconds} {sec}";
    }

    private async Task CreateAsync()
    {
        var me = _session.CurrentUser;
        if (me == null) return;
        await _tournaments.CreateAsync(NewName.Trim(), NewGameType, NewMaxPlayers, NewDurationMinutes, me.Id);
        IsCreating = false;
        NewName = string.Empty;
        await LoadAsync();
    }

    private async Task JoinAsync(TournamentRow row)
    {
        var me = _session.CurrentUser;
        if (me == null) return;
        var ok = await _tournaments.JoinAsync(row.Id, me.Id);
        if (!ok)
            _dialog.ShowError(LanguageService.Instance["TournamentJoinFailed"]);
        await LoadAsync();
    }

    private async Task LeaveAsync(TournamentRow row)
    {
        var me = _session.CurrentUser;
        if (me == null) return;
        await _tournaments.LeaveAsync(row.Id, me.Id);
        await LoadAsync();
    }

    private async Task CancelAsync(TournamentRow row)
    {
        await _tournaments.CancelAsync(row.Id);
        await LoadAsync();
    }
}
