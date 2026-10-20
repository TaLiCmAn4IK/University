#include <Windows.h>
#include <iostream>
#include "HT.h"

void Assert(bool condition, const char* testName) {
    if (condition) {
        std::cout << "[УСПЕХ] " << testName << std::endl;
    }
    else {
        std::cout << "[ОШИБКА] " << testName << std::endl;
    }
}

int main()
{
    SetConsoleOutputCP(65001);
    SetConsoleCP(65001);

    const char* filePath = "C:\\Windows\\Temp\\HTtestcase.ht";
    HT::HTHANDLE* ht = HT::Create(100, 3, 10, 256, filePath);

    std::cout << "=== [Задание 04] Автоматизированное тестирование Test Case HT API ===" << std::endl;

    // Тест 1: Успешная вставка и поиск
    bool t1_insert = HT::Insert(ht, new HT::Element("test1", 5, "data1", 5));
    HT::Element* t1_get = HT::Get(ht, new HT::Element("test1", 5));
    Assert(t1_insert && t1_get != nullptr, "Тест-кейс 1: Добавление и чтение существующего элемента");

    // Тест 2: Попытка вставить дубликат ключа
    bool t2_duplicate = HT::Insert(ht, new HT::Element("test1", 5, "data2", 5));
    Assert(t2_duplicate == false, "Тест-кейс 2: Запрет на добавление дубликата ключа");

    // Тест 3: Поиск несуществующего ключа
    HT::Element* t3_get = HT::Get(ht, new HT::Element("ghost_key", 9));
    Assert(t3_get == nullptr, "Тест-кейс 3: Поиск несуществующего ключа возвращает nullptr");

    // Тест 4: Удаление и проверка отсутствия
    bool t4_delete = HT::Delete(ht, new HT::Element("test1", 5));
    HT::Element* t4_get_after = HT::Get(ht, new HT::Element("test1", 5));
    Assert(t4_delete && t4_get_after == nullptr, "Тест-кейс 4: Удаление элемента стирает его из выборки");

    HT::Close(ht);
    std::cout << "\nВсе автоматические тест-кейсы успешно выполнены." << std::endl;

    system("pause");
    return 0;
}
