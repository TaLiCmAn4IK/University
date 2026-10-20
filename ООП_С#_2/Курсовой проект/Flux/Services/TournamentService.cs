using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Flux.Data;
using Flux.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Services;

public interface ITournamentService
{
    Task<Tournament> CreateAsync(string name, GameType game, int maxPlayers, int durationMinutes, int creatorId);
    Task<bool> JoinAsync(int tournamentId, int userId);
    Task<bool> LeaveAsync(int tournamentId, int userId);
    Task<bool> CancelAsync(int tournamentId);
    Task<List<Tournament>> GetAllAsync();

    void StartAutoRunner();
    void StopAutoRunner();
}

public class TournamentService : ITournamentService, IDisposable
{
    private readonly IServiceProvider _sp;
    private CancellationTokenSource? _cts;
    private static readonly Random Rng = new();

    public TournamentService(IServiceProvider sp) { _sp = sp; }

    public async Task<Tournament> CreateAsync(string name, GameType game, int maxPlayers, int durationMinutes, int creatorId)
    {
        if (maxPlayers < 2) maxPlayers = 2;
        if (durationMinutes < 1) durationMinutes = 1;

        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var t = new Tournament
        {
            Name = name,
            GameType = game,
            MaxPlayers = maxPlayers,
            Status = TournamentStatus.Registration,
            CreatorId = creatorId,
            CreatedAt = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddMinutes(durationMinutes)
        };
        db.Tournaments.Add(t);
        await db.SaveChangesAsync();
        return t;
    }

    public async Task<bool> JoinAsync(int tournamentId, int userId)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null || user.IsBanned) return false;

        var t = await db.Tournaments.FindAsync(tournamentId);
        if (t == null || t.Status != TournamentStatus.Registration) return false;
        if (DateTime.UtcNow >= t.EndTime) return false;

        var count = await db.TournamentParticipants.CountAsync(p => p.TournamentId == tournamentId);
        if (count >= t.MaxPlayers) return false;

        var already = await db.TournamentParticipants
            .AnyAsync(p => p.TournamentId == tournamentId && p.UserId == userId);
        if (already) return false;

        db.TournamentParticipants.Add(new TournamentParticipant
        {
            TournamentId = tournamentId, UserId = userId, JoinedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> LeaveAsync(int tournamentId, int userId)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var t = await db.Tournaments.FindAsync(tournamentId);
        if (t == null || t.Status != TournamentStatus.Registration) return false;

        var p = await db.TournamentParticipants
            .FirstOrDefaultAsync(x => x.TournamentId == tournamentId && x.UserId == userId);
        if (p == null) return false;
        db.TournamentParticipants.Remove(p);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAsync(int tournamentId)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var t = await db.Tournaments.FindAsync(tournamentId);
        if (t == null) return false;
        t.Status = TournamentStatus.Cancelled;
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<List<Tournament>> GetAllAsync()
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.Tournaments
            .AsNoTracking()
            .Include(t => t.Creator)
            .Include(t => t.Winner)
            .Include(t => t.Participants).ThenInclude(p => p.User)
            .Where(t => t.Status != TournamentStatus.Cancelled)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }


    public void StartAutoRunner()
    {
        StopAutoRunner();
        _cts = new CancellationTokenSource();
        _ = RunLoopAsync(_cts.Token);
    }

    public void StopAutoRunner()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public void Dispose() => StopAutoRunner();

    private async Task RunLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try { await TickAsync(); } catch { }
            try { await Task.Delay(TimeSpan.FromSeconds(3), ct); }
            catch (TaskCanceledException) { break; }
        }
    }

    private async Task TickAsync()
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var now = DateTime.UtcNow;

        var expired = await db.Tournaments
            .Include(t => t.Participants)
            .Where(t => t.Status == TournamentStatus.Registration && t.EndTime <= now)
            .ToListAsync();

        foreach (var t in expired)
        {
            var plist = t.Participants.ToList();
            if (plist.Count == 0)
            {
                t.Status = TournamentStatus.Cancelled;
            }
            else
            {
                var winner = plist[Rng.Next(plist.Count)];
                t.WinnerId = winner.UserId;
                t.Status = TournamentStatus.Finished;
                t.FinishedAt = now;
            }
        }
        if (expired.Count > 0) await db.SaveChangesAsync();
    }
}
