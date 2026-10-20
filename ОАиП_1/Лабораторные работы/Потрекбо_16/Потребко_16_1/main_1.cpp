#include <iostream>
using namespace std;

// Функция для создания динамической матрицы
int** createMatrix(int rows, int cols) {
    int** matrix = new int* [rows];
    for (int i = 0; i < rows; i++) {
        matrix[i] = new int[cols];
    }
    return matrix;
}

// Функция для заполнения матрицы
void fillMatrix(int** matrix, int rows, int cols) {
    cout << "Введите элементы матрицы " << rows << "x" << cols << ":\n";
    for (int i = 0; i < rows; i++) {
        for (int j = 0; j < cols; j++) {
            cin >> matrix[i][j];
        }
    }
}

// Функция для поиска индексов k
void findMatchingRowsAndColumns(int** matrix, int size) {
    cout << "Индексы k, для которых k-я строка совпадает с k-м столбцом:\n";
    for (int k = 0; k < size; k++) {
        bool match = true;
        for (int j = 0; j < size; j++) {
            if (matrix[k][j] != matrix[j][k]) {
                match = false;
                break;
            }
        }
        if (match) {
            cout << "Индекс строки = " << k << ", индекс столбца = " << k << ")\n";
        }
    }
}

// Функция для освобождения памяти
void deleteMatrix(int** matrix, int rows) {
    for (int i = 0; i < rows; i++) {
        delete[] matrix[i];
    }
    delete[] matrix;
}

int main() {
    setlocale(LC_CTYPE, "Russian");
    const int size = 4; // Размер матрицы 4x4
    int** matrix = createMatrix(size, size); // Создание матрицы
    fillMatrix(matrix, size, size); // Заполнение матрицы
    findMatchingRowsAndColumns(matrix, size); // Поиск совпадений
    deleteMatrix(matrix, size); // Освобождение памяти
    return 0;
}