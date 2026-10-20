using System.Collections.Generic;
using System.Linq;

namespace Flux.Services.Chess;

public enum MoveQuality { Normal, Good, Brilliant, Inaccuracy, Mistake, Blunder }

public static class MoveClassifier
{
    private static readonly Dictionary<PieceType, int> PieceValue = new()
    {
        { PieceType.Pawn, 1 }, { PieceType.Knight, 3 }, { PieceType.Bishop, 3 },
        { PieceType.Rook, 5 }, { PieceType.Queen, 9 },  { PieceType.King, 0 }
    };

    public static MoveQuality Classify(ChessBoard boardBefore, Move move)
    {
        int gained = 0;
        if (move.Kind == MoveKind.EnPassant) gained = 1;
        else if (boardBefore[move.To] is { } captured)
            gained = PieceValue.GetValueOrDefault(captured.Type);

        int bestAlternative = 0;
        foreach (var m in boardBefore.GetLegalMoves())
        {
            int val = 0;
            if (m.Kind == MoveKind.EnPassant) val = 1;
            else if (boardBefore[m.To] is { } t)
                val = PieceValue.GetValueOrDefault(t.Type);
            if (val > bestAlternative) bestAlternative = val;
        }

        var mover = boardBefore[move.From]!;
        bool pieceHangs = IsPieceHangingAfter(boardBefore, move, mover);

        if (move.IsCheckmate) return MoveQuality.Brilliant;
        if (gained >= 5 && bestAlternative <= gained) return MoveQuality.Brilliant;
        if (gained >= 3) return MoveQuality.Good;
        if (gained >= 1 && bestAlternative <= gained + 1) return MoveQuality.Good;

        if (pieceHangs && PieceValue.GetValueOrDefault(mover.Type) >= 5) return MoveQuality.Blunder;
        if (pieceHangs && PieceValue.GetValueOrDefault(mover.Type) >= 3) return MoveQuality.Mistake;
        if (pieceHangs) return MoveQuality.Inaccuracy;

        if (bestAlternative >= gained + 5) return MoveQuality.Blunder;
        if (bestAlternative >= gained + 3) return MoveQuality.Mistake;
        if (bestAlternative >= gained + 2) return MoveQuality.Inaccuracy;

        return MoveQuality.Normal;
    }

    private static bool IsPieceHangingAfter(ChessBoard before, Move move, Piece mover)
    {
        var clone = before.Clone();
        var applied = clone.TryMakeMove(move.From, move.To,
            move.Kind == MoveKind.Promotion ? move.PromotionTo : PieceType.Queen,
            classify: false);
        if (applied == null) return false;

        var oppColor = mover.Color == PieceColor.White ? PieceColor.Black : PieceColor.White;
        bool attacked = clone.IsSquareAttacked(move.To, oppColor);
        if (!attacked) return false;
        bool defended = clone.IsSquareAttacked(move.To, mover.Color);
        return !defended;
    }

    public static MoveQualityCounts Summarize(IEnumerable<MoveQuality> qualities)
    {
        var c = new MoveQualityCounts();
        foreach (var q in qualities)
        {
            switch (q)
            {
                case MoveQuality.Good:        c.Good++; break;
                case MoveQuality.Brilliant:   c.Brilliant++; break;
                case MoveQuality.Inaccuracy:  c.Inaccuracies++; break;
                case MoveQuality.Mistake:     c.Mistakes++; break;
                case MoveQuality.Blunder:     c.Blunders++; break;
            }
        }
        return c;
    }
}

public class MoveQualityCounts
{
    public int Good { get; set; }
    public int Brilliant { get; set; }
    public int Inaccuracies { get; set; }
    public int Mistakes { get; set; }
    public int Blunders { get; set; }
}
