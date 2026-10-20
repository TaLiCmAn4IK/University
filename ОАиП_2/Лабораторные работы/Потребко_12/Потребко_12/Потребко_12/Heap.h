#pragma once
#include <iostream>

struct AAA
{
    int x;
    void print() { std::cout << x; }
};

namespace heap
{
    enum CMP
    {
        LESS = -1,
        EQUAL = 0,
        GREAT = 1
    };

    struct Heap
    {
        int size;       // текущее число элементов
        int maxSize;    // максимальный размер
        void** storage; // массив указателей на элементы
        CMP(*compare)(void*, void*); // функция сравнения

        Heap(int maxsize, CMP(*f)(void*, void*));

        int left(int ix);
        int right(int ix);
        int parent(int ix);

        bool isFull() const;
        bool isEmpty() const;

        void swap(int i, int j);
        void heapify(int ix);
        void insert(void* x);
        void* extractMax();
        void scan(int i) const;

        // Новые функции
        void* extractMin();
        void* extractI(int i);
        void unionHeap(const Heap& h2);
    };

    Heap create(int maxsize, CMP(*f)(void*, void*));
}
