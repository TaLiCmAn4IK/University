#include "Boat.h"
#include "Combi.h"

namespace boatfnc
{
    // Для версии с центровкой (размещения)
    bool check_centering(combi::accomodation& acc, const int minv[], const int maxv[], const int v[])
    {
        for (int i = 0; i < acc.m; i++)
        {
            int weight = v[acc.ntx(i)];
            if (weight < minv[i] || weight > maxv[i])
                return false;
        }
        return true;
    }

    int calc_income(combi::accomodation& acc, const int c[])
    {
        int sum = 0;
        for (int i = 0; i < acc.m; i++)
            sum += c[acc.ntx(i)];
        return sum;
    }

    void copycomb(short m, short* r1, const short* r2)
    {
        for (int i = 0; i < m; i++)
            r1[i] = r2[i];
    }
}

// Версия с центровкой
int boat_c(
    short m,
    const int minv[],
    const int maxv[],
    short n,
    const int v[],
    const int c[],
    short r[])
{
    combi::accomodation acc(n, m);  // генератор размещений
    int best_income = 0;
    int state = acc.getfirst();

    while (state >= 0)
    {
        // Проверяем центровку
        if (boatfnc::check_centering(acc, minv, maxv, v))
        {
            int income = boatfnc::calc_income(acc, c);
            if (income > best_income)
            {
                best_income = income;
                boatfnc::copycomb(m, r, acc.sset);
            }
        }
        state = acc.getnext();
    }

    return best_income;
}





