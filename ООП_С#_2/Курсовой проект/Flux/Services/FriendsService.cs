using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flux.Data;
using Flux.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Services;

public enum FriendRequestResult { Sent, AlreadyFriends, AlreadyPending, UserNotFound, SameUser }

public class LobbyInviteEventArgs : EventArgs
{
    public int InviteId { get; set; }
    public int FromUserId { get; set; }
    public string FromNickname { get; set; } = string.Empty;
    public int ToUserId { get; set; }
}

public class FriendRequestEventArgs : EventArgs
{
    public int RequestId { get; set; }
    public int FromUserId { get; set; }
    public string FromNickname { get; set; } = string.Empty;
    public int ToUserId { get; set; }
}

public interface IFriendsService
{
    void Online(int userId);
    void Offline(int userId);
    bool IsOnline(int userId);

    Task<IReadOnlyList<User>> GetFriendsAsync(int userId);
    Task<IReadOnlyList<FriendRequest>> GetIncomingRequestsAsync(int userId);
    Task<FriendRequestResult> SendFriendRequestAsync(int fromUserId, int toUserId);
    Task<bool> AcceptRequestAsync(int requestId);
    Task<bool> DeclineRequestAsync(int requestId);
    Task<bool> RemoveFriendAsync(int userA, int userB);

    void SendLobbyInvite(int fromUserId, string fromNickname, int toUserId);
    void AcceptLobbyInvite(int inviteId);
    void DeclineLobbyInvite(int inviteId);

    event EventHandler<FriendRequestEventArgs>? FriendRequestReceived;
    event EventHandler<int>? FriendRequestAnswered;   
    event EventHandler<LobbyInviteEventArgs>? LobbyInviteReceived;
    event EventHandler<int>? LobbyInviteAccepted;     
    event EventHandler<int>? LobbyInviteDeclined;
}

public class FriendsService : IFriendsService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentDictionary<int, byte> _online = new();
    private readonly ConcurrentDictionary<int, LobbyInviteEventArgs> _lobbyInvites = new();
    private int _nextInviteId = 1;

    public FriendsService(IServiceScopeFactory scopeFactory) { _scopeFactory = scopeFactory; }

    public void Online(int userId) => _online[userId] = 1;
    public void Offline(int userId) => _online.TryRemove(userId, out _);
    public bool IsOnline(int userId) => _online.ContainsKey(userId);

    public event EventHandler<FriendRequestEventArgs>? FriendRequestReceived;
    public event EventHandler<int>? FriendRequestAnswered;
    public event EventHandler<LobbyInviteEventArgs>? LobbyInviteReceived;
    public event EventHandler<int>? LobbyInviteAccepted;
    public event EventHandler<int>? LobbyInviteDeclined;

    public async Task<IReadOnlyList<User>> GetFriendsAsync(int userId)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var rows = await db.Friendships.AsNoTracking()
            .Include(f => f.UserA).Include(f => f.UserB)
            .Where(f => f.UserAId == userId || f.UserBId == userId)
            .ToListAsync();
        var friends = rows.Select(f => f.UserAId == userId ? f.UserB : f.UserA).ToList();
        return friends;
    }

    public async Task<IReadOnlyList<FriendRequest>> GetIncomingRequestsAsync(int userId)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await db.FriendRequests.AsNoTracking()
            .Include(r => r.FromUser)
            .Where(r => r.ToUserId == userId && r.Status == FriendRequestStatus.Pending)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<FriendRequestResult> SendFriendRequestAsync(int fromUserId, int toUserId)
    {
        if (fromUserId == toUserId) return FriendRequestResult.SameUser;
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var toUser = await db.Users.FirstOrDefaultAsync(u => u.Id == toUserId);
        if (toUser == null) return FriendRequestResult.UserNotFound;

        bool alreadyFriends = await db.Friendships.AnyAsync(f =>
            (f.UserAId == fromUserId && f.UserBId == toUserId) ||
            (f.UserAId == toUserId && f.UserBId == fromUserId));
        if (alreadyFriends) return FriendRequestResult.AlreadyFriends;

        bool pending = await db.FriendRequests.AnyAsync(r =>
            r.Status == FriendRequestStatus.Pending &&
            ((r.FromUserId == fromUserId && r.ToUserId == toUserId) ||
             (r.FromUserId == toUserId && r.ToUserId == fromUserId)));
        if (pending) return FriendRequestResult.AlreadyPending;

        var fromUser = await db.Users.FirstAsync(u => u.Id == fromUserId);
        var req = new FriendRequest
        {
            FromUserId = fromUserId,
            ToUserId = toUserId,
            Status = FriendRequestStatus.Pending
        };
        db.FriendRequests.Add(req);
        await db.SaveChangesAsync();

        FriendRequestReceived?.Invoke(this, new FriendRequestEventArgs
        {
            RequestId = req.Id,
            FromUserId = fromUserId,
            FromNickname = fromUser.Nickname,
            ToUserId = toUserId
        });
        return FriendRequestResult.Sent;
    }

    public async Task<bool> AcceptRequestAsync(int requestId)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var req = await db.FriendRequests.FirstOrDefaultAsync(r => r.Id == requestId);
        if (req == null || req.Status != FriendRequestStatus.Pending) return false;
        req.Status = FriendRequestStatus.Accepted;
        int a = System.Math.Min(req.FromUserId, req.ToUserId);
        int b = System.Math.Max(req.FromUserId, req.ToUserId);
        if (!await db.Friendships.AnyAsync(f => f.UserAId == a && f.UserBId == b))
        {
            db.Friendships.Add(new Friendship { UserAId = a, UserBId = b });
        }
        await db.SaveChangesAsync();
        FriendRequestAnswered?.Invoke(this, requestId);
        return true;
    }

    public async Task<bool> DeclineRequestAsync(int requestId)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var req = await db.FriendRequests.FirstOrDefaultAsync(r => r.Id == requestId);
        if (req == null || req.Status != FriendRequestStatus.Pending) return false;
        req.Status = FriendRequestStatus.Declined;
        await db.SaveChangesAsync();
        FriendRequestAnswered?.Invoke(this, requestId);
        return true;
    }

    public async Task<bool> RemoveFriendAsync(int userA, int userB)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var rows = await db.Friendships.Where(f =>
            (f.UserAId == userA && f.UserBId == userB) ||
            (f.UserAId == userB && f.UserBId == userA)).ToListAsync();
        if (rows.Count == 0) return false;
        db.Friendships.RemoveRange(rows);
        await db.SaveChangesAsync();
        return true;
    }

    public void SendLobbyInvite(int fromUserId, string fromNickname, int toUserId)
    {
        if (!IsOnline(toUserId)) return;
        var inv = new LobbyInviteEventArgs
        {
            InviteId = System.Threading.Interlocked.Increment(ref _nextInviteId),
            FromUserId = fromUserId,
            FromNickname = fromNickname,
            ToUserId = toUserId
        };
        _lobbyInvites[inv.InviteId] = inv;
        LobbyInviteReceived?.Invoke(this, inv);
    }

    public void AcceptLobbyInvite(int inviteId)
    {
        if (_lobbyInvites.TryRemove(inviteId, out _))
            LobbyInviteAccepted?.Invoke(this, inviteId);
    }
    public void DeclineLobbyInvite(int inviteId)
    {
        if (_lobbyInvites.TryRemove(inviteId, out _))
            LobbyInviteDeclined?.Invoke(this, inviteId);
    }
}
