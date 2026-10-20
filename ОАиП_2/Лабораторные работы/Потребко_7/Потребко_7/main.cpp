#include "stack.h"
#include <iostream>

using namespace std;

void меню()
{
    cout << "\n=== Меню работы со стеком ===\n"
        << "1. Добавить элемент\n"
        << "2. Удалить элемент\n"
        << "3. Показать верхний элемент\n"
        << "4. Очистить стек\n"
        << "5. Сохранить в файл\n"
        << "6. Загрузить из файла\n"
        << "7. Разделить на чётные / нечётные элементы\n"
        << "8. Показать стек\n"
        << "0. Выход\n> ";
}

int main()
{
    setlocale(LC_ALL, "rus");
    Стек стек;   init(стек);
    Стек чётные, нечётные; init(чётные); init(нечётные);

    int выбор;
    while (true)
    {
        меню();
        if (!(cin >> выбор)) break;

        switch (выбор)
        {
        case 1: {
            int значение; cout << "Значение: "; cin >> значение;
            push(стек, значение);
            break;
        }
        case 2: {
            int значение;
            if (pop(стек, значение)) cout << "Удалён: " << значение << '\n';
            else cout << "Стек пуст\n";
            break;
        }
        case 3: {
            int значение;
            if (top(стек, значение)) cout << "Верхний элемент: " << значение << '\n';
            else cout << "Стек пуст\n";
            break;
        }
        case 4:
            clear(стек); cout << "Стек очищен\n"; break;

        case 5: {
            cout << (saveToFile(стек, "stack.txt") ? "Сохранено в stack.txt\n" : "Ошибка\n");
            break;
        }
        case 6: {
            cout << (loadFromFile(стек, "stack.txt") ? "Загружено\n" : "Ошибка\n");
            break;
        }
        case 7:
            buildEvenOdd(стек, чётные, нечётные);
            cout << "Чётные:  "; print(чётные);
            cout << "Нечётные: "; print(нечётные);
            break;

        case 8:
            print(стек); break;

        case 0:
            clear(стек); clear(чётные); clear(нечётные);
            return 0;

        default: cout << "Неверный выбор\n";
        }
    }
}
