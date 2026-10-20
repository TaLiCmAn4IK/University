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

public class InvitationReceivedEventArgs : EventArgs
{
    public int InvitationId { get; init; }
    public int FromUserId { get; init; }
    public string FromNickname { get; init; } = string.Empty;
    public GameType GameType { get; init; }
}

public class InvitationAcceptedEventArgs : EventArgs
{
    public int InvitationId { get; init; }
    public int FromUserId { get; init; }
    public int ToUserId { get; init; }
    public string ToNickname { get; init; } = string.Empty;
}

public interface IInvitationService
{
    event EventHandler<InvitationReceivedEventArgs>? InvitationReceived;
    event EventHandler<InvitationAcceptedEventArgs>? InvitationAccepted;

    void Start(int userId);
    void Stop();

    Task<int> SendAsync(int fromUserId, int toUserId, GameType gameType);
    Task AcceptAsync(int invitationId);
    Task DeclineAsync(int invitationId);
    Task CancelAsync(int invitationId);
    Task ExpireOldAsync();
}

public class InvitationService : IInvitationService, IDisposable
{
    private readonly IServiceProvider _sp;
    private CancellationTokenSource? _cts;
    private int _userId;
    private readonly HashSet<int> _seenIncoming = new();
    private readonly HashSet<int> _seenAccepted = new();

    public event EventHandler<InvitationReceivedEventArgs>? InvitationReceived;
    public event EventHandler<InvitationAcceptedEventArgs>? InvitationAccepted;

    public InvitationService(IServiceProvider sp)
    {
        _sp = sp;
    }

    public void Start(int userId)
    {
        Stop();
        _userId = userId;
        _seenIncoming.Clear();
        _seenAccepted.Clear();
        _cts = new CancellationTokenSource();
        _ = PollLoopAsync(_cts.Token);
    }

    public void Stop()
    {
        _cts?.Cancel();
        _cts?.Dispose();
        _cts = null;
        _userId = 0;
    }

    public void Dispose() => Stop();

    private async Task PollLoopAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try { await PollOnceAsync(); }
            catch {  }

            try { await Task.Delay(TimeSpan.FromSeconds(2), ct); }
            catch (TaskCanceledException) { break; }
        }
    }

    private async Task PollOnceAsync()
    {
        if (_userId == 0) return;
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var incoming = await db.LobbyInvitations
            .Include(i => i.FromUser)
            .AsNoTracking()
            .Where(i => i.ToUserId == _userId && i.Status == InvitationStatus.Pending)
            .ToListAsync();

        foreach (var inv in incoming)
        {
            if (_seenIncoming.Add(inv.Id))
            {
                InvitationReceived?.Invoke(this, new InvitationReceivedEventArgs
                {
                    InvitationId = inv.Id,
                    FromUserId = inv.FromUserId,
                    FromNickname = inv.FromUser?.Nickname ?? "?",
                    GameType = inv.GameType
                });
            }
        }

        var acceptedReplies = await db.LobbyInvitations
            .Include(i => i.ToUser)
            .AsNoTracking()
            .Where(i => i.FromUserId == _userId && i.Status == InvitationStatus.Accepted)
            .ToListAsync();

        foreach (var inv in acceptedReplies)
        {
            if (_seenAccepted.Add(inv.Id))
            {
                InvitationAccepted?.Invoke(this, new InvitationAcceptedEventArgs
                {
                    InvitationId = inv.Id,
                    FromUserId = inv.FromUserId,
                    ToUserId = inv.ToUserId,
                    ToNickname = inv.ToUser?.Nickname ?? "?"
                });
            }
        }
    }

    public async Task<int> SendAsync(int fromUserId, int toUserId, GameType gameType)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var stale = await db.LobbyInvitations
            .Where(i => i.FromUserId == fromUserId && i.ToUserId == toUserId
                     && i.Status == InvitationStatus.Pending)
            .ToListAsync();
        foreach (var s in stale) s.Status = InvitationStatus.Cancelled;

        var inv = new LobbyInvitation
        {
            FromUserId = fromUserId,
            ToUserId = toUserId,
            GameType = gameType,
            Status = InvitationStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
        db.LobbyInvitations.Add(inv);
        await db.SaveChangesAsync();
        return inv.Id;
    }

    public async Task AcceptAsync(int invitationId)
        => await SetStatusAsync(invitationId, InvitationStatus.Accepted);

    public async Task DeclineAsync(int invitationId)
        => await SetStatusAsync(invitationId, InvitationStatus.Declined);

    public async Task CancelAsync(int invitationId)
        => await SetStatusAsync(invitationId, InvitationStatus.Cancelled);

    private async Task SetStatusAsync(int invitationId, InvitationStatus status)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var inv = await db.LobbyInvitations.FirstOrDefaultAsync(i => i.Id == invitationId);
        if (inv == null) return;
        inv.Status = status;
        inv.RespondedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    public async Task ExpireOldAsync()
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var threshold = DateTime.UtcNow.AddMinutes(-2);
        var old = await db.LobbyInvitations
            .Where(i => i.Status == InvitationStatus.Pending && i.CreatedAt < threshold)
            .ToListAsync();
        foreach (var o in old) o.Status = InvitationStatus.Expired;
        await db.SaveChangesAsync();
    }
}
