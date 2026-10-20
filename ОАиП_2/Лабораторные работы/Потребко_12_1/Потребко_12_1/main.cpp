#include <iostream>
#include <iomanip>
#include <algorithm>
using namespace std;

class AAA {
public:
    int x;
    void print() { cout << x; }
};

namespace heap {
    enum CMP { LESS = -1, EQUAL = 0, GREAT = 1 };

    class Heap {
    private:
        void** storage;
        int size;
        int maxSize;
        CMP(*compare)(void*, void*);

    public:
        Heap(int maxsize, CMP(*f)(void*, void*)) {
            size = 0;
            maxSize = maxsize;
            storage = new void* [maxSize];
            compare = f;
        }

        ~Heap() {
            delete[] storage;
        }

        int getSize() const { return size; }
        bool isEmpty() const { return size == 0; }
        bool isFull() const { return size == maxSize; }

        int left(int ix) const { return (2 * ix + 1 < size) ? (2 * ix + 1) : -1; }
        int right(int ix) const { return (2 * ix + 2 < size) ? (2 * ix + 2) : -1; }
        int parent(int ix) const { return (ix > 0) ? (ix - 1) / 2 : -1; }

        void swap(int i, int j) {
            void* buf = storage[i];
            storage[i] = storage[j];
            storage[j] = buf;
        }

        bool isGreat(void* a1, void* a2) const { return compare(a1, a2) == GREAT; }
        bool isLess(void* a1, void* a2) const { return compare(a1, a2) == LESS; }

        void heapify(int ix) {
            int l = left(ix), r = right(ix), largest = ix;

            if (l != -1 && isGreat(storage[l], storage[largest]))
                largest = l;

            if (r != -1 && isGreat(storage[r], storage[largest]))
                largest = r;

            if (largest != ix) {
                swap(ix, largest);
                heapify(largest);
            }
        }

        void insert(void* x) {
            if (isFull()) {
                cout << "Куча переполнена!\n";
                return;
            }

            storage[size] = x;
            int i = size++;

            while (i > 0 && isLess(storage[parent(i)], storage[i])) {
                swap(parent(i), i);
                i = parent(i);
            }
        }

        void* extractMax() {
            if (isEmpty()) return nullptr;

            void* max = storage[0];
            storage[0] = storage[--size];
            heapify(0);

            return max;
        }

        void* extractMin() {
            if (isEmpty()) return nullptr;

            // Находим минимальный элемент (в листьях)
            int minIndex = size / 2;
            for (int i = size / 2 + 1; i < size; i++) {
                if (isLess(storage[i], storage[minIndex])) {
                    minIndex = i;
                }
            }

            void* min = storage[minIndex];
            storage[minIndex] = storage[--size];

            if (minIndex < size) {
                heapify(minIndex);
                while (minIndex > 0 && isGreat(storage[minIndex], storage[parent(minIndex)])) {
                    swap(minIndex, parent(minIndex));
                    minIndex = parent(minIndex);
                }
            }

            return min;
        }

        void* extractI(int index) {
            if (index < 0 || index >= size) return nullptr;

            void* extracted = storage[index];
            storage[index] = storage[--size];

            heapify(index);
            while (index > 0 && isGreat(storage[index], storage[parent(index)])) {
                swap(index, parent(index));
                index = parent(index);
            }

            return extracted;
        }

        void scan() const {
            cout << "\nСостояние кучи (размер " << size << "):\n";
            if (isEmpty()) {
                cout << "Куча пуста\n";
                return;
            }

            int level = 0;
            int levelEnd = 1;
            for (int i = 0; i < size; i++) {
                if (i == levelEnd) {
                    cout << endl;
                    level++;
                    levelEnd += (1 << level);
                }

                cout << setw((1 << (4 - level))) << ' ';
                ((AAA*)storage[i])->print();
            }
            cout << "\n\n";
        }

        friend Heap unionHeap(const Heap& h1, const Heap& h2, CMP(*f)(void*, void*));
    };

    Heap create(int maxsize, CMP(*f)(void*, void*)) {
        return Heap(maxsize, f);
    }

    Heap unionHeap(const Heap& h1, const Heap& h2, CMP(*f)(void*, void*)) {
        int newSize = h1.getSize() + h2.getSize();
        Heap newHeap(newSize, f);

        // Копируем элементы из первой кучи
        for (int i = 0; i < h1.getSize(); i++) {
            newHeap.insert(h1.storage[i]);
        }

        // Копируем элементы из второй кучи
        for (int i = 0; i < h2.getSize(); i++) {
            newHeap.insert(h2.storage[i]);
        }

        return newHeap;
    }
}

// Функция сравнения для кучи
heap::CMP cmpAAA(void* a1, void* a2) {
    int x1 = ((AAA*)a1)->x;
    int x2 = ((AAA*)a2)->x;
    return x1 > x2 ? heap::GREAT : (x1 < x2 ? heap::LESS : heap::EQUAL);
}

int main() {
    setlocale(LC_ALL, "rus");
    int k, choice, index;
    heap::Heap h1 = heap::create(30, cmpAAA);
    heap::Heap h2 = heap::create(30, cmpAAA);

    while (true) {
        cout << "\nМеню:\n";
        cout << "1 - вывод кучи 1\n";
        cout << "2 - вывод кучи 2\n";
        cout << "3 - добавить элемент в кучу 1\n";
        cout << "4 - добавить элемент в кучу 2\n";
        cout << "5 - удалить максимальный элемент из кучи 1\n";
        cout << "6 - удалить минимальный элемент из кучи 1\n";
        cout << "7 - удалить i-й элемент из кучи 1\n";
        cout << "8 - объединить кучи\n";
        cout << "0 - выход\n";
        cout << "Выберите действие: ";
        cin >> choice;

        switch (choice) {
        case 0:
            return 0;
        case 1:
            h1.scan();
            break;
        case 2:
            h2.scan();
            break;
        case 3: {
            AAA* a = new AAA;
            cout << "Введите ключ: "; cin >> a->x;
            h1.insert(a);
            break;
        }
        case 4: {
            AAA* a = new AAA;
            cout << "Введите ключ: "; cin >> a->x;
            h2.insert(a);
            break;
        }
        case 5: {
            AAA* max = (AAA*)h1.extractMax();
            if (max) {
                cout << "Удален максимальный: " << max->x << endl;
                delete max;
            }
            else {
                cout << "Куча пуста\n";
            }
            break;
        }
        case 6: {
            AAA* min = (AAA*)h1.extractMin();
            if (min) {
                cout << "Удален минимальный: " << min->x << endl;
                delete min;
            }
            else {
                cout << "Куча пуста\n";
            }
            break;
        }
        case 7: {
            cout << "Введите индекс: "; cin >> index;
            AAA* elem = (AAA*)h1.extractI(index);
            if (elem) {
                cout << "Удален элемент: " << elem->x << endl;
                delete elem;
            }
            else {
                cout << "Неверный индекс или куча пуста\n";
            }
            break;
        }
        case 8: {
            heap::Heap h3 = heap::unionHeap(h1, h2, cmpAAA);
            cout << "\nОбъединенная куча:";
            h3.scan();
            break;
        }
        default:
            cout << "Неверный выбор\n";
        }
    }
}