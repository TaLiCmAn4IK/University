#include "stack.h"
#include <fstream>
#include <iostream>

using namespace std;

// базовые операции
void init(Стек& s) { s.вершина = nullptr; }

bool isEmpty(const Стек& s) { return s.вершина == nullptr; }

void push(Стек& s, int значение)
{
    Узел* n = new Узел{ значение, s.вершина };
    s.вершина = n;
}

bool pop(Стек& s, int& выход)
{
    if (isEmpty(s)) return false;
    Узел* tmp = s.вершина;
    выход = tmp->данные;
    s.вершина = tmp->следующий;
    delete tmp;
    return true;
}

bool top(const Стек& s, int& выход)
{
    if (isEmpty(s)) return false;
    выход = s.вершина->данные;
    return true;
}

// очистка
void clear(Стек& s)
{
    int _;
    while (pop(s, _)) { /* ничего */ }
}

// сохранение / загрузка
bool saveToFile(const Стек& s, const string& filename)
{
    ofstream ofs(filename, ios::trunc);
    if (!ofs) return false;

    
    Стек temp; init(temp);
    for (Узел* p = s.вершина; p; p = p->следующий) push(temp, p->данные);

    int значение;
    while (pop(temp, значение)) ofs << значение << '\n';
    return true;
}

bool loadFromFile(Стек& s, const string& filename)
{
    ifstream ifs(filename);
    if (!ifs) return false;

    clear(s);               // очищаем старое содержимое

    int значение;
    while (ifs >> значение)    // читаем “снизу‑вверх”
        push(s, значение);     // push в „правильном“ порядке
    return true;
}

// построение чётного / нечётного
void buildEvenOdd(const Стек& src, Стек& even, Стек& odd)
{
    clear(even); clear(odd);
    for (Узел* p = src.вершина; p; p = p->следующий)
        (p->данные % 2 == 0 ? push(even, p->данные)
            : push(odd, p->данные));
}

// утилита для вывода
void print(const Стек& s)
{
    cout << "[Верх] ";
    for (Узел* p = s.вершина; p; p = p->следующий)
        cout << p->данные << ' ';
    cout << "[Низ]\n";
}
