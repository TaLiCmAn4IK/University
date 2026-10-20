const sudokuBoard = document.getElementById("sudoku-board");
const generateBoardButton = document.getElementById("generate-board");
const checkBoardButton = document.getElementById("check-board");
const generateSolvedButton = document.getElementById("generate-solved");

const solvedBoardData = [
    [8, 2, 7, 1, 5, 4, 3, 9, 6],
    [9, 6, 5, 3, 2, 7, 1, 4, 8],
    [3, 4, 1, 6, 8, 9, 7, 5, 2],
    [5, 9, 3, 4, 6, 8, 2, 7, 1],
    [4, 7, 2, 5, 1, 3, 6, 8, 9],
    [6, 1, 8, 9, 7, 2, 4, 3, 5],
    [7, 8, 6, 2, 3, 5, 9, 1, 4],
    [1, 5, 4, 7, 9, 6, 8, 2, 3],
    [2, 3, 9, 8, 4, 1, 5, 6, 7]
];

function generateEmptyBoard() {
    sudokuBoard.classList.remove('board-correct');
    sudokuBoard.innerHTML = "";
    let board = [];
    for (let row = 0; row < 9; row++) {
        const rowArr = [];
        for (let col = 0; col < 9; col++) {
            const cell = document.createElement("input");
            cell.type = "number";
            cell.min = "1";
            cell.max = "9";
            cell.addEventListener("input", () => handleInputChange(cell));
            sudokuBoard.appendChild(cell);
            rowArr.push(cell);
        }
        board.push(rowArr);
    }
    return board;
}

let board = generateEmptyBoard();

function handleInputChange(cell) {
    const value = parseInt(cell.value);
    if (isNaN(value) || value < 1 || value > 9) {
        cell.value = "";
        cell.classList.add("invalid");
    } else {
        cell.classList.remove("invalid");
    }
}

function generateSolvedBoard() {
    sudokuBoard.classList.remove('board-correct');
    for (let row = 0; row < 9; row++) {
        for (let col = 0; col < 9; col++) {
            board[row][col].value = solvedBoardData[row][col];
            board[row][col].disabled = true;
            board[row][col].classList.add("correct");
        }
    }
}

function checkBoard() {
    let isValid = true;
    sudokuBoard.classList.remove('board-correct');

    const allCells = Array.from(sudokuBoard.querySelectorAll('input'));
    allCells.forEach(cell => {
        cell.classList.remove('invalid', 'correct');
    });

    for (let row = 0; row < 9; row++) {
        const seen = new Set();
        for (let col = 0; col < 9; col++) {
            const cell = board[row][col];
            const value = parseInt(cell.value);
            if (!value) continue;
            if (seen.has(value)) {
                isValid = false;
                highlightRow(row, 'invalid');
                break;
            }
            seen.add(value);
        }
    }

    for (let col = 0; col < 9; col++) {
        const seen = new Set();
        for (let row = 0; row < 9; row++) {
            const cell = board[row][col];
            const value = parseInt(cell.value);
            if (!value) continue;
            if (seen.has(value)) {
                isValid = false;
                highlightColumn(col, 'invalid');
                break;
            }
            seen.add(value);
        }
    }

    for (let boxRow = 0; boxRow < 3; boxRow++) {
        for (let boxCol = 0; boxCol < 3; boxCol++) {
            const seen = new Set();
            for (let r = 0; r < 3; r++) {
                for (let c = 0; c < 3; c++) {
                    const row = boxRow * 3 + r;
                    const col = boxCol * 3 + c;
                    const cell = board[row][col];
                    const value = parseInt(cell.value);
                    if (!value) continue;
                    if (seen.has(value)) {
                        isValid = false;
                        highlightSquare(boxRow, boxCol, 'invalid');
                        r = 3; c = 3;
                        break;
                    }
                    seen.add(value);
                }
            }
        }
    }

    if (isValid && allCells.every(c => c.value !== "")) {
        sudokuBoard.classList.add('board-correct');
    }
}

function highlightRow(row, className) {
    for (let col = 0; col < 9; col++) {
        board[row][col].classList.add(className);
    }
}


function highlightColumn(col, className) {
    for (let row = 0; row < 9; row++) {
        board[row][col].classList.add(className);
    }
}

function highlightSquare(boxRow, boxCol, className) {
    for (let r = 0; r < 3; r++) {
        for (let c = 0; c < 3; c++) {
            const row = boxRow * 3 + r;
            const col = boxCol * 3 + c;
            board[row][col].classList.add(className);
        }
    }
}

generateBoardButton.addEventListener('click', () => {
    board = generateEmptyBoard();
});

checkBoardButton.addEventListener('click', () => {
    checkBoard();
});

generateSolvedButton.addEventListener('click', () => {
    generateSolvedBoard();
});



























