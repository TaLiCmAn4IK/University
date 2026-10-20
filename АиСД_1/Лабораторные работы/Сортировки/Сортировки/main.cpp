#include <iostream>
#include <vector>
#include <cstdlib>
#include <ctime>
#include <chrono>
using namespace std;
using namespace chrono;
//Сортировка пузырьком
void bubbleSort(vector<int>& a) {
    int N = a.size();
    for (int i = 0; i < N - 1; i++) {
        for (int j = 0; j < N - i - 1; j++) {
            if (a[j] > a[j + 1]) {
                swap(a[j], a[j + 1]);
            }
        }
    }
}
//Сортировка вставками
void insertionSort(vector<int>& a) {
    int N = a.size();
    for (int i = 1; i < N; i++) {
        int buff = a[i];
        int j = i - 1;
        while (j >= 0 && a[j] > buff) {
            a[j + 1] = a[j];
            j--;
        }
        a[j + 1] = buff;
    }
}
//Сортировка выбором
void selectionSort(vector<int>& a) {
    int N = a.size();
    for (int i = 0; i < N - 1; ++i) {
        int min = i;
        for (int j = i + 1; j < N; ++j)
            if (a[j] < a[min])
                min = j;
        if (i != min)
            swap(a[i], a[min]);
    }
}
//Быстрая сортировка
void quickSort(vector<int>& a, int first, int last) {
    int mid, count;
    int f = first, l = last;
    mid = a[(f + l) / 2];
    do {
        while (a[f] < mid) f++;
        while (a[l] > mid) l--;
        if (f <= l) {
            count = a[f];
            a[f] = a[l];
            a[l] = count;
            f++; l--;
        }
    } while (f <= l);
    if (first < l) quickSort(a, first, l);
    if (f < last) quickSort(a, f, last);
}
//Вывод массива
void printArray(const vector<int>& a) {
    for (int val : a) cout << val << " ";
    cout << "\n";
}
//Замер времени и сортировка
template<typename SortFunc>
void measureSort(SortFunc func, vector<int>& a, const string& name) {
    auto start = high_resolution_clock::now();
    func(a);
    auto end = high_resolution_clock::now();
    auto duration_ns = duration_cast<nanoseconds>(end - start);

    cout << "\nМассив " << name << " после сортировки:\n";
    printArray(a);
    cout << "Время сортировки: " << duration_ns.count() << " нс ( "
        << duration_ns.count() / 1'000'000.0 << " мс)\n";
}

int main() {
    setlocale(LC_ALL, "Russian");
    int N;
    cout << "Введите количество элементов массива (N): ";
    cin >> N;

    //Генерация массива A
    vector<int> A(N);
    srand(time(nullptr));
    for (int i = 0; i < N; ++i) A[i] = rand() % 10000;

    cout << "\nИсходный массив A:\n";
    printArray(A);

    //Копирование
    vector<int> B = A;
    vector<int> C = A;
    vector<int> D = A;
    vector<int> E = A;

    //Сортировки
    measureSort(bubbleSort, B, "B (пузырьковая сортировка)");
    measureSort(insertionSort, C, "C (сортировка вставками)");
    measureSort(selectionSort, D, "D (сортировка выбором)");

    auto quickSortWrapper = [](vector<int>& a) {
        quickSort(a, 0, a.size() - 1);
        };
    measureSort(quickSortWrapper, E, "E (быстрая сортировка)");

    return 0;
}