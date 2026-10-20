#include <iostream>
#include <cfloat>
#include "Varparm.h"
#include <climits>

namespace Varparm {
    // Найти произведение параметров
    int ivarparm(int count, ...) {
        if (count <= 0) {
            return 0;
        }

        int* ptr = (int*)(&count + 1);
        int result = 1;

        for (int i = 0; i < count; i++) {
            result *= ptr[i];
        }

        return result;
    }

    // Найти максимальное значение параметров
    int svarparm(short count, ...) {
        if (count <= 0) {
            return 0;
        }

        int* ptr = (int*)(&count + 1);
        int maxVal = ptr[0];

        for (short i = 1; i < count; ++i) {
            if (ptr[i] > maxVal) {
                maxVal = ptr[i];
            }
        }

        return maxVal;
    }

    // Вычислить сумму значений всех параметров
    double fvarparm(float first, ...) {
        if (first == FLT_MAX) return 0;

        double sum = (double)first;
        float* ptr = &first + 1;

        while (*ptr != FLT_MAX) {
            sum += *ptr;
            ptr++;
        }

        return sum;
    }

    // Вычислить сумму значений всех параметров
    double dvarparm(double first, ...) {
        if (first == DBL_MAX) return 0;

        double sum = first;
        double* ptr = &first + 1;

        while (*ptr != DBL_MAX) {
            sum += *ptr;
            ptr++;
        }

        return sum;
    }
}