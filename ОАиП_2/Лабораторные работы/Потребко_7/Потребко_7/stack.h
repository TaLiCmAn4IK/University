#pragma once
#include <string>

struct Узел
{
    int данные;
    Узел* следующий;
};

struct Стек
{
    Узел* вершина;          // вершина стека
};

// базовые операции
void   init(Стек& s);
bool   isEmpty(const Стек& s);
void   push(Стек& s, int значение);
bool   pop(Стек& s, int& выход);   // возвращает false, если стек пуст
bool   top(const Стек& s, int& выход);

// сервис
void   clear(Стек& s);
bool   saveToFile(const Стек& s, const std::string& filename);
bool   loadFromFile(Стек& s, const std::string& filename);

// дополнительная задача
void   buildEvenOdd(const Стек& src, Стек& even, Стек& odd);
void   print(const Стек& s);             // для отладки/демонстрации
