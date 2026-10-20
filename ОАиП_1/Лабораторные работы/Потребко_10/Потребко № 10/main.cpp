#include <iostream>
#include <vector>
#include <cstdlib> 
#include <ctime>   

using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    srand((unsigned)time(NULL));

    int size;
    cout << "Введите размер массива: ";
    cin >> size;
    
    if (size <= 0) {
        cout << "Размер массива должен быть положительным." << endl;
        return 1;
    }

    vector<int> x(size);

    
    cout << "Сгенерированный массив:" << endl;
    for (int i = 0; i < size; ++i) {
        x[i] = rand() % 100;
        cout << x[i] << " ";
    }
    cout << endl;

    int minElement = x[0]; 
    for (int i = 1; i < size; ++i) {
        if (x[i] < minElement) {
            minElement = x[i];
        }
    }

    for (int i = 0; i < size; i += 2) { 
        x[i] = minElement;
    }

    cout << "Измененный массив:" << endl;
    for (int i = 0; i < size; ++i) {
        cout << x[i] << " ";
    }
    cout << endl;

    return 0;
}