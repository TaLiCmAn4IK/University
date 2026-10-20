#include <iostream>
using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    srand((unsigned)time(NULL));

    int n;
    cout << "Введите размер массива: ";
    cin >> n;

   
    int* A = new int[n]; //дин.массив

    for (int i = 0; i < n; ++i) {
        A[i] = rand() % 10;
    }

    cout << "Массив A: ";
    for (int i = 0; i < n; ++i) {
        cout << A[i] << " ";
    }
    cout << endl;

    int k;
    cout << "Введите число k: ";
    cin >> k;

    cout << "Индексы элементов, равных " << k << ": ";
    for (int i = 0; i < n; ++i) {
        if (*(A + i) == k) {
            cout << i << " ";
        }
    }

    cout << "\nИндексы элементов, больших " << k << ": ";
    for (int i = 0; i < n; ++i) {
        if (*(A + i) > k) {
            cout << i << " ";
        }
    }


    cout << "\nИндексы элементов, меньших " << k << ": ";
    for (int i = 0; i < n; ++i) {
        if (*(A + i) < k) {
            cout << i << " ";
        }
    }

    return 0;
}