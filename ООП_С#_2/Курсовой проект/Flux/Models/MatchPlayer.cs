namespace Flux.Models;

public enum TeamSide
{
    None = 0,
    Team1 = 1,
    Team2 = 2
}

public class MatchPlayer
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public Match Match { get; set; } = null!;
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public TeamSide Team { get; set; }

    // Статистика CS2
    public int Kills { get; set; }
    public int Deaths { get; set; }
    public int Assists { get; set; }
    public int Headshots { get; set; }
    public double Adr { get; set; }
    public double HltvRating { get; set; }

    public bool IsWinner { get; set; }

    // Специфично для шахмат
    public int MoveCount { get; set; }
    public int GoodMoves { get; set; }
    public int BrilliantMoves { get; set; }
    public int Inaccuracies { get; set; }
    public int Mistakes { get; set; }
    public int Blunders { get; set; }

    public int EloBefore { get; set; }
    public int EloAfter { get; set; }
    public int EloChange => EloAfter - EloBefore;
}
