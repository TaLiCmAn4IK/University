using System;

namespace Flux.Models;

public enum InvitationStatus
{
    /// <summary>Отправлено, ждёт реакции получателя.</summary>
    Pending = 0,
    /// <summary>Получатель принял приглашение — лидер увидит и добавит в лобби.</summary>
    Accepted = 1,
    /// <summary>Получатель отклонил приглашение.</summary>
    Declined = 2,
    /// <summary>Истекло (не ответили в срок).</summary>
    Expired = 3,
    /// <summary>Отменено отправителем.</summary>
    Cancelled = 4
}

/// <summary>
/// Приглашение в лобби. Хранится в SQL Server — это позволяет двум независимым
/// экземплярам приложения (на одной БД) обмениваться приглашениями: отправитель
/// пишет запись Pending, получатель её опрашивает; статус меняется на Accepted/Declined,
/// отправитель видит ответ при следующем поллинге.
/// </summary>
public class LobbyInvitation
{
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
    public GameType GameType { get; set; }
    public InvitationStatus Status { get; set; } = InvitationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }

    public User? FromUser { get; set; }
    public User? ToUser { get; set; }
}
