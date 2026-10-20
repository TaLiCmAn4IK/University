#include "Dictionary.h"           
#include <iostream>               
#include <cstring>                 
#include <algorithm>               

using namespace Dictionary;


// Функция Create — создание словаря
Instance Dictionary::Create(const char name[DICTNAMEMAXSIZE], int size) {
    if (strlen(name) >= DICTNAMEMAXSIZE) throw std::runtime_error(THROW01);  
    if (size > DICTMAXSIZE) throw std::runtime_error(THROW02); 

    Instance inst{};                    
    strncpy_s(inst.name, name, DICTNAMEMAXSIZE); // Копируем имя словаря в структуру
    inst.maxsize = size;                    
    inst.size = 0;                       
    inst.dictionary = new Entry[size];      

    return inst;                            
}


// Функция AddEntry — добавление записи
void Dictionary::AddEntry(Instance& inst, Entry ed) {
    if (inst.size >= inst.maxsize) throw std::runtime_error(THROW03);

    for (int i = 0; i < inst.size; i++)
        if (inst.dictionary[i].id == ed.id)
            throw std::runtime_error(THROW04);

    inst.dictionary[inst.size++] = ed;
}

// Функция DelEntry — удаление записи по ID
void Dictionary::DelEntry(Instance& inst, int id) {
   
    for (int i = 0; i < inst.size; i++) {
        
        if (inst.dictionary[i].id == id) {
            
            std::copy(inst.dictionary + i + 1, inst.dictionary + inst.size, inst.dictionary + i);
            inst.size--;   
            return;       
        }
    }
    throw std::runtime_error(THROW06);
}

// Функция UpdEntry — обновление записи
void Dictionary::UpdEntry(Instance& inst, int id, Entry new_ed) {
    for (int i = 0; i < inst.size; i++) {
        if (inst.dictionary[i].id == id) {
            // Проверка на уникальность нового id
            if (new_ed.id != id) {
                for (int j = 0; j < inst.size; j++)
                    if (inst.dictionary[j].id == new_ed.id)
                        throw std::runtime_error(THROW08); 
            }
            inst.dictionary[i] = new_ed;
            return; 
        }
    }
    throw std::runtime_error(THROW07);
}


// Функция GetEntry — получить запись по ID
Entry Dictionary::GetEntry(Instance inst, int id) {
    
    for (int i = 0; i < inst.size; i++)
        if (inst.dictionary[i].id == id)
            return inst.dictionary[i]; // Возвращаем найденную запись

  
    throw std::runtime_error(THROW05);
}

// Функция Print — печать словаря
void Dictionary::Print(Instance d) {
    std::cout << d.name << "\n";  
    for (int i = 0; i < d.size; i++)                
        std::cout << d.dictionary[i].id << " " << d.dictionary[i].name << "\n";   
}


// Функция Delete — удалить словарь (освободить память)
void Dictionary::Delete(Instance& d) {
    delete[] d.dictionary;    
    d.dictionary = nullptr;   
    d.size = 0;               
}
