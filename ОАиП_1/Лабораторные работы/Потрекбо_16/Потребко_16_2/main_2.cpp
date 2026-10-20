#include <iostream>
using namespace std;

// Прототипы функций
void replaceDiagonalWithMin(int** A, int row, int col);
void findEqualRowsAndColumns(int** A, int size);
int** createMatrix(int rows, int cols);
void fillMatrix(int** matrix, int rows, int cols);
void deleteMatrix(int** matrix, int rows);
void matrixReplacement();
void matrixMatching();

int main() {
    setlocale(LC_CTYPE, "Russian");

    int choice;
    do {
        cout << "Выберите вариант работы:" << endl;
        cout << "1 - Задание 1" << endl;
        cout << "2 - Задание 2" << endl;
        cout << "3 - Выход" << endl;
        cin >> choice;

        switch (choice) {
        case 1:
            matrixReplacement();
            break;

        case 2:
            matrixMatching();
            break;

        case 3:
            break;

        default:
            cout << "Неверный ввод! Пожалуйста, попробуйте снова." << endl;
            break;
        }
    } while (choice != 3);

    return 0;
}

void matrixReplacement() {
    int** A;
    int row, col;

    cout << "Введите число строк матрицы (d <= 16): ";
    cin >> row;
    cout << "Введите число столбцов (p <= 18): ";
    cin >> col;

    // Проверка на правильность введенных размеров
    if (row > 16 || col > 18) {
        cout << "Ошибка: превышены максимальные размеры матрицы." << endl;
        return;
    }

    A = createMatrix(row, col); // Создание матрицы
    fillMatrix(A, row, col); // Заполнение матрицы

    // Выполнение замены диагонали на минимальные значения
    replaceDiagonalWithMin(A, row, col);

    // Освобождение памяти
    deleteMatrix(A, row);
}

void matrixMatching() {
    int** A;
    int row, col;

    cout << "Введите число строк и столбцов матрицы: ";
    cin >> row;

    // Проверка на правильность введенных размеров
    if (row > 16 || row < 1) {
        cout << "Ошибка: превышены максимальные размеры матрицы или некорректный ввод." << endl;
        return;
    }

    col = row; // Для квадратной матрицы

    A = createMatrix(row, col); // Создание матрицы
    fillMatrix(A, row, col); // Заполнение матрицы

    // Поиск совпадающих строк и столбцов
    findEqualRowsAndColumns(A, row);

    // Освобождение памяти
    deleteMatrix(A, row);
}

int** createMatrix(int rows, int cols) {
    int** matrix = new int* [rows];
    for (int i = 0; i < rows; i++) {
        matrix[i] = new int[cols];
    }
    return matrix;
}

void fillMatrix(int** matrix, int rows, int cols) {
    cout << "Введите элементы матрицы " << rows << "x" << cols << ":\n";
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            cout << "Введите A[" << i << "][" << j << "]: ";
            cin >> matrix[i][j];
        }
    }
}

void replaceDiagonalWithMin(int** A, int row, int col) {
    for (int i = 0; i < row && i < col; i++) {
        int minElement = A[i][0];
        // Нахождение минимального значения в строке
        for (int j = 1; j < col; j++) {
            if (A[i][j] < minElement) {
                minElement = A[i][j];
            }
        }
        // Замена элемента на главной диагонали
        A[i][i] = minElement;
    }

    // Вывод результата после замены диагонали
    cout << "Матрица после замены элементов главной диагонали:" << endl;
    for (int i = 0; i < row; i++) {
        for (int j = 0; j < col; j++) {
            cout << A[i][j] << "\t";
        }
        cout << endl;
    }
}

void findEqualRowsAndColumns(int** A, int size) {
    cout << "Индексы k, для которых k-я строка совпадает с k-м столбцом:\n";
    for (int k = 0; k < size; k++) {
        bool equal = true;
        for (int j = 0; j < size; j++) {
            if (A[k][j] != A[j][k]) {
                equal = false;
                break;
            }
        }
        if (equal) {
            cout << "Индекс строки = " << k << ", индекс столбца = " << k << endl;
        }
    }
}

void deleteMatrix(int** matrix, int rows) {
    for (int i = 0; i < rows; i++) {
        delete[] matrix[i];
    }
    delete[] matrix;
}