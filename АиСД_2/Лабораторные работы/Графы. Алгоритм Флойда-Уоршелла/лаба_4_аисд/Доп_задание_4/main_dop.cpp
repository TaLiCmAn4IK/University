#include <iostream>
#include <vector>
#include <iomanip>
using namespace std;

const int INF = 1e9;

void printMatrix(const vector<vector<int>>& M, const string& name) {
    cout << name << ":\n";
    for (int i = 0; i < M.size(); i++) {
        for (int j = 0; j < M.size(); j++) {
            if (M[i][j] == INF) cout << setw(4) << "∞";
            else cout << setw(4) << M[i][j];
        }
        cout << endl;
    }
    cout << endl;
}

// Восстановление пути
vector<int> getPath(int u, int v, const vector<vector<int>>& S) {
    vector<int> path;
    if (S[u][v] == 0) return path; // пути нет

    path.push_back(u + 1);
    while (u != v) {
        u = S[u][v] - 1;
        path.push_back(u + 1);
    }
    return path;
}

// Вывод пути
void printPath(int u, int v, const vector<vector<int>>& S, const vector<vector<int>>& D) {
    if (D[u][v] == INF) {
        cout << "Пути из " << u + 1 << " в " << v + 1 << " не существует.\n\n";
        return;
    }

    vector<int> path = getPath(u, v, S);
    cout << "Кратчайший путь из " << u + 1 << " в " << v + 1 << ": ";
    for (int i = 0; i < path.size(); i++) {
        cout << path[i];
        if (i != path.size() - 1) cout << " > ";
    }
    cout << "\nДлина пути: " << D[u][v] << "\n\n";
}

int main() {
    setlocale(LC_ALL, "RUS");
    int n = 6;
    vector<vector<int>> D = {
        {0, 28, 21, 59, 12, 27},
        {7, 0, 24, INF, 21, 9},
        {9, 32, 0, 13, 11, INF},
        {8, INF, 5, 0, 16, INF},
        {14, 13, 15, 10, 0, 22},
        {15, 18, INF, INF, 6, 0}
    };

    vector<vector<int>> S(n, vector<int>(n, 0));
    for (int i = 0; i < n; i++)
        for (int j = 0; j < n; j++)
            if (i != j && D[i][j] < INF)
                S[i][j] = j + 1;

    // Алгоритм Флойда–Уоршелла
    for (int k = 0; k < n; k++) {
        for (int i = 0; i < n; i++) {
            for (int j = 0; j < n; j++) {
                if (D[i][k] < INF && D[k][j] < INF && D[i][k] + D[k][j] < D[i][j]) {
                    D[i][j] = D[i][k] + D[k][j];
                    S[i][j] = S[i][k];
                }
            }
        }
    }

    printMatrix(D, "Матрица D");
    printMatrix(S, "Матрица S");

    while (true) {
        cout << "Введите начальную и конечную вершины (от 1 до " << n << "):";

        int a, b;
        cin >> a;
        if (a == 0) {
            cout << "Выход из программы.\n";
            break;
        }
        cin >> b;

        if (a < 1 || a > n || b < 1 || b > n) {
            cout << "Неверный ввод! Вершины должны быть от 1 до " << n << ".\n\n";
            continue;
        }

        printPath(a - 1, b - 1, S, D);
    }

    return 0;
}
