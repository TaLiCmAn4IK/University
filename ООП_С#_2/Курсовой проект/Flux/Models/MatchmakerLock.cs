using System;

namespace Flux.Models;

/// <summary>
/// Эксклюзивный лок «кто сейчас формирует матчи». Только один клиент держит лок
/// в любой момент времени. Лок имеет TTL — если клиент умер, через TTL лок захватит другой.
///
/// В таблице всегда максимум одна строка с фиксированным Id=1 (singleton row).
/// </summary>
public class MatchmakerLock
{
    public int Id { get; set; }              // всегда 1
    public Guid OwnerToken { get; set; }     // уникальный токен текущего владельца
    public DateTime AcquiredAt { get; set; }
    public DateTime ExpiresAt { get; set; }  // если истекло — лок свободен
}
