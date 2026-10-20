#include "Heap.h"
#include <iomanip>

namespace heap
{

    Heap create(int maxsize, CMP(*f)(void*, void*))
    {
        return *(new Heap(maxsize, f));
    }

    Heap::Heap(int maxsize, CMP(*f)(void*, void*))
    {
        size = 0;
        maxSize = maxsize;
        storage = new void* [maxSize];
        compare = f;
    }

    int Heap::left(int ix)
    {
        int res = 2 * ix + 1;
        return (res >= size) ? -1 : res;
    }

    int Heap::right(int ix)
    {
        int res = 2 * ix + 2;
        return (res >= size) ? -1 : res;
    }

    int Heap::parent(int ix)
    {
        return (ix + 1) / 2 - 1;
    }

    bool Heap::isFull() const
    {
        return (size >= maxSize);
    }

    bool Heap::isEmpty() const
    {
        return (size <= 0);
    }

    void Heap::swap(int i, int j)
    {
        void* buf = storage[i];
        storage[i] = storage[j];
        storage[j] = buf;
    }

    void Heap::heapify(int ix)
    {
        int l = left(ix);
        int r = right(ix);
        int irl = ix;

        if (l > 0 && compare(storage[l], storage[irl]) == GREAT) irl = l;
        if (r > 0 && compare(storage[r], storage[irl]) == GREAT) irl = r;

        if (irl != ix)
        {
            swap(ix, irl);
            heapify(irl);
        }
    }

    void Heap::insert(void* x)
    {
        if (isFull())
        {
            std::cerr << "Куча заполнена" << std::endl;
            return;
        }

        int i = size++;
        storage[i] = x;

        while (i > 0 && compare(storage[parent(i)], storage[i]) == LESS)
        {
            swap(parent(i), i);
            i = parent(i);
        }
    }

    void* Heap::extractMax()
    {
        if (isEmpty()) return nullptr;

        void* maxElem = storage[0];
        storage[0] = storage[--size];
        heapify(0);
        return maxElem;
    }

    void Heap::scan(int i) const
    {
        int spacing = 20;
        std::cout << '\n';

        if (size == 0)
        {
            std::cout << "Куча пуста" << std::endl;
            return;
        }

        int nextLevel = 1;
        for (int u = 0; u < size; ++u)
        {
            std::cout << std::setw(spacing) << std::setfill(' ');
            ((AAA*)storage[u])->print();

            if (u + 1 == nextLevel)
            {
                std::cout << '\n';
                nextLevel = nextLevel * 2 + 1;
                spacing /= 2;
            }
        }
        std::cout << '\n';
    }

    // --- Добавленные функции ---

    void* Heap::extractMin()
    {
        if (isEmpty()) return nullptr;

        // Минимальный элемент среди листьев
        int start = size / 2;
        int minIndex = start;
        for (int i = start + 1; i < size; ++i)
        {
            if (compare(storage[i], storage[minIndex]) == LESS)
                minIndex = i;
        }

        void* minElem = storage[minIndex];
        storage[minIndex] = storage[--size];
        heapify(minIndex);
        return minElem;
    }

    void* Heap::extractI(int i)
    {
        if (i < 0 || i >= size) return nullptr;

        void* elem = storage[i];
        storage[i] = storage[--size];

        int p = parent(i);
        if (i > 0 && compare(storage[p], storage[i]) == LESS)
        {
            // Просеять вверх
            int current = i;
            while (current > 0 && compare(storage[parent(current)], storage[current]) == LESS)
            {
                swap(current, parent(current));
                current = parent(current);
            }
        }
        else
        {
            // Просеять вниз
            heapify(i);
        }

        return elem;
    }

    void Heap::unionHeap(const Heap& h2)
    {
        if (size + h2.size > maxSize)
        {
            std::cerr << "Невозможно объединить: превышен максимальный размер кучи" << std::endl;
            return;
        }

        for (int i = 0; i < h2.size; ++i)
        {
            insert(h2.storage[i]);
        }
    }

} // namespace heap
