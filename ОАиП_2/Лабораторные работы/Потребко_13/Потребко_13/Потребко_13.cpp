#include <iostream>
#include <string>
#include <vector>
using namespace std;

struct HashEntry {
    int    key;
    string value;
    bool   isActive;
    bool   wasOccupied;
    HashEntry() : key(0), value(""), isActive(false), wasOccupied(false) {}
};

class HashTable {
private:
    HashEntry* table;
    int  size;
    int  count;

    int hashFunc(int key) const { return key % size; }

    void rehash() {
        int oldSize = size;
        HashEntry* oldTable = table;

        size *= 2;                        // удваиваем
        table = new HashEntry[size];
        count = 0;

        for (int i = 0; i < oldSize; ++i)
            if (oldTable[i].isActive)
                insert(oldTable[i].key, oldTable[i].value);

        delete[] oldTable;
        cout << "Таблица увеличена до размера " << size << '\n';
    }

public:
    explicit HashTable(int s) : size(s), count(0) {
        table = new HashEntry[size];
    }
    ~HashTable() { delete[] table; }

    void insert(int key, const string& value) {
        // ► теперь смотрим, не достигнем ли 50 % после вставки
        if (count + 1 >= size * 0.5)
            rehash();

        int index = hashFunc(key);
        int start = index;
        int hPrime = -1;

        while (table[index].isActive) {
            if (table[index].key == key && table[index].value == value)
                return;                    // уже есть дубликат

            if (hPrime == -1) hPrime = index; // фиксируем первую коллизию
            index = (index + 1) % size;
            if (index == start) {          // полностью обошли — делаем rehash
                rehash();
                index = start = hashFunc(key);
            }
        }

        table[index].key = key;
        table[index].value = value;
        table[index].isActive = true;
        table[index].wasOccupied = true;
        ++count;

        if (hPrime != -1)
            cout << "Коллизия при вставке (" << key << ", " << value
            << "). h'(key) = " << hPrime << '\n';
    }

    // удаляем ВСЕ записи с данным ключом
    bool remove(int key) {
        bool removed = false;
        int index = hashFunc(key);
        int start = index;

        while (table[index].wasOccupied) {
            if (table[index].isActive && table[index].key == key) {
                table[index].isActive = false;
                --count;
                removed = true;
            }
            index = (index + 1) % size;
            if (index == start) break;
        }
        return removed;
    }

    vector<string> search(int key) const {
        vector<string> res;
        int index = hashFunc(key);
        int start = index;

        while (table[index].wasOccupied) {
            if (table[index].isActive && table[index].key == key)
                res.push_back(table[index].value);
            index = (index + 1) % size;
            if (index == start) break;
        }
        return res;
    }

    void display() const {
        for (int i = 0; i < size; ++i) {
            cout << i << ": ";
            if (table[i].isActive)
                cout << '(' << table[i].key << ", " << table[i].value << ')';
            else if (table[i].wasOccupied)
                cout << "[удалено]";
            else
                cout << "[пусто]";
            cout << '\n';
        }
    }

    int collisions() const {
        int c = 0;
        for (int i = 0; i < size; ++i)
            if (table[i].isActive && hashFunc(table[i].key) != i)
                ++c;
        return c;
    }
};

int main() {
    setlocale(LC_ALL, "rus");

    int n;
    cout << "Начальный размер таблицы: ";
    if (!(cin >> n) || n <= 0) return 0;
    HashTable ht(n);

    int choice;
    do {
        cout << "\n1. Показать таблицу\n"
            "2. Добавить элемент\n"
            "3. Удалить элемент по ключу\n"
            "4. Найти по ключу\n"
            "5. Количество коллизий\n"
            "6. Выход\n"
            "Выбор: ";
        cin >> choice;

        if (choice == 1) {
            ht.display();
        }
        else if (choice == 2) {
            int k; string v;
            cout << "Ключ и значение: ";
            cin >> k >> v;
            ht.insert(k, v);
        }
        else if (choice == 3) {
            int k;
            cout << "Ключ для удаления: ";
            cin >> k;
            cout << (ht.remove(k) ? "Удалено\n" : "Не найдено\n");
        }
        else if (choice == 4) {
            int k;
            cout << "Ключ для поиска: ";
            cin >> k;
            auto vals = ht.search(k);
            if (vals.empty())
                cout << "Не найдено\n";
            else {
                cout << "Найденные значения: ";
                for (auto& x : vals) cout << x << ' ';
                cout << '\n';
            }
        }
        else if (choice == 5) {
            cout << "Количество коллизий: " << ht.collisions() << '\n';
        }
    } while (choice != 6);

    return 0;
}
