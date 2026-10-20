#pragma once
#define LEXEMA_FIXSIZE 20
#define LT_MAXSIZE 4096
#define LT_TI_NULLIDX 0xFFFFFFFF

namespace LT    // таблица лексем
{
    struct Entry    // строка таблицы лексем
    {
        char lexema[LEXEMA_FIXSIZE];  // лексема
        int sn;    // номер строки в исходном тексте
        int idxTI;    // индекс в таблице идентификаторов или LT_TI_NULLIDX
    };

    struct LexTable    // экземпл€р таблицы лексем
    {
        int maxsize;    // емкость таблицы лексем < LT_MAXSIZE
        int size;    // текущий размер таблицы лексем < maxsize
        Entry* table;    // массив строк таблицы лексем
    };

    LexTable Create(    // создать таблицу лексем
        int size    // емкость таблицы лексем < LT_MAXSIZE
    );
    void Add(    // добавить строку в таблицу лексем
        LexTable& lextable,   // экземпл€р таблицы лексем
        Entry entry    // строка таблицы лексем
    );

    Entry GetEntry(    // получить строку таблицы лексем
        LexTable& lextable,   // экземпл€р таблицы лексем
        int n    // номер получаемой строки
    );
    void Delete(LexTable& lextable);    // удалить таблицу лексем (освободить пам€ть)
}