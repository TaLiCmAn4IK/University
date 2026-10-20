using System;

namespace Flux.Models;

/// <summary>
/// Запись о подтверждении конкретного игрока для конкретного pending-матча.
/// Хранится в БД — оба клиента видят прогресс готовности.
///   PendingMatchId — общий id предложения (генерируется MatchmakingService).
///   UserId — кто должен принять.
///   Status — Pending / Accepted / Declined.
/// </summary>
public enum MatchAckStatus
{
    Pending = 0,
    Accepted = 1,
    Declined = 2
}

public class MatchAck
{
    public int Id { get; set; }
    public int PendingMatchId { get; set; }
    public int UserId { get; set; }
    public bool IsBot { get; set; }
    public MatchAckStatus Status { get; set; } = MatchAckStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
    /// <summary>JSON-снимок предложения матча (для отображения у второго клиента, см. MatchmakingService).</summary>
    public string ProposalJson { get; set; } = "{}";

    public User? User { get; set; }
}
