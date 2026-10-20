#include <iostream>
#include <vector>
#include <iomanip>
using namespace std;

const int INF = 1e9;
const int NEG_INF = -1e9;  // Добавляем отрицательную бесконечность

void printMatrix(const vector<vector<int>>& M, const string& name) {
    cout << name << ":\n";
    for (int i = 0; i < M.size(); i++) {
        for (int j = 0; j < M.size(); j++) {
            if (M[i][j] == INF) cout << setw(4) << "INF";
            else if (M[i][j] == NEG_INF) cout << setw(4) << "NINF";
            else cout << setw(4) << M[i][j];
        }
        cout << endl;
    }
    cout << endl;
}

int main() {
    setlocale(LC_ALL, "RUS");
    int n = 6;

    // Исходная матрица смежности
    vector<vector<int>> original = {
        {0, 28, 21, 59, 12, 27},
        {7, 0, 24, INF, 21, 9},
        {9, 32, 0, 13, 11, INF},
        {8, INF, 5, 0, 16, INF},
        {14, 13, 15, 10, 0, 22},
        {15, 18, INF, INF, 6, 0}
    };

    // МАТРИЦА ДЛЯ МАКСИМАЛЬНЫХ ПУТЕЙ (противоположный алгоритм)
    vector<vector<int>> D_max(n, vector<int>(n, NEG_INF));
    vector<vector<int>> S_max(n, vector<int>(n, 0));

    // Инициализация матрицы максимальных путей
    for (int i = 0; i < n; i++) {
        for (int j = 0; j < n; j++) {
            if (i == j) {
                D_max[i][j] = INF;  // Путь из вершины в саму себя
                S_max[i][j] = i + 1;
            }
            else if (original[i][j] != INF) {
                D_max[i][j] = original[i][j];
                S_max[i][j] = j + 1;
            }
        }
    }

    // Алгоритм для максимальных путей (противоположный Флойду-Уоршеллу)
    for (int k = 0; k < n; k++) {
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                if (D_max[i][k] != NEG_INF && D_max[k][j] != NEG_INF) {
                    // Ищем МАКСИМУМ от МИНИМУМА (бутылочное горлышко)
                    int bottleneck = min(D_max[i][k], D_max[k][j]);
                    if (bottleneck > D_max[i][j]) {
                        D_max[i][j] = bottleneck;
                        S_max[i][j] = S_max[i][k];
                    }
                }
            }
        }
    }

    cout << "ИСХОДНАЯ МАТРИЦА:" << endl;
    printMatrix(original, "Original");

    cout << "МАТРИЦА МАКСИМАЛЬНЫХ ПУТЕЙ (максимальное бутылочное горлышко):" << endl;
    printMatrix(D_max, "Матрица D_max");

    cout << "МАТРИЦА СЛЕДОВАНИЯ ДЛЯ МАКСИМАЛЬНЫХ ПУТЕЙ:" << endl;
    printMatrix(S_max, "Матрица S_max");

    // Вывод интерпретации результатов
    cout << "ИНТЕРПРЕТАЦИЯ РЕЗУЛЬТАТОВ:" << endl;
    cout << "D_max[i][j] показывает максимальный вес самого 'слабого' ребра на пути из i в j" << endl;
    cout << "Чем БОЛЬШЕ значение, тем ЛУЧШЕ путь" << endl << endl;

    // Примеры некоторых путей
    cout << "ПРИМЕРЫ:" << endl;
    cout << "Путь из 1 в 4: максимальное минимальное ребро = " << D_max[0][3] << endl;
    cout << "Путь из 2 в 6: максимальное минимальное ребро = " << D_max[1][5] << endl;
    cout << "Путь из 3 в 5: максимальное минимальное ребро = " << D_max[2][4] << endl;
    cout << "Путь из 5 в 1: максимальное минимальное ребро = " << D_max[4][0] << endl;

    return 0;
}