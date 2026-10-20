using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flux.Data;
using Flux.Infrastructure;
using Flux.Localization;
using Flux.Models;
using Flux.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using SkiaSharp;

namespace Flux.ViewModels;

public partial class CS2StatsBlock : ObservableObject
{
    [ObservableProperty] private int _elo;
    [ObservableProperty] private int _totalMatches;
    [ObservableProperty] private int _wins;
    [ObservableProperty] private int _losses;
    [ObservableProperty] private double _winrate;

    [ObservableProperty] private double _kills;
    [ObservableProperty] private double _deaths;
    [ObservableProperty] private double _kd;
    [ObservableProperty] private double _kr;
    [ObservableProperty] private double _adr;
    [ObservableProperty] private double _hsPercent;
    [ObservableProperty] private string _kdaLine = "0 / 0 / 0";
}

public partial class ChessStatsBlock : ObservableObject
{
    [ObservableProperty] private int _elo;
    [ObservableProperty] private int _totalMatches;
    [ObservableProperty] private int _wins;
    [ObservableProperty] private int _losses;
    [ObservableProperty] private int _draws;
    [ObservableProperty] private double _winrate;

    [ObservableProperty] private int _totalMoves;
    [ObservableProperty] private double _avgMovesPerGame;
    [ObservableProperty] private double _avgGood;
    [ObservableProperty] private double _avgBrilliant;
    [ObservableProperty] private double _avgInaccuracies;
    [ObservableProperty] private double _avgMistakes;
    [ObservableProperty] private double _avgBlunders;
}

/// <summary>Строка в списке последних матчей на странице профиля.</summary>
public partial class MatchHistoryItem : ObservableObject
{
    public int MatchId { get; set; }
    public GameType GameType { get; set; }
    public DateTime PlayedAt { get; set; }
    public string GameTypeLabel => GameType == GameType.CS2 ? "CS2" : "Chess";
    public string ResultLabel { get; set; } = "";
    public string ResultBadge { get; set; } = "";  // W / L / D
    public string ScoreLabel { get; set; } = "";
    public int EloChange { get; set; }
    public string EloChangeLabel => EloChange >= 0 ? $"+{EloChange}" : EloChange.ToString();
    public bool IsWin { get; set; }
    public bool IsDraw { get; set; }
    public string DateLabel => FormatDate(PlayedAt);

    private static string FormatDate(DateTime utc)
    {
        var local = utc.ToLocalTime();
        var diff = DateTime.Now - local;
        var L = LanguageService.Instance;
        if (diff.TotalMinutes < 1) return L["JustNow"];
        if (diff.TotalMinutes < 60) return $"{(int)diff.TotalMinutes} {L["MinAgo"]}";
        if (diff.TotalHours < 24)   return $"{(int)diff.TotalHours} {L["HoursAgo"]}";
        if (diff.TotalDays < 7)     return $"{(int)diff.TotalDays} {L["DaysAgo"]}";
        return local.ToString("dd.MM.yyyy");
    }
}

public partial class ProfileViewModel : ViewModelBase
{
    private readonly ISessionService _session;
    private readonly IServiceProvider _sp;
    private readonly INavigationService _nav;
    private readonly IAvatarService _avatarService;

    [ObservableProperty] private string _nickname = string.Empty;
    [ObservableProperty] private int _selectedTab = 0;  // 0=CS2, 1=Chess
    [ObservableProperty] private bool _canGoBack;

    [ObservableProperty] private byte[]? _avatarBytes;
    [ObservableProperty] private bool _isOwnProfile = true;
    [ObservableProperty] private bool _hasCustomAvatar;

    // K/D chart
    [ObservableProperty] private ISeries[] _kdSeries = Array.Empty<ISeries>();
    [ObservableProperty] private Axis[] _xAxes = Array.Empty<Axis>();
    [ObservableProperty] private Axis[] _yAxes = Array.Empty<Axis>();
    [ObservableProperty] private bool _hasEnoughKdData;
    [ObservableProperty] private string _notEnoughDataText = "";

    // Match history
    public ObservableCollection<MatchHistoryItem> RecentCs2Matches { get; } = new();
    public ObservableCollection<MatchHistoryItem> AllCs2Matches { get; } = new();
    public ObservableCollection<MatchHistoryItem> RecentChessMatches { get; } = new();
    public ObservableCollection<MatchHistoryItem> AllChessMatches { get; } = new();
    [ObservableProperty] private bool _isFullHistoryVisible;
    [ObservableProperty] private bool _hasAnyMatches;
    [ObservableProperty] private int _historyTab; // 0 = CS2, 1 = Chess
    [ObservableProperty] private string _historyToggleText = "";

    public CS2StatsBlock CS2 { get; } = new();
    public ChessStatsBlock Chess { get; } = new();

    public ICommand BackCommand { get; }
    public ICommand SelectCS2TabCommand { get; }
    public ICommand SelectChessTabCommand { get; }
    public ICommand UploadAvatarCommand { get; }
    public ICommand RemoveAvatarCommand { get; }
    public ICommand ToggleFullHistoryCommand { get; }
    public ICommand ShowCs2HistoryCommand { get; }
    public ICommand ShowChessHistoryCommand { get; }
    public IAsyncRelayCommand<MatchHistoryItem?> OpenMatchDetailsCommand { get; }

    private int _targetUserId;

    public ProfileViewModel(ISessionService session, IServiceProvider sp, INavigationService nav, IAvatarService avatarService)
    {
        _session = session;
        _sp = sp;
        _nav = nav;
        _avatarService = avatarService;

        BackCommand = new RelayCommand(() =>
        {
            if (_nav.CanGoBack) _nav.GoBack();
            else _nav.NavigateRoot<MainViewModel>();
        });
        SelectCS2TabCommand = new RelayCommand(() => SelectedTab = 0);
        SelectChessTabCommand = new RelayCommand(() => SelectedTab = 1);
        UploadAvatarCommand = new AsyncRelayCommand(UploadAvatarAsync);
        RemoveAvatarCommand = new AsyncRelayCommand(RemoveAvatarAsync);
        ToggleFullHistoryCommand = new RelayCommand(() =>
        {
            IsFullHistoryVisible = !IsFullHistoryVisible;
            UpdateHistoryToggleText();
        });
        ShowCs2HistoryCommand = new RelayCommand(() => HistoryTab = 0);
        ShowChessHistoryCommand = new RelayCommand(() => HistoryTab = 1);
        OpenMatchDetailsCommand = new AsyncRelayCommand<MatchHistoryItem?>(OpenMatchDetailsAsync);
        UpdateHistoryToggleText();

        // Перезагружаем подписи (относительное время, причина окончания, win/loss и т.д.) при смене языка.
        LanguageService.Instance.LanguageChanged += async (_, _) =>
        {
            UpdateHistoryToggleText();
            if (_session.CurrentUser != null) await LoadAsync(_targetUserId == 0 ? null : _targetUserId);
        };
    }

    private void UpdateHistoryToggleText()
    {
        HistoryToggleText = IsFullHistoryVisible
            ? LanguageService.Instance["ShowLess"]
            : LanguageService.Instance["SeeFullHistory"];
    }

    private async Task OpenMatchDetailsAsync(MatchHistoryItem? item)
    {
        if (item == null) return;
        var vm = _sp.GetRequiredService<MatchDetailsViewModel>();
        await vm.LoadAsync(item.MatchId);
        _nav.NavigateTo(vm);
    }

    private async Task UploadAvatarAsync()
    {
        if (!IsOwnProfile || _session.CurrentUser == null) return;
        var dlg = new OpenFileDialog
        {
            Filter = "Images|*.png;*.jpg;*.jpeg",
            Title = LanguageService.Instance["UploadAvatar"]
        };
        if (dlg.ShowDialog() != true) return;

        var ok = await _avatarService.SaveAvatarAsync(_session.CurrentUser.Id, dlg.FileName);
        if (!ok) return;

        // Перечитываем bytes обратно (чтобы у текущей сессии тоже обновилось)
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == _session.CurrentUser.Id);
        if (user == null) return;

        _session.CurrentUser.AvatarBytes = user.AvatarBytes;
        AvatarBytes = null;            // сброс для гарантированного обновления MultiBinding
        AvatarBytes = user.AvatarBytes;
        HasCustomAvatar = user.AvatarBytes != null && user.AvatarBytes.Length > 0;
    }

    private async Task RemoveAvatarAsync()
    {
        if (!IsOwnProfile || _session.CurrentUser == null) return;
        await _avatarService.DeleteAvatarAsync(_session.CurrentUser.Id);
        _session.CurrentUser.AvatarBytes = null;
        AvatarBytes = null;
        HasCustomAvatar = false;
    }

    public async Task LoadAsync(int? userId = null)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        _targetUserId = userId ?? _session.CurrentUser!.Id;
        IsOwnProfile = _session.CurrentUser != null && _targetUserId == _session.CurrentUser.Id;

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == _targetUserId);
        if (user == null) return;

        Nickname = user.Nickname;
        CanGoBack = _nav.CanGoBack;
        AvatarBytes = user.AvatarBytes;
        HasCustomAvatar = IsOwnProfile && user.AvatarBytes != null && user.AvatarBytes.Length > 0;
        NotEnoughDataText = LanguageService.Instance["NotEnoughData"];

        // ---------- CS2 ----------
        var cs2 = await db.MatchPlayers.AsNoTracking()
            .Include(p => p.Match)
            .Where(p => p.UserId == _targetUserId && p.Match.GameType == GameType.CS2)
            .OrderByDescending(p => p.Match.MatchDate)
            .ToListAsync();

        CS2.Elo = user.EloCS2;
        CS2.TotalMatches = cs2.Count;
        CS2.Wins = cs2.Count(p => p.IsWinner);
        CS2.Losses = CS2.TotalMatches - CS2.Wins;
        CS2.Winrate = CS2.TotalMatches == 0 ? 0 : Math.Round(100.0 * CS2.Wins / CS2.TotalMatches, 1);

        var last30 = cs2.Take(30).ToList();
        if (last30.Count > 0)
        {
            CS2.Kills = Math.Round(last30.Average(p => p.Kills), 1);
            CS2.Deaths = Math.Round(last30.Average(p => p.Deaths), 1);
            double totalKills = last30.Sum(p => p.Kills);
            double totalDeaths = last30.Sum(p => p.Deaths);
            double totalHs = last30.Sum(p => p.Headshots);
            CS2.Kd = totalDeaths == 0 ? totalKills : Math.Round(totalKills / totalDeaths, 2);
            CS2.Adr = Math.Round(last30.Average(p => p.Adr), 1);
            CS2.HsPercent = totalKills == 0 ? 0 : Math.Round(100.0 * totalHs / totalKills);
            double kr = (totalKills + totalDeaths) == 0 ? 0 : totalKills / (totalKills + totalDeaths);
            CS2.Kr = Math.Round(kr, 2);
            CS2.KdaLine = $"{(int)Math.Round(last30.Average(p => p.Kills))} / {(int)Math.Round(last30.Average(p => p.Deaths))} / {(int)Math.Round(last30.Average(p => p.Assists))}";
        }
        else
        {
            CS2.Kills = CS2.Deaths = CS2.Kd = CS2.Kr = CS2.Adr = CS2.HsPercent = 0;
            CS2.KdaLine = "0 / 0 / 0";
        }

        BuildKdChart(cs2);

        // ---------- Chess ----------
        var chess = await db.MatchPlayers.AsNoTracking()
            .Include(p => p.Match)
            .Where(p => p.UserId == _targetUserId && p.Match.GameType == GameType.Chess)
            .OrderByDescending(p => p.Match.MatchDate)
            .ToListAsync();

        Chess.Elo = user.EloChess;
        Chess.TotalMatches = chess.Count;
        Chess.Wins = chess.Count(p => p.IsWinner);
        Chess.Draws = chess.Count(p => p.Match.IsDraw);
        Chess.Losses = Chess.TotalMatches - Chess.Wins - Chess.Draws;
        Chess.Winrate = Chess.TotalMatches == 0 ? 0 : Math.Round(100.0 * Chess.Wins / Chess.TotalMatches, 1);

        var last30Chess = chess.Take(30).ToList();
        if (last30Chess.Count > 0)
        {
            Chess.TotalMoves = last30Chess.Sum(p => p.MoveCount);
            Chess.AvgMovesPerGame = Math.Round(last30Chess.Average(p => p.MoveCount), 1);
            Chess.AvgGood = Math.Round(last30Chess.Average(p => p.GoodMoves), 1);
            Chess.AvgBrilliant = Math.Round(last30Chess.Average(p => p.BrilliantMoves), 1);
            Chess.AvgInaccuracies = Math.Round(last30Chess.Average(p => p.Inaccuracies), 1);
            Chess.AvgMistakes = Math.Round(last30Chess.Average(p => p.Mistakes), 1);
            Chess.AvgBlunders = Math.Round(last30Chess.Average(p => p.Blunders), 1);
        }
        else
        {
            Chess.TotalMoves = 0;
            Chess.AvgMovesPerGame = Chess.AvgGood = Chess.AvgBrilliant = 0;
            Chess.AvgInaccuracies = Chess.AvgMistakes = Chess.AvgBlunders = 0;
        }

        BuildMatchHistory(cs2, chess);
        UpdateHistoryToggleText();
    }

    /// <summary>Строит данные графика K/D из CS2-матчей (старые → новые, последние 30).</summary>
    private void BuildKdChart(IReadOnlyList<MatchPlayer> cs2Matches)
    {
        if (cs2Matches.Count < 2)
        {
            HasEnoughKdData = false;
            KdSeries = Array.Empty<ISeries>();
            XAxes = Array.Empty<Axis>();
            YAxes = Array.Empty<Axis>();
            return;
        }

        // От старых к новым, не более 30
        var ordered = cs2Matches
            .OrderBy(p => p.Match.MatchDate)
            .Reverse()
            .Take(30)
            .Reverse()
            .ToList();

        var kd = ordered
            .Select(p => p.Deaths == 0 ? (double)p.Kills : Math.Round((double)p.Kills / p.Deaths, 2))
            .ToArray();

        HasEnoughKdData = true;
        KdSeries = new ISeries[]
        {
            new LineSeries<double>
            {
                Values = kd,
                Fill = new SolidColorPaint(new SKColor(255, 204, 0, 25)),
                Stroke = new SolidColorPaint(new SKColor(255, 204, 0)) { StrokeThickness = 2.5f },
                GeometryFill = new SolidColorPaint(new SKColor(255, 204, 0)),
                GeometryStroke = new SolidColorPaint(new SKColor(10, 10, 10)) { StrokeThickness = 2 },
                GeometrySize = 8,
                LineSmoothness = 0.5,
                Name = "K/D"
            }
        };

        XAxes = new[]
        {
            new Axis
            {
                Labeler = v => $"#{(int)v + 1}",
                MinStep = 1,
                ForceStepToMin = true,
                TextSize = 11,
                LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                SeparatorsPaint = new SolidColorPaint(new SKColor(60, 60, 60))
            }
        };
        YAxes = new[]
        {
            new Axis
            {
                MinLimit = 0,
                MaxLimit = Math.Max(3.0, kd.Max() + 0.3),
                MinStep = 1,
                TextSize = 11,
                LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                SeparatorsPaint = new SolidColorPaint(new SKColor(60, 60, 60))
            }
        };
    }

    /// <summary>Строит списки истории по каждому типу игры (Recent = 5, All = 50).</summary>
    private void BuildMatchHistory(IReadOnlyList<MatchPlayer> cs2, IReadOnlyList<MatchPlayer> chess)
    {
        var cs2Items = new List<MatchHistoryItem>();
        var chessItems = new List<MatchHistoryItem>();

        foreach (var p in cs2)
        {
            var d = SafeDeserialize<DTOs.CS2MatchDetails>(p.Match.ResultDetails);
            bool isDraw = d.ScoreTeam1 == d.ScoreTeam2;
            string result = isDraw ? LanguageService.Instance["Draw"] : (p.IsWinner ? LanguageService.Instance["Win"] : LanguageService.Instance["Loss"]);
            string badge = isDraw ? "D" : (p.IsWinner ? "W" : "L");
            string score = p.Team == TeamSide.Team1
                ? $"{d.ScoreTeam1}:{d.ScoreTeam2}"
                : $"{d.ScoreTeam2}:{d.ScoreTeam1}";
            cs2Items.Add(new MatchHistoryItem
            {
                MatchId = p.MatchId,
                GameType = GameType.CS2,
                PlayedAt = p.Match.MatchDate,
                ResultLabel = result,
                ResultBadge = badge,
                ScoreLabel = score,
                EloChange = p.EloAfter - p.EloBefore,
                IsWin = p.IsWinner,
                IsDraw = isDraw
            });
        }

        foreach (var p in chess)
        {
            var d = SafeDeserialize<DTOs.ChessMatchDetails>(p.Match.ResultDetails);
            bool isDraw = p.Match.IsDraw;
            string result = isDraw ? LanguageService.Instance["Draw"] : (p.IsWinner ? LanguageService.Instance["Win"] : LanguageService.Instance["Loss"]);
            string badge = isDraw ? "D" : (p.IsWinner ? "W" : "L");
            string rawReason = string.IsNullOrWhiteSpace(d.EndReason) ? "" : d.EndReason;
            string reason = rawReason switch
            {
                "Checkmate"            => LanguageService.Instance["Checkmate"],
                "Stalemate"            => LanguageService.Instance["Stalemate"],
                "Resignation"          => LanguageService.Instance["Resignation"],
                "Agreement"            => LanguageService.Instance["DrawByAgreement"],
                "InsufficientMaterial" => LanguageService.Instance["InsufficientMaterial"],
                _ => LanguageService.Instance["GameOver"]
            };
            string score = $"{reason} · {p.MoveCount} {LanguageService.Instance["MovesLower"]}";
            chessItems.Add(new MatchHistoryItem
            {
                MatchId = p.MatchId,
                GameType = GameType.Chess,
                PlayedAt = p.Match.MatchDate,
                ResultLabel = result,
                ResultBadge = badge,
                ScoreLabel = score,
                EloChange = p.EloAfter - p.EloBefore,
                IsWin = p.IsWinner,
                IsDraw = isDraw
            });
        }

        cs2Items = cs2Items.OrderByDescending(i => i.PlayedAt).ToList();
        chessItems = chessItems.OrderByDescending(i => i.PlayedAt).ToList();

        RecentCs2Matches.Clear();
        foreach (var i in cs2Items.Take(5)) RecentCs2Matches.Add(i);
        AllCs2Matches.Clear();
        foreach (var i in cs2Items.Take(50)) AllCs2Matches.Add(i);

        RecentChessMatches.Clear();
        foreach (var i in chessItems.Take(5)) RecentChessMatches.Add(i);
        AllChessMatches.Clear();
        foreach (var i in chessItems.Take(50)) AllChessMatches.Add(i);

        HasAnyMatches = (cs2Items.Count + chessItems.Count) > 0;
    }

    private static T SafeDeserialize<T>(string json) where T : new()
    {
        try { return System.Text.Json.JsonSerializer.Deserialize<T>(json) ?? new T(); }
        catch { return new T(); }
    }
}
