class Sudoku {
  constructor(initialBoard = null) {
    this.originalBoard =
      initialBoard
        ? Sudoku.cloneBoard(initialBoard)
        : Sudoku.createEmptyBoard();
    this.board = Sudoku.cloneBoard(this.originalBoard);
  }

  static createEmptyBoard() {
    return Array.from({ length: 9 }, () => Array(9).fill(0));
  }

  static cloneBoard(board) {
    return board.map(row => row.slice());
  }

  static isAllowedValue(v) {
    return v === 0 ||
      (Number.isInteger(v) &&
        v >= 1 && v <= 9);
  }

  static hasDuplicates(values) {
    const seen = new Set();
    for (const v of values) {
      if (v === 0) continue;
      if (seen.has(v)) return true;
      seen.add(v);
    }
    return false;
  }

  static squareNumber(squareRow, squareCol) {
    return squareRow * 3 + squareCol + 1;
  }

  resetBoard() {
    this.board = Sudoku.cloneBoard(this.originalBoard);
  }

  checkRow(row) {
    return !Sudoku.hasDuplicates(this.board[row]);
  }

  checkColumn(col) {
    const values = [];
    for (let r = 0; r < 9; r++)
      values.push(this.board[r][col]);
    return !Sudoku.hasDuplicates(values);
  }

  checkSquare(squareRow, squareCol) {
    const values = [];
    for (let r = squareRow * 3;
      r < squareRow * 3 + 3;
      r++) {
      for (let c = squareCol * 3;
        c < squareCol * 3 + 3;
        c++) {
        values.push(this.board[r][c]);
      }
    }
    return !Sudoku.hasDuplicates(values);
  }

  checkBoard() {
    let ok = true;

    for (let r = 0; r < 9; r++) {
      for (let c = 0; c < 9; c++) {
        const v = this.board[r][c];
        if (!Sudoku.isAllowedValue(v)) {
          console.log(
            `Ошибка: недопустимое значение в клетке (${r + 1}, ${c + 1}) = ${v}`
          );
          ok = false;
        }
      }
    }

    for (let r = 0; r < 9; r++) {
      if (!this.checkRow(r)) {
        console.log(`Ошибка: строка ${r + 1}`);
        ok = false;
      }
    }

    for (let c = 0; c < 9; c++) {
      if (!this.checkColumn(c)) {
        console.log(`Ошибка: столбец ${c + 1}`);
        ok = false;
      }
    }

    for (let sr = 0; sr < 3; sr++) {
      for (let sc = 0; sc < 3; sc++) {
        if (!this.checkSquare(sr, sc)) {
          console.log(
            `Ошибка: квадрат ${Sudoku.squareNumber(sr, sc)}`
          );
          ok = false;
        }
      }
    }

    return ok;
  }

  generateBoard() {
    const base = 3;
    const side = 9;

    const pattern = (r, c) =>
      (base * (r % base) +
        Math.floor(r / base) + c) %
      side;

    const shuffle = (arr) => {
      const a = arr.slice();
      for (let i = a.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [a[i], a[j]] = [a[j], a[i]];
      }
      return a;
    };

    const rBase = [0, 1, 2];

    const rows = shuffle(rBase)
      .flatMap(g =>
        shuffle(rBase)
          .map(r => g * base + r)
      );
    const cols = shuffle(rBase)
      .flatMap(g =>
        shuffle(rBase).map(c => g * base + c)
      );

    const nums = shuffle([1, 2, 3, 4, 5, 6, 7, 8, 9]);

    const newBoard =
      Array.from({ length: side }, (_, r) =>
        Array.from({ length: side }, (_, c) =>
          nums[pattern(rows[r], cols[c])]
        )
      );

    this.board = Sudoku.cloneBoard(newBoard);
    this.originalBoard = Sudoku.cloneBoard(newBoard);
    return this.board;
  }

  setCell(row, col, value) {
    this.board[row][col] = value;
  }

  printBoard() {
    console.table(this.board);
  }
}

const sudoku = new Sudoku();
sudoku.generateBoard();
console.log("Сгенерированное поле:");
sudoku.printBoard();
setCell(1,1,9)
console.log(
  "Проверка поля (должно быть true):",
  sudoku.checkBoard()
);


sudoku.printBoard();
console.log(
  "Проверка после внесения ошибки (должно быть false):",
  sudoku.checkBoard()
);

sudoku.resetBoard();
console.log("После resetBoard():");
sudoku.printBoard();
