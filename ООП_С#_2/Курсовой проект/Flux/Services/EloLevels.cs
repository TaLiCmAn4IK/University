namespace Flux.Services;

public record EloLevelInfo(
    int Level,
    int CurrentElo,
    int LevelMin,
    int LevelMax,
    int NextLevelAt);

public static class EloLevels
{
    private static readonly (int Level, int Min, int Max)[] Table =
    {
        (10, 2001, int.MaxValue),
        ( 9, 1751, 2000),
        ( 8, 1531, 1750),
        ( 7, 1351, 1530),
        ( 6, 1201, 1350),
        ( 5, 1051, 1200),
        ( 4,  901, 1050),
        ( 3,  751,  900),
        ( 2,  501,  750),
        ( 1,  100,  500),
    };

    public static (int Level, int Min, int Max)[] All => Table;

    public static EloLevelInfo GetInfo(int elo)
    {
        foreach (var (lvl, min, max) in Table)
        {
            if (elo >= min && elo <= max)
            {
                int nextAt = lvl == 10 ? max : max + 1;
                return new EloLevelInfo(lvl, elo, min, max, nextAt);
            }
        }
        return new EloLevelInfo(1, elo, 100, 500, 501);
    }
}
