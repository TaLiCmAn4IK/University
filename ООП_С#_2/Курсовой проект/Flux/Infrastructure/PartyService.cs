using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using Flux.Data;
using Flux.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.Infrastructure;

public class Party : ObservableObject
{
    private readonly List<User> _members = new();

    public IReadOnlyList<User> Members => _members;
    public int Count => _members.Count;

    public event EventHandler? Changed;

    private static bool SameAvatar(byte[]? a, byte[]? b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a == null || b == null) return false;
        return a.AsSpan().SequenceEqual(b);
    }

    public void Replace(IEnumerable<User> members)
    {
        var list = members.ToList();
        if (list.Count == _members.Count &&
            list.Zip(_members, (a, b) => a.Id == b.Id
                && a.EloCS2 == b.EloCS2
                && a.EloChess == b.EloChess
                && a.Nickname == b.Nickname
                && SameAvatar(a.AvatarBytes, b.AvatarBytes)).All(x => x))
            return;

        _members.Clear();
        _members.AddRange(list);
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(nameof(Members));
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public void Reset(User leader)
    {
        _members.Clear();
        _members.Add(leader);
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(nameof(Members));
        Changed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Попытаться добавить пользователя в лобби</summary>
    public bool TryAdd(User user, int max)
    {
        if (_members.Any(u => u.Id == user.Id)) return false;
        if (_members.Count >= max) return false;
        _members.Add(user);
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(nameof(Members));
        Changed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public bool Remove(int userId)
    {
        var existing = _members.FirstOrDefault(u => u.Id == userId);
        if (existing == null) return false;
        // Лидер (слот 0) никогда не удаляется
        if (_members[0].Id == userId) return false;
        _members.Remove(existing);
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(nameof(Members));
        Changed?.Invoke(this, EventArgs.Empty);
        return true;
    }

    public double AverageElo(GameType game)
        => _members.Count == 0 ? 1000
           : _members.Average(u => game == GameType.CS2 ? u.EloCS2 : u.EloChess);
}

public interface IPartyService : INotifyPropertyChanged
{
    Party Party { get; }
    int CurrentLeaderId { get; }

    void Start(int userId);
    void Stop();

    Task EnsureLeaderAsync(User leader);
    Task JoinAsync(int leaderId, User self);

    Task RemoveAsync(int userId);

    Task DisbandAsync();

    bool Contains(int userId);
}

public class PartyService : ObservableObject, IPartyService, IDisposable
{
    private readonly IServiceProvider _sp;
    private readonly Application? _app;
    private CancellationTokenSource? _cts;
    private int _userId;

    public Party Party { get; } = new();
    public int CurrentLeaderId { get; private set; }

    public PartyService(IServiceProvider sp)
    {
        _sp = sp;
        _app = Application.Current;
    }

    public void Start(int userId)
    {
        Stop();
        _userId = userId;
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
            try { await SyncFromDbAsync(); }
            catch { }

            try { await Task.Delay(TimeSpan.FromSeconds(1.2), ct); }
            catch (TaskCanceledException) { break; }
        }
    }

    private async Task SyncFromDbAsync()
    {
        if (_userId == 0) return;
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var myRow = await db.LobbyMembers
            .AsNoTracking()
            .Where(m => m.UserId == _userId)
            .FirstOrDefaultAsync();

        if (myRow == null)
        {
            try
            {
                db.LobbyMembers.Add(new LobbyMember
                {
                    LeaderId = _userId,
                    UserId = _userId,
                    JoinedAt = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
            }
            catch
            {
                return;
            }
            myRow = new LobbyMember { LeaderId = _userId, UserId = _userId };
        }

        var leaderId = myRow.LeaderId;

        var members = await db.LobbyMembers
            .AsNoTracking()
            .Include(m => m.User)
            .Where(m => m.LeaderId == leaderId)
            .OrderBy(m => m.UserId == m.LeaderId ? 0 : 1)
            .ThenBy(m => m.JoinedAt)
            .Select(m => m.User!)
            .ToListAsync();

        _app?.Dispatcher.Invoke(() =>
        {
            CurrentLeaderId = leaderId;
            Party.Replace(members);
        });
    }

    public async Task EnsureLeaderAsync(User leader)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var existing = await db.LobbyMembers
            .Where(m => m.UserId == leader.Id)
            .FirstOrDefaultAsync();

        if (existing != null)
        {
        }
        else
        {
            db.LobbyMembers.Add(new LobbyMember
            {
                LeaderId = leader.Id,
                UserId = leader.Id,
                JoinedAt = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        await SyncFromDbAsync();
    }

    public async Task JoinAsync(int leaderId, User self)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var others = await db.LobbyMembers.Where(m => m.UserId == self.Id).ToListAsync();
        db.LobbyMembers.RemoveRange(others);

        var asLeader = await db.LobbyMembers.Where(m => m.LeaderId == self.Id).ToListAsync();
        db.LobbyMembers.RemoveRange(asLeader);

        db.LobbyMembers.Add(new LobbyMember
        {
            LeaderId = leaderId,
            UserId = self.Id,
            JoinedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        await SyncFromDbAsync();
    }

    public async Task RemoveAsync(int userId)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var rows = await db.LobbyMembers
            .Where(m => m.UserId == userId && m.LeaderId != userId)
            .ToListAsync();
        db.LobbyMembers.RemoveRange(rows);
        await db.SaveChangesAsync();
        await SyncFromDbAsync();
    }

    public async Task DisbandAsync()
    {
        if (CurrentLeaderId == 0) return;
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var rows = await db.LobbyMembers
            .Where(m => m.LeaderId == CurrentLeaderId)
            .ToListAsync();
        db.LobbyMembers.RemoveRange(rows);
        await db.SaveChangesAsync();
        await SyncFromDbAsync();
    }

    public bool Contains(int userId) => Party.Members.Any(u => u.Id == userId);
}
