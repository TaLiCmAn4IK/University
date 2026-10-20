#include <iostream>
#include <cmath>

using namespace std;

// Функция для первого уравнения: 2x + x^3 - 7
double equation1(double x) {
    return 2 * x + pow(x, 3) - 7;
}

// Функция для второго уравнения: e^x + 2x
double equation2(double x) {
    return exp(x) + 2 * x;
}

// Функция для метода дихотомии
double bisectionMethod(double (*func)(double), double a, double b, double e) {
    double x = (a + b) / 2;
    while (abs(a - b) > (2 * e)) {
        x = (a + b) / 2;
        if (func(x) * func(a) <= 0) {
            b = x;
        }
        else {
            a = x;
        }
    }
    return x;
}

int main() {
    setlocale(LC_CTYPE, "Russian");
    double a1, b1, a2, b2, e = 0.001;

    // Ввод исходных данных для первого уравнения
    cout << "Введите границы для первого уравнения (a1, b1): ";
    cin >> a1 >> b1;

    // Ввод исходных данных для второго уравнения
    cout << "Введите границы для второго уравнения (a2, b2): ";
    cin >> a2 >> b2;

    // Вычисление корней
    double root1 = bisectionMethod(equation1, a1, b1, e);
    double root2 = bisectionMethod(equation2, a2, b2, e);

    cout << "Корень первого уравнения: x = " << root1 << endl;
    cout << "Корень второго уравнения: x = " << root2 << endl;

    return 0;
}