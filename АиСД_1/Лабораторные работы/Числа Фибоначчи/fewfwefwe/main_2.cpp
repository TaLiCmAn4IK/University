#include <iostream>
#include <chrono>   // Для замера времени

// Итеративная (циклическая) функция вычисления N-го числа Фибоначчи
long long fibIterative(int n) {
    if (n < 2) return n;
    long long f0 = 0, f1 = 1, fn = 0;
    for (int i = 2; i <= n; i++) {
        fn = f0 + f1;
        f0 = f1;
        f1 = fn;
    }
    return fn;
}

// Рекурсивная функция вычисления N-го числа Фибоначчи
long long fibRecursive(int n) {
    if (n < 2) return n;
    return fibRecursive(n - 1) + fibRecursive(n - 2);
}

int main() {
    setlocale(LC_ALL, ""); // Чтобы корректно отображался русский текст в консоли
    int N;
    std::cout << "Введите число N: ";
    std::cin >> N;