using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flux.Data;
using Flux.DTOs;
using Flux.Infrastructure;
using Flux.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.ViewModels;

public partial class MatchDetailsViewModel : ViewModelBase
{
    private readonly IServiceProvider _sp;
    private readonly INavigationService _nav;

    [ObservableProperty] private GameType _gameType;
    [ObservableProperty] private string _dateText = string.Empty;
    [ObservableProperty] private string _mapText = string.Empty;
    [ObservableProperty] private string _serverText = string.Empty;
    [ObservableProperty] private int _scoreTeam1;
    [ObservableProperty] private int _scoreTeam2;
    [ObservableProperty] private string _resultText = string.Empty;
    [ObservableProperty] private string _chessMovesText = string.Empty;
    [ObservableProperty] private string _chessEndReason = string.Empty;
    [ObservableProperty] private string _chessFinalFen = string.Empty;
    [ObservableProperty] private string _whiteName = string.Empty;
    [ObservableProperty] private string _blackName = string.Empty;
    [ObservableProperty] private int _whiteGood;
    [ObservableProperty] private int _whiteBrilliant;
    [ObservableProperty] private int _whiteInaccuracies;
    [ObservableProperty] private int _whiteMistakes;
    [ObservableProperty] private int _whiteBlunders;
    [ObservableProperty] private int _blackGood;
    [ObservableProperty] private int _blackBrilliant;
    [ObservableProperty] private int _blackInaccuracies;
    [ObservableProperty] private int _blackMistakes;
    [ObservableProperty] private int _blackBlunders;

    public ObservableCollection<CS2PlayerRow> Team1 { get; } = new();
    public ObservableCollection<CS2PlayerRow> Team2 { get; } = new();

    public ICommand BackCommand { get; }

    public MatchDetailsViewModel(IServiceProvider sp, INavigationService nav)
    {
        _sp = sp; _nav = nav;
        BackCommand = new RelayCommand(() =>
        {
            if (_nav.CanGoBack) _nav.GoBack();
            else _nav.NavigateRoot<MainViewModel>();
        });
    }

    public async Task LoadAsync(int matchId)
    {
        Team1.Clear();
        Team2.Clear();

        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var match = await db.Matches.AsNoTracking()
            .Include(m => m.Players).ThenInclude(p => p.User)
            .FirstOrDefaultAsync(m => m.Id == matchId);
        if (match == null) return;

        GameType = match.GameType;
        DateText = match.MatchDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm");

        if (match.GameType == GameType.CS2)
        {
            var d = Safe<CS2MatchDetails>(match.ResultDetails);
            MapText = d.Map;
            ServerText = d.Server;
            ScoreTeam1 = d.ScoreTeam1;
            ScoreTeam2 = d.ScoreTeam2;
            ResultText = d.ScoreTeam1 == d.ScoreTeam2 ? "Draw" :
                (d.ScoreTeam1 > d.ScoreTeam2 ? "Team 1 wins" : "Team 2 wins");
            foreach (var p in match.Players.OrderByDescending(p => p.HltvRating))
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
                    Team = p.Team == TeamSide.Team1 ? 1 : 2
                };
                if (p.Team == TeamSide.Team1) Team1.Add(row); else Team2.Add(row);
            }
        }
        else
        {
            var d = Safe<ChessMatchDetails>(match.ResultDetails);
            ResultText = d.Result;
            ChessEndReason = LocalizeEndReason(d.EndReason);
            ChessFinalFen = d.FinalFen;
            ChessMovesText = FormatMoves(d.Moves);
            var white = match.Players.FirstOrDefault(p => p.Team == TeamSide.Team1);
            var black = match.Players.FirstOrDefault(p => p.Team == TeamSide.Team2);
            WhiteName = white?.User.Nickname ?? "";
            BlackName = black?.User.Nickname ?? "";
            if (white != null)
            {
                WhiteGood = white.GoodMoves;
                WhiteBrilliant = white.BrilliantMoves;
                WhiteInaccuracies = white.Inaccuracies;
                WhiteMistakes = white.Mistakes;
                WhiteBlunders = white.Blunders;
            }
            if (black != null)
            {
                BlackGood = black.GoodMoves;
                BlackBrilliant = black.BrilliantMoves;
                BlackInaccuracies = black.Inaccuracies;
                BlackMistakes = black.Mistakes;
                BlackBlunders = black.Blunders;
            }
        }
    }

    private static string FormatMoves(System.Collections.Generic.List<string> moves)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < moves.Count; i++)
        {
            if (i % 2 == 0) sb.Append($"{(i / 2) + 1}. ");
            sb.Append(moves[i]);
            sb.Append(i % 2 == 0 ? ' ' : '\n');
        }
        return sb.ToString();
    }

    private static T Safe<T>(string json) where T : new()
    {
        try { return JsonSerializer.Deserialize<T>(json) ?? new T(); }
        catch { return new T(); }
    }

    /// <summary>Маппинг строковых имён EndReason из БД в локализованные подписи.</summary>
    private static string LocalizeEndReason(string raw) => raw switch
    {
        "Checkmate"            => Localization.LanguageService.Instance["Checkmate"],
        "Stalemate"            => Localization.LanguageService.Instance["Stalemate"],
        "Resignation"          => Localization.LanguageService.Instance["Resignation"],
        "Agreement"            => Localization.LanguageService.Instance["DrawByAgreement"],
        "InsufficientMaterial" => Localization.LanguageService.Instance["InsufficientMaterial"],
        _ => raw
    };
}
