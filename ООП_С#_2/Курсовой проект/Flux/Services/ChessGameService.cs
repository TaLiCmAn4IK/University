using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Flux.Data;
using Flux.DTOs;
using Flux.Models;
using Flux.Services.Chess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Flux.Services;

public interface IChessGameService
{
    Task<Match> FinishMatchAsync(User white, User black, ChessBoard finalBoard);
}

public class ChessGameService : IChessGameService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IRatingService _rating;
    private readonly ILogger<ChessGameService> _logger;

    public ChessGameService(IServiceScopeFactory scopeFactory, IRatingService rating, ILogger<ChessGameService> logger)
    {
        _scopeFactory = scopeFactory;
        _rating = rating;
        _logger = logger;
    }

    public async Task<Match> FinishMatchAsync(User whiteIn, User blackIn, ChessBoard board)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var white = await db.Users.FirstAsync(u => u.Id == whiteIn.Id);
        var black = await db.Users.FirstAsync(u => u.Id == blackIn.Id);

        double whiteScore = board.Result switch
        {
            GameResult.WhiteWins => 1.0,
            GameResult.BlackWins => 0.0,
            _ => 0.5
        };
        double blackScore = 1.0 - whiteScore;

        int whiteDelta = _rating.CalculateDelta(white.EloChess, black.EloChess, whiteScore);
        int blackDelta = _rating.CalculateDelta(black.EloChess, white.EloChess, blackScore);

        var whiteQ = MoveClassifier.Summarize(
            board.History.Where(m => m.MovedBy == PieceColor.White).Select(m => m.Quality));
        var blackQ = MoveClassifier.Summarize(
            board.History.Where(m => m.MovedBy == PieceColor.Black).Select(m => m.Quality));

        var details = new ChessMatchDetails
        {
            Result = board.Result switch
            {
                GameResult.WhiteWins => "1-0",
                GameResult.BlackWins => "0-1",
                _ => "1/2-1/2"
            },
            EndReason = board.EndReason.ToString(),
            FinalFen = board.ToFen()
        };
        foreach (var m in board.History) details.Moves.Add(m.ToNotation());

        var match = new Match
        {
            GameType = GameType.Chess,
            IsDraw = board.Result == GameResult.Draw,
            WinnerId = board.Result switch
            {
                GameResult.WhiteWins => white.Id,
                GameResult.BlackWins => black.Id,
                _ => null
            },
            ResultDetails = JsonSerializer.Serialize(details)
        };
        db.Matches.Add(match);
        await db.SaveChangesAsync();

        int whiteBefore = white.EloChess, blackBefore = black.EloChess;
        white.EloChess += whiteDelta;
        black.EloChess += blackDelta;

        db.MatchPlayers.Add(new MatchPlayer
        {
            MatchId = match.Id,
            UserId = white.Id,
            Team = TeamSide.Team1,
            IsWinner = board.Result == GameResult.WhiteWins,
            MoveCount = (board.History.Count + 1) / 2,
            GoodMoves = whiteQ.Good,
            BrilliantMoves = whiteQ.Brilliant,
            Inaccuracies = whiteQ.Inaccuracies,
            Mistakes = whiteQ.Mistakes,
            Blunders = whiteQ.Blunders,
            EloBefore = whiteBefore,
            EloAfter = white.EloChess
        });
        db.MatchPlayers.Add(new MatchPlayer
        {
            MatchId = match.Id,
            UserId = black.Id,
            Team = TeamSide.Team2,
            IsWinner = board.Result == GameResult.BlackWins,
            MoveCount = board.History.Count / 2,
            GoodMoves = blackQ.Good,
            BrilliantMoves = blackQ.Brilliant,
            Inaccuracies = blackQ.Inaccuracies,
            Mistakes = blackQ.Mistakes,
            Blunders = blackQ.Blunders,
            EloBefore = blackBefore,
            EloAfter = black.EloChess
        });

        await db.SaveChangesAsync();

        whiteIn.EloChess = white.EloChess;
        blackIn.EloChess = black.EloChess;

        _logger.LogInformation("Chess match {Id} finished: {Result}", match.Id, details.Result);
        return match;
    }
}
