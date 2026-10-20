#pragma once
#define LEXEMA_FIXSIZE 20
#define LT_MAXSIZE 4096
#define LT_TI_NULLIDX 0xFFFFFFFF

namespace LT
{
    struct Entry
    {
        char lexema[LEXEMA_FIXSIZE];
        int sn;
        int idxTI;
    };

    struct LexTable
    {
        int maxsize;
        int size;
        Entry* table;
    };

    LexTable Create(int size);
    void Add(LexTable& lextable, Entry entry);
    Entry GetEntry(LexTable& lextable, int n);
    void Delete(LexTable& lextable);
}