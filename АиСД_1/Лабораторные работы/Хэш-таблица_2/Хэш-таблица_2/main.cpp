#include <iostream>
#include <string>

using namespace std;

const int MAX_PHONE = 32767;

// Структура для элемента хэш-таблицы
struct Entry {
    int phone;
    string name;
    Entry* next;
};

// Функция хэширования 
int hashFunction(int phone, int tableSize) {
    return phone % tableSize;
}

// Добавление элемента в хэш-таблицу
void addEntry(Entry** hashTable, int tableSize, int phone, string name) {
    int index = hashFunction(phone, tableSize);

    Entry* newEntry = new Entry;
    newEntry->phone = phone;
    newEntry->name = name;
    newEntry->next = nullptr;

    if (hashTable[index] == nullptr) {
        hashTable[index] = newEntry;
    }
    else {
        Entry* current = hashTable[index];
        while (current->next != nullptr) {
            current = current->next;
        }
        current->next = newEntry;
    }
}

// Поиск элемента по номеру телефона
Entry* findEntry(Entry** hashTable, int tableSize, int phone) {
    int index = hashFunction(phone, tableSize);
    Entry* current = hashTable[index];

    while (current != nullptr) {
        if (current->phone == phone) {
            return current;
        }
        current = current->next;
    }

    return nullptr;
}

// Удаление элемента по номеру телефона
bool removeEntry(Entry** hashTable, int tableSize, int phone) {
    int index = hashFunction(phone, tableSize);
    Entry* current = hashTable[index];
    Entry* prev = nullptr;

    while (current != nullptr) {
        if (current->phone == phone) {
            if (prev == nullptr) {
                hashTable[index] = current->next;
            }
            else {
                prev->next = current->next;
            }
            delete current;
            return true;
        }
        prev = current;
        current = current->next;
    }

    return false;
}

// Вывод всей хэш-таблицы
void printHashTable(Entry** hashTable, int tableSize) {
    for (int i = 0; i < tableSize; i++) {
        cout << "[" << i << "]: ";
        Entry* current = hashTable[i];
        while (current != nullptr) {
            cout << current->phone << " - " << current->name;
            if (current->next != nullptr) {
                cout << " -> ";
            }
            current = current->next;
        }
        cout << endl;
    }
}

// Освобождение памяти
void freeHashTable(Entry** hashTable, int tableSize) {
    for (int i = 0; i < tableSize; i++) {
        Entry* current = hashTable[i];
        while (current != nullptr) {
            Entry* temp = current;
            current = current->next;
            delete temp;
        }
    }
    delete[] hashTable;
}

int main() {
    setlocale(LC_ALL, "Rus");
    int tableSize;
    cout << "Введите размер хэш-таблицы: ";
    cin >> tableSize;


    Entry** hashTable;
    hashTable = new Entry * [tableSize];
    for (int i = 0; i < tableSize; i++) {
        hashTable[i] = nullptr;
    }

    int choice;
    do {
        cout << "\nМеню:\n";
        cout << "1. Добавить запись\n";
        cout << "2. Найти запись\n";
        cout << "3. Удалить запись\n";
        cout << "4. Вывести всю таблицу\n";
        cout << "0. Выход\n";
        cout << "Выберите действие: ";
        cin >> choice;
        switch (choice) {
        case 1: {
            int phone;
            string name;
            cout << "Введите номер телефона: ";
            cin >> phone;
            if (phone > MAX_PHONE) {
                cout << "Ошибка: номер телефона превышает максимальное значение " << MAX_PHONE << endl;
                break;
            }
            cout << "Введите ФИО владельца: ";
            cin.ignore();
            getline(cin, name);
            addEntry(hashTable, tableSize, phone, name);
            cout << "Запись добавлена.\n";
            break;
        }
        case 2: {
            int phone;
            cout << "Введите номер телефона для поиска: ";
            cin >> phone;
            Entry* found = findEntry(hashTable, tableSize, phone);
            if (found != nullptr) {
                cout << "Найдено: " << found->phone << " - " << found->name << endl;
            }
            else {
                cout << "Запись не найдена.\n";
            }
            break;
        }
        case 3: {
            int phone;
            cout << "Введите номер телефона для удаления: ";
            cin >> phone;
            if (removeEntry(hashTable, tableSize, phone)) {
                cout << "Запись удалена.\n";
            }
            else {
                cout << "Запись не найдена.\n";
            }
            break;
        }
        case 4:
            cout << "\nХэш-таблица:\n";
            printHashTable(hashTable, tableSize);
            break;
        case 0:
            cout << "Выход...\n";
            break;
        default:
            cout << "Неверный выбор. Попробуйте снова.\n";
        }
    } while (choice != 0);

    freeHashTable(hashTable, tableSize);
    return 0;
}