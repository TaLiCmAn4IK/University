#include <iostream>
#include <random>

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

    random_device rd;
    mt19937 gen(rd());
    uniform_int_distribution<> dis(1, 100); 

    int n;
    cout << "Введите количество участников:\n";
    cin >> n;

    if (n <= 0 || n >= 10000) {
        cout << "Неправильно введен размер массива\n";
        return 1;
    }

    int* A = new int[n];
    for (int i = 0; i < n; i++) {
        A[i] = dis(gen);
    }

    cout << "Сгенерированные результаты участников:\n";
    for (int i = 0; i < n; i++) {
        cout << A[i] << " ";
    }
    cout << "\n";

    selectionSort(A, n);

    int countDiplomas = 0; 
    int numPrizeWinners = 0; 
    int lastScore = -1;

    for (int i = n - 1; i >= 0; i--) {
        if (A[i] != lastScore) {
            countDiplomas++;
            if (countDiplomas > 3) break;
            lastScore = A[i];
        }
        if (countDiplomas <= 3) {
            numPrizeWinners++;
        }
    }

    cout << "Количество призёров: " << numPrizeWinners << "\n";

    delete[] A;

    return 0;
}
