#include <iostream>
#include <cstdlib> 
#include <ctime>   

using namespace std;

int main() {
    setlocale(LC_CTYPE, "Russian");
    srand((unsigned)time(NULL));

    int n;

    cout << "Введите количество элементов в массивах: ";
    cin >> n;

    int* A = new int[n];
    int* B = new int[n];

    cout << "Массив A: ";
    for (int i = 0; i < n; ++i) {
        A[i] = rand() % 10;
        B[i] = rand() % 10;
        cout << A[i] << " ";
    }
    cout << endl;

    cout << "Массив B: ";
    for (int i = 0; i < n; ++i) {
        cout << B[i] << " ";
    }
    cout << endl;

    int x = 0;
    int y = 0;
    int z = 0;

    for (int i = 0; i < n; ++i){
        if (*(A + i) == *(B + i))
            x++;
        else if (*(A + i) > *(B + i))
            y++;
        else if (*(A + i) < *(B + i))
            z++;
    }

    cout << "Количество элементов, для которых A[k] = B[k]: " << x << endl;
    cout << "Количество элементов, для которых A[k] > B[k]: " << y << endl;
    cout << "Количество элементов, для которых A[k] < B[k]: " << z << endl;

    return 0;
}