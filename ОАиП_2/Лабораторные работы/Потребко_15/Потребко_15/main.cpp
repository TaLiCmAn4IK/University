#include <iostream>
#include <vector>
#include <cstdlib>   // rand, srand
#include <ctime>     // time
#include <chrono>    // chrono

using namespace std;
using namespace chrono;

// ───────────────────────── Генерация случайного массива A
vector<int> generateRandomArrayA() {
    vector<int> a;
    int n;

    cout << "Введите количество элементов массива A: ";
    cin >> n;

    srand(static_cast<unsigned>(time(nullptr)));

    cout << "Сгенерированный массив A: ";
    for (int i = 0; i < n; ++i) {
        int val = rand() % 100;
        a.push_back(val);
        cout << val << ' ';
    }
    cout << '\n';

    return a;
}

// ───────────────────────── Формирование массива B из ЧЁТНЫХ ЭЛЕМЕНТОВ массива A
vector<int> createArrayB(const vector<int>& a) {
    vector<int> b;
    for (int x : a)
        if (x % 2 == 0)
            b.push_back(x);
    return b;
}

// ───────────────────────── Сортировка пузырьком
void bubbleSort(vector<int>& v) {
    for (size_t i = 0; i + 1 < v.size(); ++i)
        for (size_t j = 0; j + 1 < v.size() - i; ++j)
            if (v[j] > v[j + 1])
                swap(v[j], v[j + 1]);
}

// ───────────────────────── Вспомогательная процедура слияния
void merge(vector<int>& v, int l, int m, int r) {
    int n1 = m - l + 1, n2 = r - m;
    vector<int> L(n1), R(n2);

    for (int i = 0; i < n1; ++i) L[i] = v[l + i];
    for (int j = 0; j < n2; ++j) R[j] = v[m + 1 + j];

    int i = 0, j = 0, k = l;
    while (i < n1 && j < n2)
        v[k++] = (L[i] <= R[j]) ? L[i++] : R[j++];
    while (i < n1) v[k++] = L[i++];
    while (j < n2) v[k++] = R[j++];
}

// ───────────────────────── Рекурсивная сортировка слиянием
void mergeSort(vector<int>& v, int l, int r) {
    if (l >= r) return;
    int m = l + (r - l) / 2;
    mergeSort(v, l, m);
    mergeSort(v, m + 1, r);
    merge(v, l, m, r);
}

// ───────────────────────── Точка входа
int main() {
    setlocale(LC_ALL, "Russian");

    vector<int> A = generateRandomArrayA();
    vector<int> B = createArrayB(A);
    if (B.empty()) {
        cout << "В массиве A нет чётных элементов. Массив B пуст.\n";
        return 0;
    }

    cout << "Массив B до сортировки: ";
    for (int x : B) cout << x << ' ';
    cout << '\n';

    // ───── Сортировка пузырьком с замером времени (в наносекундах)
    vector<int> bubble = B;
    auto start_bubble = high_resolution_clock::now();
    bubbleSort(bubble);
    auto end_bubble = high_resolution_clock::now();
    auto duration_bubble = duration_cast<nanoseconds>(end_bubble - start_bubble).count();

    cout << "Массив B после сортировки пузырьком: ";
    for (int x : bubble) cout << x << ' ';
    cout << "\nВремя сортировки пузырьком: " << duration_bubble << " нс\n";

    // ───── Сортировка слиянием с замером времени (в наносекундах)
    vector<int> mergeS = B;
    auto start_merge = high_resolution_clock::now();
    mergeSort(mergeS, 0, static_cast<int>(mergeS.size()) - 1);
    auto end_merge = high_resolution_clock::now();
    auto duration_merge = duration_cast<nanoseconds>(end_merge - start_merge).count();

    cout << "Массив B после сортировки слиянием: ";
    for (int x : mergeS) cout << x << ' ';
    cout << "\nВремя сортировки слиянием: " << duration_merge << " нс\n";

    return 0;
}
