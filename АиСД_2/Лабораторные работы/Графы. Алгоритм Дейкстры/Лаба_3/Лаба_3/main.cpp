#include <iostream>
#include <vector>
#include <queue>
#include <climits>
#include <unordered_map>
#include <string>

using namespace std;

typedef pair<int, int> pii;

void dijkstra(const vector<vector<pii>>& graph, int start, vector<int>& distances) {
    int n = graph.size();
    distances.assign(n, INT_MAX);
    distances[start] = 0;
    priority_queue<pii, vector<pii>, greater<pii>> pq;
    pq.push({ 0, start });

    while (!pq.empty()) {
        int current_dist = pq.top().first;
        int u = pq.top().second;
        pq.pop();

        if (current_dist > distances[u]) continue;

        for (const auto& edge : graph[u]) {
            int v = edge.first;
            int weight = edge.second;
            int dist = current_dist + weight;

            if (dist < distances[v]) {
                distances[v] = dist;
                pq.push({ dist, v });
            }
        }
    }
}

int main() {
    setlocale(LC_ALL, "rus");
    unordered_map<string, int> vertex_index = {
        {"A", 0}, {"B", 1}, {"C", 2}, {"D", 3}, {"E", 4}, {"F", 5}, {"G", 6}, {"H", 7}, {"I", 8}
    };
    vector<string> index_vertex = { "A", "B", "C", "D", "E", "F", "G", "H", "I" };

    vector<vector<pii>> graph(9);

    // Правильное добавление рёбер на основе изображения
    // Вершина A
    graph[vertex_index["A"]].push_back({ vertex_index["B"], 7 });
    graph[vertex_index["A"]].push_back({ vertex_index["C"], 10 });


    // Вершина B
    graph[vertex_index["B"]].push_back({ vertex_index["G"], 27 });
    graph[vertex_index["B"]].push_back({ vertex_index["F"], 9 });
    graph[vertex_index["B"]].push_back({ vertex_index["A"], 7 });
    // Вершина C
    graph[vertex_index["C"]].push_back({ vertex_index["F"], 8 });
    graph[vertex_index["C"]].push_back({ vertex_index["E"], 31 });
    graph[vertex_index["C"]].push_back({ vertex_index["A"], 10 });


    // Вершина D
    graph[vertex_index["D"]].push_back({ vertex_index["H"], 17 });
    graph[vertex_index["D"]].push_back({ vertex_index["E"], 32 });
    graph[vertex_index["D"]].push_back({ vertex_index["I"], 21 });
    // Вершина E
    graph[vertex_index["E"]].push_back({ vertex_index["C"], 31 });
    graph[vertex_index["E"]].push_back({ vertex_index["D"], 32 });


    // Вершина F
    graph[vertex_index["F"]].push_back({ vertex_index["B"], 9 });
    graph[vertex_index["F"]].push_back({ vertex_index["C"], 8 });
    graph[vertex_index["F"]].push_back({ vertex_index["H"], 11 });

    // Вершина G
    graph[vertex_index["G"]].push_back({ vertex_index["B"], 27 });
    graph[vertex_index["G"]].push_back({ vertex_index["I"], 15 });

    // Вершина H
    graph[vertex_index["H"]].push_back({ vertex_index["F"], 11 });
    graph[vertex_index["H"]].push_back({ vertex_index["I"], 15 });
    graph[vertex_index["H"]].push_back({ vertex_index["D"], 17 });

    // Вершина I
    graph[vertex_index["I"]].push_back({ vertex_index["H"], 15 });
    graph[vertex_index["I"]].push_back({ vertex_index["D"], 21 });
    graph[vertex_index["I"]].push_back({ vertex_index["G"], 15 });

    string start_vertex;
    cout << "Введите стартовую вершину (A-I): ";
    cin >> start_vertex;

    if (vertex_index.find(start_vertex) == vertex_index.end()) {
        cout << "Неверная вершина!" << endl;
        return 1;
    }

    int start_index = vertex_index[start_vertex];
    vector<int> distances;
    dijkstra(graph, start_index, distances);

    cout << "Расстояния от вершины " << start_vertex << " до:" << endl;
    for (int i = 0; i < distances.size(); ++i) {
        cout << index_vertex[i] << ": ";
        if (distances[i] == INT_MAX) {
            cout << "недостижима";
        }
        else {
            cout << distances[i];
        }
        cout << endl;
    }

    return 0;
}