#pragma once
#include <iostream>
#include <cstring>

namespace FST
{
    struct RELATION
    {
        char symbol;      // Символ перехода
        short nnode;      // Номер целевой вершины
        RELATION(
            char c = 0x00,
            short ns = 0
        );
    };

    struct NODE
    {
        short n_relations;       // Количество рёбер
        RELATION* relations;     // Массив рёбер
        NODE();
        NODE(short n, RELATION rel, ...);
    };

    struct FST
    {
        char* string;        // Входная цепочка
        short position;      // Текущая позиция в цепочке
        short nstates;       // Количество состояний
        NODE* nodes;         // Массив состояний
        short* rstates;      // Массив результирующих состояний
        FST(char* s, short ns, NODE n, ...);
    };

    bool Execute(FST& fst);     // Выполнение автомата
    bool step(FST& fst, short*& rstates);  // Шаг автомата
}
