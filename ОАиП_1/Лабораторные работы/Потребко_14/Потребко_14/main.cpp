#include <iostream>
using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    const int ROWS = 3, COLS = 4; 
    int A[ROWS][COLS];

    cout << "Введите элементы массива: " << endl;
    for (int i = 0; i < ROWS; i++) {
        for (int j = 0; j < COLS; j++) {
            cin >> A[i][j];
        }
    }

    bool flag = true; // Флаг для проверки

    // Проверка на положительные элементы
    for (int i = 0; i < ROWS; i++) {
        bool hasPositive = false;
        for (int j = 0; j < COLS; j++) {
            if (A[i][j] > 0) {
                hasPositive = true; // В строке есть положительный элемент
                break;
            }
        }
        if (!hasPositive) {
            flag = false; // В строке нет положительных элементов
            break;
        }
    }

    if (flag) {
        for (int i = 0; i < ROWS; i++) {
            for (int j = 0; j < COLS; j++) {
                A[i][j] = -(A[i][j]); // изменение знаков
            }
        }
        cout << "Все строки содержат хотя бы один положительный элемент. Знаки элементов изменены." << endl;
    }
    else {
        cout << "Не все строки содержат хотя бы один положительный элемент." << endl;
    }

    cout << "Конечная матрица: " << endl;
    for (int i = 0; i < ROWS; i++) {
        for (int j = 0; j < COLS; j++) {
            cout << A[i][j] << "\t";
        }
        cout << endl;
    }

    return 0;
}
