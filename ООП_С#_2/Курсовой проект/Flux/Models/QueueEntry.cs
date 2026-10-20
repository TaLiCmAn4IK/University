using System;

namespace Flux.Models;

public class QueueEntry
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public GameType GameType { get; set; }
    public int EloAtQueue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
