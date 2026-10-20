#include <iostream>
#include <vector>

using namespace std;

int main() {
    setlocale(LC_ALL, "RUS");
    int n;
    cout << "Введите размер массива: ";
    cin >> n;

    vector<int> Massive(n);
    cout << "Введите элементы массива:\n";
    for (int i = 0; i < n; i++) {
        cin >> Massive[i];
    }

    int maxCount = 1, Count = 1;
    for (int i = 1; i < n; i++) {
        if (Massive[i] == Massive[i - 1]) {
            Count++;
        }
        else {
            Count = 1;
        }
        if (Count > maxCount) {
            maxCount = Count;
        }
    }

    cout << "Наибольшее число подряд идущих одинаковых элементов: " << maxCount << endl;

    return 0;
}