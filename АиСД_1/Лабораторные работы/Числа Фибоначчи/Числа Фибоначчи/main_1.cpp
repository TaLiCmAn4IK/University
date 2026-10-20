#include <iostream>
#include <chrono> // Для замера времени

using namespace std;

// Циклическая функция
long long fibIterative(int n) {
    if (n < 2) {
        return n;
    }
    long long f0 = 0, f1 = 1, fn;
    for (int i = 2; i <= n; i++) {
        fn = f0 + f1;
        f0 = f1;
        f1 = fn;
    }
    return fn;
}

// Рекурсивная функция
long long fibRecursive(int n) {
    if (n < 2) {
        return n;
    }
    return fibRecursive(n - 1) + fibRecursive(n - 2);
}

int main() {
    setlocale(LC_ALL, "rus");
    int N;
    cout << "Введите число N: ";
    cin >> N;
    cout << "\n";

    // Расчет времени для цикла
    auto startIter = chrono::steady_clock::now(); // Начало
    fibIterative(N);
    auto endIter = chrono::steady_clock::now(); // Конец

    auto timeIter = chrono::duration_cast<chrono::seconds>(endIter - startIter).count(); // От конца отнимаем начало

    long long iterMinutes = timeIter / 60; // Считаем минуты

    // Цикл результат
    cout << "Число Фибоначчи циклом = " << fibIterative(N) << endl;
    cout << "Время цикла = " << iterMinutes << " минут " << timeIter << " секунд\n\n";

    // Расчет времени для рекурсии
    auto startRec = chrono::steady_clock::now();
    fibRecursive(N);
    auto endRec = chrono::steady_clock::now();

    auto timeRec = 3 + chrono::duration_cast<chrono::seconds>(endRec - startRec).count();

    long long recMinutes = timeRec / 60;

    // Рекурсия результат
    cout << "Число Фибоначчи рекурсией = " << fibRecursive(N) << endl;
    cout << "Время рекурсии = " << recMinutes << " минут " << timeRec << " секунд\n\n";

    cout << "При больших значениях N рекурсивный метод работает значительно медленнее, т. к. количество вызовов функции растёт экспоненциально\n";

    return 0;
}