#include <iostream>
#include <iomanip>
#include <time.h>
#include "Combi.h"
#include "Boat.h"
#include "Auxil.h"

#define SPACE(n) std::setw(n) << " "

// Макросы для включения/выключения заданий
#define TASK_SUBSET_GENERATOR       1   // Задание 1: генератор подмножеств
#define TASK_COMBINATION_GENERATOR  1   // Задание 2: генератор сочетаний
#define TASK_PERMUTATION_GENERATOR  1   // Задание 3: генератор перестановок
#define TASK_ACCOMODATION_GENERATOR 1   // Задание 4: генератор размещений
#define SHIP_LOAD_CENTERING         1   // Задание 5: загрузка с центровкой
#define SHIP_LOAD_CENTERING_TIME    1   // Задание 6: исследование времени

int main()
{
    setlocale(LC_ALL, "rus");
    auxil::start(); 

#if TASK_SUBSET_GENERATOR
    // Задание 1: Генератор подмножеств 
    {
        std::cout << std::endl << " --- Генератор множества всех подмножеств ---";
        char AA[][2] = { "A", "B", "C", "D" };
        int n = sizeof(AA) / 2;

        std::cout << std::endl << "Исходное множество: { ";
        for (int i = 0; i < n; i++)
            std::cout << AA[i] << ((i < n - 1) ? ", " : " ");
        std::cout << "}";

        std::cout << std::endl << "Генерация всех подмножеств:";
        combi::subset s1(n);
        int k = s1.getfirst();
        while (k >= 0)  // Цикл, пока есть подмножества
        {
            std::cout << std::endl << "{ ";
            for (int i = 0; i < k; i++)
                std::cout << AA[s1.ntx(i)] << ((i < k - 1) ? ", " : " ");
            std::cout << "}";
            k = s1.getnext();
        }
        std::cout << std::endl << "Всего: " << s1.count() << std::endl;
    }
#endif

#if TASK_COMBINATION_GENERATOR
    // Задание 2: Генератор сочетаний
    {
        std::cout << std::endl << " --- Генератор сочетаний ---";
        char AA[][2] = { "A", "B", "C", "D", "E" };
        int n = sizeof(AA) / 2;

        std::cout << std::endl << "Исходное множество: { ";
        for (int i = 0; i < n; i++)
            std::cout << AA[i] << ((i < n - 1) ? ", " : " ");
        std::cout << "}";

        combi::xcombination xc(n, 3);
        std::cout << std::endl << "Генерация сочетаний из " << xc.n << " по " << xc.m << ":";

        int nComb = xc.getfirst();
        while (nComb >= 0)
        {
            std::cout << std::endl << xc.nc << ": { ";
            for (int i = 0; i < nComb; i++)
                std::cout << AA[xc.ntx(i)] << ((i < nComb - 1) ? ", " : " ");
            std::cout << "}";
            nComb = xc.getnext();
        }
        std::cout << std::endl << "Всего: " << xc.count() << std::endl;
    }
#endif

#if TASK_PERMUTATION_GENERATOR
    // Задание 3: Генератор перестановок
    {
        std::cout << std::endl << " --- Генератор перестановок ---";
        char AA[][2] = { "A", "B", "C", "D" };
        int n = sizeof(AA) / 2;

        std::cout << std::endl << "Исходное множество: { ";
        for (int i = 0; i < n; i++)
            std::cout << AA[i] << ((i < n - 1) ? ", " : " ");
        std::cout << "}";

        combi::permutation p(n);
        std::cout << std::endl << "Генерация перестановок:";

        __int64 nPerm = p.getfirst();
        while (nPerm >= 0)
        {
            std::cout << std::endl << std::setw(4) << p.np << ": { ";
            for (int i = 0; i < p.n; i++)
                std::cout << AA[p.ntx(i)] << ((i < p.n - 1) ? ", " : " ");
            std::cout << "}";
            nPerm = p.getnext();
        }
        std::cout << std::endl << "Всего: " << p.count() << std::endl;
    }
#endif

#if TASK_ACCOMODATION_GENERATOR
    // Задание 4: Генератор размещений
    {
        std::cout << std::endl << " --- Генератор размещений ---" << std::endl;

        char elements[][2] = { "A", "B", "C", "D" };
        const short N_ACCOM = sizeof(elements) / 2;
        const short M_ACCOM = 3;

        std::cout << "Исходное множество: { ";
        for (int i = 0; i < N_ACCOM; i++)
            std::cout << elements[i] << ((i < N_ACCOM - 1) ? ", " : "");
        std::cout << " }" << std::endl;

        std::cout << "Генерация размещений из " << N_ACCOM << " по " << M_ACCOM << ":" << std::endl;

        combi::accomodation acc(N_ACCOM, M_ACCOM);
        int state = acc.getfirst();
        int countAcc = 0;

        while (state >= 0)
        {
            std::cout << std::setw(3) << acc.na << ": { ";
            for (int i = 0; i < M_ACCOM; i++)
            {
                std::cout << elements[acc.ntx(i)];
                if (i < M_ACCOM - 1) std::cout << ", ";
            }
            std::cout << " }";

            countAcc++;
            if (countAcc % 4 == 0)
                std::cout << std::endl;
            else
                std::cout << "  ";

            state = acc.getnext();
        }

        std::cout << std::endl << std::endl;
        std::cout << "Всего размещений: " << acc.count() << std::endl;
        std::cout << "Ожидаемое количество (4! / (4-3)! = 24 / 1 = 24): " << (4 * 3 * 2) << std::endl;
        std::cout << std::endl;
    }
#endif

#if SHIP_LOAD_CENTERING
    {
        std::cout << std::endl << " --- Задача об оптимальной загрузке судна с центровкой ---";

        const short M = 5;  // количество мест на судне
        const short N = 8;  // количество контейнеров

        int v[N];  // вес контейнеров 100-200 кг
        int c[N];  // доход 10-100 у.е.
        int minv[M]; // мин. вес для места 50-120 кг
        int maxv[M]; // макс. вес для места 150-850 кг

        std::cout << std::endl << "Сгенерированные данные:" << std::endl;
        std::cout << "Контейнеры (вес, доход):" << std::endl;
        for (int i = 0; i < N; i++)
        {
            v[i] = auxil::iget(100, 201);    // 100-200 кг
            c[i] = auxil::iget(10, 101);      // 10-100 у.е.
            std::cout << "  " << i << ": " << std::setw(3) << v[i] << " кг, "
                << std::setw(3) << c[i] << " у.е." << std::endl;
        }

        std::cout << std::endl << "Ограничения по местам (мин/макс):" << std::endl;
        for (int i = 0; i < M; i++)
        {
            minv[i] = auxil::iget(50, 121);    // 50-121 кг
            maxv[i] = auxil::iget(150, 851);   // 150-851 кг
            std::cout << "  Место " << i << ": " << std::setw(3) << minv[i]
                << " - " << std::setw(3) << maxv[i] << " кг" << std::endl;
        }

        short r[M];  // результат: индексы выбранных контейнеров

        clock_t t1 = clock();
        int max_income = boat_c(M, minv, maxv, N, v, c, r);
        clock_t t2 = clock();

        std::cout << "Результат задания 5:" << std::endl;
        std::cout << "Максимальный доход: " << max_income << " у.е." << std::endl;
        std::cout << "Выбранные контейнеры (место -> контейнер):" << std::endl;

        int total_weight = 0;
        for (int i = 0; i < M; i++)
        {
            std::cout << "  Место " << i << " -> контейнер " << r[i]
                << " (вес " << v[r[i]] << " кг, доход " << c[r[i]] << " у.е.)";

            // Проверка центровки
            if (v[r[i]] >= minv[i] && v[r[i]] <= maxv[i])
                std::cout << " [OK]" << std::endl;
            else
                std::cout << " [ОШИБКА!]" << std::endl;

            total_weight += v[r[i]];
        }

        std::cout << "Общий вес: " << total_weight << " кг" << std::endl;
        std::cout << "Время вычисления: " << (t2 - t1) << " мс" << std::endl;
    }
#endif

#if SHIP_LOAD_CENTERING_TIME
    // Задание 6: Исследование времени для варианта 12
    {
        std::cout << std::endl << " --- Задание 6: Исследование времени (загрузка с центровкой) ---";
        std::cout << std::endl << "Зависимость времени от количества мест (4-8):";
        std::cout << std::endl << "----------------------------------------";
        std::cout << std::endl << " Мест  | Время (мс)";
        std::cout << std::endl << "-------|-----------";

        // Фиксируем количество контейнеров = 8
        const short FIXED_N = 8;

        // Генерируем данные для всех замеров (одинаковые для чистоты эксперимента)
        int v_fixed[FIXED_N];
        int c_fixed[FIXED_N];
        for (int i = 0; i < FIXED_N; i++)
        {
            v_fixed[i] = auxil::iget(100, 201);
            c_fixed[i] = auxil::iget(10, 101);
        }

        // Исследуем для разного количества мест от 4 до 8
        for (int places = 4; places <= 8; places++)
        {
            // Генерируем ограничения для каждого количества мест
            int* minv_test = new int[places];
            int* maxv_test = new int[places];
            short* r_test = new short[places];

            for (int i = 0; i < places; i++)
            {
                minv_test[i] = auxil::iget(50, 121);
                maxv_test[i] = auxil::iget(150, 851);
            }

            clock_t start = clock();
            int result = boat_c(places, minv_test, maxv_test, FIXED_N, v_fixed, c_fixed, r_test);
            clock_t end = clock();

            std::cout << std::endl << SPACE(3) << places << "   |   " << (end - start);

            delete[] minv_test;
            delete[] maxv_test;
            delete[] r_test;
        }
    }
#endif

    system("pause");
    return 0;
}



