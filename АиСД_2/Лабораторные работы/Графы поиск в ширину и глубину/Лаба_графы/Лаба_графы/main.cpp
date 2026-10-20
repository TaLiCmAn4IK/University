#include <iostream>
#include <vector>
#include <queue>
#include <stack>
#include <algorithm>
#include <unordered_set>

using namespace std;

// Базовый класс для представления графа
class Graph {
public:
    virtual void addEdge(int u, int v) = 0;
    virtual vector<int> getNeighbors(int u) = 0;
    virtual void printStructure() = 0;
    vector<int> BFS(int start);
    vector<int> DFS(int start);
};

// Реализация BFS
vector<int> Graph::BFS(int start) {
    vector<int> order;
    unordered_set<int> visited;
    queue<int> q;
    q.push(start);
    visited.insert(start);

    while (!q.empty()) {
        int u = q.front();
        q.pop();
        order.push_back(u);
        for (int v : getNeighbors(u)) {
            if (visited.find(v) == visited.end()) {
                visited.insert(v);
                q.push(v);
            }
        }
    }
    return order;
}

// Реализация DFS
vector<int> Graph::DFS(int start) {
    vector<int> order;
    unordered_set<int> visited;
    stack<int> st;
    st.push(start);
    visited.insert(start);

    while (!st.empty()) {
        int u = st.top();
        st.pop();
        order.push_back(u);
        for (int v : getNeighbors(u)) {
            if (visited.find(v) == visited.end()) {
                visited.insert(v);
                st.push(v);
            }
        }
    }
    return order;
}

// Граф на основе списка ребер
class EdgeListGraph : public Graph {
    vector<pair<int, int>> edges;
    int n;

public:
    EdgeListGraph(int n) : n(n) {}
    void addEdge(int u, int v) override {
        edges.push_back({ u, v });
        edges.push_back({ v, u });
    }
    vector<int> getNeighbors(int u) override {
        vector<int> neighbors;
        for (auto& e : edges) {
            if (e.first == u) {
                neighbors.push_back(e.second);
            }
        }
        return neighbors;
    }
    void printStructure() override {
        cout << "Список ребер:\n";
        for (auto& e : edges) {
            cout << "(" << e.first << ", " << e.second << ") ";
        }
        cout << endl;
    }
};

// Граф на основе матрицы смежности
class AdjMatrixGraph : public Graph {
    vector<vector<bool>> adj;
    int n;

public:
    AdjMatrixGraph(int n) : n(n), adj(n + 1, vector<bool>(n + 1, false)) {}
    void addEdge(int u, int v) override {
        adj[u][v] = true;
        adj[v][u] = true;
    }
    vector<int> getNeighbors(int u) override {
        vector<int> neighbors;
        for (int v = 1; v <= n; ++v) {
            if (adj[u][v]) {
                neighbors.push_back(v);
            }
        }
        return neighbors;
    }
    void printStructure() override {
        cout << "Матрица смежности:\n";
        for (int i = 1; i <= n; ++i) {
            for (int j = 1; j <= n; ++j) {
                cout << adj[i][j] << " ";
            }
            cout << endl;
        }
    }
};

// Граф на основе списка смежности
class AdjListGraph : public Graph {
    vector<vector<int>> adj;
    int n;

public:
    AdjListGraph(int n) : n(n), adj(n + 1) {}
    void addEdge(int u, int v) override {
        adj[u].push_back(v);
        adj[v].push_back(u);
    }
    vector<int> getNeighbors(int u) override {
        return adj[u];
    }
    void printStructure() override {
        cout << "Список смежности:\n";
        for (int i = 1; i <= n; ++i) {
            cout << i << ": ";
            for (int v : adj[i]) {
                cout << v << " ";
            }
            cout << endl;
        }
    }
};

int main() {
    setlocale(LC_ALL, "RUS");
    int n = 10;
    vector<pair<int, int>> edges = {
         {1,2}, {1,5}, {2,7}, {2,8}, {3,8}, {3,7}, {4,6}, {4,9}, {5,6}, {6,9}, {7,8}, {9,10}

    };

    // Инициализация графов
    EdgeListGraph g1(n);
    AdjMatrixGraph g2(n);
    AdjListGraph g3(n);

    for (auto& e : edges) {
        g1.addEdge(e.first, e.second);
        g2.addEdge(e.first, e.second);
        g3.addEdge(e.first, e.second);
    }



    // Вывод структуры графов
    g1.printStructure();
    g2.printStructure();
    g3.printStructure();
    // BFS и DFS для каждого представления
    vector<int> bfs_order = g1.BFS(1);
    vector<int> dfs_order = g1.DFS(1);

    cout << "BFS порядок (список ребер): ";
    for (int v : bfs_order) cout << v << " ";
    cout << "\nDFS порядок (список ребер): ";
    for (int v : dfs_order) cout << v << " ";
    cout << endl;

    bfs_order = g2.BFS(1);
    dfs_order = g2.DFS(1);
    cout << "BFS порядок (матрица смежности): ";
    for (int v : bfs_order) cout << v << " ";
    cout << "\nDFS порядок (матрица смежности): ";
    for (int v : dfs_order) cout << v << " ";
    cout << endl;

    bfs_order = g3.BFS(1);
    dfs_order = g3.DFS(1);
    cout << "BFS порядок (список смежности): ";
    for (int v : bfs_order) cout << v << " ";
    cout << "\nDFS порядок (список смежности): ";
    for (int v : dfs_order) cout << v << " ";
    cout << endl;

    return 0;
}