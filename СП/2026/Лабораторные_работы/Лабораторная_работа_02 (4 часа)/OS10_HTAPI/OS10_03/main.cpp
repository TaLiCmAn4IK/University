#include <Windows.h>
#include <iostream>
#include "HT.h"

int main()
{
    SetConsoleOutputCP(65001);
    SetConsoleCP(65001);

    const char* file1 = "C:\\Windows\\Temp\\HTspace1.ht";
    const char* file2 = "C:\\Windows\\Temp\\HTspace2.ht";

    HT::HTHANDLE* ht1 = nullptr;
    HT::HTHANDLE* ht2 = nullptr;

    try
    {
        std::cout << "=== [Задание 03] Создание двух независимых хранилищ ===" << std::endl;

        ht1 = HT::Create(500, 3, 10, 256, file1);
        if (ht1 && ht1->File != INVALID_HANDLE_VALUE) std::cout << "Хранилище 1 создано: " << file1 << std::endl;
        else throw "Ошибка при создании Хранилища 1";

        ht2 = HT::Create(500, 3, 10, 256, file2);
        if (ht2 && ht2->File != INVALID_HANDLE_VALUE) std::cout << "Хранилище 2 создано: " << file2 << std::endl;
        else throw "Ошибка при создании Хранилища 2";

        std::cout << "\n=== Вставка разных элементов в каждое хранилище ===" << std::endl;
        if (HT::Insert(ht1, new HT::Element("A-111", 5, "Данные Первого Хранилища", 44))) {
            std::cout << "В Хранилище 1 добавлен ключ 'A-111'" << std::endl;
        }
        if (HT::Insert(ht2, new HT::Element("B-222", 5, "Данные Второго Хранилища", 42))) {
            std::cout << "В Хранилище 2 добавлен ключ 'B-222'" << std::endl;
        }

        std::cout << "\n=== Проверка изоляции хранилищ через функцию Get ===" << std::endl;
        std::cout << "Ищем ключ 'A-111' в Хранилище 1:" << std::endl;
        HT::Element* e1_ok = HT::Get(ht1, new HT::Element("A-111", 5));
        HT::print(e1_ok);

        std::cout << "Пытаемся найти ключ 'A-111' в Хранилище 2 (должна быть ошибка):" << std::endl;
        HT::Element* e2_fail = HT::Get(ht2, new HT::Element("A-111", 5));
        if (!e2_fail) {
            std::cout << "Как и ожидалось, ключ не найден в БД №2. Ошибка СУБД: " << HT::GetLastError(ht2) << std::endl;
        }

        std::cout << "\n=== Чтение из Хранилища 2 ===" << std::endl;
        HT::Element* e2_ok = HT::Get(ht2, new HT::Element("B-222", 5));
        HT::print(e2_ok);

        std::cout << "\n=== Закрытие обоих хранилищ ===" << std::endl;
        HT::Close(ht1);
        HT::Close(ht2);
        std::cout << "Оба экземпляра СУБД успешно закрыты. Тест завершен!" << std::endl;
    }
    catch (const char* msg)
    {
        std::cout << "Критический сбой: " << msg << std::endl;
    }

    std::cout << std::endl;
    system("pause");
    return 0;
}
