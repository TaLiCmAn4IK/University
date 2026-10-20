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

    int count = 0;
    for (int i = 1; i < n; i++) {
        if (Massive[i] == Massive[i - 1]) {
            count++;
        }
    }

    cout << "Количество пар соседних элементов с одинаковыми значениями: " << count << endl;

    return 0;
}