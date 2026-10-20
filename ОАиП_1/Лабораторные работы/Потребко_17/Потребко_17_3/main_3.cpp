#include <iostream>
#include <cmath>

int sum(int* arr, int size) {
    int sum = 0;
    for (int i = 0; i < size; ++i) {
        if (arr[i] % 2 == 0) {
            sum += arr[i];
        }
    }
    return sum;
}

bool no(int** matrix, int rows, int cols) {
    for (int i = 0; i < rows; ++i) {
        for (int j = 0; j < cols; ++j) {
            if (matrix[i][j] == 0) {
                return false;
            }
        }
    }
    return true;
}

int** multiply(int** matA, int** matB, int rowsA, int colsA, int colsB) {
    int** result = new int* [rowsA];
    for (int i = 0; i < rowsA; ++i) {
        result[i] = new int[colsB]();
    }
    for (int i = 0; i < rowsA; ++i) {
        for (int j = 0; j < colsB; ++j) {
            for (int k = 0; k < colsA; ++k) {
                result[i][j] += matA[i][k] * matB[k][j];
            }
        }
    }
    return result;
}

int main() {
    int arr[15];
    for (int i = 0; i < 15; ++i) {
        std::cin >> arr[i];
    }
    int sumEven = sum(arr, 15);
    std::cout << "Sum of even elements: " << sumEven << std::endl;

    int rowsA, colsA, rowsB, colsB;
    std::cin >> rowsA >> colsA >> rowsB >> colsB;
    if (colsA != rowsB) {
        std::cout << "Invalid." << std::endl;
        return 1;
    }

    int** matA = new int* [rowsA];
    int** matB = new int* [rowsB];
    for (int i = 0; i < rowsA; ++i) {
        matA[i] = new int[colsA];
    }
    for (int i = 0; i < rowsB; ++i) {
        matB[i] = new int[colsB];
    }

    for (int i = 0; i < rowsA; ++i) {
        for (int j = 0; j < colsA; ++j) {
            std::cin >> matA[i][j];
        }
    }
    for (int i = 0; i < rowsB; ++i) {
        for (int j = 0; j < colsB; ++j) {
            std::cin >> matB[i][j];
        }
    }

    if (no(matA, rowsA, colsA) && no(matB, rowsB, colsB)) {
        int** result = multiply(matA, matB, rowsA, colsA, colsB);
        for (int i = 0; i < rowsA; ++i) {
            for (int j = 0; j < colsB; ++j) {
                std::cout << result[i][j] << ' ';
            }
            std::cout << std::endl;
        }
        for (int i = 0; i < rowsA; ++i) {
            delete[] result[i];
        }
        delete[] result;
    }
    else {
        std::cout << " contains zero elements." << std::endl;
    }

    for (int i = 0; i < rowsA; ++i) {
        delete[] matA[i];
    }
    delete[] matA;

    for (int i = 0; i < rowsB; ++i) {
        delete[] matB[i];
    }
    delete[] matB;

    return 0;
}