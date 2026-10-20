namespace Flux.Services.Chess;

public enum MoveKind { Normal, DoublePawnPush, EnPassant, CastleKingside, CastleQueenside, Promotion }

public class Move
{
    public Square From { get; }
    public Square To { get; }
    public MoveKind Kind { get; }
    public PieceType PromotionTo { get; }
    public Piece? Captured { get; set; }
    public bool IsCheck { get; set; }
    public bool IsCheckmate { get; set; }
    public MoveQuality Quality { get; set; } = MoveQuality.Normal;
    public PieceColor MovedBy { get; set; }

    public Move(Square from, Square to, MoveKind kind = MoveKind.Normal, PieceType promotion = PieceType.None)
    {
        From = from; To = to; Kind = kind; PromotionTo = promotion;
    }

    /// <summary>Возвращает a short algebraic-like notation (e.g., e2e4, e7e8Q, O-O).</summary>
    public string ToNotation()
    {
        if (Kind == MoveKind.CastleKingside) return "O-O";
        if (Kind == MoveKind.CastleQueenside) return "O-O-O";
        var promo = PromotionTo switch
        {
            PieceType.Queen => "Q",
            PieceType.Rook => "R",
            PieceType.Bishop => "B",
            PieceType.Knight => "N",
            _ => ""
        };
        var suffix = IsCheckmate ? "#" : (IsCheck ? "+" : "");
        return $"{From.Algebraic}{To.Algebraic}{promo}{suffix}";
    }
}
