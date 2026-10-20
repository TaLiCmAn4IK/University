#include <iostream>
using namespace std;

// Прототипы функций
void inputMatrix(int**& matrix, int rows, int cols);
void checkNegativeInRows(int** matrix, int rows, int cols, int*& rowNums, int& count);
void halveColumn(int** matrix, int rows, int colNum);
void deleteMatrix(int** matrix, int rows);

int main() {
    setlocale(LC_CTYPE, "Russian");

    int rows, cols;
    cout << "Введите количество строк матрицы: ";
    cin >> rows;
    cout << "Введите количество столбцов матрицы: ";
    cin >> cols;

    // Создание динамического двумерного массива
    int** matrix = new int* [rows];
    for (int i = 0; i < rows; i++) {
        matrix[i] = new int[cols];
    }

    // Ввод элементов матрицы
    inputMatrix(matrix, rows, cols);

    // Проверка наличия отрицательных элементов в строках
    int* rowNums = new int[rows]; // Массив для хранения номеров строк с отрицательными элементами
    int count = 0; // Счетчик строк с отрицательными элементами
    checkNegativeInRows(matrix, rows, cols, rowNums, count);

    if (count > 0) {
        cout << "Существуют строки с отрицательными элементами: ";
        for (int i = 0; i < count; i++) {
            cout << rowNums[i] << " ";
            // Уменьшение элементов столбца с найденным номером вдвое
            halveColumn(matrix, rows, rowNums[i]);
        }
        cout << endl;


        cout << "Матрица после уменьшения элементов соответствующих столбцов вдвое:" << endl;
        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cols; j++) {
                cout << matrix[i][j] << "\t";
            }
            cout << endl;
        }
    }
    else {
        cout << "Нет строк с отрицательными элементами." << endl;
    }

    // Освобождение памяти
    delete[] rowNums;
    deleteMatrix(matrix, rows);

    return 0;
}

// Функция для ввода элементов матрицы
void inputMatrix(int**& matrix, int rows, int cols) {
    cout << "Введите элементы матрицы:" << endl;
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            cout << "matrix[" << i << "][" << j << "]: ";
            cin >> matrix[i][j];
        }
    }
}

// Функция для проверки наличия отрицательных элементов в строках
void checkNegativeInRows(int** matrix, int rows, int cols, int*& rowNums, int& count) {
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            if (matrix[i][j] < 0) {
                rowNums[count] = i; // Запоминаем номер строки
                count++; // Увеличиваем счетчик
                break; // Переходим к следующей строке
            }
        }
    }
}

// Функция для уменьшения элементов столбца вдвое
void halveColumn(int** matrix, int rows, int colNum) {
    for (int i = 0; i < rows; i++) {
        matrix[i][colNum] /= 2; // Делим элементы столбца на 2
    }
}

// Функция для освобождения памяти
void deleteMatrix(int** matrix, int rows) {
    for (int i = 0; i < rows; i++) {
        delete[] matrix[i];
    }
    delete[] matrix;
}