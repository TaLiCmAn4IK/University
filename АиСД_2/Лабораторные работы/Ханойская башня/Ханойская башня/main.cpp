#include <iostream>
#include <vector>
#include <map>
using namespace std;

// Кэш для минимального количества ходов
map<pair<int, int>, long long> memo;

/*
 * Рекурсивная функция для вычисления минимального числа ходов
 * Алгоритм Фрейма–Стюарта:
 * minMoves(n, k) = min( 2 * minMoves(m, k) + minMoves(n-m, k-1) )
 */
long long minMoves(int n, int k) {
    if (n == 0) return 0;
    if (n == 1) return 1;
    if (k == 3) return (1LL << n) - 1; // классическая формула 2^n - 1

    auto key = make_pair(n, k);
    if (memo.count(key)) return memo[key];

    long long best = LLONG_MAX;
    for (int m = 1; m < n; m++) {
        best = min(best, 2 * minMoves(m, k) + minMoves(n - m, k - 1));
    }
    return memo[key] = best;
}

/*
 * Рекурсивная функция для вывода пошагового решения
 * n    - количество дисков
 * from - начальный стержень
 * to   - конечный стержень
 * pegs - список доступных стержней
 */
void hanoiK(int n, int from, int to, vector<int> pegs) {
    if (n == 0) return;

    if (n == 1) {
        cout << "Переместить диск 1 с " << from << " на " << to << " стержень" << endl;
        return;
    }

    if (pegs.size() == 3) { // классическая задача
        int aux = 0;
        for (int p : pegs) if (p != from && p != to) aux = p;
        hanoiK(n - 1, from, aux, { from, to, aux });
        cout << "Переместить диск " << n << " с " << from << " на " << to << " стержень" << endl;
        hanoiK(n - 1, aux, to, { from, to, aux });
        return;
    }

    // Находим оптимальное m
    long long best = LLONG_MAX;
    int bestM = 1;
    for (int m = 1; m < n; m++) {
        long long moves = 2 * minMoves(m, pegs.size()) + minMoves(n - m, pegs.size() - 1);
        if (moves < best) {
            best = moves;
            bestM = m;
        }
    }

    // Выбираем вспомогательный стержень
    int aux = -1;
    for (int p : pegs) {
        if (p != from && p != to) {
            aux = p;
            break;
        }
    }

    // Шаги алгоритма
    hanoiK(bestM, from, aux, pegs);                 // Переносим m дисков на вспомогательный
    vector<int> reducedPegs = pegs;
    reducedPegs.erase(find(reducedPegs.begin(), reducedPegs.end(), aux));
    hanoiK(n - bestM, from, to, reducedPegs);       // Переносим оставшиеся диски на конечный
    hanoiK(bestM, aux, to, pegs);                   // Переносим m дисков на конечный
}

int main() {
    setlocale(LC_ALL, "Russian");

    int N, K;
    cout << "Введите количество дисков N: ";
    cin >> N;
    cout << "Введите количество стержней K (>=3): ";
    cin >> K;

    if (K < 3) {
        cout << "Количество стержней должно быть >= 3!" << endl;
        return 1;
    }

    vector<int> pegs;
    for (int i = 1; i <= K; i++) pegs.push_back(i);

    cout << "\nМинимальное количество ходов: " << minMoves(N, K) << endl;
    cout << "Пошаговое решение:\n";
    hanoiK(N, 1, K, pegs);

    return 0;
}
