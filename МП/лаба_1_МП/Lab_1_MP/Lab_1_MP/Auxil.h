#pragma once
#include <cstdlib>

namespace auxil
{
    void start();                       // старт генератора сл. чисел
    double dget(double rmin, double rmax); // случайное double
    int    iget(int rmin, int rmax);       // случайное int
}
