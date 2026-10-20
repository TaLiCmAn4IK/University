#include "Heap.h"
#include <iostream>

using namespace std;

heap::CMP cmpAAA(void* a1, void* a2)
{
    AAA* A1 = (AAA*)a1;
    AAA* A2 = (AAA*)a2;

    if (A1->x > A2->x)
        return heap::GREAT;
    if (A1->x < A2->x)
        return heap::LESS;
    return heap::EQUAL;
}

int main()
{
    setlocale(LC_ALL, "rus");

    heap::Heap h1 = heap::create(30, cmpAAA);

    // Вставим несколько элементов
    for (int val : {10, 20, 15, 30, 40, 5, 7})
    {
        AAA* a = new AAA;
        a->x = val;
        h1.insert(a);
    }

    cout << "Куча после вставки элементов:" << endl;
    h1.scan(0);

    // Удалим максимальный элемент
    AAA* maxEl = (AAA*)h1.extractMax();
    if (maxEl) {
        cout << "Удалён max элемент: " << maxEl->x << endl;
        delete maxEl;
    }
    h1.scan(0);

    // Удалим минимальный элемент
    AAA* minEl = (AAA*)h1.extractMin();
    if (minEl) {
        cout << "Удалён min элемент: " << minEl->x << endl;
        delete minEl;
    }
    h1.scan(0);

    // Удалим элемент с индексом 2
    AAA* idxEl = (AAA*)h1.extractI(2);
    if (idxEl) {
        cout << "Удалён элемент по индексу 2: " << idxEl->x << endl;
        delete idxEl;
    }
    h1.scan(0);

    // Создадим вторую кучу для объединения
    heap::Heap h2 = heap::create(10, cmpAAA);
    for (int val : {50, 60, 5})
    {
        AAA* a = new AAA;
        a->x = val;
        h2.insert(a);
    }
    cout << "Вторая куча:" << endl;
    h2.scan(0);

    // Объединим кучи
    h1.unionHeap(h2);
    cout << "Куча после объединения с второй:" << endl;
    h1.scan(0);

    // Очистка памяти (важно!)
    while (!h1.isEmpty())
    {
        AAA* el = (AAA*)h1.extractMax();
        delete el;
    }
    while (!h2.isEmpty())
    {
        AAA* el = (AAA*)h2.extractMax();
        delete el;
    }

    return 0;
}