#include "stdafx.h"
#include "IT.h"

namespace IT {

    // Создание таблицы идентификаторов заданного размера
    IdTable Create(int size) {
        IdTable idtable;
        idtable.maxsize = size;   
        idtable.size = 0;          
        idtable.table = new Entry[size];  // Выделение памяти для массива записей
        return idtable;
    }

    void Add(IdTable& idtable, Entry entry) {

        if (idtable.size < idtable.maxsize) {
            idtable.table[idtable.size++] = entry;  // добавление записи и увеличение счетчика
        }
    }

 
    Entry GetEntry(IdTable& idtable, int n) {
        return idtable.table[n]; 
    }

    // Поиск идентификатора в таблице по имени
    int IsId(IdTable& idtable, char id[ID_MAXSIZE]) {
    
        for (int i = 0; i < idtable.size; i++) {
            // Сравнение имени идентификатора
            if (strcmp(idtable.table[i].id, id) == 0) {
                return i; 
            }
        }
        return TI_NULLIDX;  // Возврат константы "не найден"
    }

    void Delete(IdTable& idtable) {
        delete[] idtable.table;    
        idtable.size = 0;         
        idtable.maxsize = 0;      
    }
}