using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flux.Services.Chess;

public enum GameResult { Ongoing, WhiteWins, BlackWins, Draw }
public enum EndReason { None, Checkmate, Stalemate, Agreement, Resignation, InsufficientMaterial }

public class ChessBoard
{
    private readonly Piece?[,] _squares = new Piece?[8, 8];
    public PieceColor SideToMove { get; private set; } = PieceColor.White;
    public Square? EnPassantTarget { get; private set; }
    public List<Move> History { get; } = new();
    public GameResult Result { get; private set; } = GameResult.Ongoing;
    public EndReason EndReason { get; private set; } = EndReason.None;

    // Индексатор для доступа к фигуре по объекту Square (клетка)
    public Piece? this[Square s] => _squares[s.File, s.Rank];
    // Индексатор для доступа к фигуре по координатам (file, rank)
    public Piece? this[int file, int rank] => _squares[file, rank];

    public ChessBoard() { SetupInitial(); }

    private void SetupInitial()
    {
        for (int f = 0; f < 8; f++)
        {
            _squares[f, 1] = new Piece(PieceType.Pawn, PieceColor.White);
            _squares[f, 6] = new Piece(PieceType.Pawn, PieceColor.Black);
        }
        PieceType[] back = { PieceType.Rook, PieceType.Knight, PieceType.Bishop, PieceType.Queen,
                             PieceType.King, PieceType.Bishop, PieceType.Knight, PieceType.Rook };
        for (int f = 0; f < 8; f++)
        {
            _squares[f, 0] = new Piece(back[f], PieceColor.White);
            _squares[f, 7] = new Piece(back[f], PieceColor.Black);
        }
    }

    public ChessBoard Clone()
    {
        var b = new ChessBoard();
        for (int f = 0; f < 8; f++)
            for (int r = 0; r < 8; r++)
                b._squares[f, r] = _squares[f, r]?.Clone();
        b.SideToMove = SideToMove;
        b.EnPassantTarget = EnPassantTarget;
        b.History.AddRange(History);
        b.Result = Result;
        b.EndReason = EndReason;
        return b;
    }

    private IEnumerable<Move> GeneratePseudoLegal(PieceColor color)
    {
        for (int f = 0; f < 8; f++)
        {
            for (int r = 0; r < 8; r++)
            {
                var p = _squares[f, r];
                if (p == null || p.Color != color) continue;
                var sq = new Square(f, r);
                foreach (var m in MovesForPiece(sq, p))
                    yield return m;
            }
        }
    }

    private IEnumerable<Move> MovesForPiece(Square from, Piece p)
    {
        return p.Type switch
        {
            PieceType.Pawn   => PawnMoves(from, p),
            PieceType.Knight => JumpMoves(from, p, KnightDeltas),
            PieceType.Bishop => SlideMoves(from, p, BishopDirs),
            PieceType.Rook   => SlideMoves(from, p, RookDirs),
            PieceType.Queen  => SlideMoves(from, p, BishopDirs.Concat(RookDirs).ToArray()),
            PieceType.King   => KingMoves(from, p),
            _ => Enumerable.Empty<Move>()
        };
    }

    private static readonly (int df, int dr)[] KnightDeltas =
        { (1,2),(2,1),(-1,2),(-2,1),(1,-2),(2,-1),(-1,-2),(-2,-1) };
    private static readonly (int df, int dr)[] BishopDirs = { (1,1),(1,-1),(-1,1),(-1,-1) };
    private static readonly (int df, int dr)[] RookDirs   = { (1,0),(-1,0),(0,1),(0,-1) };
    private static readonly (int df, int dr)[] KingDeltas =
        { (1,0),(-1,0),(0,1),(0,-1),(1,1),(1,-1),(-1,1),(-1,-1) };

    private IEnumerable<Move> JumpMoves(Square from, Piece p, (int df, int dr)[] deltas)
    {
        foreach (var (df, dr) in deltas)
        {
            var to = new Square(from.File + df, from.Rank + dr);
            if (!to.IsOnBoard) continue;
            var target = _squares[to.File, to.Rank];
            if (target == null || target.Color != p.Color)
                yield return new Move(from, to);
        }
    }

    private IEnumerable<Move> SlideMoves(Square from, Piece p, (int df, int dr)[] dirs)
    {
        foreach (var (df, dr) in dirs)
        {
            for (int i = 1; i < 8; i++)
            {
                var to = new Square(from.File + df * i, from.Rank + dr * i);
                if (!to.IsOnBoard) break;
                var target = _squares[to.File, to.Rank];
                if (target == null) { yield return new Move(from, to); continue; }
                if (target.Color != p.Color) yield return new Move(from, to);
                break;
            }
        }
    }

    private IEnumerable<Move> PawnMoves(Square from, Piece p)
    {
        int dir = p.Color == PieceColor.White ? 1 : -1;
        int startRank = p.Color == PieceColor.White ? 1 : 6;
        int promoRank = p.Color == PieceColor.White ? 7 : 0;

        var one = new Square(from.File, from.Rank + dir);
        if (one.IsOnBoard && _squares[one.File, one.Rank] == null)
        {
            if (one.Rank == promoRank)
            {
                foreach (var promo in new[] { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight })
                    yield return new Move(from, one, MoveKind.Promotion, promo);
            }
            else
            {
                yield return new Move(from, one);
                if (from.Rank == startRank)
                {
                    var two = new Square(from.File, from.Rank + 2 * dir);
                    if (_squares[two.File, two.Rank] == null)
                        yield return new Move(from, two, MoveKind.DoublePawnPush);
                }
            }
        }
        foreach (var df in new[] { -1, 1 })
        {
            var cap = new Square(from.File + df, from.Rank + dir);
            if (!cap.IsOnBoard) continue;
            var target = _squares[cap.File, cap.Rank];
            if (target != null && target.Color != p.Color)
            {
                if (cap.Rank == promoRank)
                {
                    foreach (var promo in new[] { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight })
                        yield return new Move(from, cap, MoveKind.Promotion, promo);
                }
                else
                    yield return new Move(from, cap);
            }
            else if (EnPassantTarget.HasValue && cap == EnPassantTarget.Value)
            {
                yield return new Move(from, cap, MoveKind.EnPassant);
            }
        }
    }

    private IEnumerable<Move> KingMoves(Square from, Piece p)
    {
        foreach (var (df, dr) in KingDeltas)
        {
            var to = new Square(from.File + df, from.Rank + dr);
            if (!to.IsOnBoard) continue;
            var target = _squares[to.File, to.Rank];
            if (target == null || target.Color != p.Color)
                yield return new Move(from, to);
        }

        // Рокировка
        if (!p.HasMoved && !IsInCheck(p.Color))
        {
            int rank = p.Color == PieceColor.White ? 0 : 7;
            // Короткая 
            var rookK = _squares[7, rank];
            if (rookK?.Type == PieceType.Rook && !rookK.HasMoved
                && _squares[5, rank] == null && _squares[6, rank] == null
                && !IsSquareAttacked(new Square(5, rank), Opponent(p.Color))
                && !IsSquareAttacked(new Square(6, rank), Opponent(p.Color)))
            {
                yield return new Move(from, new Square(6, rank), MoveKind.CastleKingside);
            }
            // Длинная 
            var rookQ = _squares[0, rank];
            if (rookQ?.Type == PieceType.Rook && !rookQ.HasMoved
                && _squares[1, rank] == null && _squares[2, rank] == null && _squares[3, rank] == null
                && !IsSquareAttacked(new Square(3, rank), Opponent(p.Color))
                && !IsSquareAttacked(new Square(2, rank), Opponent(p.Color)))
            {
                yield return new Move(from, new Square(2, rank), MoveKind.CastleQueenside);
            }
        }
    }

    private static PieceColor Opponent(PieceColor c) => c == PieceColor.White ? PieceColor.Black : PieceColor.White;

    /// <summary>Whether a given square is attacked by any piece of attackerColor.</summary>
    public bool IsSquareAttacked(Square sq, PieceColor attackerColor)
    {
        int dir = attackerColor == PieceColor.White ? 1 : -1;
        foreach (var df in new[] { -1, 1 })
        {
            var src = new Square(sq.File + df, sq.Rank - dir);
            if (src.IsOnBoard)
            {
                var pp = _squares[src.File, src.Rank];
                if (pp?.Type == PieceType.Pawn && pp.Color == attackerColor) return true;
            }
        }
        foreach (var (df, dr) in KnightDeltas)
        {
            var s = new Square(sq.File + df, sq.Rank + dr);
            if (!s.IsOnBoard) continue;
            var pp = _squares[s.File, s.Rank];
            if (pp?.Type == PieceType.Knight && pp.Color == attackerColor) return true;
        }
        foreach (var (df, dr) in KingDeltas)
        {
            var s = new Square(sq.File + df, sq.Rank + dr);
            if (!s.IsOnBoard) continue;
            var pp = _squares[s.File, s.Rank];
            if (pp?.Type == PieceType.King && pp.Color == attackerColor) return true;
        }
        foreach (var (df, dr) in BishopDirs)
        {
            for (int i = 1; i < 8; i++)
            {
                var s = new Square(sq.File + df * i, sq.Rank + dr * i);
                if (!s.IsOnBoard) break;
                var pp = _squares[s.File, s.Rank];
                if (pp == null) continue;
                if (pp.Color == attackerColor && (pp.Type == PieceType.Bishop || pp.Type == PieceType.Queen)) return true;
                break;
            }
        }
        foreach (var (df, dr) in RookDirs)
        {
            for (int i = 1; i < 8; i++)
            {
                var s = new Square(sq.File + df * i, sq.Rank + dr * i);
                if (!s.IsOnBoard) break;
                var pp = _squares[s.File, s.Rank];
                if (pp == null) continue;
                if (pp.Color == attackerColor && (pp.Type == PieceType.Rook || pp.Type == PieceType.Queen)) return true;
                break;
            }
        }
        return false;
    }

    private Square FindKing(PieceColor color)
    {
        for (int f = 0; f < 8; f++)
            for (int r = 0; r < 8; r++)
            {
                var p = _squares[f, r];
                if (p?.Type == PieceType.King && p.Color == color) return new Square(f, r);
            }
        return new Square(-1, -1);
    }

    public bool IsInCheck(PieceColor color)
    {
        var k = FindKing(color);
        if (!k.IsOnBoard) return false;
        return IsSquareAttacked(k, Opponent(color));
    }

    public List<Move> GetLegalMoves()
    {
        var result = new List<Move>();
        foreach (var m in GeneratePseudoLegal(SideToMove))
        {
            var clone = this.Clone();
            clone.ApplyMoveInternal(m, false);
            if (!clone.IsInCheck(SideToMove))
                result.Add(m);
        }
        return result;
    }

    public List<Move> GetLegalMovesFrom(Square from)
        => GetLegalMoves().Where(m => m.From == from).ToList();

    public Move? TryMakeMove(Square from, Square to, PieceType promotion = PieceType.Queen)
        => TryMakeMove(from, to, promotion, classify: true);

    internal Move? TryMakeMove(Square from, Square to, PieceType promotion, bool classify)
    {
        var legal = GetLegalMoves().FirstOrDefault(m =>
            m.From == from && m.To == to &&
            (m.Kind != MoveKind.Promotion || m.PromotionTo == promotion));
        if (legal == null) return null;

        var moverColor = _squares[from.File, from.Rank]!.Color;
        if (classify)
        {
            legal.Quality = MoveClassifier.Classify(this, legal);
        }
        legal.MovedBy = moverColor;

        ApplyMoveInternal(legal, true);
        UpdateStateAfterMove(legal);
        return legal;
    }

    private void ApplyMoveInternal(Move m, bool commit)
    {
        var piece = _squares[m.From.File, m.From.Rank]!;
        m.Captured = _squares[m.To.File, m.To.Rank];

        _squares[m.To.File, m.To.Rank] = piece;
        _squares[m.From.File, m.From.Rank] = null;

        switch (m.Kind)
        {
            case MoveKind.EnPassant:
            {
                int capRank = piece.Color == PieceColor.White ? m.To.Rank - 1 : m.To.Rank + 1;
                m.Captured = _squares[m.To.File, capRank];
                _squares[m.To.File, capRank] = null;
                break;
            }
            case MoveKind.CastleKingside:
            {
                int rank = m.From.Rank;
                _squares[5, rank] = _squares[7, rank];
                _squares[7, rank] = null;
                if (_squares[5, rank] != null) _squares[5, rank]!.HasMoved = true;
                break;
            }
            case MoveKind.CastleQueenside:
            {
                int rank = m.From.Rank;
                _squares[3, rank] = _squares[0, rank];
                _squares[0, rank] = null;
                if (_squares[3, rank] != null) _squares[3, rank]!.HasMoved = true;
                break;
            }
            case MoveKind.Promotion:
            {
                _squares[m.To.File, m.To.Rank] = new Piece(m.PromotionTo, piece.Color) { HasMoved = true };
                break;
            }
        }

        piece.HasMoved = true;

        EnPassantTarget = null;
        if (m.Kind == MoveKind.DoublePawnPush)
        {
            int mid = (m.From.Rank + m.To.Rank) / 2;
            EnPassantTarget = new Square(m.From.File, mid);
        }

        SideToMove = Opponent(SideToMove);

        if (commit) History.Add(m);
    }

    private void UpdateStateAfterMove(Move m)
    {
        m.IsCheck = IsInCheck(SideToMove);
        var legal = GetLegalMoves();
        if (legal.Count == 0)
        {
            if (m.IsCheck)
            {
                m.IsCheckmate = true;
                Result = SideToMove == PieceColor.White ? GameResult.BlackWins : GameResult.WhiteWins;
                EndReason = EndReason.Checkmate;
            }
            else
            {
                Result = GameResult.Draw;
                EndReason = EndReason.Stalemate;
            }
        }
        else if (HasInsufficientMaterial())
        {
            Result = GameResult.Draw;
            EndReason = EndReason.InsufficientMaterial;
        }
    }

    public void AgreeDraw()
    {
        Result = GameResult.Draw;
        EndReason = EndReason.Agreement;
    }

    public void Resign(PieceColor resigner)
    {
        Result = resigner == PieceColor.White ? GameResult.BlackWins : GameResult.WhiteWins;
        EndReason = EndReason.Resignation;
    }

    private bool HasInsufficientMaterial()
    {
        var pieces = new List<Piece>();
        for (int f = 0; f < 8; f++)
            for (int r = 0; r < 8; r++)
                if (_squares[f, r] != null) pieces.Add(_squares[f, r]!);
        if (pieces.Count == 2) return true; 
        if (pieces.Count == 3 && pieces.Any(p => p.Type == PieceType.Bishop || p.Type == PieceType.Knight))
            return true;
        return false;
    }

    public string ToFen()
    {
        var sb = new StringBuilder();
        for (int r = 7; r >= 0; r--)
        {
            int empty = 0;
            for (int f = 0; f < 8; f++)
            {
                var p = _squares[f, r];
                if (p == null) { empty++; continue; }
                if (empty > 0) { sb.Append(empty); empty = 0; }
                char c = p.Type switch
                {
                    PieceType.Pawn => 'p', PieceType.Knight => 'n', PieceType.Bishop => 'b',
                    PieceType.Rook => 'r', PieceType.Queen => 'q', PieceType.King => 'k',
                    _ => '?'
                };
                sb.Append(p.Color == PieceColor.White ? char.ToUpper(c) : c);
            }
            if (empty > 0) sb.Append(empty);
            if (r > 0) sb.Append('/');
        }
        sb.Append(' ');
        sb.Append(SideToMove == PieceColor.White ? 'w' : 'b');
        return sb.ToString();
    }
}
