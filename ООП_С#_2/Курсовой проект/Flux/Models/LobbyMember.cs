using System;

namespace Flux.Models;

/// <summary>
/// Запись о членстве пользователя в лобби. Хранится в БД для синхронизации
/// между двумя клиентами: после Accept приглашения сюда добавляется строка,
/// PartyService обоих клиентов поллит таблицу и видит одинаковый состав.
/// </summary>
public class LobbyMember
{
    public int Id { get; set; }
    /// <summary>Id лидера лобби (он же владелец партии).</summary>
    public int LeaderId { get; set; }
    /// <summary>Id участника лобби (может быть равен LeaderId — лидер сам себе участник со слотом 0).</summary>
    public int UserId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public User? Leader { get; set; }
    public User? User { get; set; }
}
