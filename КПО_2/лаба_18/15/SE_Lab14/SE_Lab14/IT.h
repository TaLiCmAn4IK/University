#pragma once
#define ID_MAXSIZE 10    // максимальное количество символов в идентификаторе
#define TI_MAXSIZE 4096    // максимальное количество строк в таблице идентификаторов
#define TI_INT_DEFAULT 0x00000000   
#define TI_STR_DEFAULT 0x00  
#define TI_NULLIDX 0xFFFFFFFF    // нет элемента таблицы идентификаторов
#define TI_STR_MAXSIZE 255  

namespace IT    // таблица идентификаторов
{
    enum IDDATATYPE { INT = 1, STR = 2 };    // типы данных идентификаторов: integer, string
    enum IDTYPE { V = 1, F = 2, P = 3, L = 4 };    // типы идентификаторов: переменна€, функци€, параметр, литерал

    struct Entry {
        int idxfirstLE;
        char id[ID_MAXSIZE];
        IDDATATYPE iddatatype;
        IDTYPE idtype;
        int parentFunctionIdx = -1; // индекс функции, если это параметр
        union {
            int vint;
            struct {
                char len;
                char str[TI_STR_MAXSIZE];
            } vstr;
        } value;
    };


    struct IdTable    // экземпл€р таблицы идентификаторов
    {
        int maxsize;    // емкость таблицы идентификаторов < TI_MAXSIZE
        int size;    // текущий размер таблицы идентификаторов < maxsize
        Entry* table;    // массив строк таблицы идентификаторов
    };

    IdTable Create(    // создать таблицу идентификаторов
        int size    // емкость таблицы идентификаторов < TI_MAXSIZE
    );
    void Add(    // добавить строку в таблицу идентификаторов
        IdTable& idtable,    // экземпл€р таблицы идентификаторов
        Entry entry    // строка таблицы идентификаторов
    );

    Entry GetEntry(    // получить строку таблицы идентификаторов
        IdTable& idtable,    // экземпл€р таблицы идентификаторов
        int n    // номер получаемой строки
    );

    int IsId(    // возврат: номер строки (если есть), TI_NULLIDX (если нет)
        IdTable& idtable,    // экземпл€р таблицы идентификаторов
        char id[ID_MAXSIZE]    // идентификатор
    );
    void Delete(IdTable& idtable);    // удалить таблицу идентификаторов (освободить пам€ть)
}