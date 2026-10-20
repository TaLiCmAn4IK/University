#include <iostream>

using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    srand((unsigned)time(NULL));

    int n;
    cout << "Введите размер массива: ";
    cin >> n;

    int* A = new int[n]; // динамическое выделение памяти для массива


    for (int i = 0; i < n; ++i) {
        int num;
        do {
            num = rand() % 1000 - 500;  // Генерируем числа от -500 до 500
        } while (num == 0);  // Повторяем, если число равно 0
        A[i] = num;
    }

    cout << "Массив A: ";
    for (int i = 0; i < n; ++i) {
        cout << A[i] << " ";
    }
    cout << endl;

    int* left = A; // указатель на начало массива
    int* right = A + n - 1; // указатель на конец массива

    while (left < right) { // меняем меставми элементы +-
        if (*left < 0 && *right > 0) { //операция "и"
            int temp = *left;
            *left = *right;
            *right = temp;

            left++;
            right--;
        }
        // если левый элемент положительный, двигаемся вправо
        else if (*left > 0) {
            left++;
        }
        // если правый элемент отрицательный, двигаемся влево
        else {
            right--;
        }
    }

    cout << "Переделанный массив: ";
    for (int i = 0; i < n; ++i) {
        cout << A[i] << " ";
    }
    cout << endl;

    return 0;
}