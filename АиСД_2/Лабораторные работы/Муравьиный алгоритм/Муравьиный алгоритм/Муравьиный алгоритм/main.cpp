#include <iostream>
#include <vector>
#include <random>
#include <string>
#include <ctime>
#include <algorithm>
#include <cmath>
#include <climits>
#include <iomanip>
#include <numeric>

using namespace std;

#define MAX_DIST 100

typedef vector<vector<int>> Matrix;
typedef vector<vector<double>> PheromoneMatrix;
typedef vector<int> Path;

struct Way {
    Path way;
    double dist;

    void clear() {
        way.clear();
        dist = 0.0;
    }

    void push_back(int value) {
        way.push_back(value);
    }

    Way() {
        this->dist = 0.0;
    }

    string to_string() {
        string text = "Путь: ";

        for (size_t i = 0; i < way.size(); i++) {
            text += std::to_string(way[i] + 1);
            if (i != way.size() - 1) text += " -> ";
        }

        text += "\nДлина пути: " + std::to_string(this->dist) + "\n";

        return text;
    }
};

// Функции для муравьиного алгоритма
Matrix createMatrix(size_t N);
PheromoneMatrix initPheromones(size_t N, double initValue);
void outputMatrix(const Matrix& matrix);
Way antAlgorithm(
    const Matrix& graph,
    PheromoneMatrix& pheromones,
    size_t nIters,
    double alpha,
    double beta,
    double evaporation,
    double Q
);

// Функции для имитации отжига
class SimulatedAnnealing {
private:
    double initialTemperature;   // Начальная температура системы (определяет вероятность принятия худших решений)
    double coolingRate;          // Коэффициент охлаждения (скорость уменьшения температуры)
    int maxIterations;           // Максимальное число итераций алгоритма
    mt19937 gen;                 // Генератор случайных чисел (Mersenne Twister)

public:
    // Конструктор инициализирует параметры алгоритма и генератор случайных чисел
    SimulatedAnnealing(double temp, double cooling, int iterations)
        : initialTemperature(temp), coolingRate(cooling),
        maxIterations(iterations), gen(static_cast<unsigned int>(time(nullptr))) {
    }

    // Генерация случайного числа в диапазоне [0.0, 1.0]
    double randomDouble() {
        uniform_real_distribution<double> dist(0.0, 1.0);
        return dist(gen);
    }

    // Вычисление длины полного цикла (замкнутого пути) для заданного маршрута
    // Суммирует расстояния между последовательными вершинами и замыкает цикл
    double calculatePathLength(const Path& path, const Matrix& graph) {
        double length = 0.0;
        for (size_t i = 0; i < path.size() - 1; i++) {
            length += graph[path[i]][path[i + 1]];
        }
        length += graph[path.back()][path[0]];  // Замыкание цикла: последняя → первая вершина
        return length;
    }

    // Генерация случайного начального маршрута (перестановки вершин)
    Path generateRandomPath(size_t N) {
        Path path(N);
        iota(path.begin(), path.end(), 0);  // Заполнение 0,1,2,...,N-1
        shuffle(path.begin(), path.end(), gen);  // Случайное перемешивание
        return path;
    }

    // Создание соседнего решения путём обмена двух случайных вершин в маршруте
    // (Один из методов генерации соседей для задачи коммивояжера)
    Path generateNeighbor(const Path& currentPath) {
        Path newPath = currentPath;

        uniform_int_distribution<size_t> dist(0, currentPath.size() - 1);
        size_t i = dist(gen);
        size_t j = dist(gen);

        while (i == j) {  // Гарантируем, что индексы разные
            j = dist(gen);
        }

        swap(newPath[i], newPath[j]);  // Обмен двух вершин местами
        return newPath;
    }

    // Вычисление вероятности принятия нового решения (метрополисовский критерий)
    // Если новое решение лучше (стоимость меньше) - вероятность 100%
    // Если хуже - вероятность exp(-ΔE/T), где ΔE = newCost - currentCost
    double acceptanceProbability(double currentCost, double newCost, double temperature) {
        if (newCost < currentCost) {
            return 1.0;  // Всегда принимаем улучшения
        }
        // Принимаем ухудшение с вероятностью, зависящей от температуры
        return exp((currentCost - newCost) / temperature);
    }

    // Основная функция алгоритма имитации отжига
    Way solve(const Matrix& graph) {
        size_t N = graph.size();

        // Инициализация начального решения
        Path currentPath = generateRandomPath(N);
        Path bestPath = currentPath;

        double currentCost = calculatePathLength(currentPath, graph);
        double bestCost = currentCost;

        double temperature = initialTemperature;  // Начальная температура

        cout << "\n=== АЛГОРИТМ ИМИТАЦИИ ОТЖИГА ===" << endl;
        cout << "Начальная температура: " << temperature << endl;
        cout << "Коэффициент охлаждения: " << coolingRate << endl;
        cout << "Максимальное число итераций: " << maxIterations << endl;
        cout << "Начальная длина маршрута: " << currentCost << endl;
        cout << endl;

        int iteration = 0;
        // Основной цикл алгоритма: выполняется пока не достигнута конечная температура или лимит итераций
        while (temperature > 1e-5 && iteration < maxIterations) {
            // Генерация соседнего решения
            Path newPath = generateNeighbor(currentPath);
            double newCost = calculatePathLength(newPath, graph);

            // Метрополисовский критерий: принимаем решение с определенной вероятностью
            if (acceptanceProbability(currentCost, newCost, temperature) > randomDouble()) {
                currentPath = newPath;
                currentCost = newCost;

                // Обновляем лучшее найденное решение
                if (currentCost < bestCost) {
                    bestPath = currentPath;
                    bestCost = currentCost;

                    // Периодический вывод прогресса (каждые 100 итераций)
                    if (iteration % 100 == 0) {
                        cout << "Итерация " << iteration
                            << ", Температура: " << scientific << temperature
                            << ", Лучшая длина: " << fixed << bestCost << endl;
                    }
                }
            }

            // Охлаждение системы (экспоненциальное затухание температуры)
            temperature *= coolingRate;
            iteration++;
        }

        // Формирование результата
        Way result;
        result.way = bestPath;
        result.dist = bestCost;

        cout << "\nРезультат имитации отжига:" << endl;
        cout << result.to_string() << endl;

        return result;
    }
};
// Функция для сравнения алгоритмов
void compareAlgorithms(const Way& antResult, const Way& saResult) {
    cout << "\n" << string(60, '=') << endl;
    cout << "               СРАВНИТЕЛЬНЫЙ АНАЛИЗ АЛГОРИТМОВ" << endl;
    cout << string(60, '=') << endl;

    cout << fixed << setprecision(2);
    cout << setw(30) << "Параметр"
        << setw(15) << "Муравьиный"
        << setw(15) << "Имитация отжига" << endl;
    cout << string(60, '-') << endl;

    cout << setw(30) << "Длина пути"
        << setw(15) << antResult.dist
        << setw(15) << saResult.dist << endl;

    string winner;
    double difference = fabs(antResult.dist - saResult.dist);

    if (antResult.dist < saResult.dist) {
        winner = "Муравьиный алгоритм";
    }
    else if (saResult.dist < antResult.dist) {
        winner = "Имитация отжига";
    }
    else {
        winner = "Одинаково хороши";
    }

    cout << string(60, '-') << endl;
    cout << "Победитель: " << winner << endl;
    cout << "Разница: " << difference << endl;

    if (difference < 1e-6) {
        cout << "Результаты практически идентичны!" << endl;
    }
    else if (difference < 5.0) {
        cout << "Разница незначительная" << endl;
    }
    else {
        cout << "Заметная разница в качестве" << endl;
    }

    // Вывод маршрутов для сравнения
    cout << "\nМуравьиный алгоритм:" << endl;
    cout << "Маршрут: ";
    for (size_t i = 0; i < antResult.way.size(); i++) {
        cout << antResult.way[i] + 1;
        if (i != antResult.way.size() - 1) cout << " -> ";
    }

    cout << "\n\nИмитация отжига:" << endl;
    cout << "Маршрут: ";
    for (size_t i = 0; i < saResult.way.size(); i++) {
        cout << saResult.way[i] + 1;
        if (i != saResult.way.size() - 1) cout << " -> ";
    }

    cout << endl << string(60, '=') << endl;
}

// Вспомогательные функции для муравьиного алгоритма
int selectNextCity(const vector<double>& probabilities, const vector<bool>& visited) {
    double sum = 0.0;
    for (size_t i = 0; i < probabilities.size(); i++) {
        if (!visited[i]) {
            sum += probabilities[i];
        }
    }

    if (sum == 0.0) {
        vector<int> available;
        for (size_t i = 0; i < visited.size(); i++) {
            if (!visited[i]) {
                available.push_back(i);
            }
        }
        if (!available.empty()) {
            return available[rand() % available.size()];
        }
        return -1;
    }

    double r = (double)rand() / RAND_MAX;
    double cumulative = 0.0;

    for (size_t i = 0; i < probabilities.size(); i++) {
        if (!visited[i]) {
            cumulative += probabilities[i] / sum;
            if (r <= cumulative) {
                return i;
            }
        }
    }

    for (int i = visited.size() - 1; i >= 0; i--) {
        if (!visited[i]) return i;
    }

    return -1;
}

Way constructAntPath(const Matrix& graph, const PheromoneMatrix& pheromones,
    int startCity, double alpha, double beta) {
    size_t N = graph.size();
    Way antWay;
    vector<bool> visited(N, false);

    int current = startCity;
    antWay.push_back(current);
    visited[current] = true;

    for (size_t step = 0; step < N - 1; step++) {
        vector<double> probabilities(N, 0.0);

        for (size_t i = 0; i < N; i++) {
            if (!visited[i] && i != current) {
                double tau = pow(pheromones[current][i], alpha);
                double eta = 1.0 / (graph[current][i] + 1e-10);
                eta = pow(eta, beta);
                probabilities[i] = tau * eta;
            }
        }

        int nextCity = selectNextCity(probabilities, visited);
        if (nextCity == -1) break;

        antWay.dist += graph[current][nextCity];
        antWay.push_back(nextCity);
        visited[nextCity] = true;
        current = nextCity;
    }

    antWay.dist += graph[current][startCity];
    antWay.push_back(startCity);

    return antWay;
}

void updatePheromones(PheromoneMatrix& pheromones, const vector<Way>& antPaths,
    double evaporation, double Q) {
    size_t N = pheromones.size();

    for (size_t i = 0; i < N; i++) {
        for (size_t j = 0; j < N; j++) {
            pheromones[i][j] *= (1.0 - evaporation);
        }
    }

    for (const auto& antPath : antPaths) {
        double delta = Q / antPath.dist;

        for (size_t i = 0; i < antPath.way.size() - 1; i++) {
            int from = antPath.way[i];
            int to = antPath.way[i + 1];
            pheromones[from][to] += delta;
            pheromones[to][from] += delta;
        }
    }
}

Matrix createMatrix(size_t N) {
    Matrix matrix(N, vector<int>(N, 0));

    for (size_t i = 0; i < N; i++) {
        for (size_t j = i + 1; j < N; j++) {
            int dist = rand() % MAX_DIST + 1;
            matrix[i][j] = dist;
            matrix[j][i] = dist;
        }
    }

    return matrix;
}

PheromoneMatrix initPheromones(size_t N, double initValue) {
    return PheromoneMatrix(N, vector<double>(N, initValue));
}

void outputMatrix(const Matrix& matrix) {
    for (size_t i = 0; i < matrix.size(); i++) {
        for (size_t j = 0; j < matrix[i].size(); j++) {
            cout << setw(4) << matrix[i][j] << " ";
        }
        cout << endl;
    }
}

Way antAlgorithm(
    const Matrix& graph,
    PheromoneMatrix& pheromones,
    size_t nIters,
    double alpha,
    double beta,
    double evaporation,
    double Q) {

    size_t N = graph.size();
    Way globalBest;
    globalBest.dist = INT_MAX;

    size_t numAnts = N;

    for (size_t iter = 0; iter < nIters; iter++) {
        vector<Way> antPaths;
        Way iterationBest;
        iterationBest.dist = INT_MAX;

        for (size_t ant = 0; ant < numAnts; ant++) {
            Way antPath = constructAntPath(graph, pheromones, ant, alpha, beta);
            antPaths.push_back(antPath);

            if (antPath.dist < iterationBest.dist) {
                iterationBest = antPath;
            }
        }

        updatePheromones(pheromones, antPaths, evaporation, Q);

        if (iterationBest.dist < globalBest.dist) {
            globalBest = iterationBest;
        }

        cout << "\nИтерация " << iter + 1 << ":" << endl;
        cout << "Лучший маршрут итерации: ";
        for (size_t i = 0; i < iterationBest.way.size(); i++) {
            cout << iterationBest.way[i] + 1;
            if (i != iterationBest.way.size() - 1) cout << " -> ";
        }
        cout << "\nРасстояние: " << iterationBest.dist << endl;
        cout << "Глобально лучшее расстояние: " << globalBest.dist << endl;
    }

    return globalBest;
}

int main() {
    setlocale(LC_ALL, "Russian");
    srand(time(NULL));

    cout << "=== СРАВНЕНИЕ АЛГОРИТМОВ: МУРАВЬИНЫЙ И ИМИТАЦИЯ ОТЖИГА ===" << endl;
    cout << "=== РЕШЕНИЕ ЗАДАЧИ КОММИВОЯЖЕРА ===\n" << endl;

    size_t N;
    cout << "Введите количество городов: ";
    cin >> N;

    Matrix cities = createMatrix(N);
    cout << "\nМатрица расстояний:\n";
    outputMatrix(cities);

    // Параметры муравьиного алгоритма
    double initPheromone, alpha, beta, evaporation, Q;
    size_t nIters;

    cout << "\n--- ПАРАМЕТРЫ МУРАВЬИНОГО АЛГОРИТМА ---" << endl;
    cout << "Введите начальное значение феромонов (например, 1.0): ";
    cin >> initPheromone;
    cout << "Введите значение alpha (влияние феромонов, обычно 1): ";
    cin >> alpha;
    cout << "Введите значение beta (влияние расстояния, обычно 2-5): ";
    cin >> beta;
    cout << "Введите коэффициент испарения феромонов (0-1, обычно 0.5): ";
    cin >> evaporation;
    cout << "Введите количество феромонов Q (обычно 100): ";
    cin >> Q;
    cout << "Введите количество итераций: ";
    cin >> nIters;

    PheromoneMatrix pheromones = initPheromones(N, initPheromone);

    // Запуск муравьиного алгоритма
    cout << "\n" << string(50, '=') << endl;
    cout << "ЗАПУСК МУРАВЬИНОГО АЛГОРИТМА" << endl;
    cout << string(50, '=') << endl;

    Way antBestWay = antAlgorithm(cities, pheromones, nIters, alpha, beta, evaporation, Q);

    // Параметры имитации отжига
    double initialTemp, coolingRate;
    int saIterations;

    cout << "\n--- ПАРАМЕТРЫ ИМИТАЦИИ ОТЖИГА ---" << endl;
    cout << "Введите начальную температуру (например, 1000): ";
    cin >> initialTemp;
    cout << "Введите коэффициент охлаждения (0-1, например 0.99): ";
    cin >> coolingRate;
    cout << "Введите количество итераций имитации отжига: ";
    cin >> saIterations;

    // Запуск имитации отжига
    SimulatedAnnealing sa(initialTemp, coolingRate, saIterations);
    Way saBestWay = sa.solve(cities);

    // Сравнение результатов
    compareAlgorithms(antBestWay, saBestWay);

    // Статистика по алгоритмам
    cout << "\n=== СТАТИСТИКА ===" << endl;
    cout << "Количество городов: " << N << endl;
    cout << "Размер матрицы расстояний: " << N << "x" << N << endl;
    cout << "Максимальное расстояние между городами: " << MAX_DIST << endl;
    cout << "\nМуравьиный алгоритм выполнил: " << nIters << " итераций" << endl;
    cout << "Имитация отжига выполнила: " << saIterations << " итераций" << endl;

    cout << "\n=== РЕКОМЕНДАЦИИ ===" << endl;
    if (antBestWay.dist < saBestWay.dist) {
        cout << "Для данной задачи муравьиный алгоритм показал лучший результат." << endl;
        cout << "Рекомендуется использовать его для точных вычислений." << endl;
    }
    else if (saBestWay.dist < antBestWay.dist) {
        cout << "Для данной задачи имитация отжига показала лучший результат." << endl;
        cout << "Рекомендуется использовать её для быстрых вычислений." << endl;
    }
    else {
        cout << "Оба алгоритма показали одинаковые результаты." << endl;
        cout << "Для скорости используйте имитацию отжига, для точности - муравьиный." << endl;
    }

    return 0;
}