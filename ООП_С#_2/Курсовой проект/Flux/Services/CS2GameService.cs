using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Flux.Data;
using Flux.DTOs;
using Flux.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flux.Services;

public interface ICS2GameService
{
    IReadOnlyList<string> MapPool { get; }
    IReadOnlyList<string> ServerPool { get; }
    Task<Match> SimulateAndSaveAsync(List<User> team1, List<User> team2, string map, string server,
        int mapPickerTeam, int serverPickerTeam, int? sourcePendingMatchId = null);
}

public class CS2GameService : ICS2GameService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRatingService _rating;
    private readonly ILogger<CS2GameService> _logger;
    private readonly Random _rng = new();

    public IReadOnlyList<string> MapPool { get; } =
        new[] { "Mirage", "Inferno", "Dust2", "Nuke", "Ancient", "Vertigo" };
    public IReadOnlyList<string> ServerPool { get; } =
        new[] { "EU East", "EU West", "Stockholm", "Frankfurt" };

    public CS2GameService(IServiceScopeFactory scopeFactory, IRatingService rating, ILogger<CS2GameService> logger)
    {
        _scopeFactory = scopeFactory;
        _rating = rating;
        _logger = logger;
    }

    public async Task<Match> SimulateAndSaveAsync(List<User> team1, List<User> team2, string map, string server,
        int mapPickerTeam, int serverPickerTeam, int? sourcePendingMatchId = null)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var t1Ids = team1.Select(u => u.Id).ToList();
        var t2Ids = team2.Select(u => u.Id).ToList();
        var t1 = await db.Users.Where(u => t1Ids.Contains(u.Id)).ToListAsync();
        var t2 = await db.Users.Where(u => t2Ids.Contains(u.Id)).ToListAsync();

        int elo1 = (int)t1.Average(u => u.EloCS2);
        int elo2 = (int)t2.Average(u => u.EloCS2);
        double exp1 = _rating.ExpectedScore(elo1, elo2);
        double winProb1 = Math.Clamp(0.3 + 0.4 * exp1 + 0.15 * (exp1 - 0.5), 0.25, 0.85);
        bool team1Wins = _rng.NextDouble() < winProb1;

        int winnerScore = 13;
        int loserScore;
        if (_rng.NextDouble() < 0.25)
        {
            winnerScore = 16;
            loserScore = _rng.Next(12, 15);
        }
        else
        {
            double closeness = 1 - Math.Abs(exp1 - 0.5) * 2;
            int max = closeness > 0.6 ? 11 : 9;
            loserScore = _rng.Next(0, max + 1);
        }

        int scoreT1 = team1Wins ? winnerScore : loserScore;
        int scoreT2 = team1Wins ? loserScore : winnerScore;
        int totalRounds = scoreT1 + scoreT2;

        var match = new Match
        {
            GameType = GameType.CS2,
            IsDraw = false,
            SourcePendingMatchId = sourcePendingMatchId,
            ResultDetails = JsonSerializer.Serialize(new CS2MatchDetails
            {
                Map = map,
                Server = server,
                ScoreTeam1 = scoreT1,
                ScoreTeam2 = scoreT2,
                PickerTeam = mapPickerTeam,
                ServerPickerTeam = serverPickerTeam
            })
        };
        db.Matches.Add(match);
        await db.SaveChangesAsync();

        var t1Rows = BuildPlayerRows(t1, TeamSide.Team1, team1Wins, totalRounds);
        var t2Rows = BuildPlayerRows(t2, TeamSide.Team2, !team1Wins, totalRounds);

        int baseDeltaT1 = _rating.CalculateDelta(elo1, elo2, team1Wins ? 1.0 : 0.0);
        int baseDeltaT2 = _rating.CalculateDelta(elo2, elo1, team1Wins ? 0.0 : 1.0);

        double t1AvgKd = t1Rows.Average(r => r.KD);
        double t1AvgAdr = t1Rows.Average(r => r.Adr);
        double t2AvgKd = t2Rows.Average(r => r.KD);
        double t2AvgAdr = t2Rows.Average(r => r.Adr);

        ApplyElo(db, match.Id, t1Rows, baseDeltaT1, team1Wins, t1AvgKd, t1AvgAdr);
        ApplyElo(db, match.Id, t2Rows, baseDeltaT2, !team1Wins, t2AvgKd, t2AvgAdr);
        await db.SaveChangesAsync();

        foreach (var u in t1) { var m = team1.First(x => x.Id == u.Id); m.EloCS2 = u.EloCS2; }
        foreach (var u in t2) { var m = team2.First(x => x.Id == u.Id); m.EloCS2 = u.EloCS2; }

        _logger.LogInformation("CS2 match {Id} {S1}:{S2} on {Map}", match.Id, scoreT1, scoreT2, map);
        return match;
    }


    private sealed class PlayerStatRow
    {
        public User User = null!;
        public TeamSide Side;
        public int Kills, Deaths, Assists, Headshots;
        public double Adr;
        public double Hltv;
        public double KD => Deaths == 0 ? Kills : (double)Kills / Deaths;
    }

    private List<PlayerStatRow> BuildPlayerRows(List<User> team, TeamSide side, bool winners, int totalRounds)
    {
        var rows = new List<PlayerStatRow>(team.Count);
        foreach (var u in team)
        {
            double kd = winners
                ? 1.2 + _rng.NextDouble() * 0.4
                : 0.55 + _rng.NextDouble() * 0.35;

            int kills = (int)Math.Round(totalRounds * (winners ? 0.7 : 0.55) * (0.85 + _rng.NextDouble() * 0.3));
            int deaths = kd > 0 ? (int)Math.Round(kills / kd) : totalRounds;
            deaths = Math.Max(1, Math.Min(deaths, totalRounds + 5));
            int assists = (int)Math.Round(kills * (0.2 + _rng.NextDouble() * 0.3));
            // Headshot rate: 35%..65%
            int headshots = (int)Math.Round(kills * (0.35 + _rng.NextDouble() * 0.30));
            double adr = winners ? 80 + _rng.NextDouble() * 30 : 55 + _rng.NextDouble() * 25;
            double hltv = winners ? 1.05 + _rng.NextDouble() * 0.30 : 0.75 + _rng.NextDouble() * 0.25;

            rows.Add(new PlayerStatRow
            {
                User = u,
                Side = side,
                Kills = kills,
                Deaths = deaths,
                Assists = assists,
                Headshots = headshots,
                Adr = Math.Round(adr, 1),
                Hltv = Math.Round(hltv, 2)
            });
        }
        return rows;
    }

    private void ApplyElo(AppDbContext db, int matchId, List<PlayerStatRow> rows, int baseDelta,
        bool winners, double teamAvgKd, double teamAvgAdr)
    {
        foreach (var r in rows)
        {
            int mod = _rating.PerformanceModifier(winners, r.KD, r.Adr, teamAvgKd, teamAvgAdr);
            int finalDelta = baseDelta + mod;

            int eloBefore = r.User.EloCS2;
            r.User.EloCS2 = Math.Max(100, r.User.EloCS2 + finalDelta);

            db.MatchPlayers.Add(new MatchPlayer
            {
                MatchId = matchId,
                UserId = r.User.Id,
                Team = r.Side,
                Kills = r.Kills,
                Deaths = r.Deaths,
                Assists = r.Assists,
                Headshots = r.Headshots,
                Adr = r.Adr,
                HltvRating = r.Hltv,
                IsWinner = winners,
                EloBefore = eloBefore,
                EloAfter = r.User.EloCS2
            });
        }
    }
}
