#include <iostream>
#include <vector>
using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    srand((unsigned)time(NULL));

    int size;
    cout << "Введите размер массива: ";
    cin >> size;

    vector<int> MAX_SIZE(size);

    cout << "Сгенерированный массив:" << endl;
    for (int i = 0; i < size; ++i) {
        MAX_SIZE[i] = rand() % 10;
        cout << MAX_SIZE[i] << " ";
    }
    cout << endl;

    int uniqueCount = 0;
    for (int i = 0; i < size; ++i) {
        if (i == 0 || MAX_SIZE[i] != MAX_SIZE[i - 1]) { //проверяется, отличается ли текущий элемент от предыдущего
            uniqueCount++;
        }
    }

    cout << "Количество различных элементов в массиве: " << uniqueCount << endl;

    return 0;
}