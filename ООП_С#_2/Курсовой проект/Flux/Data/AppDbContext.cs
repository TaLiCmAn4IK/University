using Flux.Models;
using Microsoft.EntityFrameworkCore;

namespace Flux.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MatchPlayer> MatchPlayers => Set<MatchPlayer>();
    public DbSet<QueueEntry> QueueEntries => Set<QueueEntry>();
    public DbSet<Friendship> Friendships => Set<Friendship>();
    public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
    public DbSet<LobbyInvitation> LobbyInvitations => Set<LobbyInvitation>();
    public DbSet<LobbyMember> LobbyMembers => Set<LobbyMember>();
    public DbSet<MatchAck> MatchAcks => Set<MatchAck>();
    public DbSet<MatchmakerLock> MatchmakerLocks => Set<MatchmakerLock>();
    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<TournamentParticipant> TournamentParticipants => Set<TournamentParticipant>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<User>(e =>
        {
            e.HasIndex(u => u.Login).IsUnique();
            e.HasIndex(u => u.Nickname).IsUnique();
            e.Property(u => u.Login).IsRequired().HasMaxLength(64);
            e.Property(u => u.Nickname).IsRequired().HasMaxLength(64);
            e.Property(u => u.Email).HasMaxLength(128);
            e.Property(u => u.PasswordHash).IsRequired();
        });

        mb.Entity<Match>(e =>
        {
            e.Property(m => m.ResultDetails).HasDefaultValue("{}");
            e.HasIndex(m => m.SourcePendingMatchId);
        });

        mb.Entity<MatchPlayer>(e =>
        {
            e.HasOne(mp => mp.Match)
             .WithMany(m => m.Players)
             .HasForeignKey(mp => mp.MatchId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(mp => mp.User)
             .WithMany(u => u.MatchPlayers)
             .HasForeignKey(mp => mp.UserId)
             .OnDelete(DeleteBehavior.Restrict);

            e.Ignore(mp => mp.EloChange);
        });

        mb.Entity<QueueEntry>(e =>
        {
            e.HasOne(q => q.User)
             .WithMany(u => u.QueueEntries)
             .HasForeignKey(q => q.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<Friendship>(e =>
        {
            e.HasOne(f => f.UserA).WithMany()
             .HasForeignKey(f => f.UserAId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(f => f.UserB).WithMany()
             .HasForeignKey(f => f.UserBId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(f => new { f.UserAId, f.UserBId }).IsUnique();
        });

        mb.Entity<FriendRequest>(e =>
        {
            e.HasOne(r => r.FromUser).WithMany()
             .HasForeignKey(r => r.FromUserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(r => r.ToUser).WithMany()
             .HasForeignKey(r => r.ToUserId).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<LobbyInvitation>(e =>
        {
            e.HasOne(i => i.FromUser).WithMany()
             .HasForeignKey(i => i.FromUserId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(i => i.ToUser).WithMany()
             .HasForeignKey(i => i.ToUserId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(i => new { i.ToUserId, i.Status });
            e.HasIndex(i => new { i.FromUserId, i.Status });
        });

        mb.Entity<LobbyMember>(e =>
        {
            e.HasOne(m => m.Leader).WithMany()
             .HasForeignKey(m => m.LeaderId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(m => m.User).WithMany()
             .HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(m => m.LeaderId);
            e.HasIndex(m => m.UserId).IsUnique(); // один пользователь — максимум в одном лобби
        });

        mb.Entity<MatchAck>(e =>
        {
            e.HasOne(a => a.User).WithMany()
             .HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(a => a.PendingMatchId);
            e.HasIndex(a => new { a.UserId, a.Status });
            e.Property(a => a.ProposalJson).HasDefaultValue("{}");
        });

        mb.Entity<Tournament>(e =>
        {
            e.Property(t => t.Name).IsRequired().HasMaxLength(128);
            e.HasOne(t => t.Creator).WithMany()
             .HasForeignKey(t => t.CreatorId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.Winner).WithMany()
             .HasForeignKey(t => t.WinnerId).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<TournamentParticipant>(e =>
        {
            e.HasOne(p => p.Tournament).WithMany(t => t.Participants)
             .HasForeignKey(p => p.TournamentId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(p => p.User).WithMany()
             .HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
            e.HasIndex(p => new { p.TournamentId, p.UserId }).IsUnique();
        });
    }
}
