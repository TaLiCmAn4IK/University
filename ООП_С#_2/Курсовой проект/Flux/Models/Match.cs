using System;
using System.Collections.Generic;

namespace Flux.Models;

public enum GameType
{
    CS2 = 0,
    Chess = 1
}

public class Match
{
    public int Id { get; set; }
    public GameType GameType { get; set; }
    public int? WinnerId { get; set; }
    public bool IsDraw { get; set; }
    public DateTime MatchDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Для CS2: id окна готовности (MatchAck), чтобы второй клиент нашёл тот же матч, что создал капитан.
    /// </summary>
    public int? SourcePendingMatchId { get; set; }

    /// <summary>
    /// JSON blob containing game-specific details.
    /// For CS2: { "Map": "Mirage", "Server": "EU East", "ScoreTeam1": 16, "ScoreTeam2": 12 }
    /// For Chess: { "Moves": "e4 e5 Nf3 ...", "Result": "1-0", "EndReason": "Checkmate" }
    /// </summary>
    public string ResultDetails { get; set; } = "{}";

    public ICollection<MatchPlayer> Players { get; set; } = new List<MatchPlayer>();
}
