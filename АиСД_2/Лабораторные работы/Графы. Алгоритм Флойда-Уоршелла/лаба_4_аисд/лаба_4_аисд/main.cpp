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

    return 0;
}