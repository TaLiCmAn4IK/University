using System;

namespace Flux.Services;

public interface IRatingService
{
    double ExpectedScore(int playerRating, int opponentRating);

    int CalculateDelta(int playerRating, int opponentRating, double actualScore);

    int PerformanceModifier(bool isWinner, double playerKd, double playerAdr,
        double teamAvgKd, double teamAvgAdr);
}

public class RatingService : IRatingService
{
    public const int KFactor = 32;
    public const int MaxPerfModifier = 5;

    public double ExpectedScore(int playerRating, int opponentRating)
        => 1.0 / (1.0 + Math.Pow(10.0, (opponentRating - playerRating) / 400.0));

    public int CalculateDelta(int playerRating, int opponentRating, double actualScore)
    {
        var expected = ExpectedScore(playerRating, opponentRating);
        var delta = KFactor * (actualScore - expected);
        return (int)Math.Round(delta);
    }

    public int PerformanceModifier(bool isWinner, double playerKd, double playerAdr,
        double teamAvgKd, double teamAvgAdr)
    {
        if (teamAvgKd <= 0 || teamAvgAdr <= 0) return 0;

        double kdRel  = playerKd  / teamAvgKd;
        double adrRel = playerAdr / teamAvgAdr;
        double relative = (kdRel + adrRel) / 2.0;
        double score = Math.Clamp((relative - 1.0) / 0.25, -1.0, 1.0);

        int mod = (int)Math.Round(score * MaxPerfModifier);
        return Math.Clamp(mod, -MaxPerfModifier, MaxPerfModifier);
    }
}
