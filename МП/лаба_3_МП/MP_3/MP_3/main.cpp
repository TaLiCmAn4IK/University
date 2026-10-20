#include <iostream>
#include <vector>
#include <algorithm>
#include <climits>

using namespace std;

const int INF = INT_MAX / 2;
const int NUM_CITIES = 5;

const int distances[NUM_CITIES][NUM_CITIES] = {
    {INF, 12,  31, 15,  10},
    {13,  INF, 24,  28,  56},
    {19,  24,  INF, 72,  49},
    {30,  43,  36, INF, 24},
    {73,  44,  45, 24, INF}
};

const char cities[NUM_CITIES] = { '1', '2', '3', '4', '5' };

void printRoute(const vector<int>& route) {
    cout << "1";
    for (size_t i = 0; i < route.size(); i++) {
        cout << "->" << cities[route[i]];
    }
    cout << "->1";
}

bool nextPermutation(vector<int>& perm) {
    int i = perm.size() - 2;
    while (i >= 0 && perm[i] >= perm[i + 1]) {
        i--;
    }

    if (i < 0) {
        return false;
    }

    int j = perm.size() - 1;
    while (perm[j] <= perm[i]) {
        j--;
    }

    swap(perm[i], perm[j]);
    reverse(perm.begin() + i + 1, perm.end());

    return true;
}

int calculateRouteLength(const vector<int>& route) {
    int length = 0;
    int current = 0;
    for (int next : route) {
        length += distances[current][next];
        current = next;
    }

    length += distances[current][0];

    return length;
}

void solveTravelingSalesman() {

    cout << "    1   2   3   4   5\n";
    cout << "  -------------------\n";
    for (int i = 0; i < NUM_CITIES; i++) {
        cout << i + 1 << " | ";
        for (int j = 0; j < NUM_CITIES; j++) {
            if (distances[i][j] == INF)
                cout << "INF ";
            else
                cout << distances[i][j] << "  ";
        }
        cout << "\n";
    }
    cout << "\n";

    vector<int> cities_to_visit;
    for (int i = 1; i < NUM_CITIES; i++) {
        cities_to_visit.push_back(i);
    }

    sort(cities_to_visit.begin(), cities_to_visit.end());

    int min_length = INF;
    vector<int> best_route;
    int count = 0;


    do {
        count++;
        int length = calculateRouteLength(cities_to_visit);

        cout << "Маршрут " << count << ": ";
        printRoute(cities_to_visit);
        cout << " = " << length << "\n";

        if (length < min_length) {
            min_length = length;
            best_route = cities_to_visit;
        }

    } while (nextPermutation(cities_to_visit));

    cout << "Оптимальное решение:\n";
    cout << "Маршрут: ";
    printRoute(best_route);
    cout << "\nДлина маршрута: " << min_length << "\n";
    cout << "Всего рассмотрено маршрутов: " << count << " = (" << NUM_CITIES - 1 << "!)\n";
}

int main() {
    setlocale(LC_ALL, "Russian");
    solveTravelingSalesman();

    return 0;
}