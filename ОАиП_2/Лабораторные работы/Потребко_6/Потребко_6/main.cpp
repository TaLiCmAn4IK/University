#include <iostream>
#include <fstream>
#include <sstream>    
#include <string>
#include <limits>
#include <stdexcept>
#include <cctype>    

using namespace std;

struct Node {
    double data;
    Node* next;
    Node(double value) : data(value), next(nullptr) {}
};

class LinkedList {
private:
    Node* head;
public:
    LinkedList() : head(nullptr) {}
    ~LinkedList() { clear(); }

    bool isEmpty() const {
        return (head == nullptr);
    }

    bool addElement(double value) {
        Node* newNode = new Node(value);
        if (isEmpty()) {
            head = newNode;
            return true;
        }
        Node* current = head;
        while (current->next != nullptr) {
            current = current->next;
        }
        current->next = newNode;
        return true;
    }

    bool deleteElement(double value) {
        if (isEmpty()) return false;
        if (head->data == value) {
            Node* temp = head;
            head = head->next;
            delete temp;
            return true;
        }
        Node* current = head;
        Node* previous = nullptr;
        while (current != nullptr && current->data != value) {
            previous = current;
            current = current->next;
        }
        if (current == nullptr) {
            return false; // не нашли
        }
        previous->next = current->next;
        delete current;
        return true;
    }

    bool searchElement(double value) const {
        Node* current = head;
        while (current != nullptr) {
            if (current->data == value) {
                return true;
            }
            current = current->next;
        }
        return false;
    }

    void printList() const {
        if (isEmpty()) {
            cout << "Список пуст.\n";
            return;
        }
        Node* current = head;
        while (current != nullptr) {
            cout << current->data;
            if (current->next != nullptr) {
                cout << " -> ";
            }
            current = current->next;
        }
        cout << endl;
    }

    bool writeToFile() const {
        ofstream fout("FILE.txt");
        if (!fout.is_open()) {
            return false;
        }
        Node* current = head;
        while (current != nullptr) {
            fout << current->data << "\n";
            current = current->next;
        }
        fout.close();
        return true;
    }

    bool readFromFile() {
        ifstream fin("FILE.txt");
        if (!fin.is_open()) {
            cout << "Ошибка чтения файла или файл не найден.\n";
            return false;
        }

        clear(); 

        bool hadErrors = false;  // Флаг, что хотя бы одна строка была некорректной
        string line;
        while (true) {
            if (!getline(fin, line)) {
                // Закончили читать файл
                break;
            }
            // Заменяем запятые на точки
            for (char& ch : line) {
                if (ch == ',') {
                    ch = '.';
                }
            }
            // Пробуем преобразовать через istringstream
            istringstream iss(line);
            double temp;
            if (!(iss >> temp)) {
                // Если не смогли прочитать число — строка некорректна
                cerr << "Некорректная строка: \"" << line << "\". Пропуск.\n";
                hadErrors = true;
                continue;
            }
            // Если остались лишние символы
            char leftover;
            if (iss >> leftover) {
                cerr << "Некорректная строка: \"" << line << "\". Пропуск.\n";
                hadErrors = true;
                continue;
            }
            // Если всё хорошо, добавляем в список
            addElement(temp);
        }

        fin.close();

        // Сообщим общий итог чтения
        if (hadErrors) {
            cout << "Файл считан, но некоторые строки оказались некорректными.\n";
        }
        else {
            cout << "Список считан из файла FILE.txt без ошибок.\n";
        }
        return true;
    }

    void clear() {
        Node* current = head;
        while (current != nullptr) {
            Node* temp = current;
            current = current->next;
            delete temp;
        }
        head = nullptr;
    }

    double averageOfPositive() const {
        if (isEmpty()) {
            return 0.0;
        }
        int countPos = 0;
        double sumPos = 0.0;
        Node* current = head;
        while (current != nullptr) {
            if (current->data > 0) {
                sumPos += current->data;
                ++countPos;
            }
            current = current->next;
        }
        return (countPos == 0) ? 0.0 : (sumPos / countPos);
    }
};

// Функция для считывания вещественного числа из консоли.
// Разрешаем и точку, и запятую в качестве десятичного разделителя.
bool readDoubleFromConsole(double& value) {
    // Считываем строку целиком
    string input;
    if (!getline(cin, input)) {
        // Ошибка потока (EOF или сбой)
        return false;
    }
    // Заменяем запятые на точки
    for (char& ch : input) {
        if (ch == ',') {
            ch = '.';
        }
    }
    // Парсим через istringstream
    istringstream iss(input);
    double temp;
    if (!(iss >> temp)) {
        // Не прочли число
        return false;
    }
    // Если остались ещё символы, значит ошибка
    char leftover;
    if (iss >> leftover) {
        return false;
    }
    // Успешно
    value = temp;
    return true;
}

// Пункт меню — читаем целиком строку, проверяем, что это целое число без мусора
bool readMenuChoice(int& choice) {
    string input;
    if (!getline(cin, input)) {
        // Ошибка потока
        return false;
    }
    try {
        size_t pos;
        int temp = stoi(input, &pos);
        // Если остались неиспользованные символы — ошибка
        if (pos < input.size()) {
            return false;
        }
        choice = temp;
        return true;
    }
    catch (...) {
        return false;
    }
}

void printMenu() {
    cout << "\n--- Меню ---\n";
    cout << "1. Добавить элемент\n";
    cout << "2. Удалить элемент\n";
    cout << "3. Поиск элемента\n";
    cout << "4. Вывести список\n";
    cout << "5. Записать список в файл (FILE.txt)\n";
    cout << "6. Считать список из файла (FILE.txt)\n";
    cout << "7. Найти среднее значение положительных элементов\n";
    cout << "8. Выход\n";
    cout << "Ваш выбор: ";
}

int main() {
    setlocale(LC_ALL, "Russian");

    LinkedList list;
    int choice;
    double value;

    while (true) {
        printMenu();

        if (!readMenuChoice(choice)) {
            cout << "Некорректный ввод пункта меню. Попробуйте снова.\n";
            continue;
        }

        switch (choice) {
        case 0:
            cout << "Выход из программы.\n";
            return 0;

        case 1: {
            while (true) {
                cout << "Введите вещественное число для добавления: ";
                if (readDoubleFromConsole(value)) {
                    list.addElement(value);
                    cout << "Элемент " << value << " добавлен.\n";
                    break;
                }
                else {
                    cout << "Некорректный ввод. Попробуйте снова.\n";
                }
            }
            break;
        }

        case 2: {
            while (true) {
                cout << "Введите значение элемента для удаления: ";
                if (readDoubleFromConsole(value)) {
                    if (list.deleteElement(value)) {
                        cout << "Элемент " << value << " удалён.\n";
                    }
                    else {
                        cout << "Элемент не найден.\n";
                    }
                    break;
                }
                else {
                    cout << "Некорректный ввод. Попробуйте снова.\n";
                }
            }
            break;
        }

        case 3: {
            while (true) {
                cout << "Введите значение элемента для поиска: ";
                if (readDoubleFromConsole(value)) {
                    if (list.searchElement(value)) {
                        cout << "Элемент " << value << " найден в списке.\n";
                    }
                    else {
                        cout << "Элемент не найден.\n";
                    }
                    break;
                }
                else {
                    cout << "Некорректный ввод. Попробуйте снова.\n";
                }
            }
            break;
        }

        case 4:
            cout << "Текущий список: ";
            list.printList();
            break;

        case 5: {
            if (list.writeToFile()) {
                cout << "Список записан в файл FILE.txt.\n";
            }
            else {
                cout << "Ошибка записи в файл.\n";
            }
            break;
        }

        case 6:
            list.readFromFile();
            break;

        case 7: {
            double avgPos = list.averageOfPositive();
            cout << "Среднее значение положительных элементов: " << avgPos << "\n";
            break;
        }

        default:
            cout << "Некорректный пункт меню. Попробуйте снова.\n";
        }
    }

    return 0;
}
