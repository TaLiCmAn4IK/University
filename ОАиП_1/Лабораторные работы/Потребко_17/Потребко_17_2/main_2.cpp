#include <iostream>
#include <cmath>

int MaxMin(int* arr, int size) {
    int firstMaxIdx = -1, lastMinIdx = -1;
    for (int i = 0; i < size; ++i) {
        if (firstMaxIdx == -1  arr[i] > arr[firstMaxIdx]) {
            firstMaxIdx = i;
        }
        if (lastMinIdx == -1  arr[i] <= arr[lastMinIdx]) {
            lastMinIdx = i;
        }
    }
    if (firstMaxIdx > lastMinIdx) {
        std::swap(firstMaxIdx, lastMinIdx);
    }
    int sum = 0;
    for (int i = firstMaxIdx + 1; i < lastMinIdx; ++i) {
        sum += arr[i];
    }
    return sum;
}

bool AllPositive(int** matrix, int size) {
    for (int i = 0; i < size; ++i) {
        for (int j = 0; j < size; ++j) {
            if (matrix[i][j] <= 0) {
                return false;
            }
        }
    }
    return true;
}

int** multiply(int** matA, int** matB, int size) {
    int** result = new int* [size];
    for (int i = 0; i < size; ++i) {
        result[i] = new int[size]();
    }
    for (int i = 0; i < size; ++i) {
        for (int j = 0; j < size; ++j) {
            for (int k = 0; k < size; ++k) {
                result[i][j] += matA[i][k] * matB[k][j];
            }
        }
    }
    return result;
}

int main() {
    int n;
    std::cin >> n;
    int* arr = new int[n];
    for (int i = 0; i < n; ++i) {
        std::cin >> arr[i];
    }
    int sum = MaxMin(arr, n);
    std::cout << sum << std::endl;
    delete[] arr;

    int size;
    std::cin >> size;
    int** matA = new int* [size];
    int** matB = new int* [size];
    for (int i = 0; i < size; ++i) {
        matA[i] = new int[size];
        matB[i] = new int[size];
    }
    for (int i = 0; i < size; ++i) {
        for (int j = 0; j < size; ++j) {
            std::cin >> matA[i][j];
        }
    }
    for (int i = 0; i < size; ++i) {
        for (int j = 0; j < size; ++j) {
            std::cin >> matB[i][j];
        }
    }
    if (AllPositive(matA, size) && AllPositive(matB, size)) {
        int** product = multiply(matA, matB, size);
        for (int i = 0; i < size; ++i) {
            for (int j = 0; j < size; ++j) {
                std::cout << product[i][j] << ' ';
            }
            std::cout << std::endl;
        }
        for (int i = 0; i < size; ++i) {
            delete[] product[i];
        }
        delete[] product;
    }
    else {
        std::cout << " non-positive elements." << std::endl;
    }
    for (int i = 0; i < size; ++i) {
        delete[] matA[i];
        delete[] matB[i];
    }
    delete[] matA;
    delete[] matB;

    return 0;
}