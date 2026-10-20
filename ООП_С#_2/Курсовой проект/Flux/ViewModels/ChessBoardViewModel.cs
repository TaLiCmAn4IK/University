using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Flux.Infrastructure;
using Flux.Localization;
using Flux.Models;
using Flux.Services;
using Flux.Services.Chess;
using Microsoft.Extensions.DependencyInjection;

namespace Flux.ViewModels;

public partial class ChessSquareVm : ObservableObject
{
    [ObservableProperty] private int _file;
    [ObservableProperty] private int _rank;
    [ObservableProperty] private string _glyph = string.Empty;
    [ObservableProperty] private bool _isWhitePiece;
    [ObservableProperty] private bool _isLight;
    [ObservableProperty] private bool _isSelected;
    [ObservableProperty] private bool _isLegalTarget;
    [ObservableProperty] private bool _isLastMove;
    [ObservableProperty] private bool _isInCheck;
}

public partial class ChessBoardViewModel : ViewModelBase
{
    private readonly IChessGameService _chess;
    private readonly ISessionService _session;
    private readonly INavigationService _nav;
    private readonly IServiceProvider _sp;

    private ChessBoard _board = new();
    private User _white = null!;
    private User _black = null!;
    private PieceColor _userColor;
    private Square? _selected;
    private List<Move> _legalFromSelected = new();
    private DispatcherTimer? _botTimer;
    private bool _persisted;

    public ObservableCollection<ChessSquareVm> Squares { get; } = new();
    public ObservableCollection<string> MoveHistoryDisplay { get; } = new();

    [ObservableProperty] private string _statusText = string.Empty;
    [ObservableProperty] private string _whiteName = string.Empty;
    [ObservableProperty] private string _blackName = string.Empty;
    [ObservableProperty] private int _whiteElo;
    [ObservableProperty] private int _blackElo;
    // "Top" = opponent (rendered above the board); "Bottom" = current user.
    [ObservableProperty] private string _topName = string.Empty;
    [ObservableProperty] private int _topElo;
    [ObservableProperty] private string _topSideKey = "Black"; // loc key
    [ObservableProperty] private string _bottomName = string.Empty;
    [ObservableProperty] private int _bottomElo;
    [ObservableProperty] private string _bottomSideKey = "White";
    [ObservableProperty] private bool _gameOver;
    [ObservableProperty] private string _gameOverText = string.Empty;

    public ICommand SquareClickCommand { get; }
    public ICommand OfferDrawCommand { get; }
    public ICommand ResignCommand { get; }
    public ICommand ReturnCommand { get; }

    public ChessBoardViewModel(IChessGameService chess, ISessionService session,
        INavigationService nav, IServiceProvider sp)
    {
        _chess = chess; _session = session; _nav = nav; _sp = sp;
        SquareClickCommand = new AsyncRelayCommand<ChessSquareVm>(OnSquareClickAsync!);
        OfferDrawCommand = new AsyncRelayCommand(OfferDrawAsync, () => !GameOver);
        ResignCommand = new AsyncRelayCommand(ResignAsync, () => !GameOver);
        ReturnCommand = new RelayCommand(() => _nav.NavigateRoot<MainViewModel>());

        for (int i = 0; i < 64; i++) Squares.Add(new ChessSquareVm());
    }

    public void Initialize(User playerA, User playerB)
    {
        var me = _session.CurrentUser!;
        if (Random.Shared.Next(2) == 0)
        {
            _white = me.Id == playerA.Id ? playerA : playerB;
            _black = me.Id == playerA.Id ? playerB : playerA;
        }
        else
        {
            _black = me.Id == playerA.Id ? playerA : playerB;
            _white = me.Id == playerA.Id ? playerB : playerA;
        }
        _userColor = _white.Id == me.Id ? PieceColor.White : PieceColor.Black;

        _board = new ChessBoard();
        _persisted = false;
        GameOver = false;
        GameOverText = string.Empty;
        MoveHistoryDisplay.Clear();

        WhiteName = _white.Nickname;
        BlackName = _black.Nickname;
        WhiteElo = _white.EloChess;
        BlackElo = _black.EloChess;

        // Bottom = я (текущий пользователь), Top = оппонент.
        if (_userColor == PieceColor.White)
        {
            BottomName = _white.Nickname; BottomElo = _white.EloChess;
            BottomSideKey = LanguageService.Instance["White"];
            TopName    = _black.Nickname; TopElo    = _black.EloChess;
            TopSideKey    = LanguageService.Instance["Black"];
        }
        else
        {
            BottomName = _black.Nickname; BottomElo = _black.EloChess;
            BottomSideKey = LanguageService.Instance["Black"];
            TopName    = _white.Nickname; TopElo    = _white.EloChess;
            TopSideKey    = LanguageService.Instance["White"];
        }

        UpdateStatus();
        RenderBoard();

        if (_userColor == PieceColor.Black) ScheduleBotMove();
    }

    private void RenderBoard()
    {
        // Сторона пользователя всегда внизу: если играет белыми — ранги 7..0,
        // если чёрными — переворачиваем доску: ранг 0 наверху, ранг 7 внизу.
        int idx = 0;
        if (_userColor == PieceColor.White)
        {
            for (int r = 7; r >= 0; r--)
                for (int f = 0; f < 8; f++)
                    FillCell(idx++, f, r);
        }
        else
        {
            for (int r = 0; r < 8; r++)
                for (int f = 7; f >= 0; f--)
                    FillCell(idx++, f, r);
        }
    }

    private void FillCell(int idx, int f, int r)
    {
        var sq = Squares[idx];
        sq.File = f;
        sq.Rank = r;
        sq.IsLight = (f + r) % 2 == 1;
        var piece = _board[f, r];
        sq.Glyph = piece?.Glyph ?? "";
        sq.IsWhitePiece = piece?.Color == PieceColor.White;
        sq.IsSelected = _selected.HasValue && _selected.Value.File == f && _selected.Value.Rank == r;
        sq.IsLegalTarget = _legalFromSelected.Any(m => m.To.File == f && m.To.Rank == r);
        var last = _board.History.LastOrDefault();
        sq.IsLastMove = last != null && ((last.From.File == f && last.From.Rank == r) || (last.To.File == f && last.To.Rank == r));
        sq.IsInCheck = piece?.Type == PieceType.King && _board.IsInCheck(piece.Color);
    }

    private void UpdateStatus()
    {
        if (_board.Result != GameResult.Ongoing) { StatusText = GameOverText; return; }
        var you = _userColor == PieceColor.White
            ? LanguageService.Instance["YourColorWhite"]
            : LanguageService.Instance["YourColorBlack"];
        var toMove = _board.SideToMove == PieceColor.White
            ? LanguageService.Instance["WhiteToMove"]
            : LanguageService.Instance["BlackToMove"];
        var check = _board.IsInCheck(_board.SideToMove) ? $" — {LanguageService.Instance["Check"]}" : "";
        StatusText = $"{you} · {toMove}{check}";
    }

    private async Task OnSquareClickAsync(ChessSquareVm cell)
    {
        if (GameOver) return;
        if (_board.SideToMove != _userColor) return;

        var clicked = new Square(cell.File, cell.Rank);
        if (_selected == null)
        {
            var piece = _board[clicked];
            if (piece == null || piece.Color != _userColor) return;
            _selected = clicked;
            _legalFromSelected = _board.GetLegalMovesFrom(clicked);
            RenderBoard();
            return;
        }

        var onTarget = _board[clicked];
        if (onTarget != null && onTarget.Color == _userColor)
        {
            _selected = clicked;
            _legalFromSelected = _board.GetLegalMovesFrom(clicked);
            RenderBoard();
            return;
        }

        var move = _board.TryMakeMove(_selected.Value, clicked, PieceType.Queen);
        _selected = null;
        _legalFromSelected.Clear();

        if (move != null)
        {
            MoveHistoryDisplay.Add(move.ToNotation());
            RenderBoard();
            UpdateStatus();
            if (_board.Result != GameResult.Ongoing)
            {
                await FinishAsync();
                return;
            }
            ScheduleBotMove();
        }
        else
        {
            RenderBoard();
        }
    }

    private void ScheduleBotMove()
    {
        if (_board.Result != GameResult.Ongoing) return;
        if (_board.SideToMove == _userColor) return;
        _botTimer?.Stop();
        _botTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(650 + Random.Shared.Next(600)) };
        _botTimer.Tick += async (_, __) =>
        {
            _botTimer?.Stop();
            try
            {
                await MakeBotMoveAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Chess bot move failed: " + ex);
            }
        };
        _botTimer.Start();
    }

    private async Task MakeBotMoveAsync()
    {
        if (_board.Result != GameResult.Ongoing) return;
        var legal = _board.GetLegalMoves();
        if (legal.Count == 0) { await FinishAsync(); return; }

        var scored = legal.Select(m =>
        {
            int score = 0;
            var target = _board[m.To];
            if (target != null)
                score += target.Type switch
                {
                    PieceType.Queen => 9, PieceType.Rook => 5,
                    PieceType.Bishop => 3, PieceType.Knight => 3, PieceType.Pawn => 1, _ => 0
                };
            return (m, score);
        }).OrderByDescending(x => x.score).ToList();

        Move choice = (scored[0].score > 0 && Random.Shared.NextDouble() < 0.8)
            ? scored[0].m
            : legal[Random.Shared.Next(legal.Count)];

        var applied = _board.TryMakeMove(choice.From, choice.To,
            choice.Kind == MoveKind.Promotion ? choice.PromotionTo : PieceType.Queen);
        if (applied != null) MoveHistoryDisplay.Add(applied.ToNotation());
        RenderBoard();
        UpdateStatus();
        if (_board.Result != GameResult.Ongoing) await FinishAsync();
    }

    private async Task OfferDrawAsync()
    {
        if (Random.Shared.NextDouble() < 0.3)
        {
            _board.AgreeDraw();
            await FinishAsync();
        }
        else
        {
            MessageBox.Show(LanguageService.Instance["DrawDeclined"],
                LanguageService.Instance["OfferDraw"], MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private async Task ResignAsync()
    {
        _board.Resign(_userColor);
        await FinishAsync();
    }

    private async Task FinishAsync()
    {
        if (_persisted) return;
        _persisted = true;
        GameOver = true;

        string reason = _board.EndReason switch
        {
            EndReason.Checkmate => LanguageService.Instance["Checkmate"],
            EndReason.Stalemate => LanguageService.Instance["Stalemate"],
            EndReason.Agreement => LanguageService.Instance["DrawAgreement"],
            EndReason.Resignation => LanguageService.Instance["Resign"],
            EndReason.InsufficientMaterial => LanguageService.Instance["Draw"],
            _ => ""
        };
        string outcome = _board.Result switch
        {
            GameResult.WhiteWins => (_userColor == PieceColor.White
                ? LanguageService.Instance["Victory"] : LanguageService.Instance["Defeat"]),
            GameResult.BlackWins => (_userColor == PieceColor.Black
                ? LanguageService.Instance["Victory"] : LanguageService.Instance["Defeat"]),
            _ => LanguageService.Instance["Draw"]
        };
        GameOverText = $"{outcome} — {reason}";
        UpdateStatus();

        await _chess.FinishMatchAsync(_white, _black, _board);

        var me = _session.CurrentUser!;
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<Data.AppDbContext>();
        var u = db.Users.Find(me.Id);
        if (u != null) me.EloChess = u.EloChess;

        WhiteElo = _white.EloChess;
        BlackElo = _black.EloChess;
    }
}
