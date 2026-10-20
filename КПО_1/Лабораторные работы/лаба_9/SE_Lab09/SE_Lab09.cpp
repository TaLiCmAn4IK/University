#include <iostream>
#include "Varparm.h"
#include <cfloat>
using namespace std;

void main() {
    setlocale(LC_ALL, "Rus");

    // Тесты для ivarparm (произведение)
    int result1Iva = Varparm::ivarparm(0);
    int result2Iva = Varparm::ivarparm(1, 7);
    int result3Iva = Varparm::ivarparm(2, 4, 5);
    int result4Iva = Varparm::ivarparm(6, 9, 1, 4, 11, 2, 8);
    cout << "Результат функции ivarparm(1): " << result1Iva << endl;
    cout << "Результат функции ivarparm(2): " << result2Iva << endl;
    cout << "Результат функции ivarparm(3): " << result3Iva << endl;
    cout << "Результат функции ivarparm(4): " << result4Iva << endl;
    cout << endl;

    // Тесты для svarparm (максимум)
    int result1Sva = Varparm::svarparm(0);
    int result2Sva = Varparm::svarparm(1, 5);
    int result3Sva = Varparm::svarparm(2, 2, 5);
    int result4Sva = Varparm::svarparm(6, 5, 3, 4, 1, 2, 7); // Исправлено: 6 чисел
    cout << "Результат функции svarparm(1): " << result1Sva << endl;
    cout << "Результат функции svarparm(2): " << result2Sva << endl;
    cout << "Результат функции svarparm(3): " << result3Sva << endl;
    cout << "Результат функции svarparm(4): " << result4Sva << endl;
    cout << endl;

    // Тесты для fvarparm (сумма float)
    double result1Fva = Varparm::fvarparm(FLT_MAX);
    double result2Fva = Varparm::fvarparm(1.0f, FLT_MAX);
    double result3Fva = Varparm::fvarparm(2.93f, 16.94f, FLT_MAX);
    double result4Fva = Varparm::fvarparm(2.0f, 2.0f, 2.0f, 2.0f, 2.0f, 2.0f, FLT_MAX);
    cout << "Результат функции fvarparm(1): " << result1Fva << endl;
    cout << "Результат функции fvarparm(2): " << result2Fva << endl;
    cout << "Результат функции fvarparm(3): " << result3Fva << endl;
    cout << "Результат функции fvarparm(4): " << result4Fva << endl;
    cout << endl;

    // Тесты для dvarparm (сумма double)
    double result1Dva = Varparm::dvarparm(DBL_MAX);
    double result2Dva = Varparm::dvarparm(2.0, DBL_MAX);
    double result3Dva = Varparm::dvarparm(2.0, 2.0, DBL_MAX);
    double result4Dva = Varparm::dvarparm(2.0, 2.0, 2.0, 2.0, 2.0, 2.0, DBL_MAX);
    cout << "Результат функции dvarparm(1): " << result1Dva << endl;  // Исправлено переменные
    cout << "Результат функции dvarparm(2): " << result2Dva << endl;
    cout << "Результат функции dvarparm(3): " << result3Dva << endl;
    cout << "Результат функции dvarparm(4): " << result4Dva << endl;
}