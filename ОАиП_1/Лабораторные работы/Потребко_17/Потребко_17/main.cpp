#include <iostream>
using namespace std;

// Прототипы функций
void inputArray(int*& arr, int size);
int countNegativeEvenIndices(int* arr, int size);

int main() {
    setlocale(LC_CTYPE, "Russian");

    int size;
    cout << "Введите размер массива: ";
    cin >> size;

    // Создание динамического массива
    int* arr = new int[size];

    // Ввод элементов массива
    inputArray(arr, size);

    // Подсчет количества отрицательных элементов на четных позициях
    int count = countNegativeEvenIndices(arr, size);

    // Вывод результата
    cout << "Количество отрицательных элементов на четных местах: " << count << endl;

    // Освобождение памяти
    delete[] arr;

    return 0;
}

// Функция для ввода элементов массива
void inputArray(int*& arr, int size) {
    cout << "Введите элементы массива: ";
    for (int i = 0; i < size; i++) {
        cout << "arr[" << i << "]: ";
        cin >> arr[i];
    }
}

// Функция для подсчета отрицательных элементов на четных позициях
int countNegativeEvenIndices(int* arr, int size) {
    int count = 0;
    for (int i = 0; i < size; i += 2) { // Проверяем четные индексы (0, 2, 4, ...)
        if (arr[i] < 0) {
            count++;
        }
    }
    return count;
}