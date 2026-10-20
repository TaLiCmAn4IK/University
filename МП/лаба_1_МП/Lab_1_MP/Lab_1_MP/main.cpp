#include "pch.h"      
#include "Auxil.h"
#include <iostream>
#include <ctime>
#include <locale>

// Рекурсивный Фибоначчи
long long fib(int n)
{
    if (n <= 1) return n;
    return fib(n - 1) + fib(n - 2);
}

#define CYCLE 1000000   

int main()
{
    setlocale(LC_ALL, "rus");

    // Задание 2

    {
        double av1 = 0, av2 = 0;
        clock_t t1 = 0, t2 = 0;

        auxil::start();     // старт генерации
        t1 = clock();       // фиксация времени

        for (int i = 0; i < CYCLE; i++)
        {
            av1 += (double)auxil::iget(-100, 100); // сумма случайных целых
            av2 += auxil::dget(-100, 100);         // сумма случайных действительных
        }

        t2 = clock();       // фиксация времени

        std::cout << std::endl << "количество циклов: " << CYCLE;
        std::cout << std::endl << "среднее значение (int): " << av1 / CYCLE;
        std::cout << std::endl << "среднее значение (double): " << av2 / CYCLE;
        std::cout << std::endl << "продолжительность (у.е): " << (t2 - t1);
        std::cout << std::endl << " (сек): "
            << ((double)(t2 - t1)) / ((double)CLOCKS_PER_SEC);
        std::cout << std::endl << std::endl;
    }

    // Задание 3 - числа Фибоначчи

    std::cout << "Исследование рекурсивного Фибоначчи" << std::endl;

    int ns[] = { 20, 22, 25, 28, 30, 33, 35, 38, 40, 42, 45, 48, 50 };
    int nN = sizeof(ns) / sizeof(ns[0]);

    for (int i = 0; i < nN; ++i)
    {
        int n = ns[i];

        clock_t t1 = clock();
        long long f = fib(n);
        clock_t t2 = clock();

        double ticks = (double)(t2 - t1);
        double secs = ticks / (double)CLOCKS_PER_SEC;

        std::cout << "n = " << n
            << "  fib(n) = " << f
            << "  время(тики): " << ticks
            << "  время(сек): " << secs
            << std::endl;
    }

    std::cout << std::endl;
    system("pause");
    return 0;
}
