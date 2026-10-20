#include <iostream>
#include <vector>
#include <algorithm>
#include <random>
#include <ctime>
#include <map>
#include <string>
#include <limits>
#include <iomanip>
#include <set>

using namespace std;

class GeneticTSP {
private:
    // Параметры алгоритма:
    map<string, map<string, int>> graph;
    vector<string> cities;
    int populationSize;
    int offspringCount;
    int evolutionCount;
    vector<vector<string>> population;
    mt19937 rng;

public:
    // Конструктор:инициализирует все поля
    GeneticTSP(map<string, map<string, int>>& g, int popSize, int offCount, int evolCount)
        : graph(g), populationSize(popSize), offspringCount(offCount), evolutionCount(evolCount) {
        rng.seed(time(0));
        for (auto& pair : graph) {
            cities.push_back(pair.first);
        }
    }

    // Вычисление длины маршрута
    int calculateRouteLength(const vector<string>& route) {
        int totalLength = 0;
        for (size_t i = 0; i < route.size(); i++) {
            string cityFrom = route[i];
            string cityTo = route[(i + 1) % route.size()];
            totalLength += graph[cityFrom][cityTo];
        }
        return totalLength;
    }

    // Генерация начальной популяции
    void generateInitialPopulation() {
        population.clear();
        for (int i = 0; i < populationSize; i++) {
            vector<string> route = cities;
            shuffle(route.begin(), route.end(), rng);
            population.push_back(route);
        }
    }

    // селекция - выбор двух лучших родителей
    pair<vector<string>, vector<string>> selection() {
        vector<pair<vector<string>, int>> fitness;
        for (auto& route : population) {
            fitness.push_back({ route, calculateRouteLength(route) });
        }
        sort(fitness.begin(), fitness.end(),
            [](const pair<vector<string>, int>& a, const pair<vector<string>, int>& b) {
                return a.second < b.second;
            });
        return { fitness[0].first, fitness[1].first };
    }

    // Скрещивание (Order Crossover - OX)
    pair<vector<string>, vector<string>> crossover(const vector<string>& parent1,
        const vector<string>& parent2) {
        int size = parent1.size();
        uniform_int_distribution<int> dist(0, size - 1);

        int start = dist(rng);
        int end = dist(rng);
        if (start > end) swap(start, end);

        // Создание потомка 1
        vector<string> offspring1(size, "");
        for (int i = start; i <= end; i++) {
            offspring1[i] = parent1[i];
        }

        int pointer = (end + 1) % size;
        for (int i = (end + 1) % size; pointer != start; i = (i + 1) % size) {
            string city = parent2[i];
            if (find(offspring1.begin(), offspring1.end(), city) == offspring1.end()) {
                offspring1[pointer] = city;
                pointer = (pointer + 1) % size;
            }
        }

        // Создание потомка 2
        vector<string> offspring2(size, "");
        for (int i = start; i <= end; i++) {
            offspring2[i] = parent2[i];
        }

        pointer = (end + 1) % size;
        for (int i = (end + 1) % size; pointer != start; i = (i + 1) % size) {
            string city = parent1[i];
            if (find(offspring2.begin(), offspring2.end(), city) == offspring2.end()) {
                offspring2[pointer] = city;
                pointer = (pointer + 1) % size;
            }
        }

        return { offspring1, offspring2 };
    }

    // Мутация - обмен позиций двух случайных городов
    vector<string> mutation(vector<string> route) {
        uniform_real_distribution<double> prob(0.0, 1.0);
        if (prob(rng) < 0.1) {  // Вероятность мутации 10%
            uniform_int_distribution<int> dist(0, route.size() - 1);
            int idx1 = dist(rng);
            int idx2 = dist(rng);
            swap(route[idx1], route[idx2]);
        }
        return route;
    }

    // Один шаг эволюции
    void evolve() {
        vector<vector<string>> newPopulation;
        // Элитизм - сохраняем лучшую особь
        vector<pair<vector<string>, int>> fitness;
        for (auto& route : population) {
            fitness.push_back({ route, calculateRouteLength(route) });
        }
        sort(fitness.begin(), fitness.end(),
            [](const pair<vector<string>, int>& a, const pair<vector<string>, int>& b) {
                return a.second < b.second;
            });
        newPopulation.push_back(fitness[0].first);

        // Создание новых потомков
        while (newPopulation.size() < (size_t)populationSize) {
            auto parents = selection();
            auto offspring = crossover(parents.first, parents.second);

            offspring.first = mutation(offspring.first);
            offspring.second = mutation(offspring.second);

            newPopulation.push_back(offspring.first);
            if (newPopulation.size() < (size_t)populationSize) {
                newPopulation.push_back(offspring.second);
            }
        }

        population = newPopulation;
    }

    // Запуск генетического алгоритма
    pair<vector<string>, int> run(bool verbose = true) {
        if (verbose) {
            cout << string(60, '=') << endl;
            cout << "ГЕНЕТИЧЕСКИЙ АЛГОРИТМ - ЗАДАЧА КОММИВОЯЖЁРА" << endl;
            cout << string(60, '=') << endl;
            cout << "Размер популяции: " << populationSize << endl;
            cout << "Количество потомков: " << offspringCount << endl;
            cout << "Количество эволюций: " << evolutionCount << endl;
            cout << string(60, '=') << endl;
        }

        // Генерация начальной популяции
        generateInitialPopulation();

        // Начальная популяция
        vector<pair<vector<string>, int>> fitness;
        for (auto& route : population) {
            fitness.push_back({ route, calculateRouteLength(route) });
        }
        sort(fitness.begin(), fitness.end(),
            [](const pair<vector<string>, int>& a, const pair<vector<string>, int>& b) {
                return a.second < b.second;
            });

        if (verbose) {
            cout << "\nПопуляция 0 (начальная):" << endl;
            cout << "Минимальный маршрут: ";
            for (size_t i = 0; i < fitness[0].first.size(); i++) {
                cout << fitness[0].first[i];
                if (i < fitness[0].first.size() - 1) cout << " -> ";
            }
            cout << " -> " << fitness[0].first[0] << endl;
            cout << "Длина маршрута: " << fitness[0].second << endl;
            cout << string(60, '-') << endl;
        }

        // Эволюция
        for (int gen = 1; gen <= evolutionCount; gen++) {
            evolve();

            fitness.clear();
            for (auto& route : population) {
                fitness.push_back({ route, calculateRouteLength(route) });
            }
            sort(fitness.begin(), fitness.end(),
                [](const pair<vector<string>, int>& a, const pair<vector<string>, int>& b) {
                    return a.second < b.second;
                });

            if (verbose && (gen % 10 == 0 || gen == evolutionCount)) {
                cout << "\nПопуляция " << gen << ":" << endl;
                cout << "Минимальный маршрут: ";
                for (size_t i = 0; i < fitness[0].first.size(); i++) {
                    cout << fitness[0].first[i];
                    if (i < fitness[0].first.size() - 1) cout << " -> ";
                }
                cout << " -> " << fitness[0].first[0] << endl;
                cout << "Длина маршрута: " << fitness[0].second << endl;
                cout << string(60, '-') << endl;
            }
        }

        if (verbose) {
            cout << "\n" << string(60, '=') << endl;
            cout << "РЕЗУЛЬТАТ ГЕНЕТИЧЕСКОГО АЛГОРИТМА:" << endl;
            cout << "Оптимальный маршрут: ";
            for (size_t i = 0; i < fitness[0].first.size(); i++) {
                cout << fitness[0].first[i];
                if (i < fitness[0].first.size() - 1) cout << " -> ";
            }
            cout << " -> " << fitness[0].first[0] << endl;
            cout << "Минимальная длина: " << fitness[0].second << endl;
            cout << string(60, '=') << endl;
        }

        return { fitness[0].first, fitness[0].second };
    }
};

// Жадный алгоритм для задачи коммивояжера
class GreedyTSP {
private:
    map<string, map<string, int>> graph;

public:
    GreedyTSP(map<string, map<string, int>>& g) : graph(g) {}

    // Жадный алгоритм с указанием стартового города
    pair<vector<string>, int> solveFromCity(const string& startCity) {
        vector<string> tour = { startCity };
        string current = startCity;
        set<string> visited = { startCity };

        while (visited.size() < graph.size()) {
            string nextCity;
            int minDist = numeric_limits<int>::max();

            // Ищем ближайший непосещённый город
            for (auto& neighbor : graph[current]) {
                if (visited.find(neighbor.first) == visited.end() &&
                    neighbor.second < minDist && neighbor.first != current) {
                    minDist = neighbor.second;
                    nextCity = neighbor.first;
                }
            }

            if (!nextCity.empty()) {
                tour.push_back(nextCity);
                visited.insert(nextCity);
                current = nextCity;
            }
            else {
                break; // Если не нашли следующий город
            }
        }

        // Вычисляем длину маршрута
        int totalLength = 0;
        for (size_t i = 0; i < tour.size(); i++) {
            string cityFrom = tour[i];
            string cityTo = tour[(i + 1) % tour.size()];
            totalLength += graph[cityFrom][cityTo];
        }

        return { tour, totalLength };
    }

    // Запуск жадного алгоритма из всех городов и выбор лучшего результата
    pair<vector<string>, int> solve() {
        vector<pair<vector<string>, int>> allSolutions;

        for (auto& city : graph) {
            auto solution = solveFromCity(city.first);
            allSolutions.push_back(solution);
        }

        // Выбираем лучшее решение
        auto bestSolution = min_element(allSolutions.begin(), allSolutions.end(),
            [](const pair<vector<string>, int>& a, const pair<vector<string>, int>& b) {
                return a.second < b.second;
            });

        return *bestSolution;
    }

    // Запуск жадного алгоритма с выводом
    void run() {
        cout << string(60, '=') << endl;
        cout << "ЖАДНЫЙ АЛГОРИТМ - ЗАДАЧА КОММИВОЯЖЁРА" << endl;
        cout << string(60, '=') << endl;

        auto result = solve();

        cout << "Оптимальный маршрут: ";
        for (size_t i = 0; i < result.first.size(); i++) {
            cout << result.first[i];
            if (i < result.first.size() - 1) cout << " -> ";
        }
        cout << " -> " << result.first[0] << endl;
        cout << "Длина маршрута: " << result.second << endl;
        cout << string(60, '=') << endl;
    }
};

// Функция сравнения алгоритмов
void compareAlgorithms(map<string, map<string, int>>& graph,
    int popSize, int offCount, int evolCount) {

    cout << string(80, '=') << endl;
    cout << "СРАВНЕНИЕ АЛГОРИТМОВ: ГЕНЕТИЧЕСКИЙ vs ЖАДНЫЙ" << endl;
    cout << string(80, '=') << endl;

    // Запуск генетического алгоритма
    cout << "\n>>> ЗАПУСК ГЕНЕТИЧЕСКОГО АЛГОРИТМА..." << endl;
    clock_t startTimeGA = clock();
    GeneticTSP ga(graph, popSize, offCount, evolCount);
    auto gaResult = ga.run(false);
    clock_t endTimeGA = clock();
    double timeGA = double(endTimeGA - startTimeGA) / CLOCKS_PER_SEC;

    // Запуск жадного алгоритма
    cout << "\n>>> ЗАПУСК ЖАДНОГО АЛГОРИТМА..." << endl;
    clock_t startTimeGreedy = clock();
    GreedyTSP greedy(graph);
    auto greedyResult = greedy.solve();
    clock_t endTimeGreedy = clock();
    double timeGreedy = double(endTimeGreedy - startTimeGreedy) / CLOCKS_PER_SEC;

    // Вывод результатов сравнения
    cout << string(80, '=') << endl;
    cout << "РЕЗУЛЬТАТЫ СРАВНЕНИЯ:" << endl;
    cout << string(80, '=') << endl;

    cout << "\nГЕНЕТИЧЕСКИЙ АЛГОРИТМ:" << endl;
    cout << "Маршрут: ";
    for (size_t i = 0; i < gaResult.first.size(); i++) {
        cout << gaResult.first[i];
        if (i < gaResult.first.size() - 1) cout << " -> ";
    }
    cout << " -> " << gaResult.first[0] << endl;
    cout << "Длина маршрута: " << gaResult.second << endl;
    cout << "Время выполнения: " << timeGA << " секунд" << endl;

    cout << "\nЖАДНЫЙ АЛГОРИТМ:" << endl;
    cout << "Маршрут: ";
    for (size_t i = 0; i < greedyResult.first.size(); i++) {
        cout << greedyResult.first[i];
        if (i < greedyResult.first.size() - 1) cout << " -> ";
    }
    cout << " -> " << greedyResult.first[0] << endl;
    cout << "Длина маршрута: " << greedyResult.second << endl;
    cout << "Время выполнения: " << timeGreedy << " секунд" << endl;

    cout << "\n" << string(80, '=') << endl;
    cout << "СРАВНИТЕЛЬНЫЙ АНАЛИЗ:" << endl;
    cout << string(80, '=') << endl;

    cout << fixed << setprecision(2);
    int lengthDifference = greedyResult.second - gaResult.second;
    cout << "Разница в длине маршрута: " << abs(lengthDifference);
    cout << " (в пользу " << (gaResult.second < greedyResult.second ? "генетического" : "жадного") << " алгоритма)" << endl;

    if (greedyResult.second > 0) {
        double improvement = ((double)lengthDifference / greedyResult.second) * 100;
        cout << "Относительное улучшение: " << improvement << "%" << endl;
    }

    double timeDifference = timeGA - timeGreedy;
    cout << "Разница во времени: " << abs(timeDifference) << " секунд";
    cout << " (" << (timeGA < timeGreedy ? "генетический" : "жадный") << " алгоритм быстрее)" << endl;

    // Определение победителя
    cout << "\n" << string(80, '=') << endl;
    if (gaResult.second < greedyResult.second) {
        cout << "  ГЕНЕТИЧЕСКИЙ АЛГОРИТМ ПОБЕДИЛ!" << endl;
        cout << "  Найден более короткий маршрут" << endl;
    }
    else if (gaResult.second > greedyResult.second) {
        cout << "  ЖАДНЫЙ АЛГОРИТМ ПОБЕДИЛ!" << endl;
        cout << "  Найден более короткий маршрут" << endl;
    }
    else {
        cout << "  НИЧЬЯ!" << endl;
        cout << "  Оба алгоритма нашли маршруты одинаковой длины" << endl;
    }
    cout << string(80, '=') << endl;
}

int main() {
    setlocale(LC_ALL, "rus");

    // Граф с 8 городами
    map<string, map<string, int>> graph = {
        {"A", {{"A", 0}, {"B", 20}, {"C", 42}, {"D", 35}, {"E", 30}, {"F", 34}, {"G", 12}, {"H", 55}}},
        {"B", {{"A", 20}, {"B", 0}, {"C", 30}, {"D", 34}, {"E", 40}, {"F", 25}, {"G", 15}, {"H", 45}}},
        {"C", {{"A", 42}, {"B", 30}, {"C", 0}, {"D", 12}, {"E", 20}, {"F", 30}, {"G", 35}, {"H", 25}}},
        {"D", {{"A", 35}, {"B", 34}, {"C", 12}, {"D", 0}, {"E", 15}, {"F", 18}, {"G", 25}, {"H", 30}}},
        {"E", {{"A", 30}, {"B", 40}, {"C", 20}, {"D", 15}, {"E", 0}, {"F", 22}, {"G", 28}, {"H", 18}}},
        {"F", {{"A", 34}, {"B", 25}, {"C", 30}, {"D", 18}, {"E", 22}, {"F", 0}, {"G", 20}, {"H", 24}}},
        {"G", {{"A", 12}, {"B", 15}, {"C", 35}, {"D", 25}, {"E", 28}, {"F", 20}, {"G", 0}, {"H", 40}}},
        {"H", {{"A", 55}, {"B", 45}, {"C", 25}, {"D", 30}, {"E", 18}, {"F", 24}, {"G", 40}, {"H", 0}}}
    };

    int choice;
    cout << "Выберите режим работы:" << endl;
    cout << "1 - Только генетический алгоритм" << endl;
    cout << "2 - Только жадный алгоритм" << endl;
    cout << "3 - Сравнение обоих алгоритмов" << endl;
    cout << "Ваш выбор: ";
    cin >> choice;

    if (choice == 1) {
        int populationSize, offspringCount, evolutionCount;
        cout << "Введите параметры генетического алгоритма:" << endl;
        cout << "Размер популяции: ";
        cin >> populationSize;
        cout << "Количество потомков при скрещивании: ";
        cin >> offspringCount;
        cout << "Количество эволюций: ";
        cin >> evolutionCount;

        GeneticTSP ga(graph, populationSize, offspringCount, evolutionCount);
        ga.run();
    }
    else if (choice == 2) {
        GreedyTSP greedy(graph);
        greedy.run();
    }
    else if (choice == 3) {
        int populationSize, offspringCount, evolutionCount;
        cout << "Введите параметры генетического алгоритма для сравнения:" << endl;
        cout << "Размер популяции: ";
        cin >> populationSize;
        cout << "Количество потомков при скрещивании: ";
        cin >> offspringCount;
        cout << "Количество эволюций: ";
        cin >> evolutionCount;

        compareAlgorithms(graph, populationSize, offspringCount, evolutionCount);
    }
    else {
        cout << "Неверный выбор!" << endl;
    }

    return 0;
}