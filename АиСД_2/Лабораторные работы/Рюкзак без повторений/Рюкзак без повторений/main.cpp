#include <iostream>
#include <string>
#include <Windows.h>
#include <algorithm>

using namespace std;

// Функция для жадного алгоритма
void greedyKnapsack(string* name, int* cost, int* ves, int N, int M) {
    cout << "\n=== ЖАДНЫЙ АЛГОРИТМ ===\n";

    // Создаем массив индексов для сортировки
    int* index = new int[M];
    for (int i = 0; i < M; i++) {
        index[i] = i;  // Заполняем индексами от 0 до M-1
    }

    // Сортируем предметы по убыванию стоимости
    for (int i = 0; i < M - 1; i++) {
        for (int j = i + 1; j < M; j++) {
            if (cost[index[i]] < cost[index[j]]) {  // Сравниваем стоимости предметов через массив индексов
                swap(index[i], index[j]);
            }
        }
    }

    int currentWeight = 0; // вес в рюкзаке 
    int totalCost = 0; // общая стоимость 

    cout << "Предметы, положенные в рюкзак:\n";
    for (int i = 0; i < M; i++) {  // проходим по отсортированным предметам 
        int idx = index[i];
        if (currentWeight + ves[idx] <= N) {  // помещается ли предмет в рюкзак
            cout << name[idx] << " (вес: " << ves[idx] << ", стоимость: " << cost[idx] << ")\n";
            currentWeight += ves[idx];
            totalCost += cost[idx];
        }
    }

    cout << "Максимальная стоимость предметов (жадный алгоритм): " << totalCost << "\n";
    cout << "Использованный вес: " << currentWeight << " из " << N << "\n";

    delete[] index;
}

// Функция для динамического программирования (ваш исходный код)
void dynamicProgrammingKnapsack(string* name, int* cost, int* ves, int N, int M) {
    cout << "\n=== ДИНАМИЧЕСКОЕ ПРОГРАММИРОВАНИЕ ===\n";

    int** A = new int* [M + 1];
    for (int i = 0; i < M + 1; i++) {
        A[i] = new int[N + 1];
    }

    for (int i = 0; i < M + 1; i++) {
        for (int j = 0; j < N + 1; j++) {
            A[i][j] = 0;
        }
    }

    for (int i = 1; i < M + 1; i++) {
        for (int j = 1; j < N + 1; j++) {
            if (j < ves[i - 1]) {
                A[i][j] = A[i - 1][j];
            }
            else {
                A[i][j] = max(A[i - 1][j], A[i - 1][j - ves[i - 1]] + cost[i - 1]);
            }
        }
    }

    cout << "Максимальная стоимость предметов: " << A[M][N] << "\n"
        << "Предметы, положенные в рюкзак:\n";

    int i = M, j = N;
    while (i > 0 && j > 0) {
        if (A[i][j] != A[i - 1][j]) {
            cout << name[i - 1] << " (вес: " << ves[i - 1] << ", стоимость: " << cost[i - 1] << ")\n";
            j -= ves[i - 1];
        }
        i--;
    }

    for (int i = 0; i < M + 1; i++) {
        delete[] A[i];
    }
    delete[] A;
}

void main() {
    SetConsoleCP(1251);
    SetConsoleOutputCP(1251);

    int N, M;
    do {
        cout << "Введите вместительность рюкзака: ";
        cin >> N;
        cin.clear();
        cin.ignore(32767, '\n');
    } while (N <= 0);

    do {
        cout << "Введите количество предметов: ";
        cin >> M;
        cin.clear();
        cin.ignore(32767, '\n');
    } while (M <= 0);

    string* name = new string[M];
    int* cost = new int[M];
    int* ves = new int[M];

    for (int i = 0; i < M; i++) {
        cout << "Введите название " << i + 1 << " предмета: ";
        cin >> name[i];
        do {
            cout << "Введите стоимость " << i + 1 << " предмета: ";
            cin >> cost[i];
            cin.clear();
            cin.ignore(32767, '\n');
        } while (cost[i] <= 0);
        do {
            cout << "Введите вес " << i + 1 << " предмета: ";
            cin >> ves[i];
            cin.clear();
            cin.ignore(32767, '\n');
        } while (ves[i] <= 0);
    }

    // Запускаем оба алгоритма
    dynamicProgrammingKnapsack(name, cost, ves, N, M);
    greedyKnapsack(name, cost, ves, N, M);

    // Освобождаем память
    delete[] name;
    delete[] cost;
    delete[] ves;
}