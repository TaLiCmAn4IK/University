#include <iostream>
using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    const int ROWS = 4, COLS = 2;
    int B[ROWS][COLS];

    cout << "Введите элементы массива:" << endl;
    for (int i = 0; i < ROWS; i++)
        for (int j = 0; j < COLS; j++)
            cin >> *(*(B + i) + j);

    cout << "Исходный массив:" << endl;
    for (int i = 0; i < ROWS; i++) {
        cout << "\n";
        for (int j = 0; j < COLS; j++)
            cout << "B[" << i << "," << j << "] =" << *(*(B + i) + j) << "\t";
    }

    int sum = 0;
    int product = 1;
    bool flag = false; // флаг для проверки наличия отрицательных нечётных элементов

    for (int i = 0; i < ROWS; i++) {
        for (int j = 0; j < COLS; j++) {
            if (B[i][j] < 0 && B[i][j] % 2 != 0 && abs(B[i][j]) < ROWS) { // Проверка на отрицательный нечётный элемент  
                sum += *(*(B + i) + j); // +
                product *= *(*(B + i) + j); // *
                flag = true; // флаг
            }
        }
    }

    // Вывод результата
    if (flag) {
        cout << "\nСумма отрицательных нечётных элементов: " << sum << endl;
        cout << "Произведение отрицательных нечётных элементов: " << product << endl;
    }
    else {
        cout << "\nНет отрицательных нечётных элементов в массиве." << endl;
    }

    return 0;
}