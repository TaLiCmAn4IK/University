#pragma once                          

#ifndef DICTIONARY_H                  
#define DICTIONARY_H                  

#include <stdexcept>                  


// Константы для ограничений
#define DICTNAMEMAXSIZE  20          
#define DICTMAXSIZE     100           
#define ENTRYNAMEMAXSIZE 30           

#define THROW01 "Create: слишком длинное имя словаря"     
#define THROW02 "Create: размер превышает допустимый"     
#define THROW03 "AddEntry: словарь переполнен"            
#define THROW04 "AddEntry: дублирование ID"               
#define THROW05 "GetEntry: элемент не найден"             
#define THROW06 "DelEntry: элемент не найден"             
#define THROW07 "UpdEntry: элемент не найден"             
#define THROW08 "UpdEntry: дублирование ID"               


namespace Dictionary                 
{
    
    // Структура одного элемента
    struct Entry {
        int id;                       
        char name[ENTRYNAMEMAXSIZE];  
    };

    // Структура всего словаря
    struct Instance {
        char name[DICTNAMEMAXSIZE];  
        int maxsize;                  
        int size;                     
        Entry* dictionary;            
    };


    // Объявления функций работы со словарём
    Instance Create(const char name[DICTNAMEMAXSIZE], int size); 
    void AddEntry(Instance& inst, Entry ed);                     
    void DelEntry(Instance& inst, int id);                       
    void UpdEntry(Instance& inst, int id, Entry new_ed);         
    Entry GetEntry(Instance inst, int id);                       
    void Print(Instance d);                                      
    void Delete(Instance& d);                                    
} 

#endif
