using System;

namespace Flux.Models;

public class Friendship
{
    public int Id { get; set; }
    public int UserAId { get; set; }
    public User UserA { get; set; } = null!;
    public int UserBId { get; set; }
    public User UserB { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum FriendRequestStatus
{
    Pending = 0,
    Accepted = 1,
    Declined = 2
}

public class FriendRequest
{
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public User FromUser { get; set; } = null!;
    public int ToUserId { get; set; }
    public User ToUser { get; set; } = null!;
    public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
