#include "stdafx.h"
#include "LT.h"

namespace LT {
    LexTable Create(int size) {
        LexTable lextable;
        lextable.maxsize = size;
        lextable.size = 0;
        lextable.table = new Entry[size]; 
        return lextable;
    }

    void Add(LexTable& lextable, Entry entry) {
        if (lextable.size < lextable.maxsize) {
            lextable.table[lextable.size++] = entry;
        }
    }

    Entry GetEntry(LexTable& lextable, int n) {
        return lextable.table[n];
    }

    void Delete(LexTable& lextable) {
        delete[] lextable.table;
        lextable.size = 0;
        lextable.maxsize = 0;
    }
}