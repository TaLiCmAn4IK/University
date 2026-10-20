#include <iostream>

using namespace std;

void shiftNegativeToZero(int size, int* arr) {
    int index = 0;  // Индекс для записи неотрицательных значений

    // Перемещаем неотрицательные элементы в начало массива
    for (int i = 0; i < size; ++i) {
        if (arr[i] >= 0) {
            arr[index++] = arr[i];
        }
    }

    // Задаем оставшимся элементам значение 0
    for (int i = index; i < size; ++i) {
        arr[i] = 0;
    }

    // Вывод результата
    cout << "Массив после преобразования: ";
    for (int i = 0; i < size; ++i) {
        cout << arr[i] << " ";
    }
    cout << endl;
}

int main() {
    setlocale(LC_CTYPE, "Russian");
    int size;
    cout << "Введите размер массива: ";
    cin >> size;

    // Ввод элементов массива
    int* elements = new int[size];
    cout << "Введите элементы массива: ";
    for (int i = 0; i < size; ++i) {
        cin >> elements[i];
    }

    // Вызов функции для обработки массива
    shiftNegativeToZero(size, elements);

    // Освобождение памяти
    delete[] elements;

    return 0;
}