#include "FST.h"
#include <iostream>
#include <cstring>
#include <cstdarg>

namespace FST
{
    RELATION::RELATION(char c, short ns)
    {
        symbol = c;
        nnode = ns;
    }

    NODE::NODE()
    {
        n_relation = 0;
        relations = NULL;
    }

    NODE::NODE(short n, RELATION rel, ...)
    {
        n_relation = n;
        relations = new RELATION[n];

        va_list args;
        va_start(args, rel);

        relations[0] = rel;
        for (short i = 1; i < n; i++)
        {
            relations[i] = va_arg(args, RELATION);
        }

        va_end(args);
    }

    FST::FST(const char* s, short ns, NODE n, ...)
    {
        string = s;
        nstates = ns;
        nodes = new NODE[ns];

        va_list args;
        va_start(args, n);

        nodes[0] = n;
        for (short i = 1; i < ns; i++)
        {
            nodes[i] = va_arg(args, NODE);
        }

        va_end(args);

        rstates = new short[nstates];
        memset(rstates, 0xff, sizeof(short) * nstates);// Меняем местами массивы состояний
        rstates[0] = 0;
        position = -1;
    }

    bool step(FST& fst, short*& rstates)
    {
        bool rc = false;
        std::swap(rstates, fst.rstates);

        for (short i = 0; i < fst.nstates; i++)
        {
            if (rstates[i] == fst.position)
            {    // Перебираем переходы из состояния
                for (short j = 0; j < fst.nodes[i].n_relation; j++)
                {   // Если символ перехода совпадает с текущим символом строки
                    if (fst.nodes[i].relations[j].symbol == fst.string[fst.position])
                    {
                        fst.rstates[fst.nodes[i].relations[j].nnode] = fst.position + 1;
                        rc = true;
                    }
                }
            }
        }

        return rc;
    }

    bool execute(FST& fst)
    {
        short* rstates = new short[fst.nstates];
        memset(rstates, 0xff, sizeof(short) * fst.nstates); // состояние не достижимо на текущей позиции в строке

        short lstring = strlen(fst.string);
        bool rc = true;
        // Обрабатываем строку посимвольно
        for (short i = 0; i < lstring && rc; i++)
        {
            fst.position++;
            rc = step(fst, rstates);
        }

        delete[] rstates;
        return (rc ? (fst.rstates[fst.nstates - 1] == lstring) : rc);
    }
}
