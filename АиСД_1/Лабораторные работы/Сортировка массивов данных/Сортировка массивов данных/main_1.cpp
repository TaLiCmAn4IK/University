#include <iostream>
#include <cstdlib>  
#include <ctime>    

using namespace std;

void selectionSort(int* a, int n) {
    int min;
    for (int i = 0; i < n; i++) {
        min = i;
        for (int j = i + 1; j < n; j++)
            min = (a[j] < a[min]) ? j : min;
        if (i != min) {
            int buf = a[i];
            a[i] = a[min];
            a[min] = buf;
        }
    }
}

int main() {
    setlocale(LC_ALL, "rus");

    int n;
    cout << "Введите количество товаров (размер массива):\n";
    cin >> n;

    if (n <= 0 || n >= 10000) {
        cout << "Неправильно введен размер массива\n";
        return 1;
    }

    int* A = new int[n];
    int* B = new int[n];

    // инициализация генератора случайных чисел
    srand(static_cast<unsigned int>(time(nullptr)));

    for (int i = 0; i < n; i++) {
        A[i] = 1 + rand() % 100;
    }

    cout << "Сгенерированные цены товаров:\n";
    for (int i = 0; i < n; i++) {
        cout << A[i] << " ";
    }
    cout << "\n";

    selectionSort(A, n);

    int sum = 0;
    int left = 0;      // дешевые
    int right = n - 1; //  дорогие
    int z = 0;         

    while (left <= right) {
        if (right >= left) {
            B[z++] = A[right--]; 
            sum += B[z - 1];     
        }
        if (left <= right) {
            B[z++] = A[left++];  
        }
    }

    cout << "Порядок товаров на кассе:\n";
    for (int i = 0; i < n; i++) {
        cout << B[i] << " ";
    }
    cout << "\n";

    cout << "Итоговая сумма чека равна: " << sum << "\n";

    delete[] A;
    delete[] B;

    return 0;
}
