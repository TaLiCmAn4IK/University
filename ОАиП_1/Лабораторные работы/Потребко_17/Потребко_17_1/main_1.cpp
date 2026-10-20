#include <iostream>
#include <cmath>

int count(int* arr, int size) {
    int lastMinIdx = -1;
    for (int i = 0; i < size; ++i) {
        if (lastMinIdx == -1 || arr[i] < arr[lastMinIdx]) {
            lastMinIdx = i;
        }
    }
    int count = 0;
    for (int i = lastMinIdx + 1; i < size; ++i) {
        if (arr[i] < 0) {
            count++;
        }
    }
    return count;
}

bool fixMatrix(int** matrix, int rows, int cols) {
    for (int j = 0; j < cols; ++j) {
        bool hasPos = false;
        for (int i = 0; i < rows; ++i) {
            if (matrix[i][j] > 0) {
                hasPos = true;
                break;
            }
        }
        if (!hasPos) {
            for (int i = 0; i < rows; ++i) {
                if (matrix[i][j] < 0) {
                    matrix[i][j] = std::abs(matrix[i][j]);
                }
            }
            return false;
        }
    }
    return true;
}

int main() {
    int n;
    std::cin >> n;
    int* arr = new int[n];
    for (int i = 0; i < n; ++i) {
        std::cin >> arr[i];
    }
    int negCount = count(arr, n);
    std::cout << negCount << std::endl;
    delete[] arr;

    int rows, cols;
    std::cin >> rows >> cols;
    int** matrix = new int* [rows];
    for (int i = 0; i < rows; ++i) {
        matrix[i] = new int[cols];
    }
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            std::cin >> matrix[i][j];
        }
    }
    bool allPos = fixMatrix(matrix, rows, cols);
    if (allPos) {
        std::cout << "Yes" << std::endl;
    }
    else {
        std::cout << "No" << std::endl;
        for (int i = 0; i < rows; ++i) {
            for (int j = 0; j < cols; ++j) {
                std::cout << matrix[i][j] << ' ';
            }
            std::cout << std::endl;
        }
    }
    for (int i = 0; i < rows; ++i) {
        delete[] matrix[i];
    }
    delete[] matrix;
    return 0;
}