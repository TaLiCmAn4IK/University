#include <iostream>
#include <string>
#include <chrono> // Для измерения времени

using namespace std;
using namespace std::chrono;

struct Entry {
    int    flatNumber;
    string residents;
    Entry* next;

    Entry(int flat, const string& res)
        : flatNumber(flat), residents(res), next(nullptr) {
    }
};

//------------------------------------------------------------
//   простая хеш-функция
//------------------------------------------------------------
int hashFunction(int flatNumber, int tableSize) {
    return flatNumber % tableSize;
}

//------------------------------------------------------------
//   вставка новой записи
//------------------------------------------------------------
void addEntry(Entry** hashTable, int tableSize,
    int flatNumber, const string& residents)
{
    int idx = hashFunction(flatNumber, tableSize);

    if (hashTable[idx]) // сообщение о коллизии
        cout << "Коллизия: индекс " << idx << ".\n";

    Entry* newEntry = new Entry(flatNumber, residents);
    newEntry->next = hashTable[idx];
    hashTable[idx] = newEntry;
}

//------------------------------------------------------------
//   вывод всех жильцов по указанному № квартиры с замером времени
//------------------------------------------------------------
void printResidentsByFlat(Entry** hashTable, int tableSize, int flatNumber)
{
    auto start = high_resolution_clock::now();  // начало измерения времени

    int  idx = hashFunction(flatNumber, tableSize);
    bool found = false;

    for (Entry* cur = hashTable[idx]; cur; cur = cur->next)
        if (cur->flatNumber == flatNumber) {
            cout << "Квартира " << cur->flatNumber
                << " — жильцы: " << cur->residents << '\n';
            found = true;
        }

    if (!found)
        cout << "Запись не найдена.\n";

    auto end = high_resolution_clock::now(); // окончание измерения времени
    auto duration = duration_cast<microseconds>(end - start).count();
    cout << "Время поиска: " << duration / 1000.0 << " мс\n";
}

//------------------------------------------------------------
//   удаление всех записей с данным ключом
//------------------------------------------------------------
bool removeAllEntries(Entry** hashTable, int tableSize, int flatNumber)
{
    int idx = hashFunction(flatNumber, tableSize);
    Entry* cur = hashTable[idx];
    Entry* prev = nullptr;
    bool deleted = false;

    while (cur) {
        if (cur->flatNumber == flatNumber) {
            Entry* toDelete = cur;
            if (prev == nullptr) {
                hashTable[idx] = cur->next;
                cur = hashTable[idx];
            }
            else {
                prev->next = cur->next;
                cur = cur->next;
            }
            delete toDelete;
            deleted = true;
        }
        else {
            prev = cur;
            cur = cur->next;
        }
    }

    return deleted;
}

//------------------------------------------------------------
void printHashTable(Entry** hashTable, int tableSize)
{
    cout << "\nХеш-таблица (индекс: № квартиры — жильцы)\n";
    for (int i = 0; i < tableSize; ++i) {
        cout << '[' << i << "]: ";
        for (Entry* cur = hashTable[i]; cur; cur = cur->next) {
            cout << cur->flatNumber << " — " << cur->residents;
            if (cur->next) cout << "  ->  ";
        }
        cout << '\n';
    }
}

void freeHashTable(Entry** hashTable, int tableSize)
{
    for (int i = 0; i < tableSize; ++i) {
        Entry* cur = hashTable[i];
        while (cur) {
            Entry* tmp = cur;
            cur = cur->next;
            delete tmp;
        }
    }
    delete[] hashTable;
}

//------------------------------------------------------------
int main()
{
    setlocale(LC_ALL, "Russian");

    int tableSize;
    cout << "Введите размер хеш-таблицы: ";
    cin >> tableSize;

    Entry** hashTable = new Entry * [tableSize]();
    int choice;

    do {
        cout << "\nМеню:\n"
            << "1. Добавить жильцов\n"
            << "2. Найти жильцов по № квартиры\n"
            << "3. Удалить запись о квартире\n"
            << "4. Вывод хеш-таблицы\n"
            << "0. Выход\n"
            << "Ваш выбор: ";
        cin >> choice;

        switch (choice) {
        case 1: {
            int flat;  string res;
            cout << "Введите № квартиры: "; cin >> flat;
            cout << "Введите имя жильца: ";  cin.ignore();
            getline(cin, res);
            addEntry(hashTable, tableSize, flat, res);
            cout << "Запись добавлена.\n";
            break;
        }
        case 2: {
            int flat;
            cout << "Введите № квартиры: ";
            cin >> flat;
            printResidentsByFlat(hashTable, tableSize, flat);
            break;
        }
        case 3: {
            int flat;
            cout << "Введите № квартиры для удаления: ";
            cin >> flat;
            cout << (removeAllEntries(hashTable, tableSize, flat)
                ? "Удалены все записи с этим номером квартиры.\n"
                : "Записи с таким номером квартиры не найдены.\n");
            break;
        }
        case 4:
            printHashTable(hashTable, tableSize);
            break;
        case 0:
            cout << "Выход...\n";
            break;
        default:
            cout << "Неверный пункт меню.\n";
        }
    } while (choice != 0);

    freeHashTable(hashTable, tableSize);
    return 0;
}
