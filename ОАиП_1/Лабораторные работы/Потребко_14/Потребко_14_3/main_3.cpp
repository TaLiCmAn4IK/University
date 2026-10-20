#include <iostream>
using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    const int ROWS = 4, COLS = 2;
    int B[ROWS][COLS];

    cout << "Введите элементы массива:" << endl;
    for (int i = 0; i < ROWS; i++)
        for (int j = 0; j < COLS; j++)
            cin >> B[i][j];  // Изменил доступ к элементам массива

    cout << "Исходный массив:" << endl;
    for (int i = 0; i < ROWS; i++) {
        cout << "\n";
        for (int j = 0; j < COLS; j++)
            cout << "B[" << i << "," << j << "] =" << B[i][j] << "\t";  // Изменил доступ к элементам массива
    }

    int sum = 0;
    int product = 1;
    bool flag = false; // Флаг для проверки наличия элементов по модулю меньше 2

    for (int i = 0; i < ROWS; i++) {
        for (int j = 0; j < COLS; j++) {
            if (abs(B[i][j]) < ROWS) { // Проверка на элементы по модулю меньше 2
                sum += B[i][j]; // Суммируем
                product *= B[i][j]; // Умножаем
                flag = true; // Флаг
            }
        }
    }

    // Вывод результата
    if (flag) {
        cout << "\nСумма отрицательных нечетных элементов по модулю меньше i: " << sum << endl;
        cout << "Произведение отрицательных нечетных элементов по модулю меньше i: " << product << endl;
    }
    else {
        cout << "\nНет элементов по модулю меньше i в массиве." << endl;
    }

    return 0;
}