#include <iostream>
#include <vector>
#include <algorithm>
using namespace std;

// Структура для представления ребра графа
struct Edge {
    int src, dest, weight;
};

// Функция сравнения для сортировки ребер по весу
bool compareEdges(Edge a, Edge b) {
    return a.weight < b.weight;
}

void kruskalMST(int V, vector<Edge>& edges) {
    sort(edges.begin(), edges.end(), compareEdges);

    vector<Edge> result;// Результат - ребра MST
    vector<int> parent(V);// Массив для системы непересекающихся множеств

    // Инициализация - каждая вершина в своем множестве
    for (int i = 0; i < V; i++) {
        parent[i] = i;
    }

    // Функция поиска корня множества
    auto find = [&](int i) {
        while (parent[i] != i) {
            i = parent[i];
        }
        return i;
        };

    // Функция объединения двух множеств
    auto simpleUnion = [&](int x, int y) {
        int xroot = find(x);
        int yroot = find(y);
        parent[xroot] = yroot;
        };

    // Основной цикл алгоритма Краскала
    for (Edge edge : edges) {
        // корни множеств для обеих вершин ребра
        int x = find(edge.src);
        int y = find(edge.dest);
        // Если вершины в разных множествах - нет цикла 
        if (x != y) {
            result.push_back(edge);
            simpleUnion(x, y);
        }
        if (result.size() == V - 1) break;
    }

    cout << "Минимальное остовное дерево (Краскал):" << endl;
    int totalWeight = 0;
    for (Edge edge : result) {
        cout << "v" << edge.src + 1 << " -- v" << edge.dest + 1
            << " (вес: " << edge.weight << ")" << endl;
        totalWeight += edge.weight;
    }
    cout << "Общий вес: " << totalWeight << endl << endl;
}

void primMST(vector<vector<int>>& graph) {
    int V = graph.size();
    vector<int> parent(V, -1);    // Массив родителей вершин
    vector<int> key(V, INT_MAX);  // Ключи вершин (минимальные веса)
    vector<bool> inMST(V, false); // Отметки о включении в MST
    vector<Edge> mstEdges;        // Ребра MST в порядке добавления

    key[0] = 0;

    // Основной цикл алгоритма Прима
    for (int count = 0; count < V; count++) {
        // Поиск вершины с минимальным ключом, не входящей в MST
        int u = -1;
        for (int v = 0; v < V; v++) {
            if (!inMST[v] && (u == -1 || key[v] < key[u])) {
                u = v;
            }
        }

        if (u == -1) break;

        inMST[u] = true;

        if (parent[u] != -1) {
            mstEdges.push_back({ parent[u], u, graph[parent[u]][u] });
        }

        for (int v = 0; v < V; v++) {
            if (graph[u][v] && !inMST[v] && graph[u][v] < key[v]) {
                parent[v] = u;
                key[v] = graph[u][v];
            }
        }
    }

    cout << "Минимальное остовное дерево (Прима):" << endl;
    int totalWeight = 0;
    for (Edge edge : mstEdges) {
        cout << "v" << edge.src + 1 << " -- v" << edge.dest + 1
            << " (вес: " << edge.weight << ")" << endl;
        totalWeight += edge.weight;
    }
    cout << "Общий вес: " << totalWeight << endl;
}

// Функция добавления ребра в граф
void addEdge(vector<vector<int>>& primGraph, vector<Edge>& edges,
    int v1, int v2, int weight) {
    primGraph[v1][v2] = weight;
    primGraph[v2][v1] = weight;
    edges.push_back({ v1, v2, weight }); //список ребер
}

int main() {
    setlocale(LC_ALL, "RUS");
    const int V = 8;

    vector<vector<int>> primGraph(V, vector<int>(V, 0));
    vector<Edge> edges;

    addEdge(primGraph, edges, 0, 1, 2);
    addEdge(primGraph, edges, 1, 2, 3);
    addEdge(primGraph, edges, 0, 3, 8);
    addEdge(primGraph, edges, 2, 7, 7);
    addEdge(primGraph, edges, 3, 5, 3);
    addEdge(primGraph, edges, 5, 6, 6);
    addEdge(primGraph, edges, 6, 7, 9);
    addEdge(primGraph, edges, 1, 4, 5);
    addEdge(primGraph, edges, 4, 6, 4);
    addEdge(primGraph, edges, 3, 4, 14);
    addEdge(primGraph, edges, 0, 4, 2);
    addEdge(primGraph, edges, 1, 3, 10);
    addEdge(primGraph, edges, 3, 6, 1);
    addEdge(primGraph, edges, 5, 4, 11);
    addEdge(primGraph, edges, 2, 4, 12);
    addEdge(primGraph, edges, 4, 7, 8);

    cout << "Граф с 8 вершинами:" << endl;
    cout << "Вершины: v1, v2, v3, v4, v5, v6, v7, v8" << endl << endl;

    kruskalMST(V, edges);
    primMST(primGraph);

    return 0;
}