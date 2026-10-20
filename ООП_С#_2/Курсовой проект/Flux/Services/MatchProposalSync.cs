using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Flux.Data;
using Flux.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Services;

public class ProposalSnapshot
{
    public int PendingMatchId { get; set; }
    public GameType GameType { get; set; }
    public List<int> Team1 { get; set; } = new();
    public List<int> Team2 { get; set; } = new();
    public List<int> RealUserIds { get; set; } = new();
    public string OwnerToken { get; set; } = string.Empty;
}

public class IncomingProposalEventArgs : EventArgs
{
    public int PendingMatchId { get; init; }
    public GameType GameType { get; init; }
    public List<User> Team1 { get; init; } = new();
    public List<User> Team2 { get; init; } = new();
    public List<int> RealUserIds { get; init; } = new();
    public string OwnerToken { get; init; } = string.Empty;
}

public class ProposalProgressEventArgs : EventArgs
{
    public int PendingMatchId { get; init; }
    public int Accepted { get; init; }
    public int Total { get; init; }
}

public interface IMatchProposalSync
{
    string MyToken { get; }

    event EventHandler<IncomingProposalEventArgs>? IncomingProposal;

    event EventHandler<ProposalProgressEventArgs>? ProgressChanged;

    event EventHandler<IncomingProposalEventArgs>? ProposalConfirmed;

    event EventHandler<int>? ProposalCancelled;

    void Start(int userId);
    void Stop();

    Task PublishProposalAsync(ProposalSnapshot snapshot, IReadOnlyCollection<int> botUserIds);

    Task AcceptAsync(int pendingMatchId, int userId);
    Task DeclineAsync(int pendingMatchId, int userId);

    Task CleanupAsync(int pendingMatchId);
}
public class MatchProposalSync : IMatchProposalSync, IDisposable
{
    private readonly IServiceProvider _sp;
    private CancellationTokenSource? _cts;
    private int _userId;
    private readonly HashSet<int> _seenIncoming = new();
    private readonly HashSet<int> _seenConfirmed = new();
    private readonly HashSet<int> _seenCancelled = new();
    private readonly Dictionary<int, int> _lastAcceptedCount = new();

    public string MyToken { get; } = Guid.NewGuid().ToString("N");

    public event EventHandler<IncomingProposalEventArgs>? IncomingProposal;
    public event EventHandler<ProposalProgressEventArgs>? ProgressChanged;
    public event EventHandler<IncomingProposalEventArgs>? ProposalConfirmed;
    public event EventHandler<int>? ProposalCancelled;

    public MatchProposalSync(IServiceProvider sp) { _sp = sp; }

    public void Start(int userId)
    {
        Stop();
        _userId = userId;
        _seenIncoming.Clear();
        _seenConfirmed.Clear();
        _seenCancelled.Clear();
        _lastAcceptedCount.Clear();
        _cts = new CancellationTokenSource();
        _ = PollLoopAsync(_cts.Token);
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
    }

    public void Dispose() => Stop();

    private async Task PollLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try { await PollOnceAsync(); }
            catch { }
            try { await Task.Delay(TimeSpan.FromSeconds(1.0), ct); }
            catch (TaskCanceledException) { break; }
        }
    }

    private async Task PollOnceAsync()
    {
        if (_userId == 0) return;
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            var staleThreshold = DateTime.UtcNow.AddSeconds(-90);
            var stale = await db.MatchAcks.Where(a => a.CreatedAt < staleThreshold).ToListAsync();
            if (stale.Count > 0)
            {
                db.MatchAcks.RemoveRange(stale);
                await db.SaveChangesAsync();
            }
        }
        catch {  }

        var myPending = await db.MatchAcks
            .AsNoTracking()
            .Where(a => a.UserId == _userId && a.Status == MatchAckStatus.Pending)
            .ToListAsync();

        foreach (var ack in myPending)
        {
            if (_seenIncoming.Add(ack.PendingMatchId))
            {
                var args = await BuildIncomingAsync(db, ack.PendingMatchId, ack.ProposalJson);
                if (args != null) IncomingProposal?.Invoke(this, args);
            }
        }

        var myAllAcks = await db.MatchAcks
            .AsNoTracking()
            .Where(a => a.UserId == _userId)
            .Select(a => a.PendingMatchId)
            .Distinct()
            .ToListAsync();

        foreach (var pid in myAllAcks)
        {
            if (_seenConfirmed.Contains(pid) || _seenCancelled.Contains(pid)) continue;

            var allAcks = await db.MatchAcks
                .AsNoTracking()
                .Where(a => a.PendingMatchId == pid)
                .ToListAsync();

            int total = allAcks.Count;
            int accepted = allAcks.Count(a => a.Status == MatchAckStatus.Accepted);
            int declined = allAcks.Count(a => a.Status == MatchAckStatus.Declined);

            if (declined > 0)
            {
                _seenCancelled.Add(pid);
                ProposalCancelled?.Invoke(this, pid);
                continue;
            }

            if (accepted == total && total > 0)
            {
                _seenConfirmed.Add(pid);
                var snapshot = allAcks.First().ProposalJson;
                var args = await BuildIncomingAsync(db, pid, snapshot);
                if (args != null) ProposalConfirmed?.Invoke(this, args);
                continue;
            }

            // Просто прогресс
            if (!_lastAcceptedCount.TryGetValue(pid, out var prev) || prev != accepted)
            {
                _lastAcceptedCount[pid] = accepted;
                ProgressChanged?.Invoke(this, new ProposalProgressEventArgs
                {
                    PendingMatchId = pid,
                    Accepted = accepted,
                    Total = total
                });
            }
        }

        var liveSet = myAllAcks.ToHashSet();
        foreach (var pid in _seenIncoming.ToList())
        {
            if (_seenConfirmed.Contains(pid) || _seenCancelled.Contains(pid)) continue;
            if (!liveSet.Contains(pid))
            {
                _seenCancelled.Add(pid);
                ProposalCancelled?.Invoke(this, pid);
            }
        }
    }

    private async Task<IncomingProposalEventArgs?> BuildIncomingAsync(AppDbContext db, int pendingMatchId, string proposalJson)
    {
        ProposalSnapshot? snap;
        try { snap = JsonSerializer.Deserialize<ProposalSnapshot>(proposalJson); }
        catch { return null; }
        if (snap == null) return null;

        if (snap.RealUserIds == null || snap.RealUserIds.Count == 0)
        {
            snap.RealUserIds = await db.MatchAcks.AsNoTracking()
                .Where(a => a.PendingMatchId == pendingMatchId && !a.IsBot)
                .Select(a => a.UserId)
                .Distinct()
                .ToListAsync();
        }

        var allIds = snap.Team1.Concat(snap.Team2).ToList();
        var users = await db.Users.AsNoTracking()
            .Where(u => allIds.Contains(u.Id))
            .ToListAsync();
        User Get(int id) => users.FirstOrDefault(u => u.Id == id)!;

        return new IncomingProposalEventArgs
        {
            PendingMatchId = pendingMatchId,
            GameType = snap.GameType,
            Team1 = snap.Team1.Select(Get).Where(u => u != null).ToList(),
            Team2 = snap.Team2.Select(Get).Where(u => u != null).ToList(),
            RealUserIds = snap.RealUserIds?.Count > 0 ? snap.RealUserIds.ToList() : new List<int>(),
            OwnerToken = snap.OwnerToken
        };
    }

    public async Task PublishProposalAsync(ProposalSnapshot snapshot, IReadOnlyCollection<int> botUserIds)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var json = JsonSerializer.Serialize(snapshot);
        var allIds = snapshot.Team1.Concat(snapshot.Team2).Distinct().ToList();
        var now = DateTime.UtcNow;
        foreach (var uid in allIds)
        {
            db.MatchAcks.Add(new MatchAck
            {
                PendingMatchId = snapshot.PendingMatchId,
                UserId = uid,
                IsBot = botUserIds.Contains(uid),
                Status = MatchAckStatus.Pending,
                CreatedAt = now,
                ProposalJson = json
            });
        }
        await db.SaveChangesAsync();
    }

    public async Task AcceptAsync(int pendingMatchId, int userId)
        => await SetStatusAsync(pendingMatchId, userId, MatchAckStatus.Accepted);

    public async Task DeclineAsync(int pendingMatchId, int userId)
        => await SetStatusAsync(pendingMatchId, userId, MatchAckStatus.Declined);

    private async Task SetStatusAsync(int pid, int uid, MatchAckStatus status)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var ack = await db.MatchAcks.FirstOrDefaultAsync(a => a.PendingMatchId == pid && a.UserId == uid);
        if (ack == null) return;
        ack.Status = status;
        ack.RespondedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task CleanupAsync(int pendingMatchId)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var rows = await db.MatchAcks.Where(a => a.PendingMatchId == pendingMatchId).ToListAsync();
        db.MatchAcks.RemoveRange(rows);
        await db.SaveChangesAsync();
    }
}
