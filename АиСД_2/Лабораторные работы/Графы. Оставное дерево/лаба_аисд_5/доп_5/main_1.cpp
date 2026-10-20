#include <iostream>
#include <vector>
#include <algorithm>
using namespace std;

struct Edge {
    int src, dest, weight;
};

bool compareEdges(Edge a, Edge b) {
    return a.weight < b.weight;
}

void boruvkaMST(int V, vector<Edge>& edges) {
    vector<int> parent(V);
    vector<int> cheapest(V, -1);
    vector<Edge> result;

    for (int i = 0; i < V; i++) {
        parent[i] = i;
    }

    int numComponents = V;
    int totalWeight = 0;

    cout << "Минимальное остовное дерево (алгоритм Борувки):" << endl;

    while (numComponents > 1) {
        fill(cheapest.begin(), cheapest.end(), -1);


        for (int i = 0; i < edges.size(); i++) {
            Edge edge = edges[i];
            int set1 = parent[edge.src];
            int set2 = parent[edge.dest];

            if (set1 != set2) {
                if (cheapest[set1] == -1 || edges[cheapest[set1]].weight > edge.weight) {
                    cheapest[set1] = i;
                }
                if (cheapest[set2] == -1 || edges[cheapest[set2]].weight > edge.weight) {
                    cheapest[set2] = i;
                }
            }
        }

        for (int i = 0; i < V; i++) {
            if (cheapest[i] != -1) {
                Edge edge = edges[cheapest[i]];
                int set1 = parent[edge.src];
                int set2 = parent[edge.dest];

                if (set1 != set2) {
                    result.push_back(edge);
                    totalWeight += edge.weight;

                    cout << "v" << edge.src + 1 << " -- v" << edge.dest + 1
                        << " (вес: " << edge.weight << ")" << endl;

                    for (int j = 0; j < V; j++) {
                        if (parent[j] == set2) {
                            parent[j] = set1;
                        }
                    }
                    numComponents--;
                }
            }
        }
    }

    cout << "Общий вес: " << totalWeight << endl;
}

void addEdge(vector<Edge>& edges, int v1, int v2, int weight) {
    edges.push_back({ v1, v2, weight });
}

int main() {
    setlocale(LC_ALL, "RUS");
    const int V = 8;
    vector<Edge> edges;

    addEdge(edges, 0, 1, 2);
    addEdge(edges, 1, 2, 3);
    addEdge(edges, 0, 3, 8);
    addEdge(edges, 2, 7, 7);
    addEdge(edges, 3, 5, 3);
    addEdge(edges, 5, 6, 6);
    addEdge(edges, 6, 7, 9);
    addEdge(edges, 1, 4, 5);
    addEdge(edges, 4, 6, 4);
    addEdge(edges, 3, 4, 14);
    addEdge(edges, 0, 4, 2);
    addEdge(edges, 1, 3, 10);
    addEdge(edges, 3, 6, 1);
    addEdge(edges, 5, 4, 11);
    addEdge(edges, 2, 4, 12);
    addEdge(edges, 4, 7, 8);

    cout << "Граф с 8 вершинами:" << endl;
    cout << "Вершины: v1, v2, v3, v4, v5, v6, v7, v8" << endl;

    boruvkaMST(V, edges);

    return 0;
}