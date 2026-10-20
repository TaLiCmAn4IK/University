#include "stdafx.h"
#include "Error.h"
#include "Parm.h"
#include "Log.h"
#include "In.h"
#include <fstream>

int _tmain(int argc, _TCHAR* argv[])
{
    setlocale(LC_ALL, "rus");

    try
    {
        // 1. Обработка параметров командной строки
        std::cout << "---- тест Parm::getparm ----" << std::endl;
        Parm::PARM parm = Parm::getparm(argc, argv);
        std::wcout << L"-in: " << parm.in << L", -out: " << parm.out << L", -log: " << parm.log << std::endl << std::endl;

        // 2. Создание и настройка протокола
        Log::LOG log = Log::getlog(parm.log);
        Log::WriteLine(log, "Тест:", " без ошибок", "");
        Log::WriteLog(log);
        Log::WriteParm(log, parm);

        // 3. Ввод и проверка исходного кода
        std::cout << "---- тест In::getin ----" << std::endl;
        In::IN in = In::getin(parm.in);

        // Вывод результатов в консоль
        std::cout << in.text << std::endl;
        std::cout << "Всего символов: " << in.size << std::endl;
        std::cout << "Всего строк: " << in.lines << std::endl;
        std::cout << "Пропущено: " << in.ignore << std::endl;

        // 4. ★ ЗАПИСЬ ВЫХОДНОГО ФАЙЛА ★
        std::ofstream outFile;
        outFile.open(parm.out);
        if (!outFile.is_open()) {
            throw ERROR_THROW(115); // Используем новую ошибку 115
        }
        outFile << in.text;
        outFile.close();
        std::cout << "Выходной файл создан: " << parm.out << std::endl;

        // 5. Запись информации о входных данных в протокол
        Log::WriteIn(log, in);
        Log::Close(log);

        std::cout << "Обработка завершена успешно!" << std::endl;
    }
    catch (Error::ERROR e)
    {
        // Вывод ошибки в консоль
        std::cout << "Ошибка " << e.id << ": " << e.message << std::endl;
        if (e.inext.line != -1)
        {
            std::cout << "Строка " << e.inext.line << ", позиция: " << e.inext.col << std::endl;
        }
    }

    system("pause");
    return 0;
}