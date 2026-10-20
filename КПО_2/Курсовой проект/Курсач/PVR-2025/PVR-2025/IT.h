#pragma once

// Максимальные размеры
#define ID_MAXSIZE       100        // Максимальная длина имени идентификатора
#define TI_MAXSIZE       4096       // Максимальный размер таблицы идентификаторов
#define TI_INT_DEFAULT   0x00000000 // Значение по умолчанию для short
#define TI_STR_DEFAULT   0x00       // Значение по умолчанию для string
#define TI_NULLIDX       0xffffffff // Индекс "не найдено"
#define TI_STR_MAXSIZE   255        // Максимальная длина строки

namespace IT {

    // Типы данных в языке FIO-2025
    enum IDDATATYPE {
        SHORT = 1, // целочисленный тип (2 байта)
        STR = 2, // строковый тип
        PROC = 4, // процедура (функция без возвращаемого значения)
    };

    // Тип идентификатора
    enum IDTYPE {
        V = 1, // переменная
        F = 2, // функция
        P = 3, // параметр функции
        L = 4, // литерал
        OP = 5  // оператор (+, -, ==, < и т.д.)
    };

    // Одна запись таблицы идентификаторов
    struct Entry {
        int  idxFirstLE;                 // индекс первой лексемы
        char visibility[ID_MAXSIZE];     // область видимости
        char id[ID_MAXSIZE];             // имя идентификатора

        IDDATATYPE idDataType;           // тип данных
        IDTYPE     idtype;               // тип идентификатора
        int        countOfPar = 0;       // количество параметров (для функций)

        union {
            int vint;                    // значение для SHORT/LOGICAL
            struct {
                int  len;                // длина строки
                char str[TI_STR_MAXSIZE - 1];
            } vstr;                      // значение для STR
        } value;
    };

    // Таблица идентификаторов
    struct IdTable {
        int    maxsize;  // вместимость
        int    size;     // текущее количество записей
        Entry* table;    // динамический массив
    };

    // Интерфейс работы с таблицей
    void DebugCheckDuplicates(IdTable& idtable, std::ostream& out);
    IdTable Create(int size);
    void    Add(IdTable& idtable, Entry entry);
    Entry   GetEntry(IdTable& idtable, int n);
    int     IsId(IdTable& idtable, char id[ID_MAXSIZE]);
    void    Delete(IdTable& idtable);
    void    ShowTable(IdTable& idtable, std::ostream& stream_out);
    void    PrintTable(IdTable& idtable);
}
