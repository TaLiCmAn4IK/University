#include "stdafx.h"
#include "Dictionary.h"
#include <iostream>

using namespace std;

int main()
{
    setlocale(LC_ALL, "Russian");
    try
    {
#ifdef TEST_CREATE_01
        // Тест: превышение длины имени словаря
        char longName[DICTNAMEMAXSIZE + 2];
        memset(longName, 'A', DICTNAMEMAXSIZE + 1);
        longName[DICTNAMEMAXSIZE + 1] = '\0';
        auto dict = Dictionary::Create(longName, 10);
#endif

#ifdef TEST_CREATE_02
        // Тест: превышение максимальной емкости словаря
        auto dict = Dictionary::Create("Test", DICTMAXSIZE + 1);
#endif

#ifdef TEST_ADDENTRY_03
        // Тест: переполнение словаря
        auto dict = Dictionary::Create("Test", 1);
        Dictionary::Entry e1 = { 1, "First" };
        Dictionary::Entry e2 = { 2, "Second" };
        Dictionary::AddEntry(dict, e1);
        Dictionary::AddEntry(dict, e2); // Должно вызвать исключение
#endif

#ifdef TEST_ADDENTRY_04
        // Тест: дублирование идентификатора
        auto dict = Dictionary::Create("Test", 10);
        Dictionary::Entry e1 = { 1, "First" };
        Dictionary::Entry e2 = { 1, "Duplicate" };
        Dictionary::AddEntry(dict, e1);
        Dictionary::AddEntry(dict, e2); // Должно вызвать исключение
#endif

#ifdef TEST_GETENTRY_05
        // Тест: получение несуществующего элемента
        auto dict = Dictionary::Create("Test", 10);
        Dictionary::Entry e1 = { 1, "First" };
        Dictionary::AddEntry(dict, e1);
        auto e = Dictionary::GetEntry(dict, 2); // Должно вызвать исключение
#endif

#ifdef TEST_GETENTRY_06
        // Тест: удаление несуществующего элемента
        auto dict = Dictionary::Create("Test", 10);
        Dictionary::Entry e1 = { 1, "First" };
        Dictionary::AddEntry(dict, e1);
        Dictionary::DelEntry(dict, 2); // Должно вызвать исключение
#endif

#ifdef TEST_UPDENTRY_07
        // Тест: обновление несуществующего элемента
        auto dict = Dictionary::Create("Test", 10);
        Dictionary::Entry e1 = { 1, "First" };
        Dictionary::Entry e2 = { 2, "Second" };
        Dictionary::AddEntry(dict, e1);
        Dictionary::UpdEntry(dict, 2, e2); // Должно вызвать исключение
#endif

#ifdef TEST_UPDENTRY_08
        // Тест: дублирование идентификатора при обновлении
        auto dict = Dictionary::Create("Test", 10);
        Dictionary::Entry e1 = { 1, "First" };
        Dictionary::Entry e2 = { 2, "Second" };
        Dictionary::Entry e3 = { 2, "Duplicate" };
        Dictionary::AddEntry(dict, e1);
        Dictionary::AddEntry(dict, e2);
        Dictionary::UpdEntry(dict, 1, e3); // Должно вызвать исключение (новый ID = 2 уже существует)
#endif

#ifdef TEST_DICTIONARY
        // Демонстрация работы всех функций
        try {
            // Создаем словарь преподавателей
            auto teachers = Dictionary::Create("Преподаватели", 10);

            // Добавляем преподавателей
            Dictionary::Entry t1 = { 1, "Иванов И.И." };
            Dictionary::Entry t2 = { 2, "Петров П.П." };
            Dictionary::Entry t3 = { 3, "Сидоров С.С." };
            Dictionary::Entry t4 = { 4, "Кузнецов К.К." };
            Dictionary::Entry t5 = { 5, "Смирнов С.С." };
            Dictionary::Entry t6 = { 6, "Васильев В.В." };
            Dictionary::Entry t7 = { 7, "Николаев Н.Н." };

            Dictionary::AddEntry(teachers, t1);
            Dictionary::AddEntry(teachers, t2);
            Dictionary::AddEntry(teachers, t3);
            Dictionary::AddEntry(teachers, t4);
            Dictionary::AddEntry(teachers, t5);
            Dictionary::AddEntry(teachers, t6);
            Dictionary::AddEntry(teachers, t7);

            // Создаем словарь студентов
            auto students = Dictionary::Create("Студенты", 10);

            // Добавляем студентов
            Dictionary::Entry s1 = { 101, "Алексеев А.А." };
            Dictionary::Entry s2 = { 102, "Борисов Б.Б." };
            Dictionary::Entry s3 = { 103, "Владимиров В.В." };
            Dictionary::Entry s4 = { 104, "Григорьев Г.Г." };
            Dictionary::Entry s5 = { 105, "Дмитриев Д.Д." };
            Dictionary::Entry s6 = { 106, "Егоров Е.Е." };
            Dictionary::Entry s7 = { 107, "Жуков Ж.Ж." };

            Dictionary::AddEntry(students, s1);
            Dictionary::AddEntry(students, s2);
            Dictionary::AddEntry(students, s3);
            Dictionary::AddEntry(students, s4);
            Dictionary::AddEntry(students, s5);
            Dictionary::AddEntry(students, s6);
            Dictionary::AddEntry(students, s7);

            // Печатаем словари
            Dictionary::Print(teachers);
            Dictionary::Print(students);

            // Демонстрация других функций
            // Получение элемента
            auto entry = Dictionary::GetEntry(teachers, 3);
            cout << "Найден преподаватель: " << entry.name << endl;

            // Обновление элемента
            Dictionary::Entry new_t3 = { 3, "Сидоров С.С. (профессор)" };
            Dictionary::UpdEntry(teachers, 3, new_t3);

            // Удаление элемента
            Dictionary::DelEntry(students, 104);

            // Печатаем обновленные словари
            cout << "\nПосле изменений:\n";
            Dictionary::Print(teachers);
            Dictionary::Print(students);

            // Удаляем словари
            Dictionary::Delete(teachers);
            Dictionary::Delete(students);
        }
        catch (const std::exception& e) {
            cerr << "Ошибка: " << e.what() << endl;
        }
#endif
    }
    catch (const std::exception& e)
    {
        cerr << "Поймано исключение: " << e.what() << endl;
        return 1;
    }

    return 0;
}