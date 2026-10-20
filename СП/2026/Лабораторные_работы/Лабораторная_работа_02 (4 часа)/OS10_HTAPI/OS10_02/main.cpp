#include <Windows.h>
#include <iostream>
#include "HT.h"

int main()
{
    // Настраиваем кодировку консоли на UTF-8 для корректного отображения русского текста
    SetConsoleOutputCP(65001);
    SetConsoleCP(65001);

    const char* filePath = "C:\\Windows\\Temp\\HTspace.ht";
    HT::HTHANDLE* ht = nullptr;

    try
    {
        std::cout << "=== [Задание 02] Использование статической библиотеки ===" << std::endl;
        std::cout << "=== Тестирование функции Create ===" << std::endl;
        ht = HT::Create(1000, 3, 10, 256, filePath);

        if (ht && ht->File != INVALID_HANDLE_VALUE) {
            std::cout << "-- Create: success" << std::endl;
        }
        else {
            throw "-- Create: error";
        }

        std::cout << "\n=== Тестирование функции Insert ===" << std::endl;
        if (HT::Insert(ht, new HT::Element("key222", 7, "payload", 8))) {
            std::cout << "-- Insert: success" << std::endl;
        }
        else {
            throw "-- insert: error";
        }

        std::cout << "\n=== Тестирование функции Get ===" << std::endl;
        HT::Element* hte = HT::Get(ht, new HT::Element("key222", 7));
        if (hte) {
            std::cout << "-- Get: success" << std::endl;
        }
        else {
            throw "-- Get: error";
        }

        HT::print(hte);

        std::cout << "\n=== Тестирование функции Snap (Синхронный) ===" << std::endl;
        if (HT::Snap(ht)) {
            std::cout << "-- Snap: success" << std::endl;
        }
        else {
            throw "-- Snap: error";
        }

        std::cout << "\n=== Тестирование функции Update ===" << std::endl;
        if (HT::Update(ht, hte, "newpayload", 11)) {
            std::cout << "-- Update: success" << std::endl;
        }
        else {
            throw "-- Update: error";
        }

        std::cout << "\n=== Проверка обновленных данных через Get ===" << std::endl;
        HT::Element* hte1 = HT::Get(ht, new HT::Element("key222", 7));
        if (hte1) {
            std::cout << "-- Get updated: success" << std::endl;
        }
        else {
            throw "-- Get updated: error";
        }

        HT::print(hte1);

        std::cout << "\n=== Тестирование функции Delete ===" << std::endl;
        if (HT::Delete(ht, hte1)) {
            std::cout << "-- Delete: success" << std::endl;
        }
        else {
            throw "-- Delete: error";
        }

        std::cout << "\n=== Тестирование функции Close ===" << std::endl;
        if (HT::Close(ht)) {
            std::cout << "-- Close: success" << std::endl;
        }
        else {
            throw "-- Close: error";
        }

        std::cout << "\n=== ВСЕ ТЕСТЫ ДЛЯ ЗАДАНИЯ 02 ПРОЙДЕНЫ ===" << std::endl;
    }
    catch (const char* msg)
    {
        std::cout << msg << std::endl;
        if (ht != nullptr) {
            std::cout << "Последняя ошибка СУБД: " << HT::GetLastError(ht) << std::endl;
        }
    }

    std::cout << std::endl;
    system("pause");
    return 0;
}
