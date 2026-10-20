namespace Flux.Services.Chess;

public enum PieceColor { White, Black }

public enum PieceType { None, Pawn, Knight, Bishop, Rook, Queen, King }

public readonly struct Square
{
    public int File { get; }   // 0..7 (a..h)
    public int Rank { get; }   // 0..7 (1..8)
    public Square(int file, int rank) { File = file; Rank = rank; }
    public bool IsOnBoard => File >= 0 && File <= 7 && Rank >= 0 && Rank <= 7;
    public string Algebraic => $"{(char)('a' + File)}{Rank + 1}";
    public override string ToString() => Algebraic;
    public override bool Equals(object? obj) => obj is Square s && s.File == File && s.Rank == Rank;
    public override int GetHashCode() => File * 8 + Rank;
    public static bool operator ==(Square a, Square b) => a.Equals(b);
    public static bool operator !=(Square a, Square b) => !a.Equals(b);
}

public class Piece
{
    public PieceType Type { get; }
    public PieceColor Color { get; }
    public bool HasMoved { get; set; }

    public Piece(PieceType type, PieceColor color) { Type = type; Color = color; }

    public Piece Clone() => new(Type, Color) { HasMoved = HasMoved };

    public string Glyph => Type switch
    {
        PieceType.King   => "\u2654",
        PieceType.Queen  => "\u2655",
        PieceType.Rook   => "\u2656",
        PieceType.Bishop => "\u2657",
        PieceType.Knight => "\u2658",
        PieceType.Pawn   => "\u2659",
        _ => ""
    };
}
