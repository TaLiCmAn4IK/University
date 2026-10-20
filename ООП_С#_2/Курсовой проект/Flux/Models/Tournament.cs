using System;
using System.Collections.Generic;

namespace Flux.Models;

public enum TournamentStatus
{
    /// <summary>Идёт регистрация — пользователи могут записываться.</summary>
    Registration = 0,
    /// <summary>Завершён, есть победитель (выбран случайно).</summary>
    Finished = 1,
    /// <summary>Отменён администратором.</summary>
    Cancelled = 2
}

public class Tournament
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public GameType GameType { get; set; }
    public int MaxPlayers { get; set; }
    public TournamentStatus Status { get; set; } = TournamentStatus.Registration;
    public int CreatorId { get; set; }
    public int? WinnerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    /// <summary>Время окончания регистрации и автоматического выбора победителя.</summary>
    public DateTime EndTime { get; set; } = DateTime.UtcNow.AddMinutes(5);
    public DateTime? FinishedAt { get; set; }

    public User? Creator { get; set; }
    public User? Winner { get; set; }
    public ICollection<TournamentParticipant> Participants { get; set; } = new List<TournamentParticipant>();
}

public class TournamentParticipant
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public int UserId { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public Tournament? Tournament { get; set; }
    public User? User { get; set; }
}
